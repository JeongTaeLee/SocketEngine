using System;
using System.Threading;
using System.Net.Sockets;
using SocketEngine.Logging;
using SocketEngine.Extensions;

namespace SocketEngine.Async
{
    public class AsyncSocketSessionOption : IAsyncSocketSessionOption
    {
        public string sessionId { get; private set; }
        public Socket socket { get; private set; }
        public SocketAsyncEventArgsProxy socketAsyncProxy { get; private set; }
        public ILogger logger { get; private set; }

        private AsyncSocketSessionOption() { }
        public AsyncSocketSessionOption(string sessionId, Socket socket, SocketAsyncEventArgsProxy socketAsyncProxy, ILogger logger)
        {
            this.sessionId = sessionId;
            this.socket = socket;
            this.socketAsyncProxy = socketAsyncProxy;
            this.logger = logger;
        }
    }

    public class AsyncSocketSession<TRequestInfo> : IAsyncSocketSession
        where TRequestInfo : IRequestInfo
    {
        public string sessionId { get; private set; } = string.Empty;
        public int isClosed { get; } = 0;

        public Socket socket { get; private set; } = null;
        public SocketAsyncEventArgsProxy socketAsyncProxy { get; private set; }
        public SocketAsyncEventArgs asyncEventArgsSend { get; private set; }

        public ILogger logger { get; private set; }

        public Action<ISocketSession> Closed { get; set; }

        public bool Initialize(IAsyncSocketSessionOption option)
        {
            if (option == null)
                return false;

            socket = option.socket;
            logger = option.logger;

            socketAsyncProxy = option.socketAsyncProxy;
            socketAsyncProxy.Initialize(this);

            asyncEventArgsSend = new SocketAsyncEventArgs();
            asyncEventArgsSend.UserToken = this;

            return true;
        }

        public void Start()
        {
            StartReceive(socketAsyncProxy.asyncEventArgs);
        }

        public void Close()
        {
            socket.SafeClose();
            Closed?.Invoke(this);
        }

        public void StartReceive(SocketAsyncEventArgs args)
        {
            try
            {
                bool pending = socket.ReceiveAsync(args);
                if (!pending)
                {
                    ProcessReceive(args);
                }
            }
            catch (Exception ex)
            {
                if (logger.IsErrorEnabled)
                    logger.Error(ex);
            }
        }

        public void ProcessReceive(SocketAsyncEventArgs args)
        {
            if (args.LastOperation != SocketAsyncOperation.Receive)
                return;

            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {

            }
            else
            {

            }
        }
    }
}
