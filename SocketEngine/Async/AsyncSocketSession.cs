
using SocketEngine.Bases;
using SocketEngine.Protocols;

namespace SocketEngine.Async
{
    internal sealed class AsyncSocketSession<TSessionBehavior, TRequestInfo> : SocketSession<TSessionBehavior, TRequestInfo>
        where TSessionBehavior : SocketSessionBehavior<TSessionBehavior, TRequestInfo>, new()
        where TRequestInfo : IRequestInfo
    {
        public override void Start()
        {
        }

        public override void Close()
        {
        }
    }
}
