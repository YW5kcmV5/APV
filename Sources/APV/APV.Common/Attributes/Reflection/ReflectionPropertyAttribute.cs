using System;
using System.Reflection;
using APV.Common.Reflection;

namespace APV.Common.Attributes.Reflection
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class ReflectionPropertyAttribute : Attribute
    {
        private ReflectionClassAttribute _classAttribute;
        private Type _instanceType;
        private Type _declaringType;
        private Type _propertyType;
        private PropertyInfo _property;
        private FieldInfo _field;
        private string _propertyName;
        private bool _isEnum;
        private Action<object, object> _setAcceptor;
        private Func<object, object> _getAcceptor;

        internal void InternalInit(ReflectionClassAttribute classAttribute, Type instanceType, PropertyInfo property)
        {
            _classAttribute = classAttribute;
            _instanceType = instanceType;
            _declaringType = property.DeclaringType;
            _propertyType = property.PropertyType;
            _property = property;
            _propertyName = property.Name;
            _isEnum = property.PropertyType.IsEnum;
            _setAcceptor = property.BuildSetAccessor();
            _getAcceptor = property.BuildGetAccessor();
            
            Init(classAttribute, instanceType, property);
        }

        internal void InternalInit(ReflectionClassAttribute classAttribute, Type instanceType, FieldInfo field)
        {
            _classAttribute = classAttribute;
            _instanceType = instanceType;
            _declaringType = field.DeclaringType;
            _propertyType = field.FieldType;
            _field = field;
            _propertyName = field.Name;
            _isEnum = field.FieldType.IsEnum;
            _setAcceptor = field.BuildSetAccessor();
            _getAcceptor = field.BuildGetAccessor();

            Init(classAttribute, instanceType, field);
        }

        protected virtual void Init(ReflectionClassAttribute classAttribute, Type instanceType, PropertyInfo property)
        {
        }

        protected virtual void Init(ReflectionClassAttribute classAttribute, Type instanceType, FieldInfo field)
        {
        }

        public object GetValue(object instance)
        {
            if (ReferenceEquals(instance, null))
                throw new ArgumentNullException("instance");

            return _getAcceptor(instance);
        }

        public void SetValue(object instance, object value)
        {
            if (ReferenceEquals(instance, null))
                throw new ArgumentNullException("instance");

            _setAcceptor(instance, value);
        }

        public ReflectionClassAttribute ClassAttribute
        {
            get { return _classAttribute; }
        }

        public Type InstanceType
        {
            get { return _instanceType; }
        }

        public Type DeclaringType
        {
            get { return _declaringType; }
        }

        public Type PropertyType
        {
            get { return _propertyType; }
        }

        public PropertyInfo PropertyInfo
        {
            get { return _property; }
        }

        public FieldInfo FieldInfo
        {
            get { return _field; }
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public bool IsEnum
        {
            get { return _isEnum; }
        }
    }
}