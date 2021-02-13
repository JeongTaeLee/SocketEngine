using System;
using SocketEngine.Logging;
using SocketEngine.Sockets;
using SocketEngine.Configs;

namespace SocketEngine
{
    public interface IAppServer : ILoggerProvider
    {
        IServerConfig config { get; }

        void Start();
        void Close();
        void HandleException(Exception ex);

        bool AddSession(IAppSession appSession);
        bool RemoveSession(IAppSession appSession);

        string CreateSessionId();
        IAppSession CreateAppSession();
        ILogger CreateLogger<T>() where T : class;
        ILogger CreateLogger(string name);
    }

    interface IAppServer<TAppSession> : IAppServer
        where TAppSession : IAppSession
    {
        void OnSessionConnected(TAppSession appSession);
        void OnSessionDisconnected(TAppSession appSession);
    }
}
