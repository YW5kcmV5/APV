using System.IO;
using APV.CloudSync.Common;

namespace APV.CloudSync.Core.Entities.SyncOperations
{
    public sealed class DeleteFolderSyncOperation : SyncOperation
    {
        protected override void Invoke()
        {
            if (Directory.Exists(FullPath))
            {
                Directory.Delete(FullPath, true);
            }
        }

        public DeleteFolderSyncOperation(string fullPath)
            : base(SyncOperationType.DeleteFolder, fullPath)
        {
        }
    }
}