using System;
using System.Collections.Generic;
using System.Net.Sockets;
using SocketEngine.Extensions;
using SocketEngine.Logging;

namespace SocketEngine.Async
{
    public class _AsyncServer<TSession, TRequestInfo> : ILoggerProvider
        where TSession : _AsyncSocketSession<TRequestInfo>, new()
        where TRequestInfo : IRequestInfo
    {
        private SocketServerConfig _serverConfig = null;
        private _AsyncListener _listener = null;

        private BufferManager _bufferManager = null;
        private SocketAsyncEventArgsProxyPool _socketAsyncProxyPool = null;


        private bool _completeSetUP = false;

        public ILogger logger { get; private set; } = null;
        public ILoggerFactory loggerFactory { get; private set; } = null;

        public void SetUp(SocketServerConfig serverConfig)
        {
            if (serverConfig == null)
                throw new ArgumentNullException("serverConfig");

            _serverConfig = serverConfig;

            loggerFactory = serverConfig.loggerFactory;
            logger = loggerFactory.GetLogger("AsyncServer");

            // 버퍼 매니저 초기화.
            int totalBufferSize = _serverConfig.maxConnection * _serverConfig.receiveBufferSize;
            _bufferManager = new BufferManager(totalBufferSize, _serverConfig.receiveBufferSize);
            _bufferManager.InitBuffer();

            // Receive SocketAsyncEventArgs 초기화.
            List<SocketAsyncEventArgsProxy> args = new List<SocketAsyncEventArgsProxy>(_serverConfig.maxConnection);
            for (int proxyIndex = 0; proxyIndex < _serverConfig.maxConnection; ++proxyIndex)
            {
                var tempArgs = new SocketAsyncEventArgs();
                _bufferManager.SetBuffer(tempArgs);
                
                args.Add(new SocketAsyncEventArgsProxy(tempArgs));
            }
            
            _socketAsyncProxyPool = new SocketAsyncEventArgsProxyPool(args);

            _completeSetUP = true;
        }
        
        public void Start()
        {
            if (!_completeSetUP)
                throw new Exception("SetUp must be called first before starting the server.");

            _listener = new _AsyncListener();
            _listener.accepted += OnSocketAccepted;
            _listener.throwedException += OnThrowedException;
            _listener.Start(_serverConfig);
        }

        private void OnSocketAccepted(Socket socket)
        {
            try
            {
                if (socket == null)
                    return;

                ProcessNewClient(socket);
            }
            catch (Exception ex)
            {
                if (logger.IsErrorEnabled)
                    logger.Error(ex);
            }
        }
        private void OnThrowedException(Exception ex)
        {
            if (logger.IsErrorEnabled)
                logger.Error(ex);
        }

        private void OnSessionClosed()
        {

        }

        private void ProcessNewClient(Socket socket)
        {
            var socketAsyncProxy = _socketAsyncProxyPool.Pop();
            if (socketAsyncProxy == null)
            {
                // 풀에서 Proxy를 가져오지 못하면 최대 접속 소켓 수를 넘은 것이다.
                // 소켓을 종료한다.

                this.AsyncRun(socket.SafeClose);
                
                if (logger.IsErrorEnabled)
                    logger.ErrorFormat("Max connection number {0} was reached!", _serverConfig.maxConnection);
                
                return;
            }

            TSession session = new TSession();
            if (!session.Initialize(new AsyncSocketSessionOption(string.Empty, socket, socketAsyncProxy, loggerFactory.GetLogger<TSession>())))
            {
                socketAsyncProxy.Reset();
                _socketAsyncProxyPool.Push(socketAsyncProxy);

                this.AsyncRun(socket.SafeClose);

                if (logger.IsErrorEnabled)
                    logger.ErrorFormat("failed to session initialize");

                return;
            }

        }
    }
}
