using SocketEngine.Sockets;
using System;
using System.Collections.Generic;

namespace SocketEngine.Configs
{
    public class ServerConfig : IServerConfig
    {
        #region Default Value
        public const SocketMode DefaultSocketMode = SocketMode.Tcp;
        public const int DefaultBacklog = 1000;
        public const int DefaultMaxConnection = 1000;
        public const int DefaultReceiveBufferSize = 1024;
        public const int DefaultSendBufferSize = 1024;
        public const int DefaultSendTimeOut = 5000;
        public const bool DefaultKeepAliveEnable = true;
        public const int DefaultKeepAliveTime = 600;
        public const int DefaultKeepAliveInterval = 60;
        #endregion

        public SocketMode socketMode { get; set; } = DefaultSocketMode;

        public int maxConnection { get; set; } = DefaultMaxConnection;
        public int receiveBufferSize { get; set; } = DefaultReceiveBufferSize;
        public int sendBufferSize { get; set; } = DefaultSendBufferSize;
        public int sendTimeOut { get; set; } = DefaultSendTimeOut;

        public bool keepAliveEnable { get; set; } = DefaultKeepAliveEnable;
        public int keepAliveTime { get; set; } = DefaultKeepAliveTime;
        public int keepAliveInterval { get; set; } = DefaultKeepAliveInterval;

        private List<IListenerConfig> _listenerConfig = new List<IListenerConfig>();
        public IReadOnlyList<IListenerConfig> listenerConfigs => _listenerConfig;

        public void AddListenerConfig(IListenerConfig listenerConfig)
        {
            if (listenerConfig == null) throw new ArgumentNullException("listenerConfig");
            _listenerConfig.Add(listenerConfig);
        }
    }
}
