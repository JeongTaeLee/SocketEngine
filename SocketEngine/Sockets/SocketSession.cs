using System;
using System.Net.Sockets;
using System.Threading;
using SocketEngine.Extensions;

namespace SocketEngine.Sockets
{
    abstract class SocketSession : ISocketSession
    {
        /// <summary>
        /// 소켓의 상태를 나타내는 Flag
        /// </summary>
        [Flags]
        enum SocketState
        {
            None = 0, // 아무것도 하지 않음.
            Sending = 1 << 1, // 보내즌중
            Receiving = 1 << 2, // 받는중
            Closing = 1 << 3, // 종료 중 
            Closed = 1 << 4, // 완전 종료

            ALL = int.MaxValue
        };


        private int _state = (int)SocketState.None;
        
        protected Socket socket { get; private set; } = null;

        protected IAppSession appSession { get; private set; } = null;

        public ISocketSession.CloseHandler close { get; set; }
        
        public bool Initialize(IAppSession appSession, Socket socket)
        {
            if (appSession == null) throw new ArgumentNullException(nameof(appSession));
            if (socket == null) throw new ArgumentNullException(nameof(socket));

            this.appSession = appSession;
            this.socket = socket;

            return true;

        }

        /// <summary>
        /// 세션을 시작합니다.
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 세션을 종료합니다.
        /// </summary>
        public virtual void Close()
        {
            // TODO @jeongtae.lee : 종료 처리 추가.
            // 이미 종료 처리 중이라면 종료하지 않는다.
            if (!TryAddStateFlag(SocketState.Closing))
            {
                return;
            }

            // 소켓이 없다면 종료 처리를 하지 않는다.
            if (socket == null)
                return;

            // 현재 전송 처리 중이라면 모두 보낸 후 종료 처리합니다.
            if (ContaintsState(SocketState.Sending))
            {
                
            }

            InternalClose();
        }

        /// <summary>
        /// 세션이 시작되었음을 알리는 콜백을 호출합니다.
        /// </summary>
        protected void StartSession()
        {
            appSession.OnSessionStarted();
        }

        /// <summary>
        /// 소켓을 종료 처리합니다.
        /// </summary>
        private void InternalClose()
        {

        }

        /// <summary>
        /// 매개 변수로 전달된 상태가 추가되지 않았다면 상태를 추가합니다.
        /// </summary>
        /// <param name="state">추가할 상태</param>
        /// <returns> True : 성공적으로 추가됨, False : 이미 추가된 상태여서 실패</returns>
        private bool TryAddStateFlag(SocketState state)
        {
            var intState = (int)state;

            while (true)
            {
                var oldState = _state;
                var newState = _state | intState;

                if (oldState == newState)
                {
                    return false;
                }

                if (Interlocked.CompareExchange(ref _state, newState, oldState) == oldState)
                    return true;
            }
        }

        /// <summary>
        /// 상태를 추가합니다.
        /// </summary>
        /// <param name="state">추가할 상태.</param>
        /// <returns>True : 성공적으로 추가됨 False : 실패</returns>
        private bool AddState(SocketState state)
        {
            var intState = (int)state;

            while (true)
            {
                var oldState = _state;
                var newState = _state | intState;

                if (Interlocked.CompareExchange(ref _state, newState, oldState) == oldState)
                {
                    return true;
                }
            }    
        }

        /// <summary>
        /// 상태를 제거합니다.
        /// </summary>
        /// <param name="state">제거할 상태.</param>
        /// <returns>True : 성공적으로 제거됨 False : 실패</returns>
        private bool RemoveState(SocketState state)
        {
            var intState = (int)state;

            while (true)
            {
                var oldState = _state;
                var newState = _state & (~intState);

                if (Interlocked.CompareExchange(ref _state, newState, oldState) == oldState)
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// 인자로 전달된 상태가 추가되었는지 확인합니다.
        /// </summary>
        /// <param name="state">검사할 상태.</param>
        /// <returns>True : 이미 추가됨 False : 추가되지 않음</returns>
        private bool ContaintsState(SocketState state)
        {
            return (_state & (int)state) == (int)state;
        }
    }
}
