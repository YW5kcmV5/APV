using System;
using System.Drawing;
using System.Linq;
using APV.EntityFramework;
using APV.Common;
using APV.EntityFramework.BusinessLayer.Attributes;
using APV.GraphicsLibrary;
using APV.GraphicsLibrary.Images;
using APV.Pottle.Common;
using APV.Pottle.Core.BusinessLogic.Extensions;
using APV.Pottle.Core.BusinessLogic.Helpers;
using APV.Pottle.Core.DataLayer;
using APV.Pottle.Core.Entities;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.BusinessLogic
{
    public class ImageManagement : BaseManagement<DataImageEntity, DataImageCollection, ImageDataLayerManager>
    {
        [AnonymousAccess]
        public byte[] GetHashCode(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            return ImageContainer.CalcHash(image);
        }

        [AnonymousAccess]
        public DataImageEntity GetOriginalImage(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            byte[] hashCode = GetHashCode(image);
            DataImageEntity entity = DatabaseManager.Find(hashCode);
            while ((entity != null) && (entity.OriginalImage != null))
            {
                entity = entity.OriginalImage;
            }
            return entity;
        }

        [AnonymousAccess]
        public Image GetOriginalImage(DataImageEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            while ((entity != null) && (entity.OriginalImage != null))
            {
                entity = entity.OriginalImage;
            }
            return entity.GetImage();
        }

        [AnonymousAccess]
        public Image GetImage(DataImageEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return entity.Data.ToImage();
        }

        [AnonymousAccess]
        public DataImageEntity Create(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            using (var container = new ImageContainer(image))
            {
                byte[] hashCode = container.GetHashCode();

                DataImageEntity entity = DatabaseManager.Find(hashCode);
                if (entity != null)
                {
                    return entity;
                }

                ImageFormat imageFormat = container.FileFormat;
                byte[] data = container.ToByteArray();
                entity = new DataImageEntity
                    {
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
        public DataImageEntity Create(Image image, Image originalImage)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            using (var container = new ImageContainer(image))
            {
                byte[] hashCode = container.GetHashCode();

                DataImageEntity entity = DatabaseManager.Find(hashCode);
                if (entity != null)
                {
                    return entity;
                }

                DataImageEntity originalEntity = (originalImage != null)
                                                     ? GetOriginalImage(originalImage) ?? Create(originalImage)
                                                     : null;

                if ((originalEntity != null) && (Comparator.Equals(originalEntity.HashCode, hashCode)))
                {
                    return originalEntity;
                }

                ImageFormat imageFormat = container.FileFormat;
                byte[] data = container.ToByteArray();
                entity = new DataImageEntity
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
        public DataImageEntity Create(Image image, int width, int height)
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
        public DataImageEntity Create(Image image, int size)
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
        public DataImageEntity Create(byte[] data, int? size = null)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            Bitmap[] bitmaps = IconContainer.Extract(data);
            Bitmap original = bitmaps.Last();
            DataImageEntity originalEntity = Create(original);
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
        public DataImageEntity CreateLogo(Image image)
        {
            return Create(image, Constants.LogoSize);
        }

        [AnonymousAccess]
        public DataImageEntity CreateIcon(Image image)
        {
            return Create(image, Constants.IconSize);
        }

        [AnonymousAccess]
        public DataImageEntity Resize(DataImageEntity entity, int width, int height)
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
        public DataImageEntity Resize(DataImageEntity entity, int size)
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
        public DataImageEntity ResizeHeight(DataImageEntity entity, int height)
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
        public DataImageEntity Find(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            byte[] hashCode = GetHashCode(image);
            return DatabaseManager.Find(hashCode);
        }

        [AdminAccess]
        public override void Delete(DataImageEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            DataImageCollection children = entity.Children;
            foreach (DataImageEntity child in children)
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
            DataImageEntity entity = DatabaseManager.Find(hashCode);
            if (entity != null)
            {
                Delete(entity);
            }
        }

        public static readonly ImageManagement Instance = (ImageManagement)EntityFrameworkManager.GetManagement<DataImageEntity>();
    }
}