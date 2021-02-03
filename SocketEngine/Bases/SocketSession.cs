using System.Net.Sockets;
using SocketEngine.Extensions;
using SocketEngine.Protocols;

namespace SocketEngine.Bases
{
    /// <summary>
    /// 해당 클래스는 SocketEngine을 사용하는 프로젝트에서는 공개되지 않고,
    /// 대신 SocketSessionBehavior라는 클래스를 상속받아서 해당 클래스의 동작을 정의한다.
    /// </summary>
    /// <typeparam name="TSessionBehavior"></typeparam>
    internal abstract class SocketSession<TSessionBehavior, TRequestInfo>
        where TSessionBehavior : SocketSessionBehavior<TSessionBehavior, TRequestInfo>, new()
        where TRequestInfo : IRequestInfo
    {
        public string sessionId { get; private set; } = null;
        public SocketServer<TSessionBehavior, TRequestInfo> socketServer { get; private set; } = null;
        public Socket socket { get; private set; } = null;
        public TSessionBehavior behavior { get; private set; } = null;

        public SocketSession()
        {
            this.behavior = new TSessionBehavior();
            this.behavior.Initialize(this);
        }

        public void Initialize(string sessionId, Socket socket, SocketServer<TSessionBehavior, TRequestInfo> socketServer)
        {
            ExceptionExtension.ArgumentExceptionIsNullOrEmpty(sessionId, "sessionId");
            ExceptionExtension.ArgumentNullExceptionIfNull(socket, "socket");
            ExceptionExtension.ArgumentNullExceptionIfNull(socketServer, "socketServer");

            this.sessionId = sessionId;
            this.socket = socket;
            this.socketServer = socketServer;
        }

        public abstract void Start();
        public abstract void Close();
    }
}
