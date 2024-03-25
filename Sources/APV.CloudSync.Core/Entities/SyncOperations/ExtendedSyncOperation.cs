using System;
using APV.CloudSync.Common;

namespace APV.CloudSync.Core.Entities.SyncOperations
{
    public abstract class ExtendedSyncOperation : SyncOperation
    {
        private readonly string _extendedPath;
        private string _key;
        private string _destinationEntityKey;

        protected ExtendedSyncOperation(SyncOperationType operationType, string path, string extendedPath)
            : base(operationType, path)
        {
            if (extendedPath == null)
                throw new ArgumentNullException(nameof(extendedPath));
            if (string.IsNullOrWhiteSpace(extendedPath))
                throw new ArgumentOutOfRangeException(nameof(extendedPath), "Extended path is empty or whitespace.");

            _extendedPath = extendedPath;
        }

        public override string GetKey()
        {
            return _key ?? (_key = $"{OperationType}\\{FileSystemEntity.FormatPath(FullPath)}\\{FileSystemEntity.FormatPath(_extendedPath, true)}");
        }

        public override string GetDestinationEntityKey()
        {
            return _destinationEntityKey ?? (_destinationEntityKey = FileSystemEntity.FormatPath(_extendedPath, true));
        }

        public string ExtendedPath
        {
            get { return _extendedPath; }
        }
    }
}