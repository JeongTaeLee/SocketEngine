using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SocketEngine.Logging;

namespace SocketEngine.Async
{
    class AsyncListener
    {
        private byte[] _keepAliveOptionValue = null;

        public delegate void ExceptionHandler(Exception ex);
        public delegate void SocketAcceptedHandler(Socket socket);
        
        private ServerConfig serverConfig = null;
        private ILogger _logger = null;

        private Socket listenerSocket = null;
        private SocketAsyncEventArgs _acceptEventArg = null;

        private bool _isRunning = false;

        private AutoResetEvent _resetEvent = null;
        private Task _loopTask = null;

        public event ExceptionHandler throwedException;
        public event SocketAcceptedHandler accepted;

        public void Start(ServerConfig listenerOption)
        {
            this.serverConfig = listenerOption;
            _logger = this.serverConfig.loggerFactory.GetLogger<AsyncListener>();

            uint dummy = 0;
            _keepAliveOptionValue = new byte[Marshal.SizeOf(dummy) * 3];
            int enable = serverConfig.keepAliveEnable ? 1 : 0;
            BitConverter.GetBytes((uint)enable).CopyTo(_keepAliveOptionValue, 0);
            BitConverter.GetBytes((uint)(serverConfig.keepAliveTime * 1000)).CopyTo(_keepAliveOptionValue, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)(serverConfig.keepAliveInterval * 1000)).CopyTo(_keepAliveOptionValue, Marshal.SizeOf(dummy) * 2);

            // 끝점 설정
            IPHostEntry ipHostInfo = Dns.GetHostEntry(this.serverConfig.ip);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, this.serverConfig.port);

            // 소켓 생성.
            listenerSocket = new Socket(ipAddress.AddressFamily, this.serverConfig.socketType, this.serverConfig.protocolType);
            listenerSocket.Bind(localEndPoint);
            listenerSocket.Listen(this.serverConfig.backlog);

            _acceptEventArg = new SocketAsyncEventArgs();
            _acceptEventArg.Completed += ProcessAccept;

            _resetEvent = new AutoResetEvent(false);

            _isRunning = true;

            _loopTask = Task.Run(LoopAccept);
        }

        public void Close()
        {
            _isRunning = false;

            _loopTask?.Wait();
            _loopTask = null;

            listenerSocket?.Close();
            listenerSocket?.Dispose();
            _acceptEventArg?.Dispose();
            _resetEvent?.Dispose();

            listenerSocket = null;
            _acceptEventArg = null;
            _resetEvent = null;
        }


        private void LoopAccept()
        {
            _logger.Info($"Start listening - Ip({serverConfig.ip}), Port({serverConfig.port}), Backlog({serverConfig.backlog}), SocketType({serverConfig.socketType}), ProtocolType({serverConfig.protocolType})");

            while (_isRunning)
            {
                try
                {
                    _acceptEventArg.AcceptSocket = null;

                    bool pending = listenerSocket.AcceptAsync(_acceptEventArg);
                    if (!pending)
                        ProcessAccept(null, _acceptEventArg);

                    _resetEvent?.WaitOne();
                }
                catch (Exception ex)
                {
                    throwedException?.Invoke(ex);
                }
            }

            _logger.Info("End listening");
        }

        private void ProcessAccept(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                _logger.Error($"Socket error occurred during accept - SocketError({e.SocketError})");
                return;
            }

            Socket newSocket = e.AcceptSocket;

            // 소켓 설정.
            if (serverConfig.sendTimeOut > 0)
                newSocket.SendTimeout = serverConfig.sendTimeOut;

            if (serverConfig.sendBufferSize > 0)
                newSocket.SendBufferSize = serverConfig.sendBufferSize;

            if (serverConfig.receiveBufferSize > 0)
                newSocket.ReceiveBufferSize = serverConfig.receiveBufferSize;

            // TODO : 지원 여부 알아보기 (IOControl와 함께)
            newSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, _keepAliveOptionValue);
            newSocket.NoDelay = true; // Nagle() 알고리즘 적용 여부(TRUE : 적용하지 않음). 패킷을 모아서 보낼 것인지 설정하는 옵션 즉각적인 응답이 중요한 곳에서는 사용하지 않는다)
            newSocket.LingerState = new LingerOption(true, 0); // 두번째 인자가 0이면 close 호출 시 버퍼에 남아있는 모든 송수신 데이터를 버리고 즉시 종료한다.

            accepted?.Invoke(newSocket);

            _resetEvent?.Set();
        }
    }
}
