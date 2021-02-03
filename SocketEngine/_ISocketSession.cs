//using SocketEngine.Logging;
//using System;
//using System.Net.Sockets;

//namespace SocketEngine
//{
//    public interface _ISocketSessionOption
//    {
//        string sessionId { get; }
//        Socket socket { get; }   
//        ILogger logger { get; }
//    }

//    public interface _ISocketSession : ILoggerProvider
//    {
//        string sessionId { get; }
//        int isClosed { get; }
//        Socket socket { get; }

//        Action<_ISocketSession> Closed { get; set; }
//    }
//}
