using System;
using System.Net.Sockets;
using SocketEngine.AsyncSockets;
using SocketEngine.Extensions;
using SocketEngine.Servers.Handlers;

namespace SocketEngine.Servers.Asyncs
{
    internal class AsyncSocketSession : BaseSocketSession, ISocketAsyncEventArgsProxyHandler
    {
        public SocketAsyncEventArgsProxy SocketEventProxy { get; private set; } = null;

        public AsyncSocketSession(string sessionId, 
            BaseServer server, 
            BaseSessionHandler sessionHandler, 
            ClosedHandler closed, 
            Socket socket, 
            SocketAsyncEventArgsProxy socketEventProxy)
            : base(sessionId, server, sessionHandler, closed, socket)
        {
            ExceptionExtension.ArgumentNullExceptionIfNull(socketEventProxy, "SocketAsyncEventArgsProxy");

            this.SocketEventProxy = socketEventProxy;
            this.SocketEventProxy.Initialize(this);
        }

        public override bool Start()
        {
            handler.Initialize(this);
            handler.OnStart();

            StartReceive(SocketEventProxy.socketEventArgs);
            return true;
        }

        public override void Close()
        {
            handler.OnClose();
            handler.Reset();

            SocketEventProxy?.Reset();
            closed?.Invoke(this);
        }

        public void StartReceive(SocketAsyncEventArgs args)
        {
            try
            {
                bool pending = socket.ReceiveAsync(args);
                if (!pending)
                {
                    ProcessReceive(args);
                }
            }
            catch (Exception ex)
            {
                if (logger.IsErrorEnabled)
                    logger.Error(ex);

                handler.OnThrowedException(ex);
            }
        }

        public void ProcessReceive(SocketAsyncEventArgs args)
        {
            if (args.LastOperation != SocketAsyncOperation.Receive)
                return;

            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {

            }
            else
            {

            }
        }
    }
}
