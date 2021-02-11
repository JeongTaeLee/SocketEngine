using System;
using System.Net.Sockets;

namespace SocketEngine.Sockets
{
    public interface ISocketSession
    {
        public delegate void ExceptionHandler(Exception ex);
        public delegate void CloseHandler(ISocketSession session);

        IAppSession appSession { get; }

        Socket socket { get; }

        CloseHandler close { get; }

        void Start();
        void Close();

    }
}
