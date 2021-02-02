using System.Net.Sockets;

namespace SocketEngine.Bases
{
    /// <summary>
    /// 해당 클래스는 SocketEngine을 사용하는 프로젝트에서는 공개되지 않고,
    /// 대신 SocketSessionBehavior라는 클래스를 상속받아서 해당 클래스의 동작을 정의한다.
    /// </summary>
    /// <typeparam name="TSessionBehavior"></typeparam>
    abstract class SocketSession<TSessionBehavior>
        where TSessionBehavior : SocketSessionBehavior<TSessionBehavior>, new()
    {
        public Socket socket;

        public TSessionBehavior behavior;

        public SocketSession()
        {
            behavior = new TSessionBehavior();
            behavior.Initialize(this);
        }

        public abstract void Start();
        public abstract void Close();
    }
}
