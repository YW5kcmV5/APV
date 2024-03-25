using System.Collections.Generic;
using System.Runtime.Serialization;
using APV.CloudSync.Common;

namespace APV.CloudSync.Core.Entities.Collection
{
    [CollectionDataContract(Namespace = Constants.NamespaceData, Name = @"Files", ItemName = "File")]
    public sealed class FileCollection : BaseEntityCollection<FileEntity>
    {
        private FileCollection()
        {
        }

        public FileCollection(FolderEntity folder)
            : base(folder)
        {
        }

        public FileCollection(FolderEntity owner, IEnumerable<FileEntity> entities)
            : base(owner, entities)
        {
        }
    }
}