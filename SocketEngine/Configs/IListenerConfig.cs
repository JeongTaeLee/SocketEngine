
namespace SocketEngine.Configs
{
    public interface IListenerConfig
    {
        public string ip { get; }
        public int port { get; }
        public int backlog { get; }
    }
}
