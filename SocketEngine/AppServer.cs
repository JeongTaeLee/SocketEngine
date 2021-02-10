using System;
using SocketEngine.Extensions;
using SocketEngine.Logging;
using SocketEngine.Configs;
using SocketEngine.Sockets;

namespace SocketEngine
{
    abstract partial class AppServer<TAppSession> : IAppServer<TAppSession>
        where TAppSession : IAppSession, new()
    {
        public ServerConfig config { get; private set; } = null;
        public ISocketServer socketServer { get; private set; } = null;

        public ILoggerFactory loggerFactroy { get; private set; } = null;        
        public ILogger logger { get; private set; } = null;

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
        }

        public IAppSession CreateAppSession()
        {
            return new TAppSession();
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

    abstract partial class AppServer<TAppSession>
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

        public virtual void End()
        {

        }

        public abstract void OnSessionConnect(TAppSession appSession);
        public abstract void OnSessionDisconnect(TAppSession appSession);
    }

}
