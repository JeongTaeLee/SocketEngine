using System;
using System.Net.Sockets;
using SocketEngine.Extensions;

namespace SocketEngine.Sockets.Asyncs
{
    class AsyncSocketSession : SocketSession
    {
        private SocketAsyncEventArgs recvEventArgs = null;
        private SocketAsyncEventArgs sendEventArgs = null;

        public AsyncSocketSession(SocketAsyncEventArgs recvEventArgs)
        {
            if (recvEventArgs == null) throw new ArgumentNullException(nameof(recvEventArgs));
            
            this.recvEventArgs = recvEventArgs;
        }

        public override bool Start()
        {
            StartRecv(recvEventArgs);
            return true;
        }

        public override void End()
        {

        }

        private void StartRecv(SocketAsyncEventArgs args)
        {
            try
            {            
                bool pending = socket.ReceiveAsync(args);
                if (!pending)
                    ProcessRecv(args);
            }
            catch (Exception ex)
            {
                exceptionHandler?.Invoke(ex);
            }
        }

        private void CompleteRecv(object sender, SocketAsyncEventArgs args)
        {
            if (args.LastOperation == SocketAsyncOperation.Receive)
                ProcessRecv(args);
        }

        private void ProcessRecv(SocketAsyncEventArgs args)
        {
            if (0 < args.BytesTransferred && args.SocketError == SocketError.Success)
            {

            }
            else
            {
                this.End();
            }
        }
    }
}
