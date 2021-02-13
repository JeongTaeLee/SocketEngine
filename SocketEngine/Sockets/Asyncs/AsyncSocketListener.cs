using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SocketEngine.Sockets.Asyncs
{
    class AsyncSocketListener : SocketListener
    {
        private SocketAsyncEventArgs _accepSocketEvent = null;

        private AutoResetEvent _resetEvent = new AutoResetEvent(false);
        private Task _loopTask = null;

        private bool _isRunning = false;

        public AsyncSocketListener(ListenerInfo listenerInfo)
            :base(listenerInfo)
        {
        }

        public override void Start()
        {
            _listenerSocket = new Socket(this.info.endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listenerSocket.Bind(this.info.endPoint);
                _listenerSocket.Listen(this.info.backlog);

                _accepSocketEvent = new SocketAsyncEventArgs();
                _accepSocketEvent.Completed += ProcessAccept;

                _isRunning = true;

                _loopTask = Task.Run(LoopAccept);
            }
            catch (Exception ex)
            {
                error?.Invoke(this, ex);
                throw;
            }
        }

        public override void Stop()
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
                    error?.Invoke(this, ex);
                }
            }
        }

        private void ProcessAccept(object sender, SocketAsyncEventArgs args)
        {
            Socket newSocket = null;
            if (args.SocketError != SocketError.Success)
            {
                var errorCode = (int)args.SocketError;

                //The listen socket was closed
                if (errorCode == 995 || errorCode == 10004 || errorCode == 10038)
                    return;

                error?.Invoke(this,new SocketException(errorCode));
            }
            else
            {
                newSocket = args.AcceptSocket;
            }

            args.AcceptSocket = null;

            if (newSocket != null)
                accepted?.Invoke(this, newSocket);

            _resetEvent?.Set();
        }
    }
}
