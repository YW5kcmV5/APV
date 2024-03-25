using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using APV.EntityFramework;
using APV.EntityFramework.Interfaces;
using APV.Common;
using APV.Common.Extensions;
using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;
using APV.Pottle.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace APV.Pottle.DatabaseConsoleTest
{
    public static class PerformanceTests
    {
        public const string ConnectionString = @"Data Source=(local);Initial Catalog=Pottle;Integrated Security=True;Pooling=True;Min Pool Size=3;";

        private static SqlConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        private static void CloneImageEntityPerformance()
        {
            Console.WriteLine("CloneImageEntityPerformance.");

            const int count = 50000;
            ImageManagement management = ImageManagement.Instance;
            DataImageEntity entity = management.Get(1);

            DateTime begin = DateTime.UtcNow;

            for (int i = 0; i < count; i++)
            {
                var clone = (DataImageEntity) entity.Clone();
                Assert.IsNotNull(clone);
            }

            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            Console.WriteLine("CloneImageEntityPerformance. Count={0}. Processing={1}mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void GetAllFilesTest()
        {
            Console.WriteLine("GetAllFilesTest.");
            DateTime begin = DateTime.UtcNow;
            var items = new List<DataFileEntity>();
            var command = new SqlCommand(@"SELECT * FROM [File]");
            IDataLayerManager<DataFileEntity> manager = EntityFrameworkManager.GetManager<DataFileEntity>();
            using (SqlConnection connection = CreateConnection())
            {
                command.Connection = connection;
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        DataFileEntity entity = manager.Fill(reader);
                        items.Add(entity);
                    }
                }
            }

            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            int count = items.Count;
            Console.WriteLine("GetAllFilesTest. {0} entities per {1} mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void GetAllFilesThroughDataLayerPerformance()
        {
            Console.WriteLine("GetAllFilesThroughDataLayerPerformance.");
            DateTime begin = DateTime.UtcNow;

            var manager = (FileDataLayerManager)EntityFrameworkManager.GetManager<DataFileEntity>();
            //DataFileCollection items = manager.GetList(@"SELECT * FROM [File];");
            var items = (DataFileCollection)manager.GetAll();

            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            int count = items.Count;
            Console.WriteLine("GetAllFilesThroughDataLayerPerformance. {0} entities per {1} mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void GetAllImagesTest()
        {
            Console.WriteLine("GetAllImagesTest.");
            DateTime begin = DateTime.UtcNow;
            var items = new List<DataImageEntity>();
            var command = new SqlCommand(@"SELECT [Image].*, [File].* FROM [Image] INNER JOIN [FILE] ON [Image].[FileId] = [File].[FileId]");
            IDataLayerManager<DataImageEntity> manager = EntityFrameworkManager.GetManager<DataImageEntity>();
            using (SqlConnection connection = CreateConnection())
            {
                command.Connection = connection;
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult))
                {
                    while (reader.Read())
                    {
                        DataImageEntity entity = manager.Fill(reader);
                        items.Add(entity);
                    }
                }
            }

            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            int count = items.Count;
            Console.WriteLine("GetAllImagesTest. {0} entities per {1} mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void GetAllImagesThroughDataLayerPerformance()
        {
            Console.WriteLine("GetAllImagesThroughDataLayerPerformance.");
            DateTime begin = DateTime.UtcNow;

            var manager = (ImageDataLayerManager)EntityFrameworkManager.GetManager<DataImageEntity>();
            //DataImageCollection items = manager.GetList(@"SELECT [Image].*, [File].* FROM [Image] INNER JOIN [FILE] ON [Image].[FileId] = [File].[FileId]");
            var items = (DataImageCollection)manager.GetAll();

            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            int count = items.Count;
            Console.WriteLine("GetAllImagesThroughDataLayerPerformance. {0} entities per {1} mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void GetFileThroughDataLayerPerformance()
        {
            Console.WriteLine("GetFileThroughDataLayerPerformance.");
            DateTime begin = DateTime.UtcNow;

            const int count = 10000;
            IDataLayerManager<DataFileEntity> manager = EntityFrameworkManager.GetManager<DataFileEntity>();
            for (int i = 0; i < count; i++)
            {
                DataFileEntity entity = manager.Get(1);
                Assert.IsNotNull(entity);
            }

            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            Console.WriteLine("GetFileThroughDataLayerPerformance. Count={0}. Processing={1}mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void GetFileThroughBusinessLayerPerformance()
        {
            Console.WriteLine("GetFileThroughBusinessLayerPerformance.");
            DateTime begin = DateTime.UtcNow;

            const int count = 10000;
            FileManagement management = FileManagement.Instance;
            for (int i = 0; i < count; i++)
            {
                DataFileEntity entity = management.Get(1);
                Assert.IsNotNull(entity);
            }

            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            Console.WriteLine("GetFileThroughBusinessLayerPerformance. Count={0}. Processing={1}mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void GetImageThroughDataLayerPerformance()
        {
            Console.WriteLine("GetImageThroughDataLayerPerformance.");
            DateTime begin = DateTime.UtcNow;

            const int count = 10000;
            IDataLayerManager<DataImageEntity> dataLayerManager = EntityFrameworkManager.GetManager<DataImageEntity>();
            for (int i = 0; i < count; i++)
            {
                DataImageEntity entity = dataLayerManager.Get(1);
                Assert.IsNotNull(entity);
            }

            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            Console.WriteLine("GetImageThroughDataLayerPerformance. Count={0}. Processing={1}mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void GetImageThroughBusinessLayerPerformance()
        {
            Console.WriteLine("GetImageThroughBusinessLayerPerformance.");
            DateTime begin = DateTime.UtcNow;

            const int count = 10000;
            ImageManagement management = ImageManagement.Instance;
            for (int i = 0; i < count; i++)
            {
                DataImageEntity entity = management.Get(1);
                Assert.IsNotNull(entity);
            }

            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            Console.WriteLine("GetImageThroughBusinessLayerPerformance. Count={0}. Processing={1}mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void CreateFileThroughDataLayer()
        {
            var entity = new DataFileEntity
                {
                    Data = new byte[12],
                    DataStorage = DataStorage.Database,
                    Path = null,
                };
            IDataLayerManager<DataFileEntity> dataLayerManager = EntityFrameworkManager.GetManager<DataFileEntity>();
            long id = dataLayerManager.CreateOrUpdate(entity);

            Assert.AreNotEqual(SystemConstants.UnknownId, id);
        }

        private static void CreateFileThroughBusinessLayer()
        {
            var entity = new DataFileEntity
                {
                    Data = new byte[12],
                    DataStorage = DataStorage.Database,
                    Path = null,
                };
            FileManagement management = FileManagement.Instance;
            management.Save(entity);

            long id = entity.FileId;
            Assert.AreNotEqual(SystemConstants.UnknownId, id);
        }

        private static void CreateImageThroughDataLayer()
        {
            byte[] data = Guid.NewGuid().ToByteArray();
            byte[] hashCode = data.Hash256();
            var entity = new DataImageEntity
                {
                    Data = data,
                    HashCode = hashCode,
                    DataStorage = DataStorage.Database,
                    Path = null,
                    Width = 30,
                    Height = 30,
                };
            IDataLayerManager<DataImageEntity> dataLayerManager = EntityFrameworkManager.GetManager<DataImageEntity>();
            long id = dataLayerManager.CreateOrUpdate(entity);
            Assert.AreNotEqual(SystemConstants.UnknownId, id);
        }

        private static void CreateImageThroughBusinessLayer()
        {
            byte[] data = Guid.NewGuid().ToByteArray();
            byte[] hashCode = data.Hash256();
            var entity = new DataImageEntity
                {
                    Data = data,
                    HashCode = hashCode,
                    DataStorage = DataStorage.Database,
                    Path = null,
                    Width = 30,
                    Height = 30,
                };
            ImageManagement management = ImageManagement.Instance;
            management.Save(entity);
            long id = entity.ImageId;
            Assert.AreNotEqual(SystemConstants.UnknownId, id);
        }

        private static void UpdateImageThroughDataLayer(DataImageEntity entity)
        {
            IDataLayerManager<DataImageEntity> dataLayerManager = EntityFrameworkManager.GetManager<DataImageEntity>();
            dataLayerManager.CreateOrUpdate(entity);
        }

        private static void UpdateFileThroughDataLayer(DataFileEntity entity)
        {
            IDataLayerManager<DataFileEntity> dataLayerManager = EntityFrameworkManager.GetManager<DataFileEntity>();
            dataLayerManager.CreateOrUpdate(entity);
        }

        private static void CreateImageThroughDataLayerPerformance()
        {
            Console.WriteLine("CreateImageThroughDataLayerPerformance.");
            const int count = 1000;
            DateTime begin = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                CreateImageThroughDataLayer();
            }
            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            Console.WriteLine("CreateImageThroughDataLayerPerformance. {0} operations per {1} mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void CreateImageThroughBusinessLayerPerformance()
        {
            Console.WriteLine("CreateImageThroughBusinessLayerPerformance.");
            const int count = 1000;
            DateTime begin = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                CreateImageThroughBusinessLayer();
            }
            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            Console.WriteLine("CreateImageThroughBusinessLayerPerformance. {0} operations per {1} mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void UpdateFileThroughDataLayerPerformance()
        {
            IDataLayerManager<DataFileEntity> dataLayerManager = EntityFrameworkManager.GetManager<DataFileEntity>();
            DataFileEntity entity = dataLayerManager.Get(1);

            Console.WriteLine("UpdateFileThroughDataLayerPerformance.");
            const int count = 1000;
            DateTime begin = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                UpdateFileThroughDataLayer(entity);
            }
            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            Console.WriteLine("UpdateFileThroughDataLayerPerformance. {0} operations per {1} mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void UpdateImageThroughDataLayerPerformance()
        {
            IDataLayerManager<DataImageEntity> dataLayerManager = EntityFrameworkManager.GetManager<DataImageEntity>();
            DataImageEntity entity = dataLayerManager.Get(1);

            Console.WriteLine("UpdateImageThroughDataLayerPerformance.");
            const int count = 1000;
            DateTime begin = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                UpdateImageThroughDataLayer(entity);
            }
            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            Console.WriteLine("UpdateImageThroughDataLayerPerformance. {0} operations per {1} mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void CreateFileThroughDataLayerPerformance()
        {
            Console.WriteLine("CreateFileThroughDataLayerPerformance.");
            const int count = 1000;
            DateTime begin = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                CreateFileThroughDataLayer();
            }
            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            Console.WriteLine("CreateFileThroughDataLayerPerformance. {0} operations per {1} mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        private static void CreateFileThroughBusinessLayerPerformance()
        {
            Console.WriteLine("CreateFileThroughBusinessLayerPerformance.");
            const int count = 1000;
            DateTime begin = DateTime.UtcNow;
            for (int i = 0; i < count; i++)
            {
                CreateFileThroughBusinessLayer();
            }
            var processing = (int)(DateTime.UtcNow - begin).TotalMilliseconds;
            Console.WriteLine("CreateFileThroughBusinessLayerPerformance. {0} operations per {1} mlsec ({2} mlsec for 1 operation).", count, processing, (float)processing / count);
            Console.WriteLine();
        }

        public static void Execute()
        {
            try
            {
                TestManager.Login();

                CloneImageEntityPerformance();

                GetFileThroughDataLayerPerformance();
                GetFileThroughBusinessLayerPerformance();

                GetImageThroughDataLayerPerformance();
                GetImageThroughBusinessLayerPerformance();

                CreateFileThroughDataLayerPerformance();
                CreateFileThroughBusinessLayerPerformance();

                CreateImageThroughDataLayerPerformance();
                CreateImageThroughBusinessLayerPerformance();

                UpdateFileThroughDataLayerPerformance();
                UpdateImageThroughDataLayerPerformance();

                GetAllFilesTest();
                GetAllFilesThroughDataLayerPerformance();

                GetAllImagesTest();
                GetAllImagesThroughDataLayerPerformance();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}