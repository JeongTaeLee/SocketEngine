using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using SocketEngine.Extensions;
using SocketEngine.Logging;
using SocketEngine.Protocols;

namespace SocketEngine.Bases
{
    public abstract class SocketServer<TSessionBehavior, TRequestInfo> : ILoggerProvider
        where TSessionBehavior : SocketSessionBehavior<TSessionBehavior, TRequestInfo>, new()
        where TRequestInfo : IRequestInfo
    {
        protected Socket socket { get; private set; } = null;
        
        public SocketServerConfig config { get; private set; } = null;
        public ILogger logger { get; private set; } = null;
        
        private SocketSessionManager<TSessionBehavior, TRequestInfo> _sessionManager = new SocketSessionManager<TSessionBehavior, TRequestInfo>();

        public SocketServer(SocketServerConfig config)
        {
            ExceptionExtension.ArgumentNullExceptionIfNull(config, "config");

            var logger = config.loggerFactory.GetLogger<SocketServer<TSessionBehavior, TRequestInfo>>();
            ExceptionExtension.ExceptionIfNull(logger, "Unable to get the logger from the loggerFactory.");

            this.logger = logger; 
            this.config = config;

        }

        public abstract void Start();
        public abstract void Close();

        internal virtual void CreateSocket()
        {
            ExceptionExtension.ExceptionIfNoneNull(socket, "The socket has already been initialized.");

            IPHostEntry ipHostInfo = Dns.GetHostEntry(config.ip);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, config.port);

            socket = new Socket(ipAddress.AddressFamily, config.socketType, config.protocolType);
            socket.Bind(localEndPoint);
            socket.Listen(config.backlog);
        }

        internal virtual void DisposeSocket()
        {
            if (socket?.Connected ?? false)
                socket.Disconnect(false);

            socket?.Dispose();
            socket = null;
        }

        internal bool AddSession(SocketSession<TSessionBehavior, TRequestInfo> socketSession)
        {
            if (socketSession == null)
            {
                logger.Error("SocketSession is null");
                return false;
            }

            if (string.IsNullOrEmpty(socketSession.sessionId))
            {
                logger.Error("SessionId is null or empty");
                return false;
            }

            return _sessionManager.AddSession(socketSession);
        }

        internal bool RemoveSession(SocketSession<TSessionBehavior, TRequestInfo> socketSession)
        {
            return _sessionManager.RemoveSession(socketSession);
        }

        internal string GenerateSessionId() => _sessionManager.GenerateSessionId();
    }
}
