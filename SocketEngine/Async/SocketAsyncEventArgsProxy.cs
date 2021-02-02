using System.Net.Sockets;

namespace SocketEngine.Async
{
    public class SocketAsyncEventArgsProxy
    {
        public SocketAsyncEventArgs asyncEventArgs { get; private set; }
        public int originOffset { get; private set; }
        public int originCount { get; private set; }

        public SocketAsyncEventArgsProxy(SocketAsyncEventArgs args)
        {
            asyncEventArgs = args;
            asyncEventArgs.Completed += AsyncEventArgs_Completed;

            originOffset = asyncEventArgs.Offset;
            originCount = asyncEventArgs.Count;
        }

        public void Initialize(_IAsyncSocketSession processor)
        {
            asyncEventArgs.UserToken = processor;
        }

        public void Reset()
        {
            asyncEventArgs.UserToken = null;
        }

        private void AsyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
        {
            var processor = asyncEventArgs.UserToken as _IAsyncSocketSession;
            
        }
    }
}
