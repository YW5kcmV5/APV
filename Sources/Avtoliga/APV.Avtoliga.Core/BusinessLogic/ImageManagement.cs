using System;
using System.Drawing;
using System.Linq;
using APV.Avtoliga.Common;
using APV.Avtoliga.Core.DataLayer;
using APV.Avtoliga.Core.Entities;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.EntityFramework.DataLayer;
using APV.Common;
using APV.GraphicsLibrary;
using APV.GraphicsLibrary.Images;

namespace APV.Avtoliga.Core.BusinessLogic
{
    public class ImageManagement : BaseManagement<ImageEntity, ImageCollection, ImageDataLayerManager>
    {
        [AdminAccess]
        public override void Save(ImageEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                entity.Name = null;
                base.Save(entity);
                return;
            }

            using (var transaction = new TransactionScope())
            {
                ImageEntity existing = FindByName(entity.Name);

                if ((existing != null) && (existing.ImageId != entity.ImageId))
                    throw new ArgumentOutOfRangeException("entity", string.Format("Image with name \"{0}\" (\"{1}\") already exists.", existing.Name, existing.FileId));

                base.Save(entity);
                transaction.Commit();
            }

            base.Save(entity);
        }

        [AnonymousAccess]
        public byte[] GetHashCode(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            return ImageContainer.CalcHash(image);
        }

        [AnonymousAccess]
        public ImageEntity GetOriginalImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            byte[] hashCode = GetHashCode(image);
            ImageEntity entity = DatabaseManager.Find(hashCode);
            while ((entity != null) && (entity.OriginalImage != null))
            {
                entity = entity.OriginalImage;
            }
            return entity;
        }

        [AnonymousAccess]
        public Image GetOriginalImage(ImageEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            while ((entity != null) && (entity.OriginalImage != null))
            {
                entity = entity.OriginalImage;
            }
            return GetImage(entity);
        }

        [AnonymousAccess]
        public Image GetImage(ImageEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return entity.Data.ToImage();
        }

        [AnonymousAccess]
        public ImageEntity Create(Image image, string name = null)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            using (var container = new ImageContainer(image))
            {
                byte[] hashCode = container.GetHashCode();

                ImageEntity entity = DatabaseManager.Find(hashCode);
                if (entity != null)
                {
                    if (entity.Name != name)
                    {
                        entity.Name = name;
                        entity.Save();
                    }
                    return entity;
                }

                ImageFormat imageFormat = container.FileFormat;
                byte[] data = container.ToByteArray();
                entity = new ImageEntity
                    {
                        Name = name,
                        Width = image.Width,
                        Height = image.Height,
                        ImageFormat = imageFormat,
                        HashCode = hashCode,
                        Data = data,
                        DataStorage = DataStorage.Database,
                    };

                entity.Save();

                return entity;
            }
        }

        [AnonymousAccess]
        public ImageEntity Create(Image image, Image originalImage)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            using (var container = new ImageContainer(image))
            {
                byte[] hashCode = container.GetHashCode();

                ImageEntity entity = DatabaseManager.Find(hashCode);
                if (entity != null)
                {
                    return entity;
                }

                ImageEntity originalEntity = (originalImage != null)
                                                     ? GetOriginalImage(originalImage) ?? Create(originalImage)
                                                     : null;

                if ((originalEntity != null) && (Comparator.Equals(originalEntity.HashCode, hashCode)))
                {
                    return originalEntity;
                }

                ImageFormat imageFormat = container.FileFormat;
                byte[] data = container.ToByteArray();
                entity = new ImageEntity
                    {
                        Width = image.Width,
                        Height = image.Height,
                        ImageFormat = imageFormat,
                        HashCode = hashCode,
                        Data = data,
                        DataStorage = DataStorage.Database,
                        OriginalImage = originalEntity,
                    };

                entity.Save();

                return entity;
            }
        }

        [AnonymousAccess]
        public ImageEntity Create(Image image, int width, int height)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if ((image.Width == width) && (image.Height == height))
            {
                return Create(image);
            }

            Image scaledImage = image.Resize(width, height);

            return Create(scaledImage, image);
        }

        [AnonymousAccess]
        public ImageEntity Create(Image image, int size)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if ((image.Width <= size) && (image.Height <= size) && ((image.Width == size) || (image.Height == size)))
            {
                return Create(image);
            }

            Image scaledImage = image.Resize(size);

            return Create(scaledImage, image);
        }

        [AnonymousAccess]
        public ImageEntity Create(byte[] data, int? size = null)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Bitmap[] bitmaps = IconContainer.Extract(data);
            Bitmap original = bitmaps.Last();
            ImageEntity originalEntity = Create(original);
            for (int i = 0; i < bitmaps.Length - 1; i++)
            {
                Create(bitmaps[i], original);
            }
            if (size != null)
            {
                return Resize(originalEntity, size.Value);
            }
            return originalEntity;
        }

        [AnonymousAccess]
        public ImageEntity CreateLogo(Image image)
        {
            return Create(image, Constants.LogoSize);
        }

        [AnonymousAccess]
        public ImageEntity Resize(ImageEntity entity, int width, int height)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (entity.IsNew)
                throw new ArgumentOutOfRangeException("entity", "Specified entity is new (is not stored in database).");

            if ((entity.Width == width) && (entity.Height == height))
            {
                return entity;
            }

            Image image = GetOriginalImage(entity);
            return Create(image, width, height);
        }

        [AnonymousAccess]
        public ImageEntity Resize(ImageEntity entity, int size)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (entity.IsNew)
                throw new ArgumentOutOfRangeException("entity", "Specified entity is new (is not stored in database).");

            int maxSize = (entity.Width > entity.Height) ? entity.Width : entity.Height;
            if (maxSize == size)
            {
                return entity;
            }

            Image image = GetOriginalImage(entity);
            return Create(image, size);
        }

        [AnonymousAccess]
        public ImageEntity ResizeHeight(ImageEntity entity, int height)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (entity.IsNew)
                throw new ArgumentOutOfRangeException("entity", "Specified entity is new (is not stored in database).");

            if (entity.Width == height)
            {
                return entity;
            }

            Image image = GetOriginalImage(entity);
            Image scaledImage = image.ResizeHeight(height);

            return Create(scaledImage, image);
        }

        [AnonymousAccess]
        public ImageEntity Find(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            byte[] hashCode = GetHashCode(image);
            return DatabaseManager.Find(hashCode);
        }

        [AnonymousAccess]
        public override ImageEntity FindByName(string name)
        {
            return base.FindByName(name);
        }

        [AdminAccess]
        public override void Delete(ImageEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ImageCollection children = entity.Children;
            foreach (ImageEntity child in children)
            {
                Delete(child);
            }

            base.Delete(entity);
        }

        [AdminAccess]
        public void Delete(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            byte[] hashCode = GetHashCode(image);
            ImageEntity entity = DatabaseManager.Find(hashCode);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        [AdminAccess]
        public void AddImageToSet(BaseEntity entity, Image image)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            if (image == null)
                throw new ArgumentNullException("image");

            ImageEntity imageEntity = Create(image);
            long imageId = imageEntity.Id;
            AddImageToSet(entity, imageId);
        }

        [AdminAccess]
        public void AddImageToSet(BaseEntity entity, long imageId)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            long entityTypeId = entity.GetTypeId();
            long entityId = entity.Id;
            DatabaseManager.AddImageToSet(entityTypeId, entityId, imageId);
        }

        [AdminAccess]
        public void DeleteImageFromSet(BaseEntity entity, long imageId)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            long entityTypeId = entity.GetTypeId();
            long entityId = entity.Id;
            DatabaseManager.DeleteImageFromSet(entityTypeId, entityId, imageId);
        }

        [AdminAccess]
        public void ClearImageSet(BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            long entityTypeId = entity.GetTypeId();
            long entityId = entity.Id;
            DatabaseManager.ClearImageSet(entityTypeId, entityId);
        }

        [AnonymousAccess]
        public long[] FindImageSet(BaseEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            long entityTypeId = entity.GetTypeId();
            long entityId = entity.Id;
            return DatabaseManager.FindImages(entityTypeId, entityId);
        }

        public static readonly ImageManagement Instance = (ImageManagement)EntityFrameworkManager.GetManagement<ImageEntity>();
    }
}