using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketEngine.Protocols
{
    interface IReceiveFilterFactory
    {
        IReceiveFilter CreateReceiveFilter();
    }
}
