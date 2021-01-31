using System.Net.Sockets;
using SocketEngine.Logging;

namespace SocketEngine.Async
{
    public interface IAsyncSocketSessionOption : ISocketSessionOption
    {
        SocketAsyncEventArgsProxy socketAsyncProxy { get; }
    }

    public interface IAsyncSocketSession : ISocketSession
    {
        SocketAsyncEventArgsProxy socketAsyncProxy { get; }

        void ProcessReceive(SocketAsyncEventArgs args);
    }
}
