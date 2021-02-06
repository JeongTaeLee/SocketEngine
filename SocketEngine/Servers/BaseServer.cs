using SocketEngine.Commons;
using SocketEngine.Extensions;
using SocketEngine.Logging;
using SocketEngine.Servers.Configs;
using SocketEngine.Servers.Handlers;

namespace SocketEngine.Servers
{
    public abstract class BaseServer : ILoggerProvider 
    {
        private SessionManager _sessionManager = new SessionManager();

        protected ServerConfig config { get; private set; } = null;
        protected BaseServerHandler handler { get; private set; } = null;
        protected ISessionHandlerFactory sessionHandlerFactory { get; private set; } = null;
        protected ILoggerFactory loggerFactroy { get; private set; } = null;

        public ILogger logger { get; private set; } = null;


        public BaseServer(ServerConfig serverConfig,
            BaseServerHandler handler, 
            ISessionHandlerFactory sessionHandlerFactory, 
            ILoggerFactory loggerFactroy)
        {
            ExceptionExtension.ArgumentNullExceptionIfNull(serverConfig, "ServerConfig");
            ExceptionExtension.ArgumentNullExceptionIfNull(handler, "ServerHandler");
            ExceptionExtension.ArgumentNullExceptionIfNull(sessionHandlerFactory, "ISessionHandlerFactory");
            ExceptionExtension.ArgumentNullExceptionIfNull(loggerFactroy, "ILoggerFactory");

            var logger = loggerFactroy.GetLogger<BaseServer>();
            ExceptionExtension.ExceptionIfNull(logger, "Unable to get the logger from the loggerFactory.");

            this.config = serverConfig;
            this.handler = handler;
            this.sessionHandlerFactory = sessionHandlerFactory;
            this.loggerFactroy = loggerFactroy;
            this.logger = logger;
        }

        public abstract void Start();
        public abstract void Close();

        internal bool AddSession(BaseSession socketSession)
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

        internal bool RemoveSession(BaseSession socketSession)
        {
            return _sessionManager.RemoveSession(socketSession);
        }

        internal string GenerateSessionId()
        {
            return _sessionManager.GenerateSessionId();
        }

        public ILogger CreateLogger<T>()
        {
            return loggerFactroy.GetLogger<T>();
        }
    }
}
