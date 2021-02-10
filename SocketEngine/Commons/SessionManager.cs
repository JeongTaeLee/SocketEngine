using System;
using System.Collections.Concurrent;
using SocketEngine.Extensions;
using SocketEngine.Sockets;

namespace SocketEngine.Commons
{
    internal class SessionManager
    {
        private ConcurrentDictionary<string, IAppSession> _sessions = new ConcurrentDictionary<string, IAppSession>();

        public bool AddSession(IAppSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (string.IsNullOrEmpty(session.sessionId)) throw new ArgumentNullException(nameof(session.sessionId));

            return _sessions.TryAdd(session.sessionId, session);
        }

        public bool RemoveSession(IAppSession session)
        {
            if (session == null) throw new ArgumentNullException(nameof(session));
            if (string.IsNullOrEmpty(session.sessionId)) throw new ArgumentNullException(nameof(session.sessionId));

            return _sessions.TryRemove(session.sessionId, out var temp);
        }

        public string GenerateSessionId()
        {
            string newSessionId = Guid.NewGuid().ToString();
            if (!_sessions.ContainsKey(newSessionId))
                return newSessionId;

            int maxLoopCount = 10000;
            int curLoopCount = 0;

            do
            {
                ++curLoopCount;
                newSessionId = Guid.NewGuid().ToString();
            } while (_sessions.ContainsKey(newSessionId) && (maxLoopCount > curLoopCount));

            if (maxLoopCount <= curLoopCount)
                return string.Empty;

            return newSessionId;
        }
    }
}
