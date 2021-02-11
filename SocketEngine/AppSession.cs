using System;
using SocketEngine.Logging;
using SocketEngine.Protocols;
using SocketEngine.Sockets;

namespace SocketEngine
{
    public class AppSession : IAppSession
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

        public void Close()
        {
            socketSession.Close();
        }

        public virtual void OnSessionStarted()
        {
        }

        public virtual void OnSessionClosed()
        {
        }

        public virtual void OnReceive(IRequestInfo info)
        {
        }
    }
}
