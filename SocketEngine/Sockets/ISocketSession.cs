using System;
using System.Net.Sockets;

namespace SocketEngine.Sockets
{

    /// <summary>
    /// 해당 소켓의 종료 이유.
    /// </summary>
    public enum CloseReason
    {
    }

    public interface ISocketSession
    {
        public delegate void ExceptionHandler(Exception ex);
        public delegate void CloseHandler(ISocketSession session);

        CloseHandler close { get; }

        void Start();
        void Close();

    }
}
