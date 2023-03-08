/*=======================================================================
PROJECT NAME: UtilityLib 
	
MODULE:   UtilityLib 
	
FILENAME: ThreadSafeCollection.cs
	
PURPOSE:  Contains the implementation for thread safe collections
			   
Copyright 2013-2016 by Intel Corp.
	
All rights reserved. This source code is protected by federal copyright law and
embodies confidential information proprietary to Intel Corp
	
No reproduction, disclosure or use permitted except as stated in the License Agreement.

AUTHOR: Long Ta
  
DESIGNER NOTES:  
    
MODIFICATIONS: 

Long Ta - 09/07/2013
 * Creation
 
=======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using NLog;

namespace Utility
{
    /// <summary>
    /// Threadsafe Queue
    /// </summary>
    public class ThreadSafeQueue<T>
    {
        /// <summary>
        /// 
        /// </summary>
        protected object _lock = new object();
        /// <summary>
        /// 
        /// </summary>
        protected Queue<T> _queue = new Queue<T>();

        /// <summary>
        /// 
        /// </summary>
        protected ManualResetEvent _QueueWaitMREvent = new ManualResetEvent(false);
        /// <summary>
        /// 
        /// </summary>
        public string _strName = "default";
        /// <summary>
        /// 
        /// </summary>
        protected int _nCount = 0;
        /// <summary>
        /// 
        /// </summary>
        protected Logger _log;

        /// <summary>
        /// 
        /// </summary>
        public ThreadSafeQueue()
        {
            //_log = CLogger.Instance().getSysLogger();
        }

        /// <summary>
        /// 
        /// </summary>
        protected object _waitLock = new object();

        /// <summary>
        /// 
        /// </summary>
        protected volatile int _nRunning = 0;

        /// <summary>
        /// wait for the queue to populate
        /// </summary>
        public void waitForQueueToPopulate()
        {
            try
            {
                lock (_waitLock)
                {
                    //This acts as a simple spin lock to only allow 1 thread to access dequeue at a time
                    while (_nRunning > 0)
                    {
                        clog.Log(string.Format("{0}: waiting for other clients to finish -- {1}", _strName, DateTime.Now.ToString()));
                    }

                    while (true)
                    {
                        if (_nCount < 1)
                        {
                            clog.Log(string.Format("{0}: waitForQueueToPopulate waiting -- {1}", _strName, DateTime.Now.ToString()));
                            _QueueWaitMREvent.WaitOne(100000, true); // waits for a max 100 seconds
                            clog.Log(string.Format("{0}: waitForQueueToPopulate free -- {1}", _strName, DateTime.Now.ToString()));
                        }
                        else
                        {
                            _nRunning++;
                            break;
                        }
                    }
                }

                clog.Log(string.Format("{0}: waitForQueueToPopulate: {1} : _nRunning == {2}", _strName, DateTime.Now.ToString(), _nRunning));
            }
            catch (Exception ex)
            {
                _log.Log(NLog.LogLevel.Error, "TargetSite={0}, message={1}, source={2}, trace={3}, ex={4}", ex.TargetSite, ex.Message, ex.Source, ex.StackTrace, ex.ToString());
            }
        }

        /// <summary>
        /// Count property
        /// </summary>
        public int Count
        {
            get
            {
                return _nCount;
            }
        }

        /// <summary>
        /// Enqueue item to queue
        /// </summary>
        /// <param name="newItem"></param>
        public void Enqueue(T newItem)
        {
            if (newItem == null) return;

            lock (_lock)
            {
                _queue.Enqueue(newItem);
                _nCount = _queue.Count;

                if (_nCount > 0)
                {
                    _QueueWaitMREvent.Set();
                    clog.Log(string.Format("{0}: _QueueWaitMREvent.Set()", _strName));
                }
            }
        }

        /// <summary>
        /// Dequeue item from queue
        /// </summary>
        /// <returns></returns>
        public T DeQueue()
        {
            T item = default(T);

            waitForQueueToPopulate();

            lock (_lock)
            {
                item = _queue.Dequeue();
                _nCount = _queue.Count;
                if (_nCount == 0)
                {
                    //System.Diagnostics.Debug.WriteLine(_strName + ": _QueueWaitMREvent.Reset()");
                    _QueueWaitMREvent.Reset();
                    clog.Log(string.Format("{0}: _QueueWaitMREvent.Reset()", _strName));
                }

                _nRunning--; //Signal completion of this dequeue to allow the next thread to dequeue when there is an item available 
                clog.Log(string.Format("{0}: DeQueue: {1} : _nRunning == {2}", _strName, DateTime.Now.ToString(), _nRunning));
            }

            return item;
        }
    }

    /// <summary>
    /// Threadsafe Dictionary
    /// </summary>
    public class ThreadSafeDictionary<K, T>
    {
        /// <summary>
        /// 
        /// </summary>
        protected object _lock = new object();
        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<K, T> _dictionary = new Dictionary<K, T>();

        /// <summary>
        /// Add item to dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Add(K key, T item)
        {
            lock (_lock)
            {
                if (_dictionary.ContainsKey(key)) return false;

                _dictionary.Add(key, item);
            }

            return true;
        }

        /// <summary>
        /// Method to determin if dictionary contains the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true if the dictionary contains the key, otherwise, false</returns>
        public bool ContainsKey(K key)
        {
            lock (_lock)
            {
                return _dictionary.ContainsKey(key);
            }
        }

        /// <summary>
        /// Get item from dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get(K key)
        {
            T item = default(T);
            lock (_lock)
            {
                if (!_dictionary.ContainsKey(key)) return default(T);

                item = _dictionary[key];
            }

            return item;
        }

        /// <summary>
        /// Remove item from dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(K key)
        {
            lock (_lock)
            {
                if (!_dictionary.ContainsKey(key)) return false;

                _dictionary.Remove(key);
            }

            return true;
        }
    }
}
