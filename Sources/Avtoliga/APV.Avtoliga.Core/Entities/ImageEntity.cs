using System;
using APV.Avtoliga.Core.Entities.Collection;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.GraphicsLibrary;

namespace APV.Avtoliga.Core.Entities
{
    [Serializable]
    [DbTable(TableName = "Image")]
    public sealed class ImageEntity : FileEntity
    {
        private ImageCollection _children;

        #region Constructors

        public ImageEntity()
        {
        }

        public ImageEntity(IEntityCollection container)
            : base(container)
        {
        }

        public ImageEntity(long id)
            : base(id)
        {
        }

        #endregion

        #region Db Fields

        [DbField(Key = true)]
        public long ImageId { get; internal set; }

        [DbField]
        public int Width { get; set; }

        [DbField]
        public int Height { get; set; }

        [DbField]
        public ImageFormat ImageFormat { get; set; }

        [DbField]
        public byte[] HashCode { get; set; }

        [DbField]
        public long? OriginalImageId { get; internal set; }

        #endregion

        #region Foreign Keys

        public ImageEntity OriginalImage
        {
            get { return GetKeyValue<ImageEntity>(() => OriginalImageId); }
            set { SetKeyValue(() => OriginalImageId, value); }
        }

        #endregion

        #region Collections

        public ImageCollection Children
        {
            get { return GetCollection<ImageCollection, ImageEntity>(() => Children); }
        }

        #endregion

        public int Size
        {
            get { return (Width > Height) ? Width : Height; }
        }
    }
}