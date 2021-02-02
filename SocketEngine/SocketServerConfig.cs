using System.Net.Sockets;

namespace SocketEngine
{
    public class SocketServerConfig
    {
        public readonly string ip = string.Empty;
        public readonly int port = 0;
        public readonly int backlog = 10;
        public readonly ProtocolType protocolType = ProtocolType.Tcp;
        public readonly SocketType socketType = SocketType.Stream;

        public readonly int maxConnection = 100;
        public readonly int receiveBufferSize = 1024;
        public readonly int sendBufferSize = 1024;
        public readonly int sendTimeOut = 5000;

        public readonly bool keepAliveEnable = true;
        public readonly int keepAliveTime = 600;
        public readonly int keepAliveInterval = 60;

        public readonly SocketEngine.Logging.ILoggerFactory loggerFactory = null;

        private SocketServerConfig(string ip,
            int port,
            int backlog,
            ProtocolType protocolType,
            SocketType socketType,
            int maxConnection,
            int receiveBufferSize,
            int sendBufferSize,
            int sendTimeOut,
            bool keepAliveEnable,
            int keepAliveTime,
            int keepAliveInterval,
            SocketEngine.Logging.ILoggerFactory loggerFactory)
        {
            this.ip = ip;
            this.port = port;
            this.backlog = backlog;
            this.protocolType = protocolType;
            this.socketType = socketType;
            this.maxConnection = maxConnection;
            this.receiveBufferSize = receiveBufferSize;
            this.sendBufferSize = sendBufferSize;
            this.sendTimeOut = sendTimeOut;
            this.keepAliveEnable = keepAliveEnable;
            this.keepAliveTime = keepAliveTime;
            this.keepAliveInterval = keepAliveInterval;
        }


        public class Builder
        {
            private string _ip = string.Empty;
            private int _port = 0;
            private int _backlog = 10;
            private SocketType _socketType = SocketType.Stream;
            private ProtocolType _protocolType = ProtocolType.Tcp;

            private int _maxConnection = 100;
            private int _receiveBufferSize = 1024;
            private int _sendBufferSize = 1024;
            private int _sendTimeOut = 5000;

            private bool _keepAliveEnable = true;
            private int _keepAliveTime = 600;
            private int _keepAliveInterval = 60;

            private Logging.ILoggerFactory _loggerFactory = null;

            public Builder(string ip, int port, int backlog)
            {
                _ip = ip;
                _port = port;
                _backlog = backlog;
            }

            public SocketServerConfig Builde()
            {
                return new SocketServerConfig(_ip, _port, _backlog, _protocolType, _socketType,
                    _maxConnection, _receiveBufferSize, _sendBufferSize, _sendTimeOut, _keepAliveEnable, _keepAliveTime, _keepAliveInterval, _loggerFactory);
            }

            public Builder SetSocketType(SocketType socketType)
            {
                _socketType = socketType;
                return this;
            }

            public Builder SetProtocolType(ProtocolType protocolType)
            {
                _protocolType = protocolType;
                return this;
            }            

            public Builder SetMaxConnection(int maxConnection)
            {
                _maxConnection = maxConnection;
                return this;
            }

            public Builder SetReceiveBufferSize(int receiveBufferSize)
            {
                _receiveBufferSize = receiveBufferSize;
                return this;
            }

            public Builder SetSendBufferSize(int sendBufferSize)
            {
                _sendBufferSize = sendBufferSize;
                return this;
            }

            public Builder SetSendTimeOut(int sendTimeOut)
            {
                _sendTimeOut = sendTimeOut;
                return this;
            }

            public Builder SetKeepAliveEnable(bool keepAliveEnable)
            {
                _keepAliveEnable = keepAliveEnable;
                return this;
            }

            public Builder SetKeepAliveTime(int keepAliveTime)
            {
                _keepAliveTime = keepAliveTime;
                return this;
            }

            public Builder SetKeepValieInterval(int keepAliveInterval)
            {
                _keepAliveInterval = keepAliveInterval;
                return this;
            }
        }
    }
}
