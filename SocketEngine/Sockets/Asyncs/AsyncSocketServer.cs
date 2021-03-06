﻿using System;
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
        private byte[] _keepAliveOptionValue = null;

        private BufferManager _bufferManager = null;
        private SocketAsyncEventArgsPool _recvSocketEventPool = null;


        public AsyncSocketServer(IAppServer appServer)
            : base(appServer)
        {

        }

        public override void Start()
        {
            var config = appServer.config;

            // KeepAvlie 생성
            uint dummy = 0;
            _keepAliveOptionValue = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)(config.keepAliveEnable ? 1 : 0)).CopyTo(_keepAliveOptionValue, 0);
            BitConverter.GetBytes((uint)(config.keepAliveTime * 1000)).CopyTo(_keepAliveOptionValue, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)(config.keepAliveInterval * 1000)).CopyTo(_keepAliveOptionValue, Marshal.SizeOf(dummy) * 2);

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

            base.Start();
        }

        public override void Close()
        {
            base.Close();
        }

        protected override void OnSocketAccepted(ISocketListener listener, Socket socket)
        {
            if (socket == null)
                return;

            try
            {
                var config = appServer.config;

                // 소켓 옵션 설정.
                if (config.sendTimeOut > 0)
                    socket.SendTimeout = config.sendTimeOut;

                if (config.sendBufferSize > 0)
                    socket.SendBufferSize = config.sendBufferSize;

                if (config.receiveBufferSize > 0)
                    socket.ReceiveBufferSize = config.receiveBufferSize;

                //TODO @jeongtae.lee : 지원 여부 알아보기 (IOControl와 함께)
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, _keepAliveOptionValue);
                socket.NoDelay = true; // Nagle 알고리즘 적용 여부(TRUE : 적용하지 않음). 패킷을 모아서 보낼 것인지 설정하는 옵션 즉각적인 응답이 중요한 곳에서는 사용하지 않는다)
                socket.LingerState = new LingerOption(true, 0); // 두번째 인자가 0이면 close 호출 시 버퍼에 남아있는 모든 송수신 데이터를 버리고 즉시 종료한다.

                var socketEvent = _recvSocketEventPool.Pop();
                if (socketEvent == null)
                {
                    this.AsyncRun(socket.Close);

                    if (logger.IsErrorEnabled)
                        logger.ErrorFormat("Max connection number {0} was reached.", config.maxConnection);
                    
                    return;
                }

                var sessionId = appServer.CreateSessionId();
                if (string.IsNullOrEmpty(sessionId))
                {
                    this.AsyncRun(socket.Close);

                    if (logger.IsErrorEnabled)
                    {
                        logger.ErrorFormat("Failed to create session ID.");
                    }

                    return;
                }

                var appSession = appServer.CreateAppSession();
                if (appSession == null)
                {
                    this.AsyncRun(socket.Close);

                    if (logger.IsErrorEnabled)
                    {
                        logger.ErrorFormat("Failed to create AppSession");
                    }

                    return;
                }

                var socketSession = new AsyncSocketSession(socketEvent);
                if (!socketSession.Initialize(appSession, socket))
                {
                    this.AsyncRun(socket.Close);

                    if (logger.IsErrorEnabled)
                    {
                        logger.ErrorFormat("Failed to initialize AsyncSocketSession");
                    }

                    return; 
                }

                if (!appSession.Initialize(sessionId, socketSession))
                {
                    this.AsyncRun(socket.Close);

                    if (logger.IsErrorEnabled)
                    {
                        logger.ErrorFormat("Failed to initialize AppSession");
                    }

                    return;
                }

                if (!appServer.AddSession(appSession))
                {
                    this.AsyncRun(socket.Close);

                    if (logger.IsErrorEnabled)
                    {
                        logger.ErrorFormat("Failed to add app session to app server.");
                    }

                    return;
                }

                appServer.AsyncRun(socketSession.Start);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected override void OnListenerError(ISocketListener listener, Exception ex)
        {
            appServer.logger.Error(ex);
        }

        protected override ISocketListener CreateListener(ListenerInfo info)
        {
            return new AsyncSocketListener(info);
        }
    }
}
