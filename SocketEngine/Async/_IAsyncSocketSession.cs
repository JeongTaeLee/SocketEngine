using System.Net.Sockets;
using SocketEngine.Logging;

namespace SocketEngine.Async
{
    public interface _IAsyncSocketSessionOption : ISocketSessionOption
    {
        SocketAsyncEventArgsProxy socketAsyncProxy { get; }
    }

    public interface _IAsyncSocketSession : ISocketSession
    {
        SocketAsyncEventArgsProxy socketAsyncProxy { get; }

        void ProcessReceive(SocketAsyncEventArgs args);
    }
}
