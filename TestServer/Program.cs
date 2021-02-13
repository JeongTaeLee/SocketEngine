using System;
using SocketEngine;


namespace TestServer
{
    class TestSession : AppSession
    {

    }

    class TestServer : AppServer<AppSession>
    {
        public TestServer()
        {

        }

        public override void OnSessionConnected(AppSession appSession)
        {
            
        }

        public override void OnSessionDisconnected(AppSession appSession)
        {
        
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test0");
        }
    }
}
