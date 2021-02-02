using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SocketEngine.Bases;

namespace SocketEngine.Async
{
    public class AsyncSocketServer : SocketServer
    {
        private AsyncSocketListener _listener = null;
        
        public AsyncSocketServer(SocketServerConfig config)
            : base(config)
        {
        }

        public override void Start()
        {
            // 리스너 소켓 생성.
            IPHostEntry ipHostInfo = Dns.GetHostEntry(base.config.ip);
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, base.config.port);

            base.socket = new Socket(ipAddress.AddressFamily, base.config.socketType, base.config.protocolType);
            base.socket.Bind(localEndPoint);
            base.socket.Listen(base.config.backlog);

            _listener = new AsyncSocketListener(base.socket, config);
            _listener.Start();
        }

        public override void Close()
        {
            _listener.Close();
        }
    }
}
