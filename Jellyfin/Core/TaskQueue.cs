using System;
using System.Collections.Generic;
using System.Threading;

namespace Jellyfin.Core
{
    /// <inheritdoc />
    /// <summary>
    /// An implementation of producer-consumer pattern.
    /// See cref https://stackoverflow.com/a/1656662/1694775
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TaskQueue<T> : IDisposable where T : class
    {
        #region Properties

        private bool _disposed;

        private readonly object _padlock = new object();

        /// <summary>
        /// The array of workers.
        /// </summary>
        private readonly Thread[] _workers;

        /// <summary>
        /// The queue of the produced tasks to consume.
        /// </summary>
        private readonly Queue<T> _taskQueue = new Queue<T>();

        /// <summary>
        /// The passed action to be executed on each consuming.
        /// </summary>
        private readonly Action<T> _action;

        #endregion

        #region ctor

        public TaskQueue(int workerCount, Action<T> action)
        {
            _workers = new Thread[workerCount];
            _action = action;

            // Create and start a separate thread for each worker
            for (int i = 0; i < workerCount; i++)
            {
                _workers[i] = new Thread(Consume);
                _workers[i].Start();
            }
        }

        #endregion

        #region Methods

        public void EnqueueTask(T task)
        {
            lock (_padlock)
            {
                _taskQueue.Enqueue(task);
                Monitor.PulseAll(_padlock);
            }
        }

        private void Consume()
        {
            while (true)
            {
                T task;
                lock (_padlock)
                {
                    while (_taskQueue.Count == 0)
                    {
                        Monitor.Wait(_padlock);
                    }

                    task = _taskQueue.Dequeue();
                }

                _action(task);

                if (task == null)
                {
                    return;
                }
            }
        }

        #endregion

        #region IDispoasble Members

        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Enqueue one null task per worker to make each exit.
                for (int index = 0; index < _workers.Length; index++)
                {
                    EnqueueTask(null);
                }

                foreach (Thread worker in _workers)
                {
                    worker.Join();
                }
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}