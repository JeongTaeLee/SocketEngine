using System.Collections.Generic;
using System.Net.Sockets;

namespace SocketEngine
{
    internal class BufferManager
    {
        private int _byteLength; // 버퍼의 총 길이
        private byte[] _buffer;  // 버퍼

        Stack<int> _freeIndexPool; // 반환된 버퍼 인덱스    
        int _currentIndex; // 현재 인덱스
        int _stubBufferSize; // 제공할 버퍼 하나 당 사이즈

        public BufferManager(int totalBytes, int stubBufferSize)
        {
            _byteLength = totalBytes;
            _currentIndex = 0;
            _stubBufferSize = stubBufferSize;
            _freeIndexPool = new Stack<int>();
        }

        /// <summary>  
        /// Allocates buffer space used by the buffer pool  
        /// </summary>  
        public void InitBuffer()
        {
            // create one big large buffer and divide that out to each SocketAsyncEventArg object  
            _buffer = new byte[_byteLength];
        }

        /// <summary>  
        /// Assigns a buffer from the buffer pool to the specified SocketAsyncEventArgs object  
        /// </summary>  
        /// <returns>true if the buffer was successfully set, else false</returns>  
        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (_freeIndexPool.Count > 0)
            {
                args.SetBuffer(_buffer, _freeIndexPool.Pop(), _stubBufferSize);
            }
            else
            {
                if ((_byteLength - _stubBufferSize) < _currentIndex)
                {
                    return false;
                }
                args.SetBuffer(_buffer, _currentIndex, _stubBufferSize);
                _currentIndex += _stubBufferSize;
            }
            return true;
        }

        /// <summary>  
        /// Removes the buffer from a SocketAsyncEventArg object.  This frees the buffer back to the   
        /// buffer pool  
        /// </summary>  
        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            _freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}
