using SocketEngine.Extensions;
using SocketEngine.Logging;
using SocketEngine.Servers.Handlers;

namespace SocketEngine.Servers
{
    internal abstract class BaseSession : ILoggerProvider
    {
        public delegate void ClosedHandler(BaseSession session);
        
        public string sessionId { get; private set; } = string.Empty;
        public ILogger logger { get; private set; } = null;
        public BaseSessionHandler handler { get; private set; } = null;
        
        protected ClosedHandler closed { get; private set; } = null;
        protected BaseServer server { get; private set; } = null;

        public BaseSession(string sessionId, BaseServer server, BaseSessionHandler sessionHandler, ClosedHandler closed)
        {
            ExceptionExtension.ArgumentExceptionIfNullOrEmpty(sessionId, "sessionId");
            ExceptionExtension.ArgumentNullExceptionIfNull(server, "server");
            ExceptionExtension.ArgumentNullExceptionIfNull(sessionHandler, "sessionHandler");
            ExceptionExtension.ArgumentNullExceptionIfNull(closed, "closed");

            var logger = server.CreateLogger<BaseSession>();
            ExceptionExtension.ArgumentNullExceptionIfNull(logger, "logger");

            this.sessionId = sessionId;
            this.server = server;
            this.closed = closed;
            this.logger = logger;
        
        }

        public abstract bool Start();
        public abstract void Close();
    }
}
