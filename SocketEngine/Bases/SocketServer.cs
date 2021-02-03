using System.Net;
using System.Net.Sockets;
using SocketEngine.Extensions;
using SocketEngine.Logging;
using SocketEngine.Protocols;

namespace SocketEngine.Bases
{
    public abstract class SocketServer<TSessionBehavior, TRequestInfo>
        where TSessionBehavior : SocketSessionBehavior<TSessionBehavior, TRequestInfo>, new()
        where TRequestInfo : IRequestInfo
    {
        protected SocketServerConfig config { get; private set; } = null;
        protected Socket socket { get; private set; } = null;
        protected ILogger logger { get; private set; } = null;

        public SocketServer(SocketServerConfig config)
        {
            ExceptionExtension.ArgumentNullExceptionIfNull(config, "config");

            var logger = config.loggerFactory.GetLogger<SocketServer<TSessionBehavior, TRequestInfo>>();
            ExceptionExtension.ExceptionIfNull(logger, "Unable to get the logger from the loggerFactory.");

            this.config = config;
            this.logger = logger; 
        }

        public abstract void Start();
        public abstract void Close();

        protected virtual void CreateSocket()
        {
            ExceptionExtension.ExceptionIfNoneNull(socket, "The socket has already been initialized.");

            IPHostEntry ipHostInfo = Dns.GetHostEntry(config.ip);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, config.port);

            socket = new Socket(ipAddress.AddressFamily, config.socketType, config.protocolType);
            socket.Bind(localEndPoint);
            socket.Listen(config.backlog);
        }

        protected virtual void DisposeSocket()
        {
            if (socket?.Connected ?? false)
                socket.Disconnect(false);

            socket?.Dispose();
            socket = null;
        }

        internal void AddSession(SocketSession<TSessionBehavior, TRequestInfo> socketSession)
        {

        }

        internal void RemoveSession(SocketSession<TSessionBehavior, TRequestInfo> socketSession)
        {

        }
    }
}
