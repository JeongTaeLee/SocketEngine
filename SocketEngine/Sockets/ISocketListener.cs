using System;
using System.Net.Sockets;
using SocketEngine.Configs;

namespace SocketEngine.Sockets
{
    interface ISocketListener
    {
        public delegate void ErrorHandler(ISocketListener listener, Exception ex);
        public delegate void AcceptHandler(ISocketListener listener, Socket socket);

        public ListenerInfo info { get; }

        public event ErrorHandler error;
        public event AcceptHandler accepted;

        public abstract void Start();
        public abstract void Stop();
    }
}
