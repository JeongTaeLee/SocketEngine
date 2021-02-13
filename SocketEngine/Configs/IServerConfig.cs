using System.Collections.Generic;
using SocketEngine.Sockets;

namespace SocketEngine.Configs
{
    public interface IServerConfig
    {
        public SocketMode socketMode { get; }

        public int maxConnection { get; }
        public int receiveBufferSize { get; }
        public int sendBufferSize { get; }
        public int sendTimeOut { get; }

        public bool keepAliveEnable { get; }
        public int keepAliveTime { get; }
        public int keepAliveInterval { get; }

        IReadOnlyList<IListenerConfig> listenerConfigs { get; }
    }
}
