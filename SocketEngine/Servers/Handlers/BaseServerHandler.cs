using System;

namespace SocketEngine.Servers.Handlers
{
    public abstract class BaseServerHandler
    {
        public abstract void OnSessionStarted(BaseSessionHandler sessionHandler);
        public abstract void OnSessionClosed(BaseSessionHandler sessionHandler);
        public abstract void OnThrowedException(Exception ex);
    }
}
