using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using SocketEngine.Extensions;
using SocketEngine.Logging;
using SocketEngine.Configs;

namespace SocketEngine.AsyncSockets
{
    internal class AsyncSocketListener
    {

        public delegate void ExceptionHandler(Exception ex);
        public delegate void AcceptHandler(Socket socket);

        private Socket _listenerSocket = null;
        private SocketAsyncEventArgs _accepSocketEvent = null;

        private AutoResetEvent _resetEvent = new AutoResetEvent(false);
        private Task _loopTask = null;

        private ILoggerProvider _loggerProvider = null;

        private bool _isRunning = false;

        public ExceptionHandler throwedException { get; set; } = null;
        public AcceptHandler accepted { get; set; } = null;

        public AsyncSocketListener(Socket listenerSocket, ILoggerProvider loggerProvider)
        {
            if (listenerSocket == null) throw new ArgumentNullException(nameof(listenerSocket));
            if (loggerProvider == null) throw new ArgumentNullException(nameof(loggerProvider));

            _listenerSocket = listenerSocket;
            _loggerProvider = loggerProvider;
        }

        public void Start()
        {
            if (_listenerSocket == null) throw new Exception("Listener socket is not set");
            if (_loggerProvider == null) throw new Exception("Logger provider is not set");

            _accepSocketEvent = new SocketAsyncEventArgs();
            _accepSocketEvent.Completed += ProcessAccept;

            _isRunning = true;

            _loopTask = Task.Run(LoopAccept);
        }

        public void Close()
        {
            _isRunning = false;

            _resetEvent?.Dispose();
            _resetEvent = null;

            _accepSocketEvent?.Dispose();
            _accepSocketEvent = null;

            _loopTask?.Wait();
            _loopTask = null;
        }

        private void LoopAccept()
        {
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
                    _resetEvent?.Set();
                    throwedException?.Invoke(ex);
                }
            }
        }

        private void ProcessAccept(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError != SocketError.Success)
            {
                _loggerProvider.logger.Error($"Socket error occurred during accept - SocketError({args.SocketError})");
                return;
            }

            Socket newSocket = args.AcceptSocket;
            if (newSocket == null)
            {
                _loggerProvider.logger.Error($"AcceptSocket is null");
                return;
            }


            accepted?.Invoke(newSocket);

            _resetEvent?.Set();
        }
    }
}
