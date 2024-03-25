using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace RealtyStorage.Common.RealtySystem.Collections
{
    internal sealed class FunctorComparer<T> : IComparer<T>
    {
        // Fields
        private readonly Comparison<T> _comparison;

        // Methods
        public FunctorComparer(Comparison<T> comparison)
        {
            _comparison = comparison;
        }

        public int Compare(T x, T y)
        {
            return _comparison(x, y);
        }
    }

    [Serializable, DebuggerDisplay("Count = {Count}")]
    public class FastList<T> : IList<T>, IList where T : class
    {
        // Fields
        private static readonly T[] _emptyArray;
        private const int DEFAULT_CAPACITY = 300;
        private T[] _items;
        private int _size;
        private int _firstIndex;
        private int _version;

        [NonSerialized]
        private object _syncRoot;

        private static bool IsCompatibleObject(object value)
        {
            if (!(value is T) && ((value != null) || typeof(T).IsValueType))
            {
                return false;
            }
            return true;
        }

        private static void VerifyValueType(object value)
        {
            if (!IsCompatibleObject(value))
            {
                throw new ArgumentException("value");
            }
        }

        private void EnsureCapacity(int min)
        {
            int num = (_items.Length == 0) ? DEFAULT_CAPACITY : (_items.Length * 3);
            if (num < 2 * min)
            {
                num = 3 * min;
            }
            Capacity = num;
        }

        // Methods
        static FastList()
        {
            _emptyArray = new T[0];
        }

        public FastList()
        {
            _items = _emptyArray;
        }

        public FastList(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            var is2 = collection as ICollection<T>;
            if (is2 != null)
            {
                int count = is2.Count;
                _items = new T[count];
                is2.CopyTo(_items, 0);
                _size = count;
            }
            else
            {
                _size = 0;
                _items = new T[4];
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Add(enumerator.Current);
                    }
                }
            }
        }

        public FastList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException("capacity");
            }
            _items = new T[capacity];
        }

        public void Add(T item)
        {
            if (_firstIndex + _size == _items.Length)
            {
                EnsureCapacity(_firstIndex + _size + 1);
            }
            _items[_firstIndex + _size++] = item;
            _version++;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(_size, collection);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }
            if (index > _size)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            var is2 = collection as ICollection<T>;
            if (is2 != null)
            {
                int count = is2.Count;
                if (count > 0)
                {
                    EnsureCapacity(_size + count);
                    if (index < _size)
                    {
                        Array.Copy(_items, _firstIndex + index, _items, _firstIndex + index + count, _size - index);
                    }
                    if (this == is2)
                    {
                        Array.Copy(_items, _firstIndex, _items, _firstIndex + index, index);
                        Array.Copy(_items, (_firstIndex + index + count), _items, (_firstIndex + index * 2), (_size - index));
                    }
                    else
                    {
                        var array = new T[count];
                        is2.CopyTo(array, 0);
                        array.CopyTo(_items, _firstIndex + index);
                    }
                    _size += count;
                }
            }
            else
            {
                using (IEnumerator<T> enumerator = collection.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        Insert(index++, enumerator.Current);
                    }
                }
            }
            _version++;
        }

        public void Insert(int index, T item)
        {
            if (index > _size)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if (_firstIndex + _size == _items.Length)
            {
                EnsureCapacity(_firstIndex + _size + 1);
            }
            if (index == 0)
            {
                if (_firstIndex == 1)
                {
                    EnsureCapacity(_firstIndex + _size + 1);
                }
                _firstIndex--;
            }
            else if (index < _size)
            {
                Array.Copy(_items, _firstIndex + index, _items, _firstIndex + index + 1, _size - index);
            }
            _items[_firstIndex + index] = item;
            _size++;
            _version++;
        }

        public void RemoveAt(int index)
        {
            if (index >= _size)
            {
                throw new ArgumentOutOfRangeException();
            }
            _size--;
            if (index == 0)
            {
                _firstIndex++;
            }
            else if (index < _size)
            {
                Array.Copy(_items, _firstIndex + index + 1, _items, _firstIndex + index, _size - index);
            }
            _items[_firstIndex + _size] = default(T);
            _version++;
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index >= 0)
            {
                RemoveAt(index);
                return true;
            }
            return false;
        }

        public void RemoveRange(int index, int count)
        {
            if ((index < 0) || (count < 0))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if ((_size - index) < count)
            {
                throw new ArgumentException();
            }
            if (count > 0)
            {
                _size -= count;
                if (index < _size)
                {
                    Array.Copy(_items, _firstIndex + index + count, _items, _firstIndex + index, _size - index - _firstIndex);
                }
                Array.Clear(_items, _size, count);
                _version++;
            }
        }

        public void Reverse()
        {
            Reverse(0, Count);
        }

        public void Reverse(int index, int count)
        {
            if ((index < 0) || (count < 0))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if ((_size - index) < count)
            {
                throw new ArgumentException();
            }
            Array.Reverse(_items, _firstIndex + index, count);
            _version++;
        }

        public int Capacity
        {
            get
            {
                return _items.Length;
            }
            private set
            {
                if (value > 0)
                {
                    var destinationArray = new T[value];
                    int newFirstIndex = (value - _size) / 2;
                    if (_size > 0)
                    {
                        Array.Copy(_items, _firstIndex, destinationArray, newFirstIndex, _size);
                    }
                    _items = destinationArray;
                    _firstIndex = newFirstIndex;
                }
                else
                {
                    _items = _emptyArray;
                    _firstIndex = 0;
                }
            }
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(_items, item, _firstIndex, _size);
        }

        public T[] ToArray()
        {
            var destinationArray = new T[_size];
            Array.Copy(_items, _firstIndex, destinationArray, 0, _size);
            return destinationArray;
        }

        public int Count
        {
            get
            {
                return _size;
            }
        }

        public T this[int index]
        {
            get
            {
                if (index >= _size)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                return _items[_firstIndex + index];
            }
            set
            {
                if (index >= _size)
                {
                    throw new ArgumentOutOfRangeException("index");
                }
                _items[_firstIndex + index] = value;
                _version++;
            }
        }

        public bool Contains(T item)
        {
            if (item == null)
            {
                for (int j = _firstIndex; j < _firstIndex + _size; j++)
                {
                    if (_items[j] == null)
                    {
                        return true;
                    }
                }
                return false;
            }
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            for (int i = _firstIndex; i < _firstIndex + _size; i++)
            {
                if (comparer.Equals(_items[i], item))
                {
                    return true;
                }
            }
            return false;
        }

        public int BinarySearch(T item)
        {
            return BinarySearch(0, Count, item, null);
        }

        public int BinarySearch(T item, IComparer<T> comparer)
        {
            return BinarySearch(0, Count, item, comparer);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            if ((index < 0) || (count < 0))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if ((_size - index) < count)
            {
                throw new ArgumentException("index");
            }
            int result = Array.BinarySearch(_items, _firstIndex + index, count, item, comparer);
            return (result > 0) ? result - _firstIndex : result + _firstIndex;
        }

        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, _firstIndex, _size);
                _size = 0;
            }
            _version++;
        }

        public void Sort()
        {
            Sort(0, Count, null);
        }

        public void Sort(IComparer<T> comparer)
        {
            Sort(0, Count, comparer);
        }

        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                throw new ArgumentNullException("comparison");
            }
            if (_size > 0)
            {
                IComparer<T> comparer = new FunctorComparer<T>(comparison);
                Array.Sort(_items, _firstIndex, _size, comparer);
            }
        }

        public void Sort(int index, int count, IComparer<T> comparer)
        {
            if ((index < 0) || (count < 0))
            {
                throw new ArgumentOutOfRangeException("index");
            }
            if ((_size - index) < count)
            {
                throw new ArgumentException();
            }
            Array.Sort(_items, _firstIndex + index, count, comparer);
            _version++;
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(_items, _firstIndex, array, arrayIndex, _size);
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if ((_size - index) < count)
            {
                throw new ArgumentException();
            }
            Array.Copy(_items, _firstIndex + index, array, arrayIndex, count);
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return new ReadOnlyCollection<T>(this);
        }

        bool ICollection<T>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    Interlocked.CompareExchange(ref _syncRoot, new object(), null);
                }
                return _syncRoot;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                VerifyValueType(value);
                this[index] = (T)value;
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if ((array != null) && (array.Rank != 1))
            {
                throw new ArgumentException();
            }
            try
            {
                Array.Copy(_items, _firstIndex, array, arrayIndex, _size);
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException();
            }
        }

        int IList.Add(object item)
        {
            VerifyValueType(item);
            Add((T)item);
            return (Count - 1);
        }

        bool IList.Contains(object item)
        {
            return (IsCompatibleObject(item) && Contains((T)item));
        }

        int IList.IndexOf(object item)
        {
            if (IsCompatibleObject(item))
            {
                return IndexOf((T)item);
            }
            return -1;
        }

        void IList.Insert(int index, object item)
        {
            VerifyValueType(item);
            Insert(index, (T)item);
        }

        void IList.Remove(object item)
        {
            if (IsCompatibleObject(item))
            {
                Remove((T)item);
            }
        }

        // Nested Types
        [Serializable, StructLayout(LayoutKind.Sequential)]
        public struct Enumerator : IEnumerator<T>
        {
            private readonly FastList<T> _list;
            private int _index;
            private readonly int _version;
            private T _current;

            internal Enumerator(FastList<T> list)
            {
                _list = list;
                _index = 0;
                _version = list._version;
                _current = default(T);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                FastList<T> list = _list;
                if ((_version == list._version) && (_index < list._size))
                {
                    _current = list._items[_index];
                    _index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException();
                }
                _index = _list._size + 1;
                _current = default(T);
                return false;
            }

            public T Current
            {
                get
                {
                    return _current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    if ((_index == 0) || (_index == (_list._size + 1)))
                    {
                        throw new InvalidOperationException();
                    }
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                if (_version != _list._version)
                {
                    throw new InvalidOperationException();
                }
                _index = 0;
                _current = default(T);
            }
        }
    }
}
