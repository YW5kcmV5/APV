using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using APV.CloudSync.Common;

namespace APV.CloudSync.Core.Entities
{
    [KnownType(typeof(FileEntity))]
    [KnownType(typeof(FolderEntity))]
    [KnownType(typeof(RootFolderEntity))]
    [DebuggerDisplay("{RelativePath}")]
    [DataContract(Namespace = Constants.NamespaceData)]
    public abstract class FileSystemEntity : BaseEntity
    {
        private string _key;
        private FolderEntity _parent;
        private RootFolderEntity _root;
        private string _relativePath;
        private string _name;
        private int? _level;

        protected virtual void ClearCache()
        {
            _root = null;
            _relativePath = null;
            _name = null;
            _level = null;
            _key = null;
        }

        protected FileSystemEntity()
        {
        }

        protected FileSystemEntity(string path)
        {
            FullPath = FormatPath(path);
        }

        protected FileSystemEntity(FolderEntity parent, string path)
            : this(path)
        {
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));
            if (!path.StartsWith(parent.FullPath + "\\", StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentNullException(nameof(path), $"Path \"{path}\" is not part of parent path \"{parent.FullPath}\".");

            Parent = parent;
        }

        internal virtual void UpdateParentPath()
        {
            string oldKey = GetKey();
            FullPath = $"{Parent.FullPath}\\{Name}";
            ClearCache();

            Root?.OnRenameEntity(oldKey, this);
        }

        public string GetKey()
        {
            if (_key == null)
            {
                _key = FormatPath(FullPath, true);
            }
            return _key;
        }

        public virtual void Rename(string newPath)
        {
            string newFormattedPath = FormatPath(newPath);

            if (Parent != null)
            {
                string newParentKey = GetParent(newPath, true);
                if (newParentKey != Parent.GetKey())
                    throw new ArgumentOutOfRangeException(nameof(newPath), $"Path \"{newPath}\" is not part of parent path \"{Parent.FullPath}\".");
            }

            string oldKey = GetKey();
            FullPath = newFormattedPath;
            ClearCache();

            Root?.OnRenameEntity(oldKey, this);
        }

        [IgnoreDataMember]
        public bool IsFolder
        {
            get { return (this is FolderEntity); }
        }

        [IgnoreDataMember]
        public bool IsFile
        {
            get { return !IsFolder; }
        }

        [IgnoreDataMember]
        public FolderEntity Parent
        {
            get { return _parent; }
            internal set
            {
                if (_parent != value)
                {
                    _parent = value;
                    ClearCache();
                }
            }
        }

        [IgnoreDataMember]
        public RootFolderEntity Root
        {
            get
            {
                if (_root == null)
                {
                    _root = (Parent != null) ? Parent.Root : this as RootFolderEntity;
                }
                return _root;
            }
        }

        [IgnoreDataMember]
        public string RelativePath
        {
            get
            {
                if (_relativePath == null)
                {
                    _relativePath = ((Root != null) && (Root.FullPath.Length < FullPath.Length))
                        ? FullPath.Substring(Root.FullPath.Length)
                        : FullPath;
                }
                return _relativePath;
            }
        }

        [IgnoreDataMember]
        public string Name
        {
            get
            {
                if (_name == null)
                {
                    _name = Path.GetFileName(FullPath);
                }
                return _name;
            }
        }

        [IgnoreDataMember]
        public int Level
        {
            get
            {
                if (_level == null)
                {
                    _level = Parent?.Level + 1 ?? 0;
                }
                return _level.Value;
            }
        }

        [DataMember(IsRequired = true)]
        public string FullPath { get; private set; }

        public static string FormatPath(string path, bool key = false)
        {
            if (path == null)
                throw new ArgumentNullException(nameof(path));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentOutOfRangeException(nameof(path), "Path is empty or whitespace.");

            string formattedPath = path.Trim().TrimEnd('/', '\\');

            if ((string.IsNullOrWhiteSpace(formattedPath)) || (formattedPath.StartsWith("\\")))
                throw new ArgumentOutOfRangeException(nameof(path), $"Path \"{path}\" has wrong format and cannot be parsed.");

            if (key)
            {
                formattedPath = formattedPath.ToUpperInvariant();
            }

            return formattedPath;
        }

        public static string GetParent(string path, bool key = false)
        {
            path = FormatPath(path, key);

            int index = path.LastIndexOf('\\');
            if (index == -1)
                throw new ArgumentOutOfRangeException(nameof(path), $"Path \"{path}\" has wrong format and cannot be parsed.");

            string parentKey = path.Substring(0, index);
            return parentKey;
        }
    }
}