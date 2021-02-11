using System;
using System.Net.Sockets;
using SocketEngine.Configs;
using SocketEngine.Logging;

namespace SocketEngine.Sockets
{
    public interface ISocketServer : ILoggerProvider
    {
        IAppServer appServer { get; }

        ServerConfig config { get; }
        Socket socket { get; }

        void Initialize(IAppServer appServer, ServerConfig serverConfig);
        void Start();
        void End();

    }
}
