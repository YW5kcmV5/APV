using System.IO;
using APV.CloudSync.Common;

namespace APV.CloudSync.Core.Entities.SyncOperations
{
    public sealed class CreateFolderSyncOperation : SyncOperation
    {
        protected override void Invoke()
        {
            if (!Directory.Exists(FullPath))
            {
                Directory.CreateDirectory(FullPath);
            }
        }

        public CreateFolderSyncOperation(string fullPath)
            : base(SyncOperationType.CreateFolder, fullPath)
        {
        }
    }
}