using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using APV.CloudSync.Common;
using APV.CloudSync.Core.Entities.Arguments;

namespace APV.CloudSync.Core.Entities
{
    [DataContract(Namespace = Constants.NamespaceData)]
    public sealed class RootFolderEntity : FolderEntity
    {
        private Exception _exception;
        private object _lock;
        private SortedList<string, FileSystemEntity> _entities;
        private bool _sync;

        private SortedList<string, FileSystemEntity> Entities
        {
            get { return _entities ?? (_entities = new SortedList<string, FileSystemEntity>()); }
        }

        private object Lock
        {
            get { return _lock ?? (_lock = new object()); }
        }

        private void Invoke(FileSystemEntity entity, FileSystemOperation operation, string oldPath = null)
        {
            if (OnSync != null)
            {
                var arguments = new SyncEventArgs(entity, operation, oldPath);
                OnSync(this, arguments);
            }
        }

        internal bool OnAddEntity(FileSystemEntity entity)
        {
            lock (Lock)
            {
                string key = entity.GetKey();
                if (Entities.ContainsKey(key))
                {
                    return false;
                }

                var folder = entity as FolderEntity;
                if (folder != null)
                {
                    int length = folder.Files.Count;
                    for (int i = 0; i < length; i++)
                    {
                        OnAddEntity(folder.Files[i]);
                    }

                    length = folder.Folders.Count;
                    for (int i = 0; i < length; i++)
                    {
                        OnAddEntity(folder.Folders[i]);
                    }
                }

                Entities.Add(key, entity);

                if (_sync)
                {
                    FileSystemOperation opration = (entity.IsFolder) ? FileSystemOperation.FolderAdd : FileSystemOperation.FileAdd;
                    Invoke(entity, opration);
                }

                return true;
            }
        }

        internal void OnDeleteEntity(FileSystemEntity entity)
        {
            lock (Lock)
            {
                string key = entity.GetKey();

                if (!Entities.ContainsKey(key))
                    throw new InvalidOperationException($"Entity \"{key}\" cannot be removed, it is not synchronized.");

                var folder = entity as FolderEntity;
                folder?.Clear();

                Entities.Remove(key);

                if (_sync)
                {
                    FileSystemOperation operation = (entity.IsFolder) ? FileSystemOperation.FolderDelete : FileSystemOperation.FileDelete;
                    Invoke(entity, operation);
                }
            }
        }

        internal void OnRenameEntity(string oldKey, FileSystemEntity entity)
        {
            lock (Lock)
            {
                if (!Entities.ContainsKey(oldKey))
                    throw new InvalidOperationException($"Entity \"{oldKey}\" cannot be renamed, it is not synchronized.");

                Entities.Remove(oldKey);
                Entities.Add(entity.GetKey(), entity);

                if (_sync)
                {
                    FileSystemOperation operation = (entity.IsFolder) ? FileSystemOperation.FolderRename : FileSystemOperation.FileRename;
                    Invoke(entity, operation, oldKey);
                }
            }
        }

        internal void OnModifyFileEntity(FileEntity file)
        {
            lock (Lock)
            {
                string key = file.GetKey();
                if (!Entities.ContainsKey(key))
                    throw new InvalidOperationException($"File \"{key}\" cannot be modified, it is not synchronized.");

                if (_sync)
                {
                    FileSystemOperation operation = FileSystemOperation.FileModify;
                    Invoke(file, operation);
                }
            }
        }

        private RootFolderEntity()
        {
        }

        public RootFolderEntity(string path)
            : base(path)
        {
        }

        public void Sync(FileSystemEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            lock (Lock)
            {
                FolderEntity parent = null;
                _sync = true;

                try
                {

                    string path = @event.FullPath;
                    FolderEntity folder;
                    FileEntity file;

                    switch (@event.Operation)
                    {
                        case FileSystemOperation.FileAdd:
                            parent = FindParent(path);
                            if (parent != null)
                            {
                                var fileEntity = new FileEntity(parent, path);
                                parent.Files.Add(fileEntity);
                            }
                            break;

                        case FileSystemOperation.FileDelete:
                            file = Find(path) as FileEntity;
                            parent = file?.Parent;
                            parent?.Files.Delete(file);
                            break;

                        case FileSystemOperation.FileRename:
                            if (@event.NewFullPath != null)
                            {
                                file = Find(path) as FileEntity;
                                parent = file?.Parent;
                                file?.Rename(@event.NewFullPath);
                            }
                            break;

                        case FileSystemOperation.FileModify:
                            file = Find(path) as FileEntity;
                            file?.OnModify();
                            break;

                        case FileSystemOperation.FolderAdd:
                            parent = FindParent(path);
                            if (parent != null)
                            {
                                folder = new FolderEntity(parent, path);
                                parent.Folders.Add(folder);
                                folder.Sync();
                            }
                            break;

                        case FileSystemOperation.FolderDelete:
                            folder = Find(path) as FolderEntity;
                            parent = folder?.Parent;
                            parent?.Folders.Delete(folder);
                            break;

                        case FileSystemOperation.FolderRename:
                            if (@event.NewFullPath != null)
                            {
                                folder = Find(path) as FolderEntity;
                                parent = folder?.Parent;
                                folder?.Rename(@event.NewFullPath);
                            }
                            break;
                    }

                    _sync = false;
                }
                catch (Exception ex)
                {
                    _exception = ex;
                    _sync = false;
                    parent?.Sync();
                }
            }
        }

        public bool Contains(string path, bool isFolder)
        {
            FileSystemEntity entity = Find(path, isFolder);
            return (entity != null);
        }

        public FileSystemEntity Find(string path, bool isFolder)
        {
            FileSystemEntity entity = Find(path);
            if ((entity != null) && (entity.IsFolder == isFolder))
            {
                return entity;
            }
            return null;
        }

        public FileSystemEntity Find(string path)
        {
            string key = FormatPath(path, true);

            if (GetKey() == key)
            {
                return this;
            }

            int index = Entities.IndexOfKey(key);
            FileSystemEntity entity = (index != -1) ? Entities.Values[index] : null;
            return entity;
        }

        public FileSystemEntity Get(string path)
        {
            FileSystemEntity entity = Find(path);

            if (entity == null)
                throw new ArgumentOutOfRangeException(nameof(path), $"Entity with path \"{path}\" does not exist.");

            return entity;
        }

        public FolderEntity FindParent(string path)
        {
            string parentKey = GetParent(path, true);
            var parent = Find(parentKey) as FolderEntity;
            return parent;
        }

        public FolderEntity GetParent(string path)
        {
            FolderEntity parent = FindParent(path);

            if (parent == null)
                throw new ArgumentOutOfRangeException(nameof(path), $"Parent for path \"{path}\" does not exist.");

            return parent;
        }

        [IgnoreDataMember]
        public Exception Exception
        {
            get { return _exception; }
        }

        public event OnSyncEventDelegate OnSync;

        public delegate void OnSyncEventDelegate(RootFolderEntity sender, SyncEventArgs args);
    }
}