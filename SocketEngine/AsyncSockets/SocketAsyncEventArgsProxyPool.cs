﻿using System.Collections.Generic;
using System.Collections.Concurrent;
using SocketEngine.Extensions;

namespace SocketEngine.AsyncSockets
{
    internal class SocketAsyncEventArgsProxyPool
    {
        private ConcurrentStack<SocketAsyncEventArgsProxy> _pools;

        // Initializes the object pool to the specified size  
        //  
        // The "capacity" parameter is the maximum number of   
        // SocketAsyncEventArgs objects the pool can hold  
        public SocketAsyncEventArgsProxyPool()
        {
            _pools = new ConcurrentStack<SocketAsyncEventArgsProxy>();
        }

        public SocketAsyncEventArgsProxyPool(IEnumerable<SocketAsyncEventArgsProxy> enumerator)
        {
            ExceptionExtension.ArgumentNullExceptionIfNull(enumerator, "enumerator");

            _pools = new ConcurrentStack<SocketAsyncEventArgsProxy>(enumerator);
        }

        // Add a SocketAsyncEventArg instance to the pool  
        //  
        //The "item" parameter is the SocketAsyncEventArgs instance   
        // to add to the pool  
        public void Push(SocketAsyncEventArgsProxy item)
        {
            ExceptionExtension.ArgumentNullExceptionIfNull(item, "item");

            item.Reset();
            _pools.Push(item);
        }

        // Removes a SocketAsyncEventArgs instance from the pool  
        // and returns the object removed from the pool  
        public SocketAsyncEventArgsProxy Pop()
        {
            if (!_pools.TryPop(out var pair))
                return null;

            return pair;
        }

        // The number of SocketAsyncEventArgs instances in the pool  
        public int Count
        {
            get { return _pools.Count; }
        }
    }
}
