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
            this.recvEventArgs.UserToken = this;
        }

        public override void Start()
        {
            StartRecv(recvEventArgs);

            StartSession();
        }

        public override void Close()
        {
            base.Close();
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
                appSession.HandleException(ex);
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
               
            }
        }
    }
}
