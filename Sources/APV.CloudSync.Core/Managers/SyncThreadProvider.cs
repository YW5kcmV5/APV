using System;
using System.Collections.Generic;
using APV.CloudSync.Core.Entities;
using APV.CloudSync.Core.Entities.Arguments;
using APV.CloudSync.Core.Entities.SyncOperations;

namespace APV.CloudSync.Core.Managers
{
    public sealed class SyncThreadProvider
    {
        private readonly List<SyncThread> _threads = new List<SyncThread>();

        private readonly SortedList<string, List<SyncOperation>> _syncOperations =
            new SortedList<string, List<SyncOperation>>();

        public void AddOperation(SyncOperation operation)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
        }

        public void AddOperation(RootFolderEntity source, SyncOperation syncOperation, SyncEventArgs args)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var completedThreads = new List<SyncThread>();

            bool newThreadIsNeeded = true;
            lock (_threads)
            {
                int length = _threads.Count;
                for (int index = 0; index < length; index++)
                {
                    SyncThread thread = _threads[index];
                    lock (thread.Lock)
                    {
                        if (!thread.InProcess)
                        {
                            completedThreads.Add(thread);
                        }
                        else if (thread.Folder.HasDependencies(args.Entity))
                        {
                            thread.AddOperation(syncOperation);
                            newThreadIsNeeded = false;
                            break;
                        }
                    }
                }

                foreach (SyncThread completedThread in completedThreads)
                {
                    _threads.Remove(completedThread);
                }

                if (newThreadIsNeeded)
                {
                    FolderEntity folder = (args.Entity.IsFolder) ? (FolderEntity) args.Entity : args.Entity.Parent;
                    var newThread = new SyncThread(folder, syncOperation);
                    _threads.Add(newThread);
                }
            }

            lock (_syncOperations)
            {
                string entityKey = $"{source.GetKey()}|{syncOperation.GetDestinationEntityKey()}";
                int index = _syncOperations.IndexOfKey(entityKey);
                List<SyncOperation> syncOperations;
                if (index == -1)
                {
                    syncOperations = new List<SyncOperation>();
                    _syncOperations.Add(entityKey, syncOperations);
                }
                else
                {
                    syncOperations = _syncOperations.Values[index];
                }
                syncOperations.Add(syncOperation);
            }
        }

        public bool DeleteEvent(RootFolderEntity source, SyncEventArgs eventArgs)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (eventArgs == null)
                throw new ArgumentNullException(nameof(eventArgs));

            lock (_syncOperations)
            {
                string entityKey = $"{source.GetKey()}|{eventArgs.GetEntityKey()}";
                int index = _syncOperations.IndexOfKey(entityKey);
                if (index != -1)
                {
                    List<SyncOperation> syncOperations = _syncOperations.Values[index];
                    int length = syncOperations.Count;
                    for (int i = 0; i < length; i++)
                    {
                        SyncOperation operation = syncOperations[i];
                        if (eventArgs.CorrespondsTo(operation))
                        {
                            syncOperations.RemoveRange(0, i + 1);
                            if (syncOperations.Count == 0)
                            {
                                _syncOperations.RemoveAt(index);
                            }
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}