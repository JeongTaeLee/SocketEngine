using System;
using SocketEngine.Logging;
using SocketEngine.Protocols;
using SocketEngine.Sockets;

namespace SocketEngine
{
    class AppSession<TRequestInfo> : IAppSession<TRequestInfo>
        where TRequestInfo : IRequestInfo
    {
        public string sessionId { get; private set; } = null;
        public ISocketSession socketSession { get; private set; } = null;
        
        public ILogger logger { get; private set; } = null;

        public bool Initialize(string sessionId, ISocketSession socketSession)
        {
            if (string.IsNullOrEmpty(sessionId)) throw new ArgumentNullException(nameof(sessionId));
            if (socketSession == null) throw new ArgumentNullException(nameof(socketSession));

            this.sessionId = sessionId;
            this.socketSession = socketSession;

            return false;
        }

        public virtual void OnReceive(TRequestInfo info)
        {

        }
    }
}
