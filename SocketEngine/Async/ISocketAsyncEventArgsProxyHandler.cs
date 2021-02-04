
using System.Net.Sockets;

namespace SocketEngine.Async
{
    internal interface ISocketAsyncEventArgsProxyHandler
    {
        void ProcessReceive(SocketAsyncEventArgs args);
    }
}
