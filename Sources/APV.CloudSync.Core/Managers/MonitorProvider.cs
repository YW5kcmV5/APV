using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using APV.CloudSync.Common;
using APV.CloudSync.Core.Entities;

namespace APV.CloudSync.Core.Managers
{
    public sealed class MonitorProvider
    {
        private readonly List<FileSystemEvent> _events = new List<FileSystemEvent>();
        private readonly RootFolderEntity _folder;
        private readonly FileSystemWatcher _watcher;
        private readonly Timer _syncTimer;
        private bool _inProcess;

        private void OnChanged(object sender, FileSystemEventArgs args)
        {
            string path = args.FullPath;
            FileSystemEvent @event = null;

            bool isFolder;
            switch (args.ChangeType)
            {
                case WatcherChangeTypes.Created:
                    isFolder = Directory.Exists(args.FullPath);
                    @event = new FileSystemEvent(path, (isFolder) ? FileSystemOperation.FolderAdd : FileSystemOperation.FileAdd);
                    break;

                case WatcherChangeTypes.Renamed:
                    var renamedArguments = args as RenamedEventArgs;
                    if (renamedArguments != null)
                    {
                        string oldPath = renamedArguments.OldFullPath;
                        isFolder = Directory.Exists(path);
                        @event = new FileSystemEvent(oldPath, (isFolder) ? FileSystemOperation.FolderRename : FileSystemOperation.FileRename, path);
                    }
                    break;

                case WatcherChangeTypes.Changed:
                    isFolder = Directory.Exists(path);
                    if (!isFolder)
                    {
                        @event = new FileSystemEvent(path, FileSystemOperation.FileModify);
                    }
                    break;

                case WatcherChangeTypes.Deleted:
                    FileSystemEntity entity = _folder.Find(path);
                    if (entity != null)
                    {
                        @event = new FileSystemEvent(path, entity.IsFolder ? FileSystemOperation.FolderDelete : FileSystemOperation.FileDelete);
                    }
                    break;
            }

            if (@event != null)
            {
                lock (_events)
                {
                    _events.Add(@event);
                }
            }
        }

        private void OnSyncTimer(object sender)
        {
            if (_inProcess)
            {
                return;
            }
            _inProcess = true;

            try
            {
                var events = new List<FileSystemEvent>();
                lock (_events)
                {
                    if (_events.Count == 0)
                    {
                        return;
                    }
                    events.AddRange(_events);
                    _events.Clear();
                }

                int length = events.Count;
                for (int i = 0; i < length; i++)
                {
                    _folder.Sync(events[i]);
                }
            }
            finally
            {
                _inProcess = false;
            }
        }

        public MonitorProvider(string directory)
            : this(new RootFolderEntity(directory))
        {
        }

        public MonitorProvider(RootFolderEntity directory)
        {
            if (directory == null)
                throw new ArgumentNullException(nameof(directory));
            if (!Directory.Exists(directory.FullPath))
                throw new ArgumentOutOfRangeException(nameof(directory), $"Directory \"{directory.FullPath}\" does not exist.");

            _folder = directory;
            _folder.Sync();

            _watcher = new FileSystemWatcher
            {
                IncludeSubdirectories = true,
                Path = directory.FullPath,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName |
                               NotifyFilters.Attributes | NotifyFilters.CreationTime |
                               NotifyFilters.DirectoryName | NotifyFilters.LastAccess |
                               NotifyFilters.Security | NotifyFilters.Size,
            };

            _watcher.Changed += OnChanged;
            _watcher.Created += OnChanged;
            _watcher.Deleted += OnChanged;
            _watcher.Renamed += OnChanged;
            //_watcher.Error += OnError;

            _syncTimer = new Timer(OnSyncTimer, 0, 0, 100);

            _watcher.EnableRaisingEvents = true;
        }
    }
}