using System;

namespace SocketEngine.Protocols
{
    interface IReceiveFilter
    {
        IRequestInfo Filter(ArraySegment<byte> segment);
    }
}
