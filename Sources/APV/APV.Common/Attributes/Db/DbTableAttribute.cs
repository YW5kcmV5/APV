using System;
using System.Collections.Generic;
using System.Linq;
using APV.Common.Attributes.Reflection;

namespace APV.Common.Attributes.Db
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DbTableAttribute : ReflectionClassAttribute
    {
        private readonly SortedList<int, DbFieldAttribute> _allFieldsCache = new SortedList<int, DbFieldAttribute>();
        private readonly SortedList<int, string> _keyFieldNames = new SortedList<int, string>();
        private string _tableName;
        private DbFieldAttribute[] _allFields;
        private DbFieldAttribute[] _fields;
        private DbFieldAttribute[] _allFieldsWithoutPrimaryKey;
        private DbFieldAttribute _primaryKey;
        private DbFieldAttribute _innerKey;
        private DbFieldAttribute _deletedField;
        private DbFieldAttribute _nameField;
        private DbFieldAttribute[] _alternativeNameFields;
        private DbFieldAttribute _descriptionField;
        private DbTableAttribute[] _innerTables;
        private DbOperations? _dbOperations;

        protected override void Init(Type instanceType)
        {
            _allFields = AllProperties.OfType<DbFieldAttribute>().ToArray();
            _allFieldsWithoutPrimaryKey = _allFields.Where(field => !field.IsPrimaryKey).ToArray();
            _fields = Properties.OfType<DbFieldAttribute>().ToArray();
            _primaryKey = _fields.Single(field => field.IsPrimaryKey);
            _deletedField = _fields.SingleOrDefault(field => field.SpecialField == DbSpecialField.Deleted);
            _nameField = _fields.SingleOrDefault(field => field.SpecialField == DbSpecialField.Name);
            _descriptionField = _fields.SingleOrDefault(field => field.SpecialField == DbSpecialField.Description);
            _alternativeNameFields = _fields.Where(field => field.SpecialField == DbSpecialField.AlternativeName).ToArray();

            foreach (DbFieldAttribute dbFieldAttribute in _allFields)
            {
                int hashCode = dbFieldAttribute.PropertyName.GetHashCode();
                _allFieldsCache.Add(hashCode, dbFieldAttribute);
            }
             
            var innerTables = new List<DbTableAttribute>();
            var innerTable = Parent as DbTableAttribute;
            _innerKey = (innerTable != null) ? innerTable.PrimaryKey : null;
            while (innerTable != null)
            {
                innerTables.Add(innerTable);
                innerTable = innerTable.Parent as DbTableAttribute;
            }
            _innerTables = innerTables.ToArray();
        }

        public DbTableAttribute()
        {
            CacheLimit = 10000;
        }

        public string TableName
        {
            get
            {
                return _tableName ?? (_tableName = (ClassName.EndsWith("Entity")
                                                        ? ClassName.Substring(0, ClassName.Length - 6)
                                                        : ClassName));
            }
            set { _tableName = value; }
        }

        /// <summary>
        /// 0 - Unlimited
        /// </summary>
        public int CacheLimit { get; set; }

        public DbOperations Operations
        {
            get
            {
                if (_dbOperations == null)
                {
                    _dbOperations = DbOperations.BaseOperations | AdditionalOperations;
                    if (_deletedField != null)
                    {
                        _dbOperations |= DbOperations.MarkAsDeleted;
                    }
                    if (_nameField != null)
                    {
                        _dbOperations |= DbOperations.GetByName;
                    }
                    if (CacheLimit == 0)
                    {
                        _dbOperations |= DbOperations.GetAll;
                    }
                }
                return _dbOperations.Value;
            }
            set { _dbOperations = value; }
        }

        public DbOperations AdditionalOperations { get; set; }

        public DbFieldAttribute PrimaryKey
        {
            get { return _primaryKey; }
        }

        public DbFieldAttribute InnerKey
        {
            get { return _innerKey; }
        }

        public DbFieldAttribute DeletedField
        {
            get { return _deletedField; }
        }

        public DbFieldAttribute NameField
        {
            get { return _nameField; }
        }

        public DbFieldAttribute[] AlternativeNameFields
        {
            get { return _alternativeNameFields; }
        }

        public DbFieldAttribute DescriptionField
        {
            get { return _descriptionField; }
        }

        public DbFieldAttribute[] AllFields
        {
            get { return _allFields; }
        }

        public DbFieldAttribute[] AllFieldsWithoutPrimaryKey
        {
            get { return _allFieldsWithoutPrimaryKey; }
        }

        public DbFieldAttribute[] Fields
        {
            get { return _fields; }
        }

        public DbTableAttribute[] InnerTables
        {
            get { return _innerTables; }
        }

        public DbFieldAttribute GetDbField(string propertyName)
        {
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            int hashCode = propertyName.GetHashCode();
            int index = _allFieldsCache.IndexOfKey(hashCode);

            if (index == -1)
                throw new ArgumentOutOfRangeException("propertyName", string.Format("No db field found for property \"{0}\".", propertyName));

            return _allFieldsCache.Values[index];
        }

        public object GetDbFieldValue(object instance, string propertyName)
        {
            DbFieldAttribute attribute = GetDbField(propertyName);
            return attribute.GetValue(instance);
        }

        public bool Support(DbOperation operation)
        {
            var operations = (DbOperations) (int) operation;
            return ((Operations & (operations)) == operations);
        }

        public string GetKeyFieldName(Type keyEntityType)
        {
            if (keyEntityType == null)
                throw new ArgumentNullException("keyEntityType");

            lock (_keyFieldNames)
            {
                int key = keyEntityType.FullName.GetHashCode();
                int index = _keyFieldNames.IndexOfKey(key);
                if (index != -1)
                {
                    return _keyFieldNames.Values[index];
                }

                DbTableAttribute attribute = GetAttribute(keyEntityType);
                string fieldName = string.Format("{0}Id", TableName);
                DbFieldAttribute keyField = attribute.Fields.Single(field => !field.IsPrimaryKey && field.IsIdentifier && field.FieldName.EndsWith(fieldName));
                string keyFieldName = keyField.FieldName;

                _keyFieldNames.Add(key, keyFieldName);
                return keyFieldName;
            }
        }

        public static DbTableAttribute GetAttribute(Type instanceType)
        {
            var attribute = GetAttribute<DbTableAttribute>(instanceType);
            return attribute;
        }

        public static DbTableAttribute GetAttribute<TInstance>()
        {
            DbTableAttribute attribute = GetAttribute<TInstance, DbTableAttribute>();
            return attribute;
        }
    }
}