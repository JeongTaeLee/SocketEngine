using System;
using SocketEngine.Logging;
using SocketEngine.Sockets;
using SocketEngine.Protocols;

namespace SocketEngine
{
    interface IAppSession : ILoggerProvider
    {
        string sessionId { get; }

        ISocketSession socketSession { get; }

        bool Initialize(string sessionId, ISocketSession socketSession);
    }

    interface IAppSession<TRequestInfo> : IAppSession
        where TRequestInfo : IRequestInfo
    {

        void OnReceive(TRequestInfo info);
    }

}
