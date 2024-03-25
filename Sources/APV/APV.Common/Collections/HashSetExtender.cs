using APV.Common.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace RealtyStorage.Common.RealtySystem.Collections
{
    public static class HashSetExtender
    {
        /*
        public static HashSet<T> BinaryClone<T>(this HashSet<T> from)
        {
            //Serialize
            var stream = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, from);
            stream.Seek(0, SeekOrigin.Begin);
            //Deserialize
            var to = (HashSet<T>)formatter.Deserialize(stream);
            return to;
        }
        */

        /// <summary>
        /// Fast operation
        /// </summary>
        public static HashSet<T> Clone<T>(this HashSet<T> original)
        {
            var clone = (HashSet<T>)FormatterServices.GetUninitializedObject(typeof(HashSet<T>));
            Fields<T>.ComparerSet(clone, Fields<T>.ComparerGet(original));

            if (original.Count == 0)
            {
                Fields<T>.FreeListSet(clone, -1);
            }
            else
            {
                Fields<T>.CountSet(clone, original.Count);
                Fields<T>.BucketsSet(clone, ((Array)Fields<T>.BucketsGet(original)).Clone());
                Fields<T>.SlotsSet(clone, ((Array)Fields<T>.SlotsGet(original)).Clone());
                Fields<T>.FreeListSet(clone, Fields<T>.FreeListGet(original));
                Fields<T>.LastIndexSet(clone, Fields<T>.LastIndexGet(original));
                Fields<T>.VersionSet(clone, Fields<T>.VersionGet(original));
            }

            return clone;
        }

        public static HashSet<T> ReflectionClone<T>(this HashSet<T> original)
        {
            var clone = (HashSet<T>)FormatterServices.GetUninitializedObject(typeof(HashSet<T>));
            Copy(Fields<T>.Comparer, original, clone);

            if (original.Count == 0)
            {
                Fields<T>.FreeList.SetValue(clone, -1);
            }
            else
            {
                Fields<T>.Count.SetValue(clone, original.Count);
                Clone(Fields<T>.Buckets, original, clone);
                Clone(Fields<T>.Slots, original, clone);
                Copy(Fields<T>.FreeList, original, clone);
                Copy(Fields<T>.LastIndex, original, clone);
                Copy(Fields<T>.Version, original, clone);
            }

            return clone;
        }

        /// <summary>
        /// Fast operation
        /// </summary>
        public static void Assign<T>(this HashSet<T> to, HashSet<T> from)
        {
            if (from == null)
                throw new ArgumentNullException("from");

            Fields<T>.ComparerSet(to, Fields<T>.ComparerGet(from));

            if (from.Count == 0)
            {
                to.Clear();
            }
            else
            {
                Fields<T>.CountSet(to, from.Count);
                Fields<T>.BucketsSet(to, ((Array)Fields<T>.BucketsGet(from)).Clone());
                Fields<T>.SlotsSet(to, ((Array)Fields<T>.SlotsGet(from)).Clone());
                Fields<T>.FreeListSet(to, Fields<T>.FreeListGet(from));
                Fields<T>.LastIndexSet(to, Fields<T>.LastIndexGet(from));
                Fields<T>.VersionSet(to, Fields<T>.VersionGet(from));
            }
        }
        
        private static void Copy<T>(FieldInfo field, HashSet<T> source, HashSet<T> target)
        {
            field.SetValue(target, field.GetValue(source));
        }

        private static void Clone<T>(FieldInfo field, HashSet<T> source, HashSet<T> target)
        {
            field.SetValue(target, ((Array)field.GetValue(source)).Clone());
        }

        private static class Fields<T>
        {
            public static readonly FieldInfo FreeList = GetFieldInfo("m_freeList");
            public static readonly FieldInfo Buckets = GetFieldInfo("m_buckets");
            public static readonly FieldInfo Slots = GetFieldInfo("m_slots");
            public static readonly FieldInfo Count = GetFieldInfo("m_count");
            public static readonly FieldInfo LastIndex = GetFieldInfo("m_lastIndex");
            public static readonly FieldInfo Version = GetFieldInfo("m_version");
            public static readonly FieldInfo Comparer = GetFieldInfo("m_comparer");

            public static readonly Func<object, object> FreeListGet = FreeList.BuildGetAccessor();
            public static readonly Func<object, object> BucketsGet = Buckets.BuildGetAccessor();
            public static readonly Func<object, object> SlotsGet = Slots.BuildGetAccessor();
            public static readonly Func<object, object> LastIndexGet = LastIndex.BuildGetAccessor();
            public static readonly Func<object, object> VersionGet = Version.BuildGetAccessor();
            public static readonly Func<object, object> ComparerGet = Comparer.BuildGetAccessor();

            public static readonly Action<object, object> FreeListSet = FreeList.BuildSetAccessor();
            public static readonly Action<object, object> BucketsSet = Buckets.BuildSetAccessor();
            public static readonly Action<object, object> SlotsSet = Slots.BuildSetAccessor();
            public static readonly Action<object, object> CountSet = Count.BuildSetAccessor();
            public static readonly Action<object, object> LastIndexSet = LastIndex.BuildSetAccessor();
            public static readonly Action<object, object> VersionSet = Version.BuildSetAccessor();
            public static readonly Action<object, object> ComparerSet = Comparer.BuildSetAccessor();

            private static FieldInfo GetFieldInfo(string name)
            {
                return typeof(HashSet<T>).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic);
            }
        }
    }
}
