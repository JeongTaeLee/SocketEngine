

using SocketEngine.Bases;

namespace SocketEngine.Async
{
    class AsyncSocketSession<TSessionBehavior> : SocketSession<TSessionBehavior>
        where TSessionBehavior : SocketSessionBehavior<TSessionBehavior>, new()
    {
        public override void Start()
        {
        }

        public override void Close()
        {
        }
    }
}
