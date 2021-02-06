
using System.Net.Sockets;

namespace SocketEngine.AsyncSockets
{
    internal interface ISocketAsyncEventArgsProxyHandler
    {
        void ProcessReceive(SocketAsyncEventArgs args);
    }
}
