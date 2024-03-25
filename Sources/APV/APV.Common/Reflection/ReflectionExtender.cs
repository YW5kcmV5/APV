using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using APV.Common.Extensions;

namespace APV.Common.Reflection
{
    public static class ReflectionExtender
    {
        private static readonly SortedList<int, Action<object, object>> Setters = new SortedList<int, Action<object, object>>();
        private static readonly SortedList<int, Func<object, object>> Getters = new SortedList<int, Func<object, object>>();

        public static Action<object, T> BuildActionAccessor<T>(this MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (method.DeclaringType == null)
                throw new ArgumentOutOfRangeException("method", "(method.DeclaringType == null)");

            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression arg = Expression.Parameter(typeof(T));
            MethodCallExpression body = Expression.Call(Expression.Convert(instance, method.DeclaringType), method, new[] { (Expression)arg });
            Expression<Action<object, T>> expr = Expression.Lambda<Action<object, T>>(body, instance, arg);
            return expr.Compile();
        }

        public static Action<object, T1, T2> BuildActionAccessor<T1, T2>(this MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (method.DeclaringType == null)
                throw new ArgumentOutOfRangeException("method", "(method.DeclaringType == null)");

            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression arg1 = Expression.Parameter(typeof(T1), "arg1");
            ParameterExpression arg2 = Expression.Parameter(typeof(T2), "arg2");
            MethodCallExpression body = Expression.Call(Expression.Convert(instance, method.DeclaringType), method, arg1, arg2);
            Expression<Action<object, T1, T2>> expr = Expression.Lambda<Action<object, T1, T2>>(body, instance, arg1, arg2);
            return expr.Compile();
        }

        public static Action<object, T1, T2, T3> BuildActionAccessor<T1, T2, T3>(this MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (method.DeclaringType == null)
                throw new ArgumentOutOfRangeException("method", "(method.DeclaringType == null)");

            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression arg1 = Expression.Parameter(typeof(T1), "arg1");
            ParameterExpression arg2 = Expression.Parameter(typeof(T2), "arg2");
            ParameterExpression arg3 = Expression.Parameter(typeof(T3), "arg3");
            MethodCallExpression body = Expression.Call(Expression.Convert(instance, method.DeclaringType), method, arg1, arg2, arg3);
            Expression<Action<object, T1, T2, T3>> expr = Expression.Lambda<Action<object, T1, T2, T3>>(body, instance, arg1, arg2, arg3);
            return expr.Compile();
        }

        public static Action<object, T1, T2, T3, T4> BuildActionAccessor<T1, T2, T3, T4>(this MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (method.DeclaringType == null)
                throw new ArgumentOutOfRangeException("method", "(method.DeclaringType == null)");

            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression arg1 = Expression.Parameter(typeof(T1), "arg1");
            ParameterExpression arg2 = Expression.Parameter(typeof(T2), "arg2");
            ParameterExpression arg3 = Expression.Parameter(typeof(T3), "arg3");
            ParameterExpression arg4 = Expression.Parameter(typeof(T4), "arg4");
            MethodCallExpression body = Expression.Call(Expression.Convert(instance, method.DeclaringType), method, arg1, arg2, arg3, arg4);
            Expression<Action<object, T1, T2, T3, T4>> expr = Expression.Lambda<Action<object, T1, T2, T3, T4>>(body, instance, arg1, arg2, arg3, arg4);
            return expr.Compile();
        }

        public static Func<object, T, TResult> BuildFuncAccessor<T, TResult>(this MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (method.DeclaringType == null)
                throw new ArgumentOutOfRangeException("method", "(method.DeclaringType == null)");

            ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
            ParameterExpression arg = Expression.Parameter(typeof(T));
            MethodCallExpression call = Expression.Call(Expression.Convert(instance, method.DeclaringType), method, arg);
            UnaryExpression body = Expression.Convert(call, typeof(TResult));
            Expression<Func<object, T, TResult>> expr = Expression.Lambda<Func<object, T, TResult>>(body, instance, arg);
            return expr.Compile();
        }

        public static Func<object, T1, T2, TResult> BuildFuncAccessor<T1, T2, TResult>(this MethodInfo method)
        {
            return BuildFuncAccessor<object, T1, T2, TResult>(method);
        }

        public static Func<TInstance, T1, T2, TResult> BuildFuncAccessor<TInstance, T1, T2, TResult>(this MethodInfo method) where TInstance : class
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (method.DeclaringType == null)
                throw new ArgumentOutOfRangeException("method", "(method.DeclaringType == null)");

            ParameterExpression instance = Expression.Parameter(typeof(TInstance), "instance");
            ParameterExpression arg1 = Expression.Parameter(typeof(T1));
            ParameterExpression arg2 = Expression.Parameter(typeof(T2));
            MethodCallExpression call = Expression.Call(Expression.Convert(instance, method.DeclaringType), method, arg1, arg2);
            UnaryExpression body = Expression.Convert(call, typeof(TResult));
            Expression<Func<object, T1, T2, TResult>> expr = Expression.Lambda<Func<object, T1, T2, TResult>>(body, instance, arg1, arg2);
            return expr.Compile();
        }

        public static Func<object, object> BuildGetAccessor(this MethodInfo method)
        {
            if (method == null)
                throw new ArgumentNullException("method");
            if (method.DeclaringType == null)
                throw new ArgumentOutOfRangeException("method", "(method.DeclaringType == null)");

            ParameterExpression obj = Expression.Parameter(typeof(object), "obj");
            UnaryExpression body = Expression.Convert(Expression.Call(Expression.Convert(obj, method.DeclaringType), method), typeof(object));
            Expression<Func<object, object>> expr = Expression.Lambda<Func<object, object>>(body, obj);
            return expr.Compile();
        }

        public static Func<object, object> BuildGetAccessor(this FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException("field");
            if (field.DeclaringType == null)
                throw new ArgumentOutOfRangeException("field", "(field.DeclaringType == null)");

            ParameterExpression obj = Expression.Parameter(typeof(object), "obj");
            Expression<Func<object, object>> expr = Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Field(Expression.Convert(obj, field.DeclaringType), field), typeof(object)), obj);
            return expr.Compile();
        }

        public static Action<object, object> BuildSetAccessor(this FieldInfo field)
        {
            if (field == null)
                throw new ArgumentNullException("field");
            if (field.DeclaringType == null)
                throw new ArgumentOutOfRangeException("field", "(field.DeclaringType == null)");

            var instance = Expression.Parameter(typeof(Object), "i");
            var castedInstance = Expression.ConvertChecked(instance, field.DeclaringType);
            var argument = Expression.Parameter(typeof(object), "a");
            var setter = Expression.Assign(Expression.Field(castedInstance, field), Expression.Convert(argument, field.FieldType));
            return Expression.Lambda<Action<object, object>>(setter, instance, argument).Compile();
        }

        public static Func<object, object> BuildGetAccessor(this PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (property.DeclaringType == null)
                throw new ArgumentOutOfRangeException("property", "(property.DeclaringType == null)");

            ParameterExpression obj = Expression.Parameter(typeof(object), "obj");
            Expression<Func<object, object>> expr = Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Property(Expression.Convert(obj, property.DeclaringType), property), typeof(object)), obj);
            return expr.Compile();
        }

        public static Action<object, object> BuildSetAccessor(this PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (property.DeclaringType == null)
                throw new ArgumentOutOfRangeException("property", "(property.DeclaringType == null)");
            if (property.IsStatic())
                throw new ArgumentOutOfRangeException("property", "(property.IsStatic())");

            ParameterExpression instance = Expression.Parameter(typeof(Object), "i");
            UnaryExpression castedInstance = Expression.ConvertChecked(instance, property.DeclaringType);
            var argument = Expression.Parameter(typeof(object), "a");
            var setter = Expression.Assign(Expression.Property(castedInstance, property), Expression.Convert(argument, property.PropertyType));
            return Expression.Lambda<Action<object, object>>(setter, instance, argument).Compile();
        }

        public static Action<object> BuildStaticSetAccessor(this PropertyInfo property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (property.DeclaringType == null)
                throw new ArgumentOutOfRangeException("property", "(property.DeclaringType == null)");
            if (!property.IsStatic())
                throw new ArgumentOutOfRangeException("property", "(!property.IsStatic())");

            var argument = Expression.Parameter(typeof (object), "argument");
            var setter = Expression.Assign(Expression.Property(null, property), Expression.Convert(argument, property.PropertyType));
            return Expression.Lambda<Action<object>>(setter, argument).Compile();
        }

        public static void SetValue<TInstance, TValue>(this FieldInfo field, TInstance instance, TValue value)
        {
            SetValue(field, (object)instance, (object)value);
        }

        public static void SetValue(this FieldInfo field, object instance, object value)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            int hash = field.GetHashCode();
            Action<object, object> set;
            lock (Setters)
            {
                int index = Setters.IndexOfKey(hash);
                if (index != -1)
                {
                    set = Setters.Values[index];
                }
                else
                {
                    set = BuildSetAccessor(field);
                    Setters.Add(hash, set);
                }
            }

            set(instance, value);
        }

        public static TValue GetValue<TInstance, TValue>(this FieldInfo field, TInstance instance)
        {
            return (TValue)GetValue(field, instance);
        }

        public static object GetValue(this FieldInfo field, object instance)
        {
            if (field == null)
                throw new ArgumentNullException("field");

            int hash = field.GetHashCode();
            Func<object, object> get;
            lock (Setters)
            {
                int index = Getters.IndexOfKey(hash);
                if (index != -1)
                {
                    get = Getters.Values[index];
                }
                else
                {
                    get = BuildGetAccessor(field);
                    Getters.Add(hash, get);
                }
            }

            return get(instance);
        }

        public static void SetValue<TInstance, TValue>(this PropertyInfo property, TInstance instance, TValue value)
        {
            SetValue(property, (object)instance, (object)value);
        }

        public static void SetValue(this PropertyInfo property, object instance, object value)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            int hash = property.GetHashCode();
            Action<object, object> set;
            lock (Setters)
            {
                int index = Setters.IndexOfKey(hash);
                if (index != -1)
                {
                    set = Setters.Values[index];
                }
                else
                {
                    set = BuildSetAccessor(property);
                    Setters.Add(hash, set);
                }
            }

            set(instance, value);
        }

        public static TValue GetValue<TInstance, TValue>(this PropertyInfo property, TInstance instance)
        {
            return (TValue)GetValue(property, instance);
        }

        public static object GetValue(this PropertyInfo property, object instance)
        {
            if (property == null)
                throw new ArgumentNullException("property");

            int hash = property.GetHashCode();
            Func<object, object> get;
            lock (Setters)
            {
                int index = Getters.IndexOfKey(hash);
                if (index != -1)
                {
                    get = Getters.Values[index];
                }
                else
                {
                    get = BuildGetAccessor(property);
                    Getters.Add(hash, get);
                }
            }

            return get(instance);
        }
    }
}