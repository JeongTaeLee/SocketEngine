using System;
using System.Collections.Concurrent;
using SocketEngine.Bases;
using SocketEngine.Extensions;
using SocketEngine.Protocols;

namespace SocketEngine
{
    internal class SocketSessionManager<TSessionBehavior, TRequestInfo>
        where TSessionBehavior : SocketSessionBehavior<TSessionBehavior, TRequestInfo>, new()
        where TRequestInfo : IRequestInfo

    {
        private ConcurrentDictionary<string, SocketSession<TSessionBehavior, TRequestInfo>> _sessions = new ConcurrentDictionary<string, SocketSession<TSessionBehavior, TRequestInfo>>();
        
        public bool AddSession(SocketSession<TSessionBehavior, TRequestInfo> session)
        {
            if (session == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(session.sessionId))
            {
                return false;
            }

            return _sessions.TryAdd(session.sessionId, session);
        }

        public bool RemoveSession(SocketSession<TSessionBehavior, TRequestInfo> session)
        {
            if (session == null)
                return false;

            if (string.IsNullOrEmpty(session.sessionId))
                return false;

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
