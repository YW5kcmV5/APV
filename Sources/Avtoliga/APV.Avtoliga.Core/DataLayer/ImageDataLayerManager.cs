using System;
using System.Collections.Generic;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.DataLayer;
using APV.Common;

namespace APV.Avtoliga.Core.DataLayer
{
    public class ImageDataLayerManager : BaseDataLayerManager<ImageEntity, ImageCollection>
    {
        public ImageEntity Find(byte[] hashCode)
        {
            if (hashCode == null)
                throw new ArgumentNullException("hashCode");

            const string whereSql = @"WHERE [Image].[HashCode] = @HashCode";
            var @params = new Dictionary<string, object> { { "@HashCode", hashCode } };
            return Find(whereSql, @params);
        }

        public override ImageEntity FindByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            const string whereSql = @"WHERE [File].[Name] = @Name";
            var @params = new Dictionary<string, object> { {"@Name", name} };
            return Find(whereSql, @params);
        }

        public long[] FindImages(long entityTypeId, long entityId)
        {
            const string sql = @"SELECT [ImageId] FROM [ImageSet] WHERE [ImageSet].ObjectType = @EntityTypeId AND [ImageSet].[ObjectId] = @ObjectId";
            var @params = new Dictionary<string, object>
                {
                    {"@EntityTypeId", entityTypeId},
                    {"@ObjectId", entityId}
                };

            return GetKeys(sql, @params);
        }

        public void AddImageToSet(long entityTypeId, long entityId, long imageId)
        {
            if (entityId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("entityId", "Specified entity id is new (is not stored in database).");
            if (imageId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("imageId", "Specified image id is new (is not stored in database).");

            var @params = new Dictionary<string, object>
                {
                    {"@EntityTypeId", entityTypeId},
                    {"@EntityId", entityId},
                    {"@ImageId", imageId}
                };

            const string sql = @"
IF (NOT(EXISTS (SELECT [ImageSet].ImageId FROM [ImageSet] WHERE ([ImageSet].ObjectType = @EntityTypeId) AND ([ImageSet].ObjectId = @EntityId) AND ([ImageSet].ImageId = @ImageId)))) BEGIN
    INSERT INTO [ImageSet] (ObjectId, ObjectType, ImageId) VALUES (@EntityId, @EntityTypeId, @ImageId)
END";
            
            Execute(sql, @params);
        }

        public void DeleteImageFromSet(long entityTypeId, long entityId, long imageId)
        {
            if (entityId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("entityId", "Specified entity id is new (is not stored in database).");
            if (imageId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("imageId", "Specified image id is new (is not stored in database).");

            var @params = new Dictionary<string, object>
                {
                    {"@EntityTypeId", entityTypeId},
                    {"@ObjectId", entityId},
                    {"@ImageId", imageId}
                };

            const string sql = @"DELETE FROM [ImageSet] WHERE ([ImageSet].ObjectType = @EntityTypeId) AND ([ImageSet].ObjectId = @EntityId) AND ([ImageSet].ImageId = @ImageId)";

            Execute(sql, @params);
        }

        public void ClearImageSet(long entityTypeId, long entityId)
        {
            if (entityId == SystemConstants.UnknownId)
                throw new ArgumentOutOfRangeException("entityId", "Specified entity id is new (is not stored in database).");

            var @params = new Dictionary<string, object>
                {
                    {"@EntityTypeId", entityTypeId},
                    {"@ObjectId", entityId},
                };

            const string sql = @"DELETE FROM [ImageSet] WHERE ([ImageSet].ObjectType = @EntityTypeId) AND ([ImageSet].ObjectId = @EntityId)";

            Execute(sql, @params);
        }
    }
}