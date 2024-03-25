using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using APV.CloudSync.Common;
using APV.CloudSync.Core.Entities.Collection;

namespace APV.CloudSync.Core.Entities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public class FolderEntity : FileSystemEntity
    {
        private FileCollection _files;
        private FolderCollection _folders;

        protected FolderEntity()
        {
        }

        protected FolderEntity(string path)
            : base(path)
        {
        }

        public FolderEntity(FolderEntity parent, string path)
            : base(parent, path)
        {
        }

        internal override void UpdateParentPath()
        {
            base.UpdateParentPath();

            int length = Files.Count;
            for (int i = 0; i < length; i++)
            {
                Files[i].UpdateParentPath();
            }

            length = Folders.Count;
            for (int i = 0; i < length; i++)
            {
                Folders[i].UpdateParentPath();
            }
        }

        public override void Rename(string newPath)
        {
            base.Rename(newPath);

            int length = Files.Count;
            for (int i = 0; i < length; i++)
            {
                Files[i].UpdateParentPath();
            }

            length = Folders.Count;
            for (int i = 0; i < length; i++)
            {
                Folders[i].UpdateParentPath();
            }
        }

        public void Clear()
        {
            Files.Clear();
            Folders.Clear();
        }

        public void Sync()
        {
            Clear();

            string path = FullPath;

            string[] directories = Directory.GetDirectories(path);
            int length = directories.Length;
            for (int i = 0; i < length; i++)
            {
                FolderEntity subFolder = new FolderEntity(this, directories[i]);
                Folders.Add(subFolder);
                subFolder.Sync();
            }

            string[] files = Directory.GetFiles(path);
            length = files.Length;
            for (int i = 0; i < length; i++)
            {
                FileEntity file = new FileEntity(this, files[i]);
                Files.Add(file);
            }
        }

        public bool HasDependencies(FileSystemEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return HasDependencies(entity.GetKey(), entity.IsFile);
        }

        public bool HasDependencies(string path, bool isFile)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentOutOfRangeException(nameof(path), "Path is empty or whitespace.");

            string xKey = GetKey();
            string yKey = FormatPath(path, true);

            if (yKey.StartsWith(xKey + "\\"))
            {
                return true;
            }
            if ((!isFile) && (xKey.StartsWith(yKey + "\\")))
            {
                return true;
            }
            return false;
        }

        public bool IsParentOf(FileSystemEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            return IsParentOf(entity.GetKey());
        }

        public bool IsParentOf(string path)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentOutOfRangeException(nameof(path), "Path is empty or whitespace.");

            string xKey = GetKey();
            string yKey = FormatPath(path, true);

            if (yKey.StartsWith(xKey + "\\"))
            {
                return true;
            }
            return false;
        }

        public virtual FileSystemEntity[] ToList()
        {
            var result = new List<FileSystemEntity>();

            result.Add(this);
            result.AddRange(Files);

            int length = Folders.Count;
            for (int i = 0; i < length; i++)
            {
                FileSystemEntity[] subEntities = Folders[i].ToList();
                result.AddRange(subEntities);
            }

            return result.ToArray();
        }

        [DataMember(IsRequired = true)]
        public FileCollection Files
        {
            get { return (_files ?? (_files = new FileCollection(this))); }
            private set { _files = new FileCollection(this, value); }
        }

        [DataMember(IsRequired = true)]
        public FolderCollection Folders
        {
            get { return (_folders ?? (_folders = new FolderCollection(this))); }
            private set { _folders = new FolderCollection(this, value); }
        }

        public bool IsRoot
        {
            get { return (Parent == null); }
        }
    }
}