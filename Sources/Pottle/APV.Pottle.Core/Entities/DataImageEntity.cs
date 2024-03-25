using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.GraphicsLibrary;
using APV.Pottle.Core.Entities.Collection;

namespace APV.Pottle.Core.Entities
{
    [Serializable]
    [DbTable(TableName = "Image")]
    public sealed class DataImageEntity : DataFileEntity
    {
        private DataImageCollection _children;

        #region Constructors

        public DataImageEntity()
        {
        }

        public DataImageEntity(IEntityCollection container)
            : base(container)
        {
        }

        public DataImageEntity(long id)
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

        public DataImageEntity OriginalImage
        {
            get { return GetKeyValue<DataImageEntity>(() => OriginalImageId); }
            set { SetKeyValue(() => OriginalImageId, value); }
        }

        #endregion

        #region Collections

        public DataImageCollection Children
        {
            get { return GetCollection<DataImageCollection, DataImageEntity>(() => Children); }
        }

        #endregion

        public int Size
        {
            get { return (Width > Height) ? Width : Height; }
        }
    }
}