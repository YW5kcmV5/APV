using System;
using System.Threading;
using APV.CloudSync.Common;

namespace APV.CloudSync.Core.Entities.SyncOperations
{
    public abstract class SyncOperation : BaseEntity
    {
        private string _key;
        private string _destinationEntityKey;
        private readonly SyncOperationType _operationType;
        private readonly string _fullPath;
        private readonly DateTime _timestamp;

        protected SyncOperation(SyncOperationType operationType, string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (path == null)
                throw new ArgumentOutOfRangeException(nameof(path), "Path is empty or whitespace.");

            _operationType = operationType;
            _fullPath = path;
            _timestamp = DateTime.UtcNow;
        }

        protected abstract void Invoke();

        protected virtual bool IsReady()
        {
            return true;
        }

        public virtual string GetKey()
        {
            return _key ?? (_key = $"{OperationType}\\{FileSystemEntity.FormatPath(_fullPath, true)}");
        }

        public virtual string GetDestinationEntityKey()
        {
            return _destinationEntityKey ?? (_destinationEntityKey = FileSystemEntity.FormatPath(_fullPath, true));
        }

        public void Sync()
        {
            try
            {
                if (State == SyncOperationState.Initilialized)
                {
                    while (!IsReady())
                    {
                        Thread.Sleep(100);
                    }
                    State = SyncOperationState.InProcess;
                    Invoke();
                    State = SyncOperationState.Completed;
                }
            }
            catch (Exception ex)
            {
                State = SyncOperationState.Error;
                Error = ex;
            }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public string FullPath
        {
            get { return _fullPath; }
        }

        public SyncOperationType OperationType
        {
            get { return _operationType; }
        }

        public SyncOperationState State { get; protected set; }

        public Exception Error { get; set; }

        public bool IsCompleted
        {
            get { return (State == SyncOperationState.Completed); }
        }
    }
}