using System;
using SocketEngine.Servers;

namespace SocketEngine.Servers.Handlers
{
    public abstract class BaseSessionHandler 
    {
        private BaseSession _session = null;

        internal bool Initialize(BaseSession session)
        {
            _session = session;

            return true;
        }

        internal void Reset()
        {
            _session = null;
        }

        public void Close() => _session.Close();

        public abstract void OnStart();
        public abstract void OnClose();
        public abstract void OnThrowedException(Exception ex);
    }
}
