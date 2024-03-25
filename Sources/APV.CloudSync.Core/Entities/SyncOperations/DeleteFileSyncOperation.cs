using System.IO;
using APV.CloudSync.Common;

namespace APV.CloudSync.Core.Entities.SyncOperations
{
    public sealed class DeleteFileSyncOperation : SyncOperation
    {
        protected override void Invoke()
        {
            if (File.Exists(FullPath))
            {
                File.Delete(FullPath);
            }
        }

        public DeleteFileSyncOperation(string fullPath)
            : base(SyncOperationType.DeleteFile, fullPath)
        {
        }
    }
}