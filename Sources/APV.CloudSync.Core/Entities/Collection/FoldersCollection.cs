using System.Collections.Generic;
using System.Runtime.Serialization;
using APV.CloudSync.Common;

namespace APV.CloudSync.Core.Entities.Collection
{
    [CollectionDataContract(Namespace = Constants.NamespaceData, Name = @"Folders", ItemName = "Folder")]
    public sealed class FolderCollection : BaseEntityCollection<FolderEntity>
    {
        private FolderCollection()
        {
        }

        public FolderCollection(FolderEntity folder)
            : base(folder)
        {
        }

        public FolderCollection(FolderEntity owner, IEnumerable<FolderEntity> entities)
            : base(owner, entities)
        {
        }
    }
}