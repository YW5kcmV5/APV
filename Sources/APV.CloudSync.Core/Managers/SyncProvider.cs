using System;
using System.Collections.Generic;
using APV.CloudSync.Common;
using APV.CloudSync.Core.Entities;
using APV.CloudSync.Core.Entities.Arguments;
using APV.CloudSync.Core.Entities.SyncOperations;

namespace APV.CloudSync.Core.Managers
{
    public class SyncProvider
    {
        private readonly RootFolderEntity _master;
        private readonly List<RootFolderEntity> _slaves = new List<RootFolderEntity>();
        private readonly SyncThreadProvider _threadProvider = new SyncThreadProvider();

        private string TransferPath(string path, RootFolderEntity master, RootFolderEntity slave)
        {
            string convertedPath = slave.Root.FullPath + path.Substring(master.Root.FullPath.Length);
            return convertedPath;
        }

        private SyncOperation GetOperation(RootFolderEntity master, RootFolderEntity slave, SyncEventArgs args)
        {
            FileSystemOperation operation = args.Operation;
            string fullPath = args.Entity.FullPath;
            string extendedPath;

            SyncOperation syncOperation = null;

            switch (operation)
            {
                case FileSystemOperation.FileAdd:
                    extendedPath = TransferPath(fullPath, master, slave);
                    syncOperation = new CopyFileSyncOperation(fullPath, extendedPath);
                    break;

                case FileSystemOperation.FileModify:
                    extendedPath = TransferPath(fullPath, master, slave);
                    syncOperation = new CopyFileSyncOperation(fullPath, extendedPath);
                    break;

                case FileSystemOperation.FileDelete:
                    fullPath = TransferPath(fullPath, master, slave);
                    syncOperation = new DeleteFileSyncOperation(fullPath);
                    break;

                case FileSystemOperation.FileRename:
                    extendedPath = TransferPath(fullPath, master, slave);
                    fullPath = TransferPath(args.OldPath, master, slave);
                    syncOperation = new RenameFileSyncOperation(fullPath, extendedPath);
                    break;

                case FileSystemOperation.FolderAdd:
                    fullPath = TransferPath(fullPath, master, slave);
                    syncOperation = new CreateFolderSyncOperation(fullPath);
                    break;

                case FileSystemOperation.FolderDelete:
                    fullPath = TransferPath(fullPath, master, slave);
                    syncOperation = new DeleteFolderSyncOperation(fullPath);
                    break;

                case FileSystemOperation.FolderRename:
                    extendedPath = TransferPath(fullPath, master, slave);
                    fullPath = TransferPath(args.OldPath, master, slave);
                    syncOperation = new RenameFolderSyncOperation(fullPath, extendedPath);
                    break;
            }

            if (syncOperation == null)
                throw new ArgumentOutOfRangeException(nameof(operation), $"Unknown operation \"{operation}\".");

            return syncOperation;
        }

        private void OnMasterSync(RootFolderEntity sender, SyncEventArgs args)
        {
            int length = _slaves.Count;
            for (int i = 0; i < length; i++)
            {
                RootFolderEntity slave = _slaves[i];
                bool backEvent = _threadProvider.DeleteEvent(slave, args);
                if (!backEvent)
                {
                    SyncOperation syncOperation = GetOperation(_master, slave, args);
                    _threadProvider.AddOperation(_master, syncOperation, args);
                }
            }
        }

        private void OnSlaveSync(RootFolderEntity sender, SyncEventArgs args)
        {
            bool backEvent = _threadProvider.DeleteEvent(_master, args);
            if (!backEvent)
            {
                //TODO: sync back
            }
        }

        public void Sync(FileSystemEntity[] masterItems, RootFolderEntity slave, bool asMaster = false)
        {
            slave.Sync();

            int length = masterItems.Length;
            for (int i = 0; i < length; i++)
            {
                FileSystemEntity entity = masterItems[i];
                string key = TransferPath(entity.FullPath, _master, slave);
                FileSystemEntity existing = slave.Find(key, entity.IsFolder);
                if (existing == null)
                {
                    //Create on slave
                    FileSystemOperation operation = (entity.IsFolder) ? FileSystemOperation.FolderAdd : FileSystemOperation.FileAdd;
                    SyncEventArgs args = new SyncEventArgs(entity, operation);
                    OnMasterSync(_master, args);
                }
                else if (entity.IsFile)
                {
                    string entityChecksum = ((FileEntity)entity).Checksum;
                    string existingChecksum = ((FileEntity)existing).Checksum;
                    if (entityChecksum != existingChecksum)
                    {
                        //Copy to slave
                        SyncEventArgs args = new SyncEventArgs(entity, FileSystemOperation.FileModify);
                        OnMasterSync(_master, args);
                    }
                }
            }

            FileSystemEntity[] slaveItems = slave.ToList();
            length = slaveItems.Length;
            for (int i = 0; i < length; i++)
            {
                FileSystemEntity entity = slaveItems[i];
                string key = TransferPath(entity.FullPath, slave, _master);
                FileSystemEntity existing = slave.Find(key, entity.IsFolder);
                if (existing == null)
                {
                    if (asMaster)
                    {
                        //Delete from slave
                        FileSystemOperation operation = (entity.IsFile) ? FileSystemOperation.FileDelete : FileSystemOperation.FolderDelete;
                        SyncEventArgs args = new SyncEventArgs(entity, operation);
                        OnSlaveSync(_master, args);
                    }
                    else
                    {
                        //Create on master
                    }
                }
            }
        }

        public SyncProvider(RootFolderEntity master, params RootFolderEntity[] slaves)
            : this(master, false, (IEnumerable<RootFolderEntity>)slaves)
        {
        }

        public SyncProvider(RootFolderEntity master, bool asMaster, params RootFolderEntity[] slaves)
            : this(master, asMaster, (IEnumerable<RootFolderEntity>)slaves)
        {
        }

        public SyncProvider(RootFolderEntity master, bool asMaster, IEnumerable<RootFolderEntity> slaves)
        {
            if (master == null)
                throw new ArgumentNullException(nameof(master));
            if (slaves == null)
                throw new ArgumentNullException(nameof(slaves));

            foreach (RootFolderEntity slave in slaves)
            {
                if (slave != null)
                {
                    if (master.HasDependencies(slave))
                        throw new ArgumentOutOfRangeException(nameof(slave), $"Master \"{master.FullPath}\" and slave \"{slave.FullPath}\" has dependencies.");

                    _slaves.Add(slave);
                }
            }

            if (_slaves.Count == 0)
                throw new ArgumentOutOfRangeException(nameof(slaves), "As minimum one slave should be defined.");

            _master = master;
            _master.OnSync += OnMasterSync;
            new MonitorProvider(_master);
            _master.Sync();

            FileSystemEntity[] masterItems = _master.ToList();

            int length = _slaves.Count;
            for (int i = 0; i < length; i++)
            {
                RootFolderEntity slave = _slaves[i];
                slave.OnSync += OnSlaveSync;
                new MonitorProvider(slave);
                Sync(masterItems, slave, asMaster);
            }
        }
    }
}