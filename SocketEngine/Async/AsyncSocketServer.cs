using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SocketEngine.Bases;
using SocketEngine.Extensions;
using SocketEngine.Protocols;

namespace SocketEngine.Async
{
    public class AsyncSocketServer<TSessionBehavior, TRequestInfo> : SocketServer<TSessionBehavior, TRequestInfo>
        where TSessionBehavior : SocketSessionBehavior<TSessionBehavior, TRequestInfo>, new()
        where TRequestInfo : IRequestInfo
    {
        private AsyncSocketListener _listener = null;

        private BufferManager _bufferManager = null;
        private SocketAsyncEventArgsProxyPool _socketEventProxyPool = null;

        private bool _alreadyUsed = false;

        public AsyncSocketServer(SocketServerConfig config)
            : base(config)
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

            _listener = new AsyncSocketListener(base.socket, config);
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

        protected virtual void OnSessionStarted(TSessionBehavior behavior)
        {

        }

        protected virtual void OnSessionClosed(TSessionBehavior behavior)
        {

        }

        protected virtual void OnThrowedException(Exception ex)
        {

        }

        protected virtual void OnThrowedExceptionFromBehavior(TSessionBehavior behavior, Exception ex)
        {

        }

        private void SessionClosed(SocketSession<TSessionBehavior, TRequestInfo> socketSession)
        {
            try
            {
                if (socketSession == null)
                {
                    if (logger.IsErrorEnabled)
                        logger.Error("socketSession is null");

                    return;
                }

                OnSessionClosed(socketSession.behavior);

                var asyncSocketSession = socketSession as AsyncSocketSession<TSessionBehavior, TRequestInfo>;
                if (asyncSocketSession != null)
                {
                    if (logger.IsErrorEnabled)
                        _socketEventProxyPool.Push(asyncSocketSession.socketEventProxy);
                }
                else
                {
                    if (logger.IsErrorEnabled)
                        logger.Error("socketSession is not AsyncSocketSession");
                }

                var socket = socketSession.socket;
                if (socket == null)
                {
                    if (logger.IsErrorEnabled)
                        logger.Error("socket is null");
                }

                this.AsyncRun(socket.SafeClose);
            }
            catch (Exception ex)
            {
                if (logger.IsErrorEnabled)
                    logger.Error("Exception thrown during Socket Closed process: ", ex);

                OnThrowedException(ex);
            }
        }

        private void SessionExceptionThrowed(TSessionBehavior behavior, Exception ex)
        {
            OnThrowedExceptionFromBehavior(behavior, ex);
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

                AsyncSocketSession<TSessionBehavior, TRequestInfo> asyuncSocketSession = new AsyncSocketSession<TSessionBehavior, TRequestInfo>(proxy);
                if (!asyuncSocketSession.Initialize(sessionId, socket, this, SessionExceptionThrowed, SessionClosed))
                {
                    _socketEventProxyPool.Push(proxy);
                    this.AsyncRun(socket.SafeClose);

                    logger.Error("Socket session initialization failed");
                    return;
                }
                
                if (!AddSession(asyuncSocketSession))
                {
                    _socketEventProxyPool.Push(proxy);
                    this.AsyncRun(socket.SafeClose);

                    logger.Error("Failed to add socket to SocketSessionManager");
                    return;
                }
                 
                if (!asyuncSocketSession.Start())
                {
                    _socketEventProxyPool.Push(proxy);
                    this.AsyncRun(socket.SafeClose);

                    logger.Error("Failed to SocketSession Start");
                    return;
                }

                OnSessionStarted(asyuncSocketSession.behavior);
            }
            catch (Exception ex)
            {
                if (logger.IsErrorEnabled)
                    logger.Error("Exception thrown during Socket Acceptance process: ", ex);
                
                OnThrowedException(ex);
            }
        }

        private void Listener_ThrowedException(Exception ex)
        {
            if (logger.IsErrorEnabled)
                logger.Error("Exception thrown to receiver: ", ex);
        }
    }
}
