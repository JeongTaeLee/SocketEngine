using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SocketEngine.AsyncSockets;
using SocketEngine.Commons;
using SocketEngine.Extensions;
using SocketEngine.Logging;
using SocketEngine.Servers.Configs;
using SocketEngine.Servers.Handlers;

namespace SocketEngine.Servers.Asyncs
{
    public sealed class AsyncSocketServer : BaseSocketServer
    {
        private AsyncSocketListener _listener = null;
        private BufferManager _bufferManager = null;
        private SocketAsyncEventArgsProxyPool _socketEventProxyPool = null;

        private bool _alreadyUsed = false;

        public AsyncSocketServer(ServerConfig config, BaseServerHandler handler, ISessionHandlerFactory sessionHandlerFactroy, ILoggerFactory loggerFactroy)
            : base(config, handler, sessionHandlerFactroy, loggerFactroy)
        {
            int totalByte = config.receiveBufferSize * config.maxConnection;
            _bufferManager = new BufferManager(totalByte, config.receiveBufferSize);
            _bufferManager.InitBuffer();

            List<SocketAsyncEventArgsProxy> proxys = new List<SocketAsyncEventArgsProxy>();
            for (int index = 0; index < config.maxConnection; ++index)
            {
                SocketAsyncEventArgs socketEvent = new SocketAsyncEventArgs();
                _bufferManager.SetBuffer(socketEvent);

                proxys.Add(new SocketAsyncEventArgsProxy(socketEvent));
            }

            _socketEventProxyPool = new SocketAsyncEventArgsProxyPool(proxys);
        }

        public override void Start()
        {
            ExceptionExtension.ExceptionIfTrue(_alreadyUsed, "Server already used The server cannot be reused.");

            _alreadyUsed = true;

            CreateSocket();

            _listener = new AsyncSocketListener(base.socket, config, loggerFactroy.GetLogger<AsyncSocketListener>());
            _listener.accepted += Listener_Accepted;
            _listener.throwedException += Listener_ThrowedException;
            _listener.Start();
        }

        public override void Close()
        {
            _listener?.Close();
            _listener = null;

            DisposeSocket();
        }
        private void Listener_Accepted(Socket socket)
        {
            try
            {
                SocketAsyncEventArgsProxy proxy = _socketEventProxyPool.Pop();
                if (proxy == null)
                {
                    this.AsyncRun(socket.SafeClose);

                    if (logger.IsErrorEnabled)
                        logger.ErrorFormat("Max connection number {0} was reached.", config.maxConnection);
                    return;
                }

                string sessionId = GenerateSessionId();
                if (string.IsNullOrEmpty(sessionId))
                {
                    _socketEventProxyPool.Push(proxy);
                    this.AsyncRun(socket.SafeClose);

                    if (logger.IsErrorEnabled)
                        logger.Error("Failed to generate session ID.");
                    return;
                }

                var sessionHandler = sessionHandlerFactory.CreateSession();
                if (sessionHandler == null)
                {
                    _socketEventProxyPool.Push(proxy);
                    this.AsyncRun(socket.SafeClose);

                    if (logger.IsErrorEnabled)
                        logger.Error("Failed to create session handler.");

                    return;
                }

                AsyncSocketSession asyncSocketSession = new AsyncSocketSession(sessionId, this, sessionHandler, 
                    ClosedSession,  
                    socket, 
                    proxy);
     
                if (!AddSession(asyncSocketSession))
                {
                    _socketEventProxyPool.Push(proxy);
                    this.AsyncRun(socket.SafeClose);

                    logger.Error("Failed to add socket to SocketSessionManager");
                    return;
                }

                if (!asyncSocketSession.Start())
                {
                    _socketEventProxyPool.Push(proxy);
                    this.AsyncRun(socket.SafeClose);

                    logger.Error("Failed to SocketSession Start");
                    return;
                }

                handler.OnSessionStarted(sessionHandler);
            }
            catch (Exception ex)
            {
                if (logger.IsErrorEnabled)
                    logger.Error("Exception thrown during Socket Acceptance process: ", ex);

                handler.OnThrowedException(ex);
            }
        }

        private void ClosedSession(BaseSession session)
        {
            try
            {
                if (session == null)
                {
                    if (logger.IsErrorEnabled)
                        logger.Error("socketSession is null");

                    return;
                }

                var asyncSocketSession = session as AsyncSocketSession;
                if (asyncSocketSession is AsyncSocketSession)
                {
                    if (logger.IsErrorEnabled)
                        _socketEventProxyPool.Push(asyncSocketSession.SocketEventProxy);
                }
                else
                {
                    if (logger.IsErrorEnabled)
                        logger.Error("socketSession is not AsyncSocketSession");
                }

                var socket = asyncSocketSession.socket;
                if (socket == null)
                {
                    if (logger.IsErrorEnabled)
                        logger.Error("socket is null");
                }

                this.AsyncRun(socket.SafeClose);

                handler.OnSessionClosed(session.handler);
            }
            catch (Exception ex)
            {
                if (logger.IsErrorEnabled)
                    logger.Error("Exception thrown during Socket Closed process: ", ex);

                handler.OnThrowedException(ex);
            }
        }

        private void Listener_ThrowedException(Exception ex)
        {
            if (logger.IsErrorEnabled)
                logger.Error("Exception thrown to receiver: ", ex);

            handler.OnThrowedException(ex);
        }

    }
}
