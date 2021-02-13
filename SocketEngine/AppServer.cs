using System;
using SocketEngine.Extensions;
using SocketEngine.Logging;
using SocketEngine.Configs;
using SocketEngine.Sockets;
using SocketEngine.Commons;
using SocketEngine.Sockets.Asyncs;

namespace SocketEngine
{
    public abstract partial class AppServer<TAppSession> : IAppServer<TAppSession>
        where TAppSession : class, IAppSession, new()
    {
        private ISocketServer _socketServer = null;
        private ILoggerFactory _loggerFactroy = null;        
        private SessionManager _sessionManager = null;

        public IServerConfig config { get; private set; } = null;
        public ILogger logger { get; private set; } = null;
        
        public AppServer()
        {
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

        private ISocketServer CreateSocketServer()
        {
            if (config.socketMode == SocketMode.Tcp)
            {
                return new AsyncSocketServer(this);
            }

            return null;
        }

        public ILogger CreateLogger<T>()
            where T : class
        {
            if (_loggerFactroy == null) throw new Exception("Logger facotry is null");
            return _loggerFactroy.GetLogger<T>();
        }

        public ILogger CreateLogger(string name)
        {
            if (_loggerFactroy == null) throw new Exception("Logger factroy is null");
            return _loggerFactroy.GetLogger(name);
        }

    }

    public abstract partial class AppServer<TAppSession>
    {
        public virtual void SetUp(IServerConfig serverConfig, ILoggerFactory loggerFactroy)
        {
            if (serverConfig == null) throw new ArgumentNullException(nameof(serverConfig));
            if (loggerFactroy == null) throw new ArgumentNullException(nameof(loggerFactroy));

            this.config = serverConfig;
            this._loggerFactroy = loggerFactroy;
        }

        public virtual void Start()
        {
            if (config == null) throw new Exception("Logger factory is not set");
            if (_loggerFactroy == null) throw new Exception("Logger factroy is not set");

            logger = CreateLogger("AppServer");
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            
            _socketServer = CreateSocketServer();
            if (_socketServer == null) throw new Exception("Failed to create socket server");

            _socketServer.Start();
        }

        public virtual void Close()
        {
            _socketServer.Close();
        }

        public virtual void OnSessionConnected(TAppSession appSession)
        {

        }

        public virtual void OnSessionDisconnected(TAppSession appSession)
        {

        }

        public virtual void HandleException(Exception ex)
        {

        }
    }

}
