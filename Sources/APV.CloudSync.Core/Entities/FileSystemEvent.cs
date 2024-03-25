using System;
using System.Runtime.Serialization;
using APV.CloudSync.Common;

namespace APV.CloudSync.Core.Entities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public sealed class FileSystemEvent
    {
        private FileSystemEvent()
        {
        }

        public FileSystemEvent(string path, FileSystemOperation operation, string newPath = null)
        {
            FullPath = FileSystemEntity.FormatPath(path);
            Operation = operation;
            NewFullPath = (newPath != null) ? FileSystemEntity.FormatPath(newPath) : null;
        }

        [DataMember(IsRequired = true)]
        public DateTime Timestamp { get; private set; }

        [DataMember(IsRequired = true)]
        public string FullPath { get; private set; }

        [DataMember(IsRequired = true)]
        public string NewFullPath { get; private set; }

        [DataMember(IsRequired = true)]
        public FileSystemOperation Operation { get; private set; }
    }
}