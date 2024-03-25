using System;
using System.Collections.Generic;
using System.Threading;
using APV.CloudSync.Common;
using APV.CloudSync.Core.Entities;
using APV.CloudSync.Core.Entities.SyncOperations;

namespace APV.CloudSync.Core.Managers
{
    public sealed class SyncThread : IDisposable
    {
        private readonly object _lock = new object();
        private Thread _thread;
        private bool _inProcess;
        private FolderEntity _folder;
        private List<SyncOperation> _operations = new List<SyncOperation>();
        private Exception _exception;

        private void Invoke(object sender)
        {
            try
            {
                lock (_lock)
                {
                    _thread = Thread.CurrentThread;
                }

                while (_inProcess)
                {
                    SyncOperation operation = null;
                    lock (_lock)
                    {
                        if (_operations.Count == 0)
                        {
                            break;
                        }

                        SyncOperation syncOperation = _operations[0];
                        if (syncOperation.IsCompleted)
                        {
                            _operations.RemoveAt(0);
                        }
                        else if (syncOperation.State == SyncOperationState.Initilialized)
                        {
                            operation = syncOperation;
                        }
                    }

                    operation?.Sync();

                    Thread.Sleep(100);
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception ex)
            {
                _exception = ex;
            }
            finally
            {
                lock (_lock)
                {
                    _inProcess = false;
                    _thread = null;
                }
            }
        }

        public SyncThread(FolderEntity folder, SyncOperation operation)
        {
            if (folder == null)
                throw new ArgumentNullException(nameof(folder));
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            _folder = folder;
            _inProcess = true;
            AddOperation(operation);
            ThreadPool.QueueUserWorkItem(Invoke);
        }

        public void AddOperation(SyncOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            if (Completed)
                throw new InvalidOperationException("Thread is completed. Create new thread.");

            string key = operation.GetKey();
            lock (_lock)
            {
                int length = _operations.Count;
                for (int i = 0; i < length; i++)
                {
                    SyncOperation entity = _operations[i];
                    string existingKey = entity.GetKey();
                    if (existingKey == key)
                    {
                        if ((entity.State == SyncOperationState.Initilialized) && (_operations.Count > 0))
                        {
                            _operations.RemoveAt(i);
                            _operations.Add(entity);
                        }
                        break;
                    }
                }
                _operations.Add(operation);
            }
        }

        public void Stop()
        {
            lock (_lock)
            {
                _thread?.Abort();
                _thread = null;
                _inProcess = false;
                _operations?.Clear();
            }
        }

        public void Dispose()
        {
            Stop();
            _operations = null;
            _folder = null;
            _exception = null;
        }

        public FolderEntity Folder
        {
            get { return _folder; }
        }

        public Exception Error
        {
            get { return _exception; }
        }

        public bool InProcess
        {
            get
            {
                lock (_lock)
                {
                    return _inProcess;
                }
            }
        }

        public bool Completed
        {
            get { return (!InProcess); }
        }

        public object Lock
        {
            get { return _lock; }
        }

    }
}