using System;
using System.Linq;
using System.Reflection;
using APV.Common.Attributes.Reflection;

namespace APV.Common.Attributes.Db
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DbFieldAttribute : ReflectionPropertyAttribute
    {
        private DbTableAttribute _table;
        private string _fieldName;
        private bool _isPrimaryKey;
        private Action<object, object> _setDbAcceptor;
        private Func<object, object> _getDbAcceptor;
        private bool _isDateTime;
        private bool _isGuid;
        private bool _isIdentifier;
        private bool _isString;
        private DbSpecialField _specialField;
        private bool? _nullable;
        private int? _maxSize;

        #region Acceptors

        private void SetNullableAcceptor(object instance, object value)
        {
            if (value is DBNull)
            {
                value = null;
            }
            SetValue(instance, value);
        }

        private void SetDateTimeAcceptor(object instance, object value)
        {
            var dt = (DateTime)value;
            //dt = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);
            dt = new DateTime(dt.Ticks, DateTimeKind.Utc);
            SetValue(instance, dt);
        }

        private void SetNullableDateTimeAcceptor(object instance, object value)
        {
            if ((value is DBNull) || (value == null))
            {
                SetValue(instance, null);
            }
            else
            {
                var dt = new DateTime(((DateTime)value).Ticks, DateTimeKind.Utc);
                SetValue(instance, dt);
            }
        }

        private object GetNullableAcceptor(object instance)
        {
            object value = GetValue(instance);
            return value ?? DBNull.Value;
        }

        private object GetDateTimeAcceptor(object instance)
        {
            var value = (DateTime)GetValue(instance);
            if (value.Kind != DateTimeKind.Utc)
            {
                value = value.ToUniversalTime();
            }
            return value;
        }

        private object GetNullableDateTimeAcceptor(object instance)
        {
            object value = GetValue(instance);
            if (value == null)
            {
                return DBNull.Value;
            }
            var dt = (DateTime) value;
            if (dt.Kind != DateTimeKind.Utc)
            {
                dt = dt.ToUniversalTime();
            }
            return dt;
        }

        #endregion

        private void DefineDbAcceptor()
        {
            if (Nullable)
            {
                if (_isDateTime)
                {
                    _setDbAcceptor = SetNullableDateTimeAcceptor;
                    _getDbAcceptor = GetNullableDateTimeAcceptor;
                }
                else
                {
                    _setDbAcceptor = SetNullableAcceptor;
                    _getDbAcceptor = GetNullableAcceptor;
                }
            }
            else if (_isDateTime)
            {
                _setDbAcceptor = SetDateTimeAcceptor;
                _getDbAcceptor = GetDateTimeAcceptor;
            }
            else
            {
                _setDbAcceptor = SetValue;
                _getDbAcceptor = GetValue;
            }
        }

        protected override void Init(ReflectionClassAttribute classAttribute, Type instanceType, FieldInfo field)
        {
            _isDateTime = (field.FieldType == typeof(DateTime));
            _isIdentifier = ((field.FieldType == typeof(long)) || (field.FieldType == typeof(long?)));
            _isGuid = (field.FieldType == typeof(Guid));
            _isString = (field.FieldType == typeof(string));
            _isPrimaryKey = (Key) && (field.DeclaringType == instanceType);

            DefineDbAcceptor();
        }

        protected override void Init(ReflectionClassAttribute classAttribute, Type instanceType, PropertyInfo property)
        {
            _isDateTime = (property.PropertyType == typeof(DateTime));
            _isIdentifier = ((property.PropertyType == typeof(long)) || (property.PropertyType == typeof(long?)));
            _isGuid = (property.PropertyType == typeof(Guid));
            _isString = (property.PropertyType == typeof(string));
            _isPrimaryKey = (Key) && (property.DeclaringType == instanceType);

            DefineDbAcceptor();
        }

        public void SetDbValue(object instance, object value)
        {
            _setDbAcceptor(instance, value);
        }

        public object GetDbValue(object instance)
        {
            return _getDbAcceptor(instance);
        }

        public DbTableAttribute Table
        {
            get
            {
                if (_table == null)
                {
                    bool innerTable = (DeclaringType != InstanceType);
                    var classAttribute = (DbTableAttribute) ClassAttribute;
                    _table = innerTable ? classAttribute.InnerTables.SingleOrDefault(table => table.InstanceType == DeclaringType) : classAttribute;
                }
                return _table;
            }
        }

        public string FieldName
        {
            get { return _fieldName ?? (_fieldName = PropertyName); }
            set { _fieldName = value; }
        }

        public DbSpecialField SpecialField
        {
            get { return _specialField; }
            set { _specialField = value; }
        }

        public bool IsDateTime
        {
            get { return _isDateTime; }
        }

        public bool IsIdentifier
        {
            get { return _isIdentifier; }
        }

        public bool IsString
        {
            get { return _isString; }
        }

        public bool IsGuid
        {
            get { return _isGuid; }
        }

        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
        }

        public bool IsSpecialField
        {
            get { return (_specialField != DbSpecialField.None); }
        }

        public bool Key { get; set; }

        /// <summary>
        /// 0 - Unlimited
        /// </summary>
        public int MaxLength
        {
            get
            {
                if (_maxSize == null)
                {
                    if ((_specialField == DbSpecialField.Name) || (_specialField == DbSpecialField.AlternativeName))
                    {
                        _maxSize = SystemConstants.MaxNameLength;
                    }
                    else if (_specialField == DbSpecialField.Description)
                    {
                        _maxSize = SystemConstants.MaxDescriptionLength;
                    }
                    else
                    {
                        _maxSize = 0;
                    }
                }
                return _maxSize.Value;
            }
            set { _maxSize = value; }
        }

        /// <summary>
        /// "False" by default
        /// </summary>
        public bool Nullable
        {
            get
            {
                if (_nullable == null)
                {
                    _nullable = ((PropertyType.IsGenericType) && (PropertyType.GetGenericTypeDefinition() == typeof (Nullable<>)));
                    if ((!_nullable.Value) && (_specialField == DbSpecialField.Description))
                    {
                        _nullable = true;
                    }
                }
                return _nullable.Value;
            }
            set { _nullable = value; }
        }

        public int? Index { get; set; }

        public GenerationMode Generation { get; set; }

        public enum GenerationMode
        {
            None,

            OnCreate,

            OnUpdate,
        }
    }
}