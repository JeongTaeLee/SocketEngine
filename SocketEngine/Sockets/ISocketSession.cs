using System;
using System.Net.Sockets;

namespace SocketEngine.Sockets
{
    interface ISocketSession
    {
        public delegate void ExceptionHandler(Exception ex);
        public delegate void CloseHandler(ISocketSession session);

        IAppSession appSession { get; }

        Socket socket { get; }

        ExceptionHandler exceptionHandler { get; }

        bool Start();
        void End();

    }
}
