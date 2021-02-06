namespace SocketEngine.Servers.Handlers
{
    public interface ISessionHandlerFactory
    {
        BaseSessionHandler CreateSession();
    }
}
