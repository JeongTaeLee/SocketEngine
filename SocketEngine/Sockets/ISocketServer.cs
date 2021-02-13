using SocketEngine.Logging;

namespace SocketEngine.Sockets
{
    public interface ISocketServer : ILoggerProvider
    {
        void Start();
        void Close();
    }
}
