using System;
using System.Text;
using APV.CloudSync.Common;
using APV.CloudSync.Core.Entities;
using APV.CloudSync.Core.Entities.Arguments;
using APV.CloudSync.Core.Managers;
using APV.Common;

namespace APV.CloudSync.Tests.ConsoleTest
{
    class Program
    {
        private static SyncProvider _provider;

        private static void Log(RootFolderEntity sender, SyncEventArgs args)
        {
            FileSystemEntity entity = args.Entity;
            FileSystemOperation operation = args.Operation;
            string oldPath = args.OldPath;

            var sb = new StringBuilder();

            sb.Append(operation);
            sb.Append(":");
            while (sb.Length < 15)
            {
                sb.Append(" ");
            }

            if (((operation == FileSystemOperation.FolderRename) || (operation == FileSystemOperation.FileRename)))
            {
                sb.Append($"{oldPath} => {entity.FullPath}");
            }
            else
            {
                sb.Append(entity.FullPath);
            }

            Console.WriteLine(sb);
        }

        private static void BigFolderSyncTest()
        {
            const string path = @"C:\Temp";

            var folder = new RootFolderEntity(path);
            folder.Sync();

            string xml = Serializer.Serialize(folder, Serializer.Type.DataContractSerializer).OuterXml;
            folder = Serializer.Deserialize<RootFolderEntity>(xml, Serializer.Type.DataContractSerializer);
            folder.OnSync += Log;

            var manager = new MonitorProvider(folder);
        }

        static void Main(string[] args)
        {
            Console.WindowWidth *= 2;
            Console.WindowHeight *= 2;

            Console.WriteLine("Begin.");
            Console.WriteLine();

            try
            {
                //CopyFile();
                //return;

                //BigFolderSyncTest();

                const string masterPath = @"C:\Andrey Popov\My\Shared";
                const string slaveGooglePath = @"C:\Andrey Popov\Cloud\Google Drive (Google)\My";
                const string slaveOneDrivePath = @"C:\Andrey Popov\Cloud\OneDrive\My";
                const string slaveYandexDiskPath = @"C:\Andrey Popov\Cloud\YandexDisk\My";
                const string slaveDropboxPath = @"C:\Andrey Popov\Cloud\Dropbox\My";


                var masterFolder = new RootFolderEntity(masterPath);
                masterFolder.Sync();

                string xml = Serializer.Serialize(masterFolder, Serializer.Type.DataContractSerializer).OuterXml;
                masterFolder = Serializer.Deserialize<RootFolderEntity>(xml, Serializer.Type.DataContractSerializer);

                masterFolder.OnSync += Log;

                FileSystemEntity[] entities = masterFolder.ToList();
                foreach (FileSystemEntity entity in entities)
                {
                    //Console.WriteLine($"{ entity.RelativePath}");
                    var ident = new StringBuilder();
                    for (int i = 0; i < entity.Level; i++)
                    {
                        ident.Append(" ");
                    }

                    if (entity is FolderEntity)
                    {
                        Console.WriteLine($"{ident}[{entity.Name}]");
                    }
                    else
                    {
                        Console.WriteLine($"{ident}{entity.Name}");
                    }
                }

                //var manager = new MonitorProvider(folder);

                var slaveGoogleFolder = new RootFolderEntity(slaveGooglePath);
                var slaveOneDriveFolder = new RootFolderEntity(slaveOneDrivePath);
                var slaveYandexDiskFolder = new RootFolderEntity(slaveYandexDiskPath);
                var slaveDropboxFolder = new RootFolderEntity(slaveDropboxPath);

                //_provider = new SyncProvider(masterFolder, slaveGoogleFolder);
                _provider = new SyncProvider(masterFolder, true, slaveGoogleFolder, slaveOneDriveFolder, slaveYandexDiskFolder, slaveDropboxFolder);

                Console.WriteLine("Success.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception.");
                Console.WriteLine("Error={0}", ex.ToTraceString());
            }
            Console.WriteLine();
            Console.WriteLine("Press 'Enter' to exit.");
            Console.ReadLine();
        }
    }
}