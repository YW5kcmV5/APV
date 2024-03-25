using System;
using APV.EntityFramework.Interfaces;
using APV.Common.Attributes.Db;
using APV.EntityFramework.DataLayer;

namespace APV.TransControl.Core.Entities
{
    [Serializable]
    [DbTable(TableName = "MONUSER.MONCLIENT", CacheLimit = 1000)]
    public class MonClientEntity : BaseEntity
    {
        #region Constructors

        public MonClientEntity()
        {
        }

        public MonClientEntity(IEntityCollection container)
            : base(container)
        {
        }

        public MonClientEntity(long id)
            : base(id)
        {
        }

        #endregion

        #region DbFields

        [DbField(Key = true)]
        public int ClientId { get; internal set; }

        [DbField(Nullable = true, SpecialField = DbSpecialField.Name)]
        public int ParentId { get; internal set; }

        [DbField(Nullable = true, MaxLength = 20)]
        public string Name { get; internal set; }

        [DbField(Nullable = true, MaxLength = 100)]
        public string FullName { get; internal set; }

        [DbField(Nullable = true, MaxLength = 100)]
        public string ObjMask { get; internal set; }

        [DbField(Nullable = true, MaxLength = 100)]
        public string RsvrMask { get; internal set; }

        [DbField(Nullable = true, MaxLength = 100)]
        public string Constr { get; internal set; }

        [DbField(Nullable = true, MaxLength = 100)]
        public string Pswd { get; internal set; }

        [DbField(Nullable = true, MaxLength = 100)]
        public string Phone { get; internal set; }

        [DbField]
        public int Category { get; internal set; }

        [DbField(MaxLength = 4)]
        public byte[] Kind { get; internal set; }

        [DbField(MaxLength = 4)]
        public byte[] Status { get; internal set; }

        [DbField(MaxLength = 4)]
        public byte[] Privilege { get; internal set; }

        [DbField(MaxLength = 4)]
        public byte[] Conprm { get; internal set; }

        [DbField(MaxLength = 4)]
        public byte[] Phoneprm { get; internal set; }

        [DbField(MaxLength = 4)]
        public byte[] Strprm { get; internal set; }

        #endregion
    }
}