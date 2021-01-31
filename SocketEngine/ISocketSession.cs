using SocketEngine.Logging;
using System;
using System.Net.Sockets;

namespace SocketEngine
{
    public interface ISocketSessionOption
    {
        string sessionId { get; }
        Socket socket { get; }   
        ILogger logger { get; }
    }

    public interface ISocketSession : ILoggerProvider
    {
        string sessionId { get; }
        int isClosed { get; }
        Socket socket { get; }

        Action<ISocketSession> Closed { get; set; }
    }
}
