using System;
using System.Collections;
using System.Collections.Generic;
using APV.Math.MathObjects.Helpers;
using APV.Math.MathObjects.Matrixes;

namespace APV.Math.MathObjects.Vectors
{
    /// <summary>
    /// Вектор целых (int) чисел.
    /// </summary>
    [Serializable]
    public class IntVector : ICloneable, IEnumerable<int>, IComparable<IntVector>, IEquatable<IntVector>
    {
        private int _length;
        private int[] _value;

        private int? _summa;
        private int? _min;
        private int? _max;
        private int? _averange;
        private bool? _isSorted;
        private bool? _isRegular;
        private int? _minDx;
        private int? _maxDx;
        private float? _module;

        private void InvokeOnChange(int index, int value)
        {
            _isSorted = null;
            _isRegular = null;
            _min = null;
            _max = null;
            _summa = null;
            _averange = null;
            _minDx = null;
            _maxDx = null;
            _module = null;

            OnChange?.Invoke(this, index, value);
        }

        public delegate void OnChangeDelegate(IntVector sender, int index, int value);

        #region Constructors

        public IntVector()
        {
            _value = new int[0];
            Length1 = 0;
        }

        public IntVector(int length)
        {
            _value = new int[length];
            Length1 = _value.Length;
        }

        public IntVector(int length, int value)
        {
            Length1 = length;
            _value = new int[length];
            if (value != 0)
            {
                for (int i = 0; i < length; i++)
                {
                    _value[i] = value;
                }
            }
        }

        public IntVector(IntVector value)
        {
            int[] array = value._value;
            Length1 = value.Length1;
            _value = new int[Length1];
            Array.Copy(array, _value, Length1);
            OnChange = value.OnChange;

            _isSorted = value._isSorted;
            _isRegular = value._isRegular;
            _min = value._min;
            _min = value._min;
            _summa = value._summa;
            _averange = value._averange;
            _minDx = value._minDx;
            _maxDx = value._maxDx;
        }

        public IntVector(int[] value)
        {
            _value = value;
            Length1 = value.Length;
        }

        public IntVector(IntVector[] value)
        {
            var values = new List<int>();
            for (int i = 0; i < value.Length; i++)
            {
                values.AddRange(value[i]._value);
            }
            _value = values.ToArray();
            Length1 = _value.Length;
        }

        #endregion

        #region ICloneable

        public object Clone()
        {
            return new IntVector(this);
        }

        #endregion

        #region Base Operations

        /// <summary>
        /// Скалярное сложение векторов
        /// </summary>
        public IntVector Add(IntVector y)
        {
            //if (length != y._length)
            //	throw new ArgumentOutOfRangeException("y");

            var value = _value;
            var yvalue = y._value;
            int length = Length1;

            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = value[i] + yvalue[i];
            }
            return new IntVector(result);
        }

        /// <summary>
        /// Скалярное вычитание векторов
        /// </summary>
        public IntVector Sub(IntVector y)
        {
            var value = _value;
            var yvalue = y._value;
            int length = Length1;

            //if (length != y._length)
            //    throw new ArgumentOutOfRangeException("y");

            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = value[i] - yvalue[i];
            }
            return new IntVector(result);
        }

        public IntVector Mul(IntVector y)
        {
            var value = _value;
            var yvalue = y._value;
            int length = Length1;

            //if (length != y._length)
            //    throw new ArgumentOutOfRangeException("y");

            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = value[i] * yvalue[i];
            }
            return new IntVector(result);
        }

        public IntVector Mul(IntMatrix y)
        {
            var xlength = Length1;
            var rlength = y.ColumnsCount;
            var ycolumns = rlength;
            var yvalue = y.Value;
            var value = _value;
            var result = new int[rlength];
            // ReSharper disable once TooWideLocalVariableScope
            int s;
            // ReSharper disable once TooWideLocalVariableScope
            int index;

            for (int i = 0; i < rlength; i++)
            {
                s = 0;
                for (int j = 0; j < xlength; j++)
                {
                    index = j * ycolumns + i;
                    s += value[j] * yvalue[index];
                }
                result[i] = s;
            }

            return new IntVector(result);
        }

        public IntVector Add(int y)
        {
            var value = _value;
            int length = Length1;
            var result = new int[length];

            int i;
            for (i = 0; i < length; i++)
            {
                result[i] = value[i] + y;
            }

            return new IntVector(result);
        }

        public IntVector Sub(int y)
        {
            var value = _value;
            int length = Length1;
            var result = new int[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = value[i] - y;
            }

            return new IntVector(result);
        }

        public IntVector Mul(int y)
        {
            var value = _value;
            int length = Length1;
            var result = new int[length];

            int i;
            for (i = 0; i < length; i++)
            {
                result[i] = value[i] * y;
            }

            return new IntVector(result);
        }

        public IntVector Div(int y)
        {
            var value = _value;
            int length = Length1;

            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = value[i] / y;
            }
            return new IntVector(result);
        }

        /// <summary>
        /// Скалярное произведение векторов
        /// </summary>
        public int ScalarMul(IntVector y)
        {
            var value = _value;
            var yvalue = y._value;
            int length = Length1;

            //if (length != y._length)
            //    throw new ArgumentOutOfRangeException("y");

            int result = 0;
            for (int i = 0; i < length; i++)
            {
                result += value[i] * yvalue[i];
            }
            return result;
        }

        /// <summary>
        /// Определят перпендикулярны ли вектора
        /// </summary>
        public bool IsOrtogonalTo(IntVector y)
        {
            return ScalarMul(y) == 0;
        }

        /// <summary>
        /// Возвращает косинус угла между векторами
        /// </summary>
        public float Cos(IntVector y)
        {
            return ScalarMul(y) / Module * y.Module;
        }

        #endregion

        #region Compare Operation

        public bool IsEqual(IntVector y)
        {
            return DefaultComparator.Compare(this, y) == 0;
        }

        #region IComparable

        public int CompareTo(IntVector other)
        {
            return DefaultComparator.Compare(this, other);
        }

        #endregion

        #region IEquatable

        public bool Equals(IntVector y)
        {
            return (DefaultComparator.Compare(this, y) == 0);
        }

        #endregion

        public override bool Equals(object obj)
        {
            return (DefaultComparator.Compare(this, obj as IntVector) == 0);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _value.GetHashCode();
        }

        #endregion

        #region Type conversion

        public static implicit operator IntMatrix(IntVector x)
        {
            int length = x.Length1;
            var result = new IntMatrix(1, length);
            Array.Copy(x.Value, result.Value, length);
            return result;
        }

        public static explicit operator int[] (IntVector x)
        {
            int length = x.Length1;
            var result = new int[length];
            Array.Copy(x._value, result, length);
            return result;
        }

        #endregion

        #region Implicit Operations

        public static IntVector operator +(IntVector x, IntVector y)
        {
            return x.Add(y);
        }

        public static IntVector operator +(IntVector x, int y)
        {
            return x.Add(y);
        }

        public static IntVector operator +(int x, IntVector y)
        {
            return y.Add(x);
        }

        public static IntVector operator -(IntVector x, IntVector y)
        {
            return x.Sub(y);
        }

        public static IntVector operator -(IntVector x, int y)
        {
            return x.Sub(y);
        }

        public static IntVector operator -(int x, IntVector y)
        {
            int length = y.Length1;
            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = x - y._value[i];
            }
            return new IntVector(result);
        }

        public static IntVector operator *(IntVector x, IntVector y)
        {
            return x.Mul(y);
        }

        public static IntVector operator *(IntVector x, IntMatrix y)
        {
            return x.Mul(y);
        }

        public static IntVector operator *(IntVector x, int y)
        {
            return x.Mul(y);
        }

        public static IntVector operator *(int x, IntVector y)
        {
            return y.Mul(x);
        }

        public static IntVector operator /(IntVector x, int y)
        {
            return x.Div(y);
        }

        /// <summary>
        /// Скалярное умножение между векторами
        /// </summary>
        public static int operator |(IntVector x, IntVector y)
        {
            return x.ScalarMul(y);
        }

        public static bool operator ==(IntVector x, IntVector y)
        {
            return (DefaultComparator.Compare(x, y) == 0);
        }

        public static bool operator !=(IntVector x, IntVector y)
        {
            return (DefaultComparator.Compare(x, y) != 0);
        }

        public static bool operator <(IntVector x, IntVector y)
        {
            return (DefaultComparator.Compare(x, y) == -1);
        }

        public static bool operator <=(IntVector x, IntVector y)
        {
            int i = DefaultComparator.Compare(x, y);
            return ((i == -1) || (i == 0));
        }

        public static bool operator >(IntVector x, IntVector y)
        {
            return (DefaultComparator.Compare(x, y) == 1);
        }

        public static bool operator >=(IntVector x, IntVector y)
        {
            int i = DefaultComparator.Compare(x, y);
            return ((i == 1) || (i == 0));
        }

        #endregion

        #region Value

        public int[] Value
        {
            get { return _value; }
        }

        #endregion

        #region Additional Properties (Methods)

        /// <summary>
        /// Длина вектора
        /// </summary>
        public float Module
        {
            get
            {
                if (_module == null)
                {
                    _module = 0.0f;
                    int length = Length1;
                    for (int i = 0; i < length; i++)
                    {
                        _module += _value[i] * _value[i];
                    }
                    _module = (int)System.Math.Sqrt(_module.Value);
                }
                return _module.Value;
            }
        }

        /// <summary>
        /// Длина вектора
        /// </summary>
        public int Length
        {
            get { return Length1; }
        }

        /// <summary>
        /// Сумма всех элементов вектора
        /// </summary>
        public int Summa
        {
            get
            {
                if (_summa == null)
                {
                    int length = Length1;
                    int s = _value[0];
                    for (int i = 1; i < length; i++)
                    {
                        s = s + _value[i];
                    }
                    _summa = s;
                }
                return _summa.Value;
            }
        }

        /// <summary>
        /// Минимальное значение
        /// </summary>
        public int Min
        {
            get
            {
                if (_min == null)
                {
                    int length = Length1;
                    int min = _value[0];
                    for (int i = 1; i < length; i++)
                    {
                        if (_value[i] < min)
                        {
                            min = _value[i];
                        }
                    }
                    _min = min;
                }
                return _min.Value;
            }
        }

        /// <summary>
        /// Максимально значение
        /// </summary>
        public int Max
        {
            get
            {
                if (_max == null)
                {
                    int length = Length1;
                    int max = _value[0];
                    for (int i = 1; i < length; i++)
                    {
                        if (_value[i] > max)
                        {
                            max = _value[i];
                        }
                    }
                    _max = max;
                }
                return _max.Value;
            }
        }

        /// <summary>
        /// Определяет упорядочены ли значения в векторе по возрастанию
        /// </summary>
        public bool IsSorted
        {
            get
            {
                if (_isSorted == null)
                {
                    _isSorted = true;
                    for (int i = 1; i < Length1; i++)
                    {
                        if (_value[i] <= _value[i - 1])
                        {
                            _isSorted = false;
                            break;
                        }
                    }
                }
                return _isSorted.Value;
            }
        }

        /// <summary>
        /// Определяет минимальную разность между узлами сетки (если значения,
        /// равномерно распределены в векторе, то MinDx равно RegularDx)
        /// </summary>
        public int MinDx
        {
            get
            {
                if (_minDx == null)
                {
                    _minDx = 0;
                    if (Length1 != 0)
                    {
                        _minDx = _value[1] - _value[0];
                        for (int i = 2; i < Length1; i++)
                        {
                            int dx = _value[i] - _value[i - 1];
                            if (dx < _minDx)
                            {
                                _minDx = dx;
                            }
                        }
                    }
                }
                return _minDx.Value;
            }
        }

        /// <summary>
        /// Определяет максимальную разность между узлами сетки (если значения,
        /// равномерно распределены в векторе, то MaxDx равно RegularDx)
        /// </summary>
        public int MaxDx
        {
            get
            {
                if (_maxDx == null)
                {
                    _maxDx = 0;
                    if (Length1 != 0)
                    {
                        _maxDx = _value[1] - _value[0];
                        for (int i = 2; i < Length1; i++)
                        {
                            int dx = _value[i] - _value[i - 1];
                            if (dx > _maxDx)
                            {
                                _maxDx = dx;
                            }
                        }
                    }
                }
                return _maxDx.Value;
            }
        }

        /// <summary>
        /// Среднее значение
        /// </summary>
        public int Averange
        {
            get
            {
                if (_averange == null)
                {
                    _averange = Summa / Length;
                }
                return _averange.Value;
            }
        }

        /// <summary>
        /// Добавляет значение к концу вектора
        /// </summary>
        public IntVector AddValue(int x, bool clone)
        {
            int length = Length1;
            var result = new int[length + 1];
            Array.Copy(_value, result, length);
            result[length] = x;

            if (clone)
            {
                return new IntVector(result);
            }

            _value = result;
            Length1 = length + 1;
            InvokeOnChange(length, x);
            return this;
        }

        public IntVector AddValue(int x)
        {
            return AddValue(x, true);
        }

        /// <summary>
        /// Добавляет набор значений к концу вектора (конкатенация)
        /// </summary>
        public IntVector AddValue(IntVector x, bool clone)
        {
            int xlength = x.Length1;
            int length = Length1;
            var result = new int[length + xlength];

            Array.Copy(_value, result, length);
            Array.Copy(x._value, 0, result, length, xlength);

            if (clone)
            {
                return new IntVector(result);
            }

            _value = result;
            Length1 = length + xlength;
            return this;
        }

        public IntVector AddValue(IntVector x)
        {
            return AddValue(x, true);
        }

        /// <summary>
        /// Разрядить набор. Удалить близкие по значению отсчеты
        /// </summary>
        public IntVector Discharge()
        {
            int minDx = (Max - Min) / Length;
            var nx = new List<int>();
            for (int i = 1; i < Length1; i++)
            {
                int x = _value[i];
                int px = _value[i - 1];
                int dx = (x - px);
                dx = (dx >= 0.0f) ? dx : -dx;
                if (dx > minDx)
                {
                    nx.Add(x);
                }
            }
            return new IntVector(nx.ToArray());
        }

        /// <summary>
        /// Добавляет новое значение в конец вектора при этом смещая все элементы в начало, 
        /// первый элемент удаляется, т.е. длина вектора не меняется.
        /// </summary>
        public IntVector MoveValue(int x, bool clone)
        {
            int length = Length1;
            if (length == 0)
                throw new InvalidOperationException("The length of vector can not be 0");

            if (clone)
            {
                var result = new int[length];
                Array.Copy(_value, 1, result, 0, length - 1);
                result[length - 1] = x;
                return new IntVector(result);
            }

            int index;
            for (int i = 1; i < length; i++)
            {
                index = i - 1;
                int value = _value[i];
                _value[index] = value;
                InvokeOnChange(i, value);
            }
            index = length - 1;
            _value[index] = x;
            InvokeOnChange(index, x);

            return this;
        }

        public IntVector MoveValue(int x)
        {
            return MoveValue(x, true);
        }

        public IntVector Cut(int index, int length)
        {
            var result = new int[length];
            Array.Copy(_value, index, result, 0, length);
            return new IntVector(result);
        }

        /// <summary>
        /// Меняет местами два элемента в векторе
        /// </summary>
        public void Swap(int index1, int index2)
        {
            int x1 = _value[index1];
            int x2 = _value[index2];
            _value[index1] = x2;
            InvokeOnChange(index1, x2);
            _value[index2] = x1;
            InvokeOnChange(index2, x1);
        }

        /// <summary>
        /// Сортировка вектора в порядке увеличения значений
        /// </summary>
        public IntVector Sort(bool clone)
        {
            if (clone)
            {
                var result = new List<int>(_value);
                result.Sort();
                return new IntVector(result.ToArray());
            }
            QuickSort<int>.Sort(_value, Swap);
            return this;
        }

        public IntVector Sort()
        {
            return Sort(true);
        }

        public int[] ToArray()
        {
            return (int[])this;
        }

        #endregion

        #region Items

        public int this[int index]
        {
            get
            {
                return _value[index];
            }
            set
            {
                if (_value[index] != value)
                {
                    _value[index] = value;
                    InvokeOnChange(index, value);
                }
            }
        }

        public int First
        {
            get { return this[0]; }
        }

        public int Last
        {
            get { return this[Length1 - 1]; }
        }

        public int Length1 { get => _length; set => _length = value; }

        #endregion

        #region Events

        public readonly OnChangeDelegate OnChange;

        #endregion

        #region IEnumerator

        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            return (new List<int>(_value)).GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return _value.GetEnumerator();
        }

        #endregion

        #region IComparer

        /// <summary>
        /// Сравнение векторов по их длине
        /// </summary>
        [Serializable]
        public class ModuleComparator : IComparer<IntVector>
        {
            public int Compare(IntVector x, IntVector y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(x, null)) return -1;
                if (ReferenceEquals(y, null)) return 1;
                return Comparer.Default.Compare(x.Module, y.Module);
            }

            public static readonly ModuleComparator Instance = new ModuleComparator();
        }

        /// <summary>
        /// Сравнение векторов по возрастанию координат
        /// </summary>
        [Serializable]
        public class CoordinateComparator : IComparer<IntVector>
        {
            public int Compare(IntVector x, IntVector y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(x, null)) return -1;
                if (ReferenceEquals(y, null)) return 1;
                if (x.Length < y.Length) return -1;
                if (x.Length > y.Length) return 1;

                int xDim1 = x.Length1;
                int yDim1 = y.Length1;
                if (xDim1 < yDim1) return -1;
                if (xDim1 > yDim1) return 1;

                bool less = false;
                bool equal = true;
                for (int i = xDim1 - 1; i >= 0; i--)
                {
                    int xi = x[i];
                    int yi = y[i];
                    if (xi != yi)
                    {
                        equal = false;
                        if (xi > yi)
                        {
                            break;
                        }
                        if (xi < yi)
                        {
                            less = true;
                            break;
                        }
                    }
                }
                return (equal) ? 0 : (less) ? -1 : +1;
            }

            public static readonly CoordinateComparator Instance = new CoordinateComparator();
        }

        /// <summary>
        /// Компаратор по умолчанию
        /// </summary>
        private static readonly CoordinateComparator DefaultComparator = CoordinateComparator.Instance;

        #endregion
    }
}
