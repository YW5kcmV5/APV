using System;
using System.Text;
using APV.Math.MathObjects.Vectors;

namespace APV.Math.MathObjects.Matrixes
{
    [Serializable]
    public class IntMatrix : ICloneable
    {
        private int[] _value;
        private int _rowsCount;
        private int _columnsCount;

        private IntVector[] _rows;
        private IntVector[] _columns;
        private bool? _isLowerTriangular;
        private bool? _isUpperTriangular;
        private int[] _l;
        private int[] _u;
        private int[] _lu;
        private int? _tr;
        private Minor _minor;
        private Cofactor _cofactor;
        private IntMatrix _invertible;
        private int? _det;

        private void InvokeOnChange(int row, int column, float value)
        {
            _columns = null;
            _rows = null;
            _isLowerTriangular = null;
            _isUpperTriangular = null;
            _l = null;
            _u = null;
            _lu = null;
            _tr = null;
            _minor = null;
            _cofactor = null;
            _invertible = null;
            _det = null;

            OnChange?.Invoke(this, row, column, value);
        }

        public delegate void OnChangeDelegate(IntMatrix sender, int row, int column, float value);

        #region Constructors

        public IntMatrix(IntMatrix value)
        {
            int[] v = value._value;
            int length = v.Length;
            _value = new int[length];
            _rowsCount = value._rowsCount;
            _columnsCount = value._columnsCount;
            Array.Copy(v, _value, length);
        }

        public IntMatrix(int[] value, int rows, int columns)
        {
            _value = value;
            _rowsCount = rows;
            _columnsCount = columns;
        }

        public IntMatrix(int[,] value)
        {
            _rowsCount = value.GetLength(0);
            _columnsCount = value.GetLength(1);
            _value = new int[_rowsCount * _columnsCount];
            for (int row = 0; row < _rowsCount; row++)
            {
                for (int column = 0; column < _columnsCount; column++)
                {
                    _value[row * _columnsCount + column] = value[row, column];
                }
            }
        }

        public IntMatrix()
        {
        }

        public IntMatrix(int rows, int columns)
        {
            _rowsCount = rows;
            _columnsCount = columns;
            _value = new int[_rowsCount * _columnsCount];
        }

        #endregion

        #region IClonable

        public virtual object Clone()
        {
            return new IntMatrix(this);
        }

        #endregion

        #region Base Operations

        public IntVector Mul(IntVector y)
        {
            var rlength = _rowsCount;
            var yvalue = y.Value;
            var ylength = yvalue.Length;
            var columns = _columnsCount;
            var value = _value;
            var result = new int[rlength];
            // ReSharper disable once TooWideLocalVariableScope
            int s;
            // ReSharper disable once TooWideLocalVariableScope
            int index;

            for (int i = 0; i < rlength; i++)
            {
                s = 0;
                for (int j = 0; j < ylength; j++)
                {
                    index = i * columns + j;
                    s += value[index] * yvalue[j];
                }
                result[i] = s;
            }

            return new IntVector(result);
        }

        public IntMatrix Mul(IntMatrix y)
        {
            //if (y == null)
            //    throw new ArgumentNullException("y");
            //if (_columnsCount != y._rowsCount)
            //    throw new ArgumentOutOfRangeException("y");

            int xRow, yColumn, yRow;
            int s;

            var value = _value;
            var yvalue = y._value;
            int xRows = _rowsCount;
            int xColumns = _columnsCount;
            int yRows = y._rowsCount;
            int yColumns = y._columnsCount;

            int rRows = xRows;
            int rColumns = yColumns;
            int rLength = rRows * rColumns;
            var result = new int[rLength];

            for (xRow = 0; xRow < xRows; xRow++)
            {
                for (yColumn = 0; yColumn < yColumns; yColumn++)
                {
                    s = 0;
                    for (yRow = 0; yRow < yRows; yRow++)
                    {
                        s += value[xRow * xColumns + yRow] * yvalue[yRow * yColumns + yColumn];
                    }
                    result[xRow * rColumns + yColumn] = s;
                }
            }

            return new IntMatrix(result, rRows, rColumns);
        }

        public IntMatrix Add(IntMatrix y)
        {
            //if (y == null)
            //    throw new ArgumentNullException("y");
            //if ((y._rowsCount != _rowsCount) || (y._columnsCount != _columnsCount))
            //    throw new ArgumentOutOfRangeException("y");

            var value = _value;
            var yvalue = y._value;
            int length = value.Length;

            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = value[i] + yvalue[i];
            }

            return new IntMatrix(result, _rowsCount, _columnsCount);
        }

        public IntMatrix Sub(IntMatrix y)
        {
            //if (y == null)
            //    throw new ArgumentNullException("y");
            //if ((y._rowsCount != _rowsCount) || (y._columnsCount != _columnsCount))
            //    throw new ArgumentOutOfRangeException("y");

            var value = _value;
            var yvalue = y._value;
            int length = value.Length;

            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = value[i] - yvalue[i];
            }

            return new IntMatrix(result, _rowsCount, _columnsCount);
        }

        public IntMatrix Add(int y)
        {
            var value = _value;
            int length = value.Length;

            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = value[i] + y;
            }
            return new IntMatrix(result, _rowsCount, _columnsCount);
        }

        public IntMatrix Mul(int y)
        {
            var value = _value;
            int length = value.Length;

            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = value[i] * y;
            }
            return new IntMatrix(result, _rowsCount, _columnsCount);
        }

        public IntMatrix Div(int y)
        {
            var value = _value;
            int length = value.Length;

            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = value[i] / y;
            }
            return new IntMatrix(result, _rowsCount, _columnsCount);
        }

        public IntMatrix Sub(int y)
        {
            var value = _value;
            int length = value.Length;

            var result = new int[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = value[i] - y;
            }
            return new IntMatrix(result, _rowsCount, _columnsCount);
        }

        #endregion

        #region Compare Operation

        public bool IsEqual(IntMatrix y)
        {
            if (ReferenceEquals(y, null))
            {
                return false;
            }
            if (ReferenceEquals(this, y))
            {
                return true;
            }
            if ((_rowsCount != y._rowsCount) || (_columnsCount != y._columnsCount))
            {
                return false;
            }
            int length = _value.Length;
            for (int i = 0; i < length; i++)
            {
                if (_value[i] != y._value[i])
                {
                    return false;
                }
            }
            return true;
        }

        #endregion

        #region Implicit Type conversion

        public static implicit operator IntVector(IntMatrix x)
        {
            //if (!x.IsVector)
            //    throw new ArgumentOutOfRangeException("x");

            int length = x.RowsCount * x.ColumnsCount;
            var result = new IntVector(length);
            Array.Copy(x.Value, result.Value, length);
            return result;
        }

        #endregion

        #region Implicit Operations

        public static IntMatrix operator +(IntMatrix x, IntMatrix y)
        {
            return x.Add(y);
        }

        public static IntMatrix operator +(IntMatrix x, int y)
        {
            return x.Add(y);
        }

        public static IntMatrix operator +(int x, IntMatrix y)
        {
            return y.Add(x);
        }

        public static IntMatrix operator -(IntMatrix x, IntMatrix y)
        {
            return x.Sub(y);
        }

        public static IntMatrix operator -(IntMatrix x, int y)
        {
            return x.Sub(y);
        }

        public static IntMatrix operator -(int x, IntMatrix y)
        {
            return y.Sub(x);
        }

        public static IntMatrix operator *(IntMatrix x, IntMatrix y)
        {
            return x.Mul(y);
        }

        public static IntVector operator *(IntMatrix x, IntVector y)
        {
            return x.Mul(y);
        }

        public static IntMatrix operator *(IntMatrix x, int y)
        {
            return x.Mul(y);
        }

        public static IntMatrix operator *(int x, IntMatrix y)
        {
            return y.Mul(x);
        }

        public static IntMatrix operator /(IntMatrix x, int y)
        {
            return x.Div(y);
        }

        public static IntMatrix operator /(int x, IntMatrix y)
        {
            return y.Div(x);
        }

        public static bool operator ==(IntMatrix x, IntMatrix y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            return x.IsEqual(y);
        }

        public static bool operator !=(IntMatrix x, IntMatrix y)
        {
            if (ReferenceEquals(x, y)) return false;
            if (ReferenceEquals(x, null)) return true;
            return !x.IsEqual(y);
        }

        public bool Equals(IntMatrix y)
        {
            return IsEqual(y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            //if (obj.GetType() != typeof(Integer)) return false;
            return Equals((IntMatrix)obj);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _value.GetHashCode();
        }

        #endregion

        #region Additional Properties (Methods)

        public bool IsVector
        {
            get
            {
                return ((_rowsCount == 1) || (_columnsCount == 1));
            }
        }

        public int RowsCount
        {
            get { return _rowsCount; }
        }

        public int ColumnsCount
        {
            get { return _columnsCount; }
        }

        public bool IsSquare
        {
            get { return _rowsCount == _columnsCount; }
        }

        /// <summary>
        /// Определяет является ли матрица верхнетреугольной.
        /// Квадратная матрица, в которой все элементы ниже главной диагонали равны нулю.
        /// </summary>
        public bool IsUpperTriangular
        {
            get
            {
                if (_isUpperTriangular == null)
                {
                    int rowsCount = _rowsCount;
                    int columnsCount = _columnsCount;
                    _isUpperTriangular = true;
                    for (int column = 0; column < columnsCount; column++)
                    {
                        for (int row = column + 1; row < rowsCount; row++)
                        {
                            if (_value[row * columnsCount + column] != 0)
                            {
                                _isUpperTriangular = false;
                                break;
                            }
                        }
                    }
                }
                return _isUpperTriangular.Value;
            }
        }

        /// <summary>
        /// Определяет является ли матрица нижнетреугольной.
        /// Квадратная матрица, в которой все элементы выше главной диагонали равны нулю.
        /// </summary>
        public bool IsLowerTriangular
        {
            get
            {
                if (_isLowerTriangular == null)
                {
                    int columnsCount = _columnsCount;
                    _isLowerTriangular = true;
                    for (int column = 0; column < columnsCount; column++)
                    {
                        for (int row = 0; row < column; row++)
                        {
                            if (_value[row * columnsCount + column] != 0)
                            {
                                _isLowerTriangular = false;
                                break;
                            }
                        }
                    }
                }
                return _isLowerTriangular.Value;
            }
        }

        /// <summary>
        /// Определяет является ли матрица треугольной.
        /// </summary>
        public bool IsTriangle
        {
            get { return IsUpperTriangular || IsLowerTriangular; }
        }

        /// <summary>
        /// Транспонирование
        /// </summary>
        public IntMatrix Transparent()
        {
            var value = _value;
            int length = value.Length;

            var result = new int[length];
            int rColumns = _rowsCount;
            int rRows = _columnsCount;
            for (int row = 0; row < _rowsCount; row++)
            {
                for (int column = 0; column < _columnsCount; column++)
                {
                    result[column * rColumns + row] = value[row * _columnsCount + column];
                }
            }
            return new IntMatrix(result, rRows, rColumns);
        }

        /// <summary>
        /// Транспонирование
        /// </summary>
        public IntMatrix T
        {
            get { return Transparent(); }
        }

        /// <summary>
        /// След матрицы.
        /// Cумма элементов главной диагонали матрицы.
        /// Операция, отображающая пространство квадратных матриц в поле, над которым 
        /// определена матрица (для действительных матриц — в поле действительных чисел, 
        /// для комплексных матриц — в поле комплексных чисел). 
        /// </summary>
        public int Tr
        {
            get
            {
                if (_tr == null)
                {
                    int n = System.Math.Min(_columnsCount, _rowsCount);
                    int trace = 0;
                    for (int i = 0; i < n; i++)
                    {
                        trace += _value[i * _columnsCount + i];
                    }
                    _tr = trace;
                }
                return _tr.Value;
            }
        }

        /// <summary>
        /// Веса строк
        /// </summary>
        public IntVector WeightRows
        {
            get
            {
                var result = new int[_rowsCount];

                for (int row = 0; row < _rowsCount; row++)
                {
                    for (int column = 0; column < _columnsCount; column++)
                    {
                        result[row] += _value[row * _columnsCount + column];
                    }
                }

                return new IntVector(result);
            }
        }

        /// <summary>
        /// Веса колонок
        /// </summary>
        public IntVector WeightColumns
        {
            get
            {
                var result = new int[_columnsCount];

                for (int column = 0; column < _columnsCount; column++)
                {
                    for (int row = 0; row < _rowsCount; row++)
                    {
                        result[column] += _value[row * _columnsCount + column];
                    }
                }

                return new IntVector(result);
            }
        }

        public string ToIntString()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < RowsCount; i++)
            {
                for (int j = 0; j < ColumnsCount; j++)
                {
                    sb.Append(this[i, j]);
                    if (j < ColumnsCount - 1)
                    {
                        sb.Append("|");
                    }
                }
                sb.Append(";");
            }
            return sb.ToString();
        }

        public void FromIntString(string value)
        {
            string[] width = value.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            string[] height = width[0].Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            _rowsCount = width.Length;
            _columnsCount = height.Length;
            _value = new int[width.Length * height.Length];
            for (int i = 0; i < width.Length; i++)
            {
                height = width[i].Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < height.Length; j++)
                {
                    this[i, j] = int.Parse(height[j]);
                }
            }
        }

        public IntMatrix CutColumns(int index, int length)
        {
            int columns = length;
            int newLength = _rowsCount * columns;
            var result = new int[newLength];

            for (int column = 0; column < columns; column++)
            {
                for (int row = 0; row < _rowsCount; row++)
                {
                    result[row * columns + column] = _value[row * _columnsCount + (index + column)];
                }
            }

            return new IntMatrix(result, _rowsCount, columns);
        }

        public IntMatrix CutRows(int index, int length)
        {
            int rows = length;
            int newLength = rows * _columnsCount;
            var result = new int[newLength];

            Array.Copy(_value, index * _columnsCount, result, 0, newLength);
            return new IntMatrix(result, rows, _columnsCount);
        }

        public IntMatrix AddColumn(IntVector x, bool clone)
        {
            int columns = _columnsCount + 1;
            int length = _rowsCount * _columnsCount;
            var result = new int[_rowsCount * columns];
            Array.Copy(_value, result, length);

            for (int i = 0; i < x.Length; i++)
            {
                result[i * columns + _columnsCount] = x[i];
            }

            if (clone)
            {
                return new IntMatrix(result, _rowsCount, columns);
            }

            _value = result;
            _columnsCount = columns;
            InvokeOnChange(-1, columns - 1, 0.0f);
            return this;
        }

        public IntMatrix AddColumn(IntVector x)
        {
            return AddColumn(x, true);
        }

        /// <summary>
        /// Добавляет новый столбец в конец матрицы, при этом первый столбец удаляется, т.е. количество столбцов (размер матрицы) не меняется.
        /// </summary>
        public IntMatrix MoveColumn(IntVector x, bool clone = true)
        {
            int columns = _columnsCount;
            int length = _value.Length;
            var result = new int[length];

            Array.Copy(_value, 1, result, 0, length - 1);

            int lastColumnIndex = columns - 1;
            int newColumnLength = x.Length;
            int[] newColumnValue = x.Value;
            for (int i = 0; i < newColumnLength; i++)
            {
                result[i * columns + lastColumnIndex] = newColumnValue[i];
            }

            if (clone)
            {
                return new IntMatrix(result, _rowsCount, columns);
            }

            _value = result;
            InvokeOnChange(-1, -1, 0.0f);
            return this;
        }

        /// <summary>
        /// Добавляет новый столбец в конец матрицы, при этом первый столбец удаляется, т.е. количество столбцов (размер матрицы) не меняется.
        /// </summary>
        public IntMatrix MoveColumn(int defaultValue = 0)
        {
            return MoveColumn(new IntVector(_rowsCount, defaultValue));
        }

        /// <summary>
        /// Добавляет новую строку вниз матрицы, при этом первая строка удаляется, т.е. количество строк (размер матрицы) не меняется.
        /// </summary>
        public IntMatrix MoveRow(IntVector x, bool clone = true)
        {
            int rows = _rowsCount;
            int columns = _columnsCount;
            int length = _value.Length;
            var result = new int[length];

            Array.Copy(_value, columns, result, 0, length - columns);

            int lastRowIndex = rows - 1;
            int newRowLength = x.Length;
            int[] newRowValue = x.Value;
            int index = lastRowIndex * columns;
            for (int i = 0; i < newRowLength; i++)
            {
                result[index + i] = newRowValue[i];
            }

            if (clone)
            {
                return new IntMatrix(result, rows, columns);
            }

            _value = result;
            InvokeOnChange(-1, -1, 0.0f);
            return this;
        }

        /// <summary>
        /// Добавляет новую строку вниз матрицы, при этом первая строка удаляется, т.е. количество строк (размер матрицы) не меняется.
        /// </summary>
        public IntMatrix MoveRow(int defaultValue = 0)
        {
            return MoveRow(new IntVector(_columnsCount, defaultValue));
        }

        public IntMatrix AddRow(IntVector x, bool clone)
        {
            int rows = _rowsCount + 1;
            int length = _rowsCount * _columnsCount;
            var result = new int[rows * _columnsCount];
            Array.Copy(_value, result, length);
            Array.Copy(x.Value, 0, result, length, x.Length);

            if (clone)
            {
                return new IntMatrix(result, rows, _columnsCount);
            }

            _value = result;
            _rowsCount = rows;
            InvokeOnChange(rows - 1, -1, 0.0f);
            return this;
        }

        public IntMatrix AddRow(IntVector x)
        {
            return AddRow(x, true);
        }

        public void SwapColumns(int column1, int column2)
        {
            int rowsCount = _rowsCount;
            int columnsCount = _columnsCount;
            for (int row = 0; row < rowsCount; row++)
            {
                int index = row * columnsCount;
                int index1 = index + column1;
                int index2 = index + column2;
                int v = _value[index1];
                _value[index1] = _value[index2];
                _value[index2] = v;
            }
            InvokeOnChange(-1, column1, 0.0f);
            InvokeOnChange(-1, column2, 0.0f);
        }

        public void SwapRows(int row1, int row2)
        {
            int columnsCount = _columnsCount;
            for (int column = 0; column < columnsCount; column++)
            {
                int index1 = row1 * columnsCount + column;
                int index2 = row2 * columnsCount + column;
                int v = _value[index1];
                _value[index1] = _value[index2];
                _value[index2] = v;
            }
            InvokeOnChange(row1, -1, 0.0f);
            InvokeOnChange(row2, -1, 0.0f);
        }

        /// <summary>
        /// Приведение вещественной матрицы к ступенчатому виду методом Гаусса с выбором максимального 
        /// разрешающего элемента в столбце.
        /// </summary>
        public IntMatrix Gauss()
        {
            int length = _value.Length;
            int rowsCount = _rowsCount;
            int columnsCount = _columnsCount;
            var a = new int[length];
            Array.Copy(_value, a, length);
            int i, j, k, l;
            int r;

            l = 0;
            i = 0;
            j = 0;
            while (i < rowsCount && j < columnsCount)
            {
                // Инвариант: минор матрицы в столбцах 0..j-1
                // уже приведен к ступенчатому виду, и строка
                // с индексом i-1 содержит ненулевой эл-т
                // в столбце с номером, меньшим чем j

                // Ищем максимальный элемент в j-м столбце,
                // начиная с i-й строки
                r = 0;
                for (k = i; k < rowsCount; ++k)
                {
                    int v = System.Math.Abs(a[k * columnsCount + j]);
                    if (v > r)
                    {
                        l = k; // Запомним номер строки
                        r = v; // и макс. эл-т
                    }
                }
                if (r == 0)
                {
                    // Все элементы j-го столбца по абсолютной
                    // величине не превосходят eps.
                    // Обнулим столбец, начиная с i-й строки
                    for (k = i; k < rowsCount; ++k)
                    {
                        a[k * columnsCount + j] = 0;
                    }
                    ++j;      // Увеличим индекс столбца
                    continue; // Переходим к следующей итерации
                }

                if (l != i)
                {
                    // Меняем местами i-ю и l-ю строки
                    for (k = j; k < columnsCount; ++k)
                    {
                        r = a[i * columnsCount + k];
                        a[i * columnsCount + k] = a[l * columnsCount + k];
                        a[l * columnsCount + k] = (-r); // Меняем знак строки
                    }
                }

                // Утверждение: fabs(a[i*n + k]) > eps

                // Обнуляем j-й столбец, начиная со строки i+1,
                // применяя элем. преобразования второго рода
                for (k = i + 1; k < rowsCount; ++k)
                {
                    r = (-a[k * columnsCount + j] / a[i * columnsCount + j]);

                    // К k-й строке прибавляем i-ю, умноженную на r
                    a[k * columnsCount + j] = 0;
                    for (l = j + 1; l < columnsCount; ++l)
                    {
                        a[k * columnsCount + l] += r * a[i * columnsCount + l];
                    }
                }

                ++i; ++j;   // Переходим к следующему минору
            }

            return new IntMatrix(a, rowsCount, columnsCount) { _isUpperTriangular = true, };
        }

        /// <summary>
        /// LU-разложение — представление матрицы  в виде, где — нижнетреугольная матрица с единичной диагональю, 
        /// а  — верхнетреугольная. LU-разложение еще называют LU-факторизацией.
        /// Матрица  является нижнетреугольной с единичной диагональю, поэтому ее определитель равен 1. 
        /// Матрица  — верхнетреугольная матрица, значит ее определитель равен произведению элементов, 
        /// расположенных на главной диагонали.
        /// </summary>
        /// <see cref="http://ru.math.wikia.com/wiki/LU-%D1%80%D0%B0%D0%B7%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D0%B5"/>
        /// <see cref="http://ru.wikipedia.org/wiki/LU-%D1%80%D0%B0%D0%B7%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D0%B5"/>
        private void CalcLU()
        {
            int n = _rowsCount;
            int length = n * n;

            var lu = new int[length];
            var l = new int[length];
            var u = new int[length];

            for (int i = 0; i < n; i++)
            {
                int @in = i * n;
                for (int j = 0; j < n; j++)
                {
                    int jn = j * n;
                    int inj = @in + j;
                    int jni = jn + i;

                    u[i] = _value[i];
                    l[@in] = _value[@in] / u[0];

                    int sum = 0;
                    for (int k = 0; k < i; k++)
                    {
                        sum += l[@in + k] * u[k * n + j];
                    }

                    u[inj] = _value[inj] - sum;

                    if (i > j)
                    {
                        l[jni] = 0;
                    }
                    else
                    {
                        sum = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum += l[jn + k] * u[k * n + i];
                        }
                        l[jni] = (_value[jni] - sum) / u[@in + i];
                    }
                    lu[jni] = (i < j) ? l[jni] : u[jni];
                }
            }

            _lu = new int[length];
            _l = new int[length];
            _u = new int[length];

            for (int i = 0; i < length; i++)
            {
                _lu[i] = lu[i];
                _l[i] = l[i];
                _u[i] = u[i];
            }

            //float det = 1.0f;
            //for(int i = 0; i < n; i++)
            //{
            //    det *= u[i*n + i];
            //}
            //_det = (float)det;

            /*
            var a = new float[n,n];
            var u = new float[n,n];
            var l = new float[n,n];
			
            for(int row = 0; row < n; row++)
            {
                for(int column = 0; column < n; column++)
                {
                    a[row, column] = this[row, column];
                }
            }

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    u[0, i] = a[0, i];
                    l[i, 0] = a[i, 0] / u[0, 0];
                    float sum = 0;
                    for (int k = 0; k < i; k++)
                    {
                        sum += l[i, k]*u[k, j];
                    }
                    u[i, j] = a[i, j] - sum;
                    if (i > j)
                    {
                        l[j, i] = 0;
                    }
                    else
                    {
                        sum = 0;
                        for (int k = 0; k < i; k++)
                        {
                            sum += l[j, k]*u[k, i];
                        }
                        l[j, i] = (a[j, i] - sum)/u[i, i];
                    }
                }
            }
            */
        }

        /// <summary>
        /// Нижнетреугольная матрица с единичной диагональю
        /// </summary>
        /// <see cref="http://ru.math.wikia.com/wiki/LU-%D1%80%D0%B0%D0%B7%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D0%B5"/>
        /// <see cref="http://ru.wikipedia.org/wiki/LU-%D1%80%D0%B0%D0%B7%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D0%B5"/>
        public IntMatrix L
        {
            get
            {
                if (!IsSquare)
                {
                    return null;
                }
                if (_l == null)
                {
                    CalcLU();
                }
                return new IntMatrix(_l, _rowsCount, _columnsCount) { _isLowerTriangular = true };
            }
        }

        /// <summary>
        /// Верхнетреугольная матрица
        /// </summary>
        /// <see cref="http://ru.math.wikia.com/wiki/LU-%D1%80%D0%B0%D0%B7%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D0%B5"/>
        /// <see cref="http://ru.wikipedia.org/wiki/LU-%D1%80%D0%B0%D0%B7%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D0%B5"/>
        public IntMatrix U
        {
            get
            {
                if (!IsSquare)
                {
                    return null;
                }
                if (_u == null)
                {
                    CalcLU();
                }
                return new IntMatrix(_u, _rowsCount, _columnsCount) { _isUpperTriangular = true };
            }
        }

        /// <summary>
        /// LU матрица полученная в результате LU-разложения
        /// </summary>
        /// <see cref="http://ru.wikipedia.org/wiki/LU-%D1%80%D0%B0%D0%B7%D0%BB%D0%BE%D0%B6%D0%B5%D0%BD%D0%B8%D0%B5"/>
        public IntMatrix LU
        {
            get
            {
                if (!IsSquare)
                {
                    return null;
                }
                if (_lu == null)
                {
                    CalcLU();
                }
                return new IntMatrix(_lu, _rowsCount, _columnsCount);
            }
        }

        public int[,] ToArray()
        {
            var result = new int[_rowsCount, _columnsCount];
            for (int row = 0; row < _rowsCount; row++)
            {
                for (int column = 0; column < _columnsCount; column++)
                {
                    result[row, column] = _value[row * _columnsCount + column];
                }
            }
            return result;
        }

        /// <summary>
        /// Определи́тель (детермина́нт) матрицы
        /// </summary>
        public int Determinant()
        {
            if (!IsSquare)
            {
                return 0;
            }
            if (_det == null)
            {
                if (_lu == null)
                {
                    CalcLU();
                }
                _det = 1;
                for (int i = 0; i < _rowsCount; i++)
                {
                    _det *= _lu[i * _columnsCount + i];
                }
            }
            return _det.Value;
        }

        /// <summary>
        /// Определи́тель (детермина́нт) матрицы
        /// </summary>
        public int Det
        {
            get { return Determinant(); }
        }

        /// <summary>
        /// Формирование квадратной единичной матрицы размеров n
        /// </summary>
        public static IntMatrix E(int n)
        {
            var value = new int[n * n];
            int n1 = n + 1;
            for (int i = 0; i < n; i++)
            {
                value[i * n1] = 1;
            }
            return new IntMatrix(value, n, n);
        }

        #region Минор, алгеброическое дополнение, союзная матрица, обратная матрица

        [Serializable]
        public sealed class Minor
        {
            private readonly IntMatrix _m;
            private readonly IntMatrix[,] _minors;

            internal Minor(IntMatrix m)
            {
                _m = m;
                _minors = new IntMatrix[_m._rowsCount, _m._columnsCount];
            }

            private static IntMatrix GetMinorMatrix(IntMatrix m, int row, int column)
            {
                int oColumnCount = m._columnsCount;
                int rowsCount = m._rowsCount - 1;
                int columnsCount = oColumnCount - 1;
                var value = new int[rowsCount * columnsCount];
                for (int r = 0; r < rowsCount; r++)
                {
                    int or = r < row ? r : r + 1;
                    for (int c = 0; c < columnsCount; c++)
                    {
                        int oc = c < column ? c : c + 1;
                        value[r * columnsCount + c] = m._value[or * oColumnCount + oc];
                    }
                }
                return new IntMatrix(value, rowsCount, columnsCount);
            }

            public int this[int row, int column]
            {
                get
                {
                    IntMatrix m = _minors[row, column];
                    if (m == null)
                    {
                        m = GetMinorMatrix(_m, row, column);
                        _minors[row, column] = m;
                    }
                    return m.Determinant();
                }
            }
        }

        [Serializable]
        public sealed class Cofactor
        {
            private readonly IntMatrix _m;
            private readonly int?[,] _cofactors;
            private IntMatrix _c;

            internal Cofactor(IntMatrix m)
            {
                _m = m.Transparent();
                _cofactors = new int?[_m._rowsCount, _m._columnsCount];
            }

            public int this[int row, int column]
            {
                get
                {
                    int? m = _cofactors[row, column];
                    if (m == null)
                    {
                        int n = row + column;
                        m = _m.M[row, column];
                        m = (n % 2 == 0) ? m : -m;
                        _cofactors[row, column] = m;
                    }
                    return m.Value;
                }
            }

            /// <summary>
            /// C* (союзная, взаимная, присоединённая) матрица — матрица, составленная из алгебраических 
            /// дополнений для соответствующих элементов транспонированной матрицы.
            /// </summary>
            public IntMatrix C
            {
                get
                {
                    if (_c == null)
                    {
                        int rowsCount = _m._rowsCount;
                        int columnsCount = _m._columnsCount;
                        var value = new int[rowsCount * columnsCount];
                        for (int row = 0; row < rowsCount; row++)
                        {
                            for (int column = 0; column < columnsCount; column++)
                            {
                                value[row * columnsCount + column] = this[row, column];
                            }
                        }
                        _c = new IntMatrix(value, rowsCount, columnsCount);
                    }
                    return new IntMatrix(_c);
                }
            }
        }

        /// <summary>
        /// Минор квадратной матрицы
        /// Определитель матрицы, полученной из исходной вычеркиванием i-ой строки и j-го столбца.
        /// </summary>
        public Minor M
        {
            get
            {
                if (!IsSquare)
                {
                    return null;
                }
                return _minor = _minor ?? new Minor(this);
            }
        }

        /// <summary>
        /// Алгебраическое дополнение
        /// Алгебраическим дополнением элемента aij матрицы A называется число
        /// Aij = (−1)^(i + j) * Mij, где Mij — минор (определитель матрицы, 
        /// получающейся из исходной матрицы A путем вычёркивания i-й строки и j -го столбца).
        /// </summary>
        public Cofactor A
        {
            get
            {
                if (!IsSquare)
                {
                    return null;
                }
                return _cofactor = _cofactor ?? new Cofactor(this);
            }
        }

        /// <summary>
        /// C* (союзная, взаимная, присоединённая) матрица — матрица, составленная из алгебраических 
        /// дополнений для соответствующих элементов транспонированной матрицы. 
        /// Из определения следует, что присоединённая матрица рассматривается только для квадратных 
        /// матриц и сама является квадратной, ибо понятие алгебраического дополнения вводится для 
        /// квадратных матриц.
        /// </summary>
        public IntMatrix C
        {
            get
            {
                if (!IsSquare)
                {
                    return null;
                }
                return A.C;
            }
        }

        /// <summary>
        /// Обратная матрица
        /// </summary>
        public IntMatrix Invertible
        {
            get
            {
                if (!IsSquare)
                {
                    return null;
                }
                if (_invertible == null)
                {
                    int det = Det;
                    _invertible = (det != 0) ? C / det : null;
                }
                return new IntMatrix(_invertible);
            }
        }

        #endregion

        #endregion

        #region Value

        public int[] Value
        {
            get { return _value; }
        }

        #endregion

        #region Items

        public int this[int row, int column]
        {
            get
            {
                return _value[row * _columnsCount + column];
            }
            set
            {
                int index = row * _columnsCount + column;
                if (value != _value[index])
                {
                    _value[index] = value;
                    InvokeOnChange(row, column, value);
                }
            }
        }

        public IntVector this[int row]
        {
            get
            {
                var result = new int[_columnsCount];
                Array.Copy(_value, row * _columnsCount, result, 0, _columnsCount);
                return new IntVector(result);
            }
            set
            {
                Array.Copy(value.Value, 0, _value, row * _columnsCount, _columnsCount);
                InvokeOnChange(row, -1, 0.0f);
            }
        }

        public IntVector[] Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = new IntVector[_columnsCount];
                    for (int column = 0; column < _columnsCount; column++)
                    {
                        var result = new int[_rowsCount];
                        for (int row = 0; row < _rowsCount; row++)
                        {
                            result[row] = _value[row * _columnsCount + column];
                        }
                        _columns[column] = new IntVector(result);
                    }
                }
                return _columns;
            }
        }

        public IntVector[] Rows
        {
            get
            {
                if (_rows == null)
                {
                    _rows = new IntVector[_rowsCount];
                    for (int row = 0; row < _rowsCount; row++)
                    {
                        var result = new int[_columnsCount];
                        Array.Copy(_value, row * _columnsCount, result, 0, _columnsCount);
                        _rows[row] = new IntVector(result);
                    }
                }
                return _rows;
            }
        }

        #endregion

        #region Events

        public OnChangeDelegate OnChange;

        #endregion
    }
}
