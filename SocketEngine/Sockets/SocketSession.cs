using System;
using System.Net.Sockets;
using SocketEngine.Extensions;

namespace SocketEngine.Sockets
{
    abstract class SocketSession : ISocketSession
    {
        public IAppSession appSession { get; private set; }

        public Socket socket { get; private set; } = null;

        public ISocketSession.ExceptionHandler exceptionHandler { get; set; }

        public bool Initialize(IAppSession appSession, Socket socket)
        {
            if (appSession == null) throw new ArgumentNullException(nameof(appSession));
            if (socket == null) throw new ArgumentNullException(nameof(socket));

            this.appSession = appSession;
            this.socket = socket;

            return true;
        }

        public abstract bool Start();
        public abstract void End();
    }
}
