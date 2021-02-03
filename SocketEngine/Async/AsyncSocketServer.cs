using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SocketEngine.Bases;
using SocketEngine.Extensions;
using SocketEngine.Protocols;

namespace SocketEngine.Async
{
    public sealed class AsyncSocketServer<TSessionBehavior, TRequestInfo> : SocketServer<TSessionBehavior, TRequestInfo>
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
            _listener.Start();

            _listener.accepted += Listener_Accepted;
            _listener.throwedException += Listener_ThrowedException;
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
                //TODO @jeongtae.lee : Guid 발급 로직 생성하기
                var sessionId = Guid.NewGuid().ToString();

                var socketSession = new AsyncSocketSession<TSessionBehavior, TRequestInfo>();
                socketSession.Initialize(sessionId, socket, this);

                AddSession(socketSession);
            }
            catch (Exception ex)
            {
                logger.Error("Exception thrown during Socket Acceptance process.", ex);
            }
        }

        private void Listener_ThrowedException(Exception ex)
        {
            logger.Error("Exception thrown to receiver.", ex);
        }
    }
}
