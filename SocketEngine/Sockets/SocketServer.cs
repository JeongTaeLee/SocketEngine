using System;
using System.Net;
using System.Net.Sockets;
using SocketEngine.Extensions;
using SocketEngine.Configs;
using SocketEngine.Logging;

namespace SocketEngine.Sockets
{
    abstract class SocketServer<TSocketServer> : ISocketServer
        where TSocketServer : SocketServer<TSocketServer>
    {
        public IAppServer appServer { get; private set; } = null;

        public ServerConfig config { get; private set; } = null;
        public Socket socket { get; private set; } = null;

        public ILogger logger { get; private set; } = null;

        public void Initialize(IAppServer appServer, ServerConfig serverConfig)
        {
            if (appServer == null) throw new ArgumentNullException(nameof(appServer));
            if (serverConfig == null) throw new ArgumentNullException(nameof(serverConfig));

            var logger = appServer.CreateLogger<TSocketServer>();
            if (logger == null) throw new Exception("Can't get logger from logger factroy.");
            
            this.appServer = appServer;
            this.config = serverConfig;
            this.logger = logger;
        }

        public virtual void Start()
        {
            if (appServer == null) throw new Exception("App server is not set");
            if (config == null) throw new Exception("Server config is not set");

            IPHostEntry ipHostInfo = Dns.GetHostEntry(config.ip);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, config.port);

            socket = new Socket(ipAddress.AddressFamily, config.socketType, config.protocolType);
            socket.Bind(localEndPoint);
            socket.Listen(config.backlog);
        }

        public virtual void End()
        {
            this.socket.Close();
            this.socket = null;

            this.config = null;
        }
    }
}
