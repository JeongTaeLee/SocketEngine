using System;
using SocketEngine.Extensions;
using SocketEngine.Logging;
using SocketEngine.Configs;
using SocketEngine.Sockets;
using SocketEngine.Commons;

namespace SocketEngine
{
    public abstract partial class AppServer<TAppSession> : IAppServer<TAppSession>
        where TAppSession : class, IAppSession, new()
    {
        public ServerConfig config { get; private set; } = null;
        public ISocketServer socketServer { get; private set; } = null;

        public ILoggerFactory loggerFactroy { get; private set; } = null;        
        public ILogger logger { get; private set; } = null;

        private SessionManager _sessionManager = null;

        public AppServer(ServerConfig serverConfig, ISocketServer socketServer, ILoggerFactory loggerFactroy)
        {
            if (serverConfig == null) throw new ArgumentNullException(nameof(serverConfig));
            if (socketServer == null) throw new ArgumentNullException(nameof(socketServer));
            if (loggerFactroy == null) throw new ArgumentNullException(nameof(loggerFactroy));

            var logger = loggerFactroy.GetLogger("AppServer");
            if (logger == null) throw new Exception("Can't get logger from logger factroy.");
            
            this.config = serverConfig;
            this.loggerFactroy = loggerFactroy;
            this.socketServer = socketServer;
            this.logger = logger;

            _sessionManager = new SessionManager();
        }

        bool IAppServer.AddSession(IAppSession appSession)
        {
            var session = appSession as TAppSession;
            if (session == null)
                return false;

            if (!_sessionManager.AddSession(session))
            {
                return false;
            }

            OnSessionConnected(session);

            return true;
        }

        bool IAppServer.RemoveSession(IAppSession appSession)
        {
            return _sessionManager.RemoveSession(appSession);
        }

        IAppSession IAppServer.CreateAppSession()
        {
            return new TAppSession();
        }

        string IAppServer.CreateSessionId()
        {
            return _sessionManager.GenerateSessionId();   
        }

        public ILogger CreateLogger<T>()
            where T : class
        {
            if (loggerFactroy == null) throw new Exception("Logger facotry is null");
            return loggerFactroy.GetLogger<T>();
        }

        public ILogger CreateLogger(string name)
        {
            if (loggerFactroy == null) throw new Exception("Logger factroy is null");
            return loggerFactroy.GetLogger(name);
        }
    }

    public abstract partial class AppServer<TAppSession>
    {

        public virtual void Start()
        {
            if (config == null) throw new Exception("Logger factory is not set");
            if (socketServer == null) throw new Exception("Socket server is not set");
            if (loggerFactroy == null) throw new Exception("Logger factroy is not set");
            if (logger == null) throw new Exception("Logger is not set");

            socketServer.Initialize(this, config);
            socketServer.Start();
        }

        public virtual void Close()
        {

        }

        public abstract void OnSessionConnected(TAppSession appSession);
        public abstract void OnSessionDisconnected(TAppSession appSession);
    }

}
