using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using SocketEngine.Logging;

namespace SocketEngine.Async
{
    class AsyncSocketListener
    {
        public delegate void ExceptionHandler(Exception ex);
        public delegate void AcceptHandler(Socket socket);


        public event ExceptionHandler throwedException;
        public event AcceptHandler accepted;

        private SocketServerConfig _config = null;

        private Socket _listenerSocket = null;
        private SocketAsyncEventArgs _accepSocketEvent = null;

        private AutoResetEvent _resetEvent = null;
        private Task _loopTask = null;

        private ILogger _logger = null;

        private bool _isRunning = false;

        private byte[] _keepAliveOptionValue = null;

        public AsyncSocketListener(Socket listenerSocket, SocketServerConfig config)
        {
            _config = config;
            _listenerSocket = listenerSocket;
            _logger = _config.loggerFactory.GetLogger<AsyncSocketListener>();

            uint dummy = 0;
            _keepAliveOptionValue = new byte[Marshal.SizeOf(dummy) * 3];
            int enable = config.keepAliveEnable ? 1 : 0;
            BitConverter.GetBytes((uint)enable).CopyTo(_keepAliveOptionValue, 0);
            BitConverter.GetBytes((uint)(config.keepAliveTime * 1000)).CopyTo(_keepAliveOptionValue, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)(config.keepAliveInterval * 1000)).CopyTo(_keepAliveOptionValue, Marshal.SizeOf(dummy) * 2);
        }

        public bool Start()
        {
            _accepSocketEvent = new SocketAsyncEventArgs();
            _accepSocketEvent.Completed += ProcessAccept;

            _resetEvent = new AutoResetEvent(false);

            _isRunning = true;

            _loopTask = Task.Run(LoopAccept);

            return true;
        }

        public void Close()
        {

        }

        private void LoopAccept()
        {
            _logger.Info($"Start listening - Ip({_config.ip}), Port({_config.port}), Backlog({_config.backlog}), SocketType({_config.socketType}), ProtocolType({_config.protocolType})");

            while (_isRunning)
            {
                try
                {
                    _accepSocketEvent.AcceptSocket = null;

                    bool pending = _listenerSocket.AcceptAsync(_accepSocketEvent);
                    if (!pending)
                        ProcessAccept(null, _accepSocketEvent);

                    _resetEvent?.WaitOne();
                }
                catch (Exception ex)
                {
                    throwedException?.Invoke(ex);
                }
            }

            _logger.Info("End listening");
        }

        private void ProcessAccept(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                _logger.Error($"Socket error occurred during accept - SocketError({e.SocketError})");
                return;
            }

            Socket newSocket = args.AcceptSocket;

            // 소켓 설정.
            if (_config.sendTimeOut > 0)
                newSocket.SendTimeout = _config.sendTimeOut;

            if (_config.sendBufferSize > 0)
                newSocket.SendBufferSize = _config.sendBufferSize;

            if (_config.receiveBufferSize > 0)
                newSocket.ReceiveBufferSize = _config.receiveBufferSize;

            // TODO : 지원 여부 알아보기 (IOControl와 함께)
            newSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, _keepAliveOptionValue);
            newSocket.NoDelay = true; // Nagle() 알고리즘 적용 여부(TRUE : 적용하지 않음). 패킷을 모아서 보낼 것인지 설정하는 옵션 즉각적인 응답이 중요한 곳에서는 사용하지 않는다)
            newSocket.LingerState = new LingerOption(true, 0); // 두번째 인자가 0이면 close 호출 시 버퍼에 남아있는 모든 송수신 데이터를 버리고 즉시 종료한다.

            accepted?.Invoke(newSocket);

            _resetEvent?.Set();
        }
    }
}
