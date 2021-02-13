using System;
using System.Net.Sockets;
using SocketEngine.Configs;

namespace SocketEngine.Sockets
{
    abstract class SocketListener : ISocketListener
    {
        protected Socket _listenerSocket = null;

        public ListenerInfo info { get; set; } = null;
        public ISocketListener.ErrorHandler error { get; set; } = null;
        public ISocketListener.AcceptHandler accepted { get; set; } = null;

        public SocketListener(ListenerInfo listenerInfo)
        {
            if (listenerInfo == null) throw new ArgumentNullException(nameof(listenerInfo));
            this.info = listenerInfo;
        }

        public abstract void Start();
        public abstract void Stop();
    }
}
