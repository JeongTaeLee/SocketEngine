using SocketEngine.Extensions;
using System.Net.Sockets;

namespace SocketEngine.Configs
{
    public class ServerConfig
    {
        #region Default Value
        public const int DefaultBacklog = 1000;
        
        public const ProtocolType DefaultProtocolType = ProtocolType.Tcp;
        public const SocketType DefaultSocketType = SocketType.Stream;

        public const int DefaultMaxConnection = 1000;
        public const int DefaultReceiveBufferSize = 1024;
        public const int DefaultSendBufferSize = 1024;
        public const int DefaultSendTimeOut = 5000;
        public const bool DefaultKeepAliveEnable = true;
        public const int DefaultKeepAliveTime = 600;
        public const int DefaultKeepAliveInterval = 60;
        #endregion

        public readonly string ip = string.Empty;
        public readonly int port = 0;
        public readonly int backlog = DefaultBacklog;
        public readonly ProtocolType protocolType = DefaultProtocolType;
        public readonly SocketType socketType = DefaultSocketType;

        public readonly int maxConnection = DefaultMaxConnection;
        public readonly int receiveBufferSize = DefaultReceiveBufferSize;
        public readonly int sendBufferSize = DefaultSendBufferSize;
        public readonly int sendTimeOut = DefaultSendTimeOut;

        public readonly bool keepAliveEnable = DefaultKeepAliveEnable;
        public readonly int keepAliveTime = DefaultKeepAliveTime;
        public readonly int keepAliveInterval = DefaultKeepAliveInterval;

        private ServerConfig(string ip,
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
            int keepAliveInterval)
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
            private int _backlog = DefaultBacklog;
            private SocketType _socketType = DefaultSocketType;
            private ProtocolType _protocolType = DefaultProtocolType;

            private int _maxConnection = DefaultMaxConnection;
            private int _receiveBufferSize = DefaultReceiveBufferSize;
            private int _sendBufferSize = DefaultSendBufferSize;
            private int _sendTimeOut = DefaultSendTimeOut;

            private bool _keepAliveEnable = DefaultKeepAliveEnable;
            private int _keepAliveTime = DefaultKeepAliveTime;
            private int _keepAliveInterval = DefaultKeepAliveInterval;

            public Builder(string ip, int port)
            {
                _ip = ip;
                _port = port;
            }

            public ServerConfig Build()
            {
                ExceptionExtension.ArgumentExceptionIfNullOrEmpty(_ip, "ip");
                ExceptionExtension.ArgumentExceptionIfLessThanOrEqualsToZero(_port, "port");
                ExceptionExtension.ArgumentExceptionIfLessThanOrEqualsToZero(_backlog, "backlog");

                ExceptionExtension.ArgumentExceptionIfTrue(_socketType == SocketType.Unknown, "socketType");
                ExceptionExtension.ArgumentExceptionIfTrue(_protocolType == ProtocolType.Unknown, "protocolType");
                
                ExceptionExtension.ArgumentExceptionIfLessThanOrEqualsToZero(_maxConnection, "maxConnection");
                ExceptionExtension.ArgumentExceptionIfLessThanOrEqualsToZero(_receiveBufferSize, "receiveBufferSize");
                ExceptionExtension.ArgumentExceptionIfLessThanOrEqualsToZero(_sendBufferSize, "sendBufferSize");
                ExceptionExtension.ArgumentExceptionIfLessThanOrEqualsToZero(_sendTimeOut, "sendTimeOut");

                ExceptionExtension.ArgumentExceptionIfLessThanOrEqualsToZero(_keepAliveTime, "keepAliveTime");
                ExceptionExtension.ArgumentExceptionIfLessThanOrEqualsToZero(_keepAliveInterval, "keepAliveInterval");


                return new ServerConfig(_ip, _port, _backlog, _protocolType, _socketType,
                    _maxConnection, _receiveBufferSize, _sendBufferSize, _sendTimeOut, _keepAliveEnable, _keepAliveTime, _keepAliveInterval);
            }

            public Builder SetBacklog(int backlog)
            {
                _backlog = backlog;
                return this;
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
