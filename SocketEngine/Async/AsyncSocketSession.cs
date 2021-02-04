using System;
using System.Net.Sockets;
using SocketEngine.Bases;
using SocketEngine.Extensions;
using SocketEngine.Protocols;

namespace SocketEngine.Async
{
    internal sealed class AsyncSocketSession<TSessionBehavior, TRequestInfo> : SocketSession<TSessionBehavior, TRequestInfo>, ISocketAsyncEventArgsProxyHandler
        where TSessionBehavior : SocketSessionBehavior<TSessionBehavior, TRequestInfo>, new()
        where TRequestInfo : IRequestInfo
    {
        public SocketAsyncEventArgsProxy socketEventProxy { get; private set; } = null;

        public AsyncSocketSession(SocketAsyncEventArgsProxy socketEventProxy)
        {
            ExceptionExtension.ArgumentNullExceptionIfNull(socketEventProxy, "socketEventProxy");
            
            this.socketEventProxy = socketEventProxy;
            this.socketEventProxy.Initialize(this);
        }

        public override bool Start()
        {
            StartReceive(socketEventProxy.socketEventArgs);
            return true;
        }

        public override void Close()
        {
            socketEventProxy?.Reset();
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


            }
        }

        public void ProcessReceive(SocketAsyncEventArgs args)
        {
            
        }
    }
}
