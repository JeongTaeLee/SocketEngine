using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SocketEngine.Commons;
using SocketEngine.AsyncSockets;
using SocketEngine.Extensions;

namespace SocketEngine.Sockets.Asyncs
{
    class AsyncSocketServer : SocketServer<AsyncSocketServer>
    {
        private AsyncSocketListener _listener = null;

        private BufferManager _bufferManager = null;
        private SocketAsyncEventArgsPool _recvSocketEventPool = null;

        private byte[] _keepAliveOptionValue = null;

        public AsyncSocketServer()
        {

        }

        public override void Start()
        {
            base.Start();

            // 버퍼 매니저 생성.
            _bufferManager = new BufferManager(config.maxConnection * config.receiveBufferSize, config.receiveBufferSize);
            _bufferManager.InitBuffer();

            // SocketAsyncEventArgsPool 생성
            var socketEvents = new List<SocketAsyncEventArgs>(config.maxConnection);
            for (int index = 0; index < config.maxConnection; ++index)
            {
                var socketEvvent = new SocketAsyncEventArgs();
                _bufferManager.SetBuffer(socketEvvent);
                socketEvents.Add(socketEvvent);
            }

            _recvSocketEventPool = new SocketAsyncEventArgsPool(socketEvents);

            // KeepAvlie 생성
            uint dummy = 0;
            _keepAliveOptionValue = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)(config.keepAliveEnable ? 1 : 0)).CopyTo(_keepAliveOptionValue, 0);
            BitConverter.GetBytes((uint)(config.keepAliveTime * 1000)).CopyTo(_keepAliveOptionValue, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)(config.keepAliveInterval * 1000)).CopyTo(_keepAliveOptionValue, Marshal.SizeOf(dummy) * 2);

            // 리스너 생성
            this._listener = new AsyncSocketListener(socket, this);
            this._listener.accepted = OnSocketAccept;
            this._listener.throwedException = OnThrowException;

            _listener.Start();
        }

        public override void End()
        {
            _listener.Close();
            _listener = null;

            base.End();
        }

        private void OnSocketAccept(Socket socket)
        {
            if (socket == null)
                return;

            try
            {

                // 소켓 옵션 설정.
                if (config.sendTimeOut > 0)
                    socket.SendTimeout = config.sendTimeOut;

                if (config.sendBufferSize > 0)
                    socket.SendBufferSize = config.sendBufferSize;

                if (config.receiveBufferSize > 0)
                    socket.ReceiveBufferSize = config.receiveBufferSize;

                //TODO @jeongtae.lee : 지원 여부 알아보기 (IOControl와 함께)
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, _keepAliveOptionValue);
                socket.NoDelay = true; // Nagle() 알고리즘 적용 여부(TRUE : 적용하지 않음). 패킷을 모아서 보낼 것인지 설정하는 옵션 즉각적인 응답이 중요한 곳에서는 사용하지 않는다)
                socket.LingerState = new LingerOption(true, 0); // 두번째 인자가 0이면 close 호출 시 버퍼에 남아있는 모든 송수신 데이터를 버리고 즉시 종료한다.

                var socketEvent = _recvSocketEventPool.Pop();
                if (socketEvent == null)
                {
                    this.AsyncRun(socket.Close);

                    if (logger.IsErrorEnabled)
                        logger.ErrorFormat("Max connection number {0} was reached.", config.maxConnection);
                    
                    return;
                }
                
                // TODO @jeongtae.lee : App session 초기화 구현
                var appSession = appServer.CreateAppSession();
                appSession.Initialize(string.Empty, null);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void OnThrowException(Exception ex)
        {

        }
    }
}
