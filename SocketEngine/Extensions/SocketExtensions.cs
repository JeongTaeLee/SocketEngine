﻿using System.Net.Sockets;

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
    }
}
