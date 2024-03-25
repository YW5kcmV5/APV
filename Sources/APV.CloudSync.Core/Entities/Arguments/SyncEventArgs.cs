using System;
using APV.CloudSync.Common;
using APV.CloudSync.Core.Entities.SyncOperations;

namespace APV.CloudSync.Core.Entities.Arguments
{
    public class SyncEventArgs : EventArgs
    {
        private readonly FileSystemEntity _entity;
        private readonly FileSystemOperation _operation;
        private readonly string _oldPath;
        private readonly DateTime _timestamp;

        public SyncEventArgs(FileSystemEntity entity, FileSystemOperation operation, string oldPath = null)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (((operation == FileSystemOperation.FileRename) || (operation == FileSystemOperation.FolderRename)) &&
                (string.IsNullOrWhiteSpace(oldPath)))
                throw new ArgumentOutOfRangeException(nameof(oldPath),
                    $"Old path cannot be null, empty or whitespace for operation \"{operation}\".");

            _entity = entity;
            _operation = operation;
            _oldPath = oldPath;
            _timestamp = DateTime.UtcNow;
        }

        public FileSystemEntity Entity
        {
            get { return _entity; }
        }

        public FileSystemOperation Operation
        {
            get { return _operation; }
        }

        public DateTime Timestamp
        {
            get { return _timestamp; }
        }

        public string OldPath
        {
            get { return _oldPath; }
        }

        public string GetEntityKey()
        {
            return Entity.GetKey();
        }

        public bool CorrespondsTo(SyncOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            if (operation.GetDestinationEntityKey() != Entity.GetKey())
            {
                return false;
            }

            if (Timestamp < operation.Timestamp)
            {
                return false;
            }

            switch (operation.OperationType)
            {
                case SyncOperationType.CopyFile:
                    return (Operation == FileSystemOperation.FileAdd) || (Operation == FileSystemOperation.FileModify);

                case SyncOperationType.CreateFolder:
                    return (Operation == FileSystemOperation.FolderAdd);

                case SyncOperationType.DeleteFile:
                    return (Operation == FileSystemOperation.FileDelete);

                case SyncOperationType.DeleteFolder:
                    return (Operation == FileSystemOperation.FolderDelete);

                case SyncOperationType.RenameFile:
                    return (Operation == FileSystemOperation.FileRename);

                case SyncOperationType.RenameFolder:
                    return (Operation == FileSystemOperation.FolderRename);
            }

            return false;
        }
    }
}