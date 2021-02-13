using System;
using System.Net;
using System.Net.Sockets;

namespace SocketEngine.Extensions
{
    internal static class SocketExtensions
    {
        public static void SafeClose(this Socket socket)
        {
            if (socket == null)
                return;

            try
            {
                if (socket.Connected)
                    socket.Shutdown(SocketShutdown.Both);
            }
            catch { }

            try
            {
                socket.Close();
            }
            catch { }
        }

        public static IPAddress ParseIPAddress(string ip)
        {
            if (string.IsNullOrEmpty(ip) || "Any".Equals(ip, StringComparison.OrdinalIgnoreCase))
                return IPAddress.Any;
            else if ("IPv6Any".Equals(ip, StringComparison.OrdinalIgnoreCase))
                return IPAddress.IPv6Any;
            else
                return IPAddress.Parse(ip);
        }
    }
}
