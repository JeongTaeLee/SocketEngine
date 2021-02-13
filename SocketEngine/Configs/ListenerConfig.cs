namespace SocketEngine.Configs
{
    public class ListenerConfig : IListenerConfig
    {
        public string ip { get; set; }
        public int port { get; set; }
        public int backlog { get; set; }
    }
}
