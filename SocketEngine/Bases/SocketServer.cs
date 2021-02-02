using System.Net.Sockets;

namespace SocketEngine.Bases
{
    public abstract class SocketServer
    {
        protected Socket socket;
        protected SocketServerConfig config;

        public SocketServer(SocketServerConfig config)
        {
            this.config = config;
        }

        public abstract void Start();
        public abstract void Close();
    }
}
