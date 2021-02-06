using System.Net;
using System.Net.Sockets;
using SocketEngine.Extensions;
using SocketEngine.Logging;
using SocketEngine.Servers.Configs;
using SocketEngine.Servers.Handlers;

namespace SocketEngine.Servers
{
    public abstract class BaseSocketServer : BaseServer
    {
        public Socket socket { get; private set; } = null;

        public BaseSocketServer(ServerConfig config, BaseServerHandler handler, ISessionHandlerFactory sessionHandlerFactroy, ILoggerFactory loggerFactroy)
            : base(config, handler, sessionHandlerFactroy, loggerFactroy)
        {
        }

        public abstract override void Start();
        public abstract override void Close();

        internal void CreateSocket()
        {
            ExceptionExtension.ExceptionIfNoneNull(socket, "The socket has already been initialized.");

            IPHostEntry ipHostInfo = Dns.GetHostEntry(config.ip);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, config.port);

            socket = new Socket(ipAddress.AddressFamily, config.socketType, config.protocolType);
            socket.Bind(localEndPoint);
            socket.Listen(config.backlog);
        }

        internal void DisposeSocket()
        {
            if (socket?.Connected ?? false)
                socket.Disconnect(false);

            socket?.Dispose();
            socket = null;
        }
    }
}
