using System;
using System.Net.Sockets;
using SocketEngine.Logging;
using SocketEngine.Extensions;
using SocketEngine.Protocols;

namespace SocketEngine.Bases
{
    /// <summary>
    /// 해당 클래스는 SocketEngine을 사용하는 프로젝트에서는 공개되지 않고,
    /// 대신 SocketSessionBehavior라는 클래스를 상속받아서 해당 클래스의 동작을 정의한다.
    /// </summary>
    /// <typeparam name="TSessionBehavior"></typeparam>
    internal abstract class SocketSession<TSessionBehavior, TRequestInfo> : ILoggerProvider
        where TSessionBehavior : SocketSessionBehavior<TSessionBehavior, TRequestInfo>, new()
        where TRequestInfo : IRequestInfo
    {
        public delegate void ExceptionHandler(TSessionBehavior behavior, Exception ex);
        public delegate void ClosedHandler(SocketSession<TSessionBehavior, TRequestInfo> session);

        public string sessionId { get; private set; } = null;
        public SocketServer<TSessionBehavior, TRequestInfo> socketServer { get; private set; } = null;
        public Socket socket { get; private set; } = null;
        public TSessionBehavior behavior { get; private set; } = null;
        public ILogger logger { get; private set; } = null;


        protected ExceptionHandler exceptionThrowed { get; private set; } = null;
        protected ClosedHandler closed { get; private set; } = null;

        public SocketSession()
        {
            this.behavior = new TSessionBehavior();
            this.behavior.Initialize(this);
        }

        public bool Initialize(string sessionId, Socket socket, SocketServer<TSessionBehavior, TRequestInfo> socketServer, ExceptionHandler exceptionThrowed, ClosedHandler closed)
        {
            if (string.IsNullOrEmpty(sessionId)
                || null == socket
                || null == socketServer
                || null == closed)
            {
                return false;
            }

            var logger = socketServer.config.loggerFactory.GetLogger<SocketSession<TSessionBehavior, TRequestInfo>>();
            if (null == logger)
            {
                return false;
            }

            this.sessionId = sessionId;
            this.socket = socket;
            this.socketServer = socketServer;
            this.logger = logger;

            this.exceptionThrowed = exceptionThrowed;
            this.closed = closed;

            return true;
        }

        public abstract bool Start();
        public abstract void Close();
    }
}
