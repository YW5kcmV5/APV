using System;
using System.IO;
using System.Runtime.Serialization;
using APV.CloudSync.Common;
using APV.Common;

namespace APV.CloudSync.Core.Entities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public sealed class FileEntity : FileSystemEntity
    {
        private string _contentChecksum;

        protected override void ClearCache()
        {
            base.ClearCache();
            _contentChecksum = null;
        }

        private FileEntity()
        {
        }

        public void OnModify()
        {
            ClearCache();
            Root?.OnModifyFileEntity(this);
        }

        public FileEntity(FolderEntity parent, string path)
            : base(parent, path)
        {
        }

        [DataMember]
        public DateTime ModifiedAt { get; set; }

        public string Checksum
        {
            get
            {
                if (_contentChecksum == null)
                {
                    if (File.Exists(FullPath))
                    {
                        _contentChecksum = IOUtility.GetChecksum(FullPath);
                    }
                }
                return _contentChecksum;
            }
        }
    }
}