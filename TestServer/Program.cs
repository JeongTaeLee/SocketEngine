using NLog;
using SocketEngine.Logging;
using SocketEngine.Servers.Asyncs;
using SocketEngine.Servers.Configs;
using System;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            NLogLoggerFactory logFactroy = null;

#if DEBUG
            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("./NLog.config");
            logFactroy = new NLogLoggerFactory("./NLog.config");
#else
            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("./NLog.config");
            logFactroy = new NLogLoggerFactory("./NLog.config");
#endif

            ServerConfig config = new ServerConfig.Builder("127.0.0.1", 9199)
                .SetMaxConnection(1000000)
                .SetReceiveBufferSize(2048)
                .SetSendBufferSize(2048)
                .Build();


            AsyncSocketServer asyncServer = new AsyncSocketServer(config,
                new AppServerHandler(),
                new AppSessionHandlerFactroy(),
                logFactroy);

            asyncServer.Start();

            while (true)
            {

            }
        }
    }
}
