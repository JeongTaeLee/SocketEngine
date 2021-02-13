using System.Net;

namespace SocketEngine.Sockets
{
    class ListenerInfo
    {
        public IPEndPoint endPoint { get; private set; }
        public int backlog { get; private set; }

        public ListenerInfo(IPEndPoint endPoint, int backlog)
        {
            this.endPoint = endPoint;
            this.backlog = backlog;
        }
    }
}
