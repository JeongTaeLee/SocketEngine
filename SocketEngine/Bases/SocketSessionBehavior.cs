namespace SocketEngine.Bases
{
    public abstract class SocketSessionBehavior<TSessionBehvaior>
        where TSessionBehvaior : SocketSessionBehavior<TSessionBehvaior>, new()
    {
        private SocketSession<TSessionBehvaior> _session = null;

        /// <summary>
        /// 해당 함수는 SocketEngine을 사용하는 프로젝트에서는 공개되지 않은다.
        /// 오직 SocketEngine 프로젝트 내부에서만 호출된다.
        /// </summary>
        /// <param name="sesssion"></param>
        internal void Initialize(SocketSession<TSessionBehvaior> sesssion)
        {
            _session = sesssion;
        }

        public abstract void OnStarted();
        public abstract void OnClosed();
        public abstract void OnMessageReceive();
    }
}
