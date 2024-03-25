using System.IO;
using APV.CloudSync.Common;

namespace APV.CloudSync.Core.Entities.SyncOperations
{
    public sealed class RenameFolderSyncOperation : ExtendedSyncOperation
    {
        protected override void Invoke()
        {
            if ((File.Exists(FullPath)) && (!File.Exists(ExtendedPath)))
            {
                Directory.Move(FullPath, ExtendedPath);
            }
        }

        public RenameFolderSyncOperation(string fullPath, string newPath)
            : base(SyncOperationType.RenameFolder, fullPath, newPath)
        {
        }
    }
}