using System;
using System.Collections.Generic;
using System.Reflection;
using APV.Common.Extensions;

namespace APV.Common.Attributes.Reflection
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public abstract class ReflectionClassAttribute : Attribute
    {
        private static readonly SortedList<int, ReflectionClassAttribute> Attributes = new SortedList<int, ReflectionClassAttribute>();

        private Type _instanceType;
        private string _className;
        private ReflectionClassAttribute _parent;
        private ReflectionPropertyAttribute[] _allProperties;
        private ReflectionPropertyAttribute[] _properties;

        public ReflectionClassAttribute Parent
        {
            get { return _parent; }
        }

        public bool Composite
        {
            get { return (_parent != null); }
        }

        private void InternalInit<TAttribute>(Type instanceType) where TAttribute : ReflectionClassAttribute
        {
            Type baseType = instanceType.BaseType;
            _parent = GetAttribute<TAttribute>(baseType);

            _instanceType = instanceType;
            _className = instanceType.Name;

            var allReflectionProperties = new List<ReflectionPropertyAttribute>();
            var reflectionProperties = new List<ReflectionPropertyAttribute>();

            PropertyInfo[] properties = _instanceType.GetAllProperties();
            FieldInfo[] fields = _instanceType.GetAllFields();

            foreach (PropertyInfo property in properties)
            {
                IEnumerable<Attribute> attributes = property.GetCustomAttributes();
                foreach (Attribute attribute in attributes)
                {
                    var propertyAttribute = attribute as ReflectionPropertyAttribute;
                    if (propertyAttribute != null)
                    {
                        propertyAttribute.InternalInit(this, instanceType, property);
                        allReflectionProperties.Add(propertyAttribute);
                        if (property.DeclaringType == instanceType)
                        {
                            reflectionProperties.Add(propertyAttribute);
                        }
                    }
                }
            }

            foreach (FieldInfo field in fields)
            {
                IEnumerable<Attribute> attributes = field.GetCustomAttributes();
                foreach (Attribute attribute in attributes)
                {
                    var propertyAttribute = attribute as ReflectionPropertyAttribute;
                    if (propertyAttribute != null)
                    {
                        propertyAttribute.InternalInit(this, instanceType, field);
                        allReflectionProperties.Add(propertyAttribute);
                        if (field.DeclaringType == instanceType)
                        {
                            reflectionProperties.Add(propertyAttribute);
                        }
                    }
                }
            }

            _allProperties = allReflectionProperties.ToArray();
            _properties = reflectionProperties.ToArray();

            Init(instanceType);
        }

        protected virtual void Init(Type instanceType)
        {
        }

        protected static TAttribute GetAttribute<TAttribute>(Type instanceType) where TAttribute : ReflectionClassAttribute
        {
            lock (Attributes)
            {
                string hash = string.Format("{0}:{1}", instanceType.FullName, typeof(TAttribute).Name);
                int key = hash.GetHashCode();
                int index = Attributes.IndexOfKey(key);
                if (index != -1)
                {
                    return (TAttribute) Attributes.Values[index];
                }
                var attribute = (TAttribute) GetCustomAttribute(instanceType, typeof (TAttribute));

                if (attribute != null)
                {
                    attribute.InternalInit<TAttribute>(instanceType);
                    Attributes.Add(key, attribute);
                }
                return attribute;
            }
        }

        protected static TAttribute GetAttribute<TInstance, TAttribute>() where TAttribute : ReflectionClassAttribute
        {
            Type type = typeof(TInstance);
            return GetAttribute<TAttribute>(type);
        }

        public Type InstanceType
        {
            get { return _instanceType; }
        }

        public string ClassName
        {
            get { return _className; }
        }

        public ReflectionPropertyAttribute[] AllProperties
        {
            get { return _allProperties; }
        }

        public ReflectionPropertyAttribute[] Properties
        {
            get { return _properties; }
        }
    }
}