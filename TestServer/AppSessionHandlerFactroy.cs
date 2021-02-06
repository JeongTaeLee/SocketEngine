using SocketEngine.Servers.Handlers;

namespace TestServer
{
    class AppSessionHandlerFactroy : ISessionHandlerFactory
    {
        public BaseSessionHandler CreateSession() => new AppSessionHandler();
    }
}
