using System;
using SocketEngine.Logging;
using SocketEngine.Sockets;
using SocketEngine.Protocols;

namespace SocketEngine
{
    public interface IAppSession : ILoggerProvider
    {
        string sessionId { get; }

        ISocketSession socketSession { get; }

        bool Initialize(string sessionId, ISocketSession socketSession);

        void Close();

        void OnSessionStarted();
        void OnSessionClosed();
        void OnReceive(IRequestInfo info);
    }

    public interface IAppSession<TRequestInfo> : IAppSession
        where TRequestInfo : IRequestInfo
    {
    }

}
