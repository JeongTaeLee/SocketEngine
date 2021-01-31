using System.Net.Sockets;

namespace SocketEngine
{
    public class ServerConfig
    {
        public string ip { get; set; } = string.Empty;
        public int port { get; set; } = 0;
        public ProtocolType protocolType { get; set; } = ProtocolType.Tcp;
        public SocketType socketType { get; set; } = SocketType.Stream;
        public SocketEngine.Logging.ILoggerFactory loggerFactory { get; set; } = null;

        public int backlog { get; set; } = 10;
        public int maxConnection { get; set; } = 100;
        public int receiveBufferSize { get; set; } = 1024;
        public int sendBufferSize { get; set; } = 1024;
        public int sendTimeOut { get; set; } = 5000;

        public bool keepAliveEnable { get; set; } = true;
        public int keepAliveTime { get; set; } = 600;
        public int keepAliveInterval { get; set; } = 60;

    }
}
