using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using SocketEngine.Extensions;
using SocketEngine.Logging;

namespace SocketEngine.Sockets
{
    abstract class SocketServer<TSocketServer> : ISocketServer
        where TSocketServer : SocketServer<TSocketServer>
    {
        protected IAppServer appServer { get; private set; } = null;
        protected Socket socket { get; private set; } = null;
        protected IReadOnlyList<ISocketListener> listeners { get; private set; } = null;

        public ILogger logger { get; private set; } = null;
        

        public SocketServer(IAppServer appServer)
        {
            if (appServer == null) throw new ArgumentNullException(nameof(appServer));

            this.appServer = appServer;
        }

        public virtual void Start()
        {
            StartListeners();
        }

        public virtual void Close()
        {
            this.socket.SafeClose();
            this.socket = null;
        }

        private void StartListeners()
        {
            List<ISocketListener> listeners = new List<ISocketListener>();

            var config = appServer.config;

            if (config.listenerConfigs == null || 0 >= config.listenerConfigs.Count)
            {
                throw new Exception("Listener list wrong");
            }
            
            for (int index = 0; index < config.listenerConfigs.Count; ++index)
            {
                var listenerConfig = config.listenerConfigs[index];
                if (listenerConfig == null)
                {
                    throw new Exception($"Listener({index}) is null");
                }

                if (string.IsNullOrEmpty(listenerConfig.ip))
                {
                    throw new Exception($"Listener({index}) ip is invalid");
                }

                if (0 >= listenerConfig.port)
                {
                    throw new Exception($"Listener({index}) port is invalid");
                }

                var endPoint = new IPEndPoint(SocketExtensions.ParseIPAddress(listenerConfig.ip), listenerConfig.port);

                var listenerInfo = new ListenerInfo(endPoint, listenerConfig.backlog);
                
                var socketListener = CreateListener(listenerInfo);
                socketListener.accepted += new ISocketListener.AcceptHandler(OnSocketAccepted);
                socketListener.error += new ISocketListener.ErrorHandler(OnListenerError);

                socketListener.Start();
            }

            this.listeners = listeners;
        }

        protected abstract void OnSocketAccepted(ISocketListener listener, Socket socket);
        protected abstract void OnListenerError(ISocketListener listener, Exception ex);

        protected abstract ISocketListener CreateListener(ListenerInfo info);
    }
}
