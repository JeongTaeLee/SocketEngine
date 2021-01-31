namespace SocketEngine.Logging
{
    public interface ILoggerFactory
    {
        public ILogger GetLogger(string name);
        public ILogger GetLogger<TType>();
    }
}
