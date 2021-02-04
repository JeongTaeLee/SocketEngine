using System.Net.Sockets;

namespace SocketEngine.Async
{
    internal class SocketAsyncEventArgsProxy
    {
        public SocketAsyncEventArgs socketEventArgs { get; private set; }
        public int originOffset { get; private set; }
        public int originCount { get; private set; }

        public SocketAsyncEventArgsProxy(SocketAsyncEventArgs args)
        {
            socketEventArgs = args;
            socketEventArgs.Completed += AsyncEventArgs_Completed;

            originOffset = socketEventArgs.Offset;
            originCount = socketEventArgs.Count;
        }

        public void Initialize(ISocketAsyncEventArgsProxyHandler processor)
        {
            socketEventArgs.UserToken = processor;
        }

        public void Reset()
        {
            socketEventArgs.UserToken = null;
        }

        private void AsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            var processor = socketEventArgs.UserToken as ISocketAsyncEventArgsProxyHandler;
            
        }
    }
}
