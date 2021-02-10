namespace SocketEngine.Protocols
{
    class DefaultReceiveFilterFactory<TReceiveFilter> : IReceiveFilterFactory
        where TReceiveFilter : IReceiveFilter, new()
    {
        public IReceiveFilter CreateReceiveFilter()
        {
            return new TReceiveFilter();
        }
    }
}
