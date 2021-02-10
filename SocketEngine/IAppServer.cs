using System;
using SocketEngine.Logging;
using SocketEngine.Sockets;
using SocketEngine.Configs;

namespace SocketEngine
{
    interface IAppServer : ILoggerProvider
    {
        ILoggerFactory loggerFactroy { get; }

        ServerConfig config { get; }

        ISocketServer socketServer { get;  }

        void Start();
        void End();

        IAppSession CreateAppSession();
        ILogger CreateLogger<T>() where T : class;
        ILogger CreateLogger(string name);
    }

    interface IAppServer<TAppSession> : IAppServer
        where TAppSession : IAppSession
    {
        void OnSessionConnect(TAppSession appSession);
        void OnSessionDisconnect(TAppSession appSession);
    }
}
