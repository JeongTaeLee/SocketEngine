using System.Net.Sockets;
using SocketEngine.Extensions;
using SocketEngine.Servers.Handlers;

namespace SocketEngine.Servers
{
    internal abstract class BaseSocketSession : BaseSession
    {
        public Socket socket { get; private set; } = null;
    

        public BaseSocketSession(string sessionId, 
            BaseServer server,
            BaseSessionHandler sessionHandler, 
            ClosedHandler closed, 
            Socket socket)
            : base(sessionId, server, sessionHandler, closed)
        {
            ExceptionExtension.ArgumentNullExceptionIfNull(socket, "socket");

            this.socket = socket;
        }

        public abstract override bool Start();
        public abstract override void Close();
    }
}
