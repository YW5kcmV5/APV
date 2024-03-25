using APV.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RealtyStorage.Common.RealtySystem.Collections
{
    public static class SortedDictionaryExtender
    {
        private static bool _inited;

        private static FieldInfo _fieldSet;
        private static FieldInfo _fieldRoot;
        private static FieldInfo _fieldLeft;
        private static FieldInfo _fieldRight;
        private static FieldInfo _fieldItem;

        private static Func<object, object> _getterSet;
        private static Func<object, object> _getterRoot;
        private static Func<object, object> _getterLeft;
        private static Func<object, object> _getterRight;
        private static Func<object, object> _getterItem;

        private static void Init(object instance, out object root)
        {
            if (!_inited)
            {
                _inited = true;

                Type type = instance.GetType();
                _fieldSet = type.GetField("_set", BindingFlags.NonPublic | BindingFlags.Instance);
                _getterSet = _fieldSet.BuildGetAccessor();
                object set = _getterSet(instance);

                type = set.GetType();
                _fieldRoot = type.GetField("root", BindingFlags.NonPublic | BindingFlags.Instance);
                _getterRoot = _fieldRoot.BuildGetAccessor();
                root = _getterRoot(set);

                if (root != null)
                {
                    type = root.GetType();

                    _fieldLeft = type.GetField("left", BindingFlags.NonPublic | BindingFlags.Instance);
                    _fieldRight = type.GetField("right", BindingFlags.NonPublic | BindingFlags.Instance);
                    _fieldItem = type.GetField("item", BindingFlags.NonPublic | BindingFlags.Instance);

                    _getterLeft = _fieldLeft.BuildGetAccessor();
                    _getterRight = _fieldRight.BuildGetAccessor();
                    _getterItem = _fieldItem.BuildGetAccessor();
                }
            }
            else
            {
                object set = _getterSet(instance);
                root = _getterRoot(set);
            }
        }

        private static object GetLeft(object node)
        {
            return _getterLeft(node);
        }

        private static object GetRight(object node)
        {
            return _getterRight(node);
        }

        private static KeyValuePair<TKey, TValue> GetItem<TKey, TValue>(object node)
        {
            return (KeyValuePair<TKey, TValue>)_getterItem(node);
        }

        private static void Traverse<TKey, TValue>(object node, Action<KeyValuePair<TKey, TValue>> action)
        {
            object left = GetLeft(node);
            if (left != null)
            {
                Traverse(left, action);
            }

            action.Invoke(GetItem<TKey, TValue>(node));

            object right = GetRight(node);
            if (right != null)
            {
                Traverse(right, action);
            }
        }

        public static void Traverse<TKey, TValue>(this SortedDictionary<TKey, TValue> instance, Action<KeyValuePair<TKey, TValue>> action)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            object root;
            Init(instance, out root);
            if (root != null)
            {
                Traverse(root, action);
            }
        }

        public static TValue[] Sort<TKey, TValue>(this SortedDictionary<TKey, TValue> instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            int index = 0;
            var array = new TValue[instance.Count];
            Traverse(instance, item =>
            {
                array[index] = item.Value;
                index++;
            });
            return array;
        }
    }
}