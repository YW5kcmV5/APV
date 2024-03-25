using System;
using System.Text;

namespace APV.Math.MathObjects.Numbers
{
    /// <summary>
    /// Большое целое число.
    /// </summary>
    /// <see cref="http://algolist.ru/maths/longnum.php"/>
    //[NumberName("BigIntegerNumber", "Большое целое число")]
    [Serializable]
    public class BigIntegerNumber : ICloneable, IComparable, IComparable<BigIntegerNumber>, IEquatable<BigIntegerNumber>, IInteger
    {
        /// <summary>
        /// Степень основания
        /// </summary>
        public const int BaseDig = 4;

        /// <summary>
        /// Основание.
        /// Для правильной работы метода ToString() и Parse() (TryParse) основание должно быть степенью 10.
        /// </summary>
        public static readonly int Base = (int) System.Math.Pow(10, BaseDig);

        /// <summary>
        /// Максимальное количество символов (оснований) в числе
        /// </summary>
        public const int Digits = 5000;

        private int[] _digits;
        private int _length;
        private bool _isNegative;

        public bool IsNegative
        {
            get
            {
                Init();
                return _isNegative;
            }
        }

        public bool IsPositive
        {
            get
            {
                Init();
                return !IsNegative;
            }
        }

        public bool IsZero
        {
            get
            {
                Init();
                return ((_length == 1) && (_digits[0] == 0));
            }
        }

        static BigIntegerNumber()
        {
            MaxValue = new BigIntegerNumber {_digits = new int[Digits], _length = Digits};
            MinValue = new BigIntegerNumber {_digits = new int[Digits], _length = Digits, _isNegative = true};
            for (int i = 0; i < Digits; i++)
            {
                MaxValue._digits[i] = Base - 1;
                MinValue._digits[i] = Base - 1;
            }

            Zero = 0;
        }

        private void Init()
        {
            if (_digits == null)
            {
                _length = 1;
                _isNegative = false;
                _digits = new int[Digits];
            }
        }

        /// <summary>
        /// Разложение числа на десятичные множители
        /// </summary>
        private void Parse(long value)
        {
            Init();
            if (value == int.MinValue)
            {
                Assign(IntMinValue);
                return;
            }

            if (value == int.MaxValue)
            {
                Assign(IntMaxValue);
                return;
            }

            if (value == long.MinValue)
            {
                Assign(LongMinValue);
                return;
            }

            if (value == long.MaxValue)
            {
                Assign(LongMaxValue);
                return;
            }

            if (value < 0)
            {
                value = -value;
                _isNegative = true;
            }

            if (value < Base)
            {
                _length = 1;
                _digits[0] = (int) value;
                if (_digits[0] < 0)
                    throw new Exception("!!!!");

                return;
            }

            int i = 0;
            do
            {
                var carry = value / Base;
                var digit = (value - carry * Base);
                value = (value - digit) / Base;
                _digits[i] = (int) digit;
                if (_digits[i] < 0)
                    throw new Exception("!!!!");

                i++;
            } while (value > 0);

            _length = i;
        }

        public void Assign(BigIntegerNumber instance)
        {
            Init();
            _length = instance._length;
            _isNegative = instance._isNegative;
            Array.Copy(instance._digits, _digits, _length);
            //_digits = instance._digits;
        }

        public object Clone()
        {
            var result = new BigIntegerNumber();
            result.Assign(this);
            return result;
        }

        private BigIntegerNumber InternalInvert()
        {
            Init();
            _isNegative = !_isNegative && !IsZero;
            return this;
        }

        public BigIntegerNumber Invert()
        {
            Init();
            var result = new BigIntegerNumber {_length = _length, _isNegative = !IsNegative && !IsZero, _digits = new int[Digits]};
            Array.Copy(_digits, result._digits, _length);
            return result;
        }

        /// <summary>
        /// Модуль
        /// </summary>
        public BigIntegerNumber Abs()
        {
            Init();
            var result = new BigIntegerNumber {_length = _length, _isNegative = false, _digits = new int[Digits]};
            Array.Copy(_digits, result._digits, _length);
            return result;
        }

        /// <summary>
        /// Сложение
        /// </summary>
        public BigIntegerNumber Add(BigIntegerNumber y)
        {
            Init();
            bool canAdd = IsNegative == y.IsNegative;

            if (!canAdd)
            {
                return (IsNegative) ? y.Sub(InternalInvert()) : Sub(y.InternalInvert());
            }

            var digits = new int[Digits];
            int length = System.Math.Max(_length, y._length);
            int s1 = 0;
            for (int i = 0; i < length; i++)
            {
                int s2 = _digits[i] + y._digits[i] + s1;
                if (s2 >= Base)
                {
                    s1 = 1;
                    int carry = s2 / Base;
                    s2 = s2 - carry * Base;
                }
                else
                {
                    s1 = 0;
                }

                digits[i] = s2;
                if (digits[i] < 0)
                    throw new Exception("!!!!");
            }

            if (s1 > 0)
            {
                length++;
                digits[length - 1] = s1;
                if (digits[length - 1] < 0)
                    throw new Exception("!!!!");
            }

            return new BigIntegerNumber {_digits = digits, _length = length, _isNegative = _isNegative};
        }

        /// <summary>
        /// Вычитание
        /// </summary>
        public BigIntegerNumber Sub(BigIntegerNumber y)
        {
            Init();
            if ((IsPositive) && (y.IsNegative))
            {
                return Add(y.InternalInvert());
            }

            if ((IsNegative) && (y.IsPositive))
            {
                return InternalInvert().Add(y).InternalInvert();
            }

            if ((IsNegative) && (y.IsNegative))
            {
                return y.InternalInvert().Sub(InternalInvert());
            }

            if (this < y)
            {
                return y.Sub(this).InternalInvert();
            }

            var digits = new int[Digits];
            int length = System.Math.Max(_length, y._length);
            int s1 = 0;
            for (int i = 0; i < length; i++)
            {
                int s2 = _digits[i] - s1 - y._digits[i];
                if (s2 < 0)
                {
                    s1 = 1;
                    s2 = s2 + Base;
                }
                else
                {
                    s1 = 0;
                }

                digits[i] = s2;
                if (digits[i] < 0)
                    throw new Exception("!!!!");
            }

            while ((digits[length - 1] == 0) && (length > 1))
            {
                length--;
            }

            return new BigIntegerNumber {_digits = digits, _length = length};
        }

        /// <summary>
        /// Умножение
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        private BigIntegerNumber Mul(int b)
        {
            Init();
            if ((IsZero) || (b == 0))
            {
                return Zero;
            }

            bool isPositive = (IsPositive && b >= 0) || (IsNegative && b < 0);

            // ReSharper disable once TooWideLocalVariableScope
            int i, temp;
            var digits = new int[Digits];
            int length = _length;

            int carry = 0;
            for (i = 0; i < _length; i++)
            {
                temp = _digits[i] * b + carry;
                carry = temp / Base;
                digits[i] = temp - carry * Base; // с[i] = temp % BASE
                if (digits[i] < 0)
                    throw new Exception("!!!!");
            }

            if (carry > 0)
            {
                // Число удлинилось за счет переноса нового разряда
                digits[i] = carry;
                if (digits[i] < 0)
                    throw new Exception("!!!!");
                length = _length + 1;
            }

            return new BigIntegerNumber {_digits = digits, _length = length, _isNegative = !isPositive};
        }

        /// <summary>
        /// Умножение
        /// </summary>
        public BigIntegerNumber Mul(BigIntegerNumber y)
        {
            Init();
            if ((IsZero) || (y.IsZero))
            {
                return Zero;
            }

            bool isPositive = (IsPositive && y.IsPositive) || (IsNegative && y.IsNegative);

            int xLength = _length;
            int yLength = y._length;
            int length = xLength + yLength;
            int[] xDigits = _digits;
            int[] yDigits = y._digits;
            var digits = new int[Digits];

            int i;
            // ReSharper disable once TooWideLocalVariableScope
            int j;
            // ReSharper disable once TooWideLocalVariableScope
            int temp;
            // ReSharper disable once TooWideLocalVariableScope
            int carry;
            for (i = 0; i < xLength; i++)
            {
                carry = 0;
                for (j = 0; j < yLength; j++)
                {
                    temp = xDigits[i] * yDigits[j] + digits[i + j] + carry;
                    carry = temp / Base;
                    digits[i + j] = temp - carry * Base;
#if DEBUG
                    if (digits[i + j] < 0)
                        throw new InvalidOperationException("DEBUG/Mul: (digits[i + j] < 0)");
#endif
                }

                digits[i + j] = carry;
#if DEBUG
                if (digits[i + j] < 0)
                    throw new InvalidOperationException("DEBUG/Mul: (digits[i + j] < 0)");
#endif
            }

            while ((digits[length - 1] == 0) && (length > 1))
            {
                length--;
            }

            return new BigIntegerNumber {_digits = digits, _length = length, _isNegative = !isPositive};
        }

        /// <summary>
        /// Q = A/B
        /// R = A%B
        /// A, Q, R – длинные числа, B - базового типа
        /// </summary>
        private static void Div(BigIntegerNumber a, int b, out BigIntegerNumber q, out BigIntegerNumber r)
        {
            bool isPositive = (a.IsPositive && b >= 0) || (a.IsNegative && b < 0);

            int v = 0;
            var digits = new int[Digits];

            int i;
            // ReSharper disable once TooWideLocalVariableScope
            int temp;
            for (i = a._length - 1; i >= 0; i--)
            {
                temp = v * Base + a._digits[i];
                digits[i] = temp / b;
#if DEBUG
                if (digits[i] < 0)
                    throw new InvalidOperationException("DEBUG/Div: (digits[i] < 0)");
#endif

                v = temp - digits[i] * b;
            }

            // Размер частного меньше, либо равен размера делимого
            i = a._length - 1;
            while ((i > 0) && (digits[i] == 0))
            {
                i--;
            }

            int length = i + 1;

            bool isZero = (length == 1) && (digits[0] == 0);
            q = new BigIntegerNumber {_digits = digits, _length = length, _isNegative = !isPositive && !isZero};
            r = v;
            r._isNegative = a.IsNegative && (!r.IsZero);
        }

        /// <summary>
        /// q = a/b
        /// r = a%b
        /// </summary>
        public static void Div(BigIntegerNumber a, BigIntegerNumber b, out BigIntegerNumber q, out BigIntegerNumber r)
        {
            if (b.IsZero)
                throw new ArgumentOutOfRangeException(nameof(b));

            if (a.IsZero)
            {
                q = Zero;
                r = Zero;
                return;
            }

            bool isPositive = (a.IsPositive == b.IsPositive);

            // Вырожденный случай 1. Делитель больше делимого.
            if (a._length < b._length)
            {
                q = Zero;
                r = (BigIntegerNumber) a.Clone();
                return;
            }

            // Вырожденный случай 2. Делитель – число базового типа.
            if (b._length == 1)
            {
                Div(a, b._digits[0], out q, out r);
                return;
            }

            // Создать временный массив U, равный a
            // Максимальный размер U на цифру больше a, с учетом
            // возможного удлинения a при нормализации
            var u = (BigIntegerNumber) a.Clone();

            q = new BigIntegerNumber {_digits = new int[Digits]};

            int n = b._length;
            int m = u._length - b._length;

            // Нормализация
            int scale = Base / (b._digits[n - 1] + 1); // коэффициент нормализации
            if (scale > 1)
            {
                u = u.Mul(scale);
                b = b.Mul(scale);
            }

            // Указатели для быстрого доступа
            var bDigits = b._digits;
            var uDigits = u._digits;
            var qDigits = q._digits;

            int uJ, vJ, i;
            int temp1, temp2, temp;

            int qGuess, rValue; // догадка для частного и соответствующий остаток
            int borrow, carry; // переносы

            // Главный цикл шагов деления. Каждая итерация дает очередную цифру частного.
            // vJ - текущий сдвиг b относительно U, используемый при вычитании,
            // по совместительству - индекс очередной цифры частного.
            // uJ – индекс текущей цифры U
            for (vJ = m, uJ = n + vJ; vJ >= 0; --vJ, --uJ)
            {
                qGuess = (uDigits[uJ] * Base + uDigits[uJ - 1]) / bDigits[n - 1];
                rValue = (uDigits[uJ] * Base + uDigits[uJ - 1]) % bDigits[n - 1];
                // Пока не будут выполнены условия (2) уменьшать частное.
                while (rValue < Base)
                {
                    temp2 = bDigits[n - 2] * qGuess;
                    temp1 = rValue * Base + uDigits[uJ - 2];
                    if ((temp2 > temp1) || (qGuess == Base))
                    {
                        // условия не выполнены, уменьшить qGuess
                        // и досчитать новый остаток
                        --qGuess;
                        rValue += bDigits[n - 1];
                    }
                    else break;
                }

                // Теперь qGuess - правильное частное или на единицу больше q
                // Вычесть делитель b, умноженный на qGuess из делимого U,
                // начиная с позиции vJ+i
                carry = 0;
                borrow = 0;

                // цикл по цифрам b
                for (i = 0; i < n; i++)
                {
                    // получить в temp цифру произведения b*qGuess
                    temp1 = bDigits[i] * qGuess + carry;
                    carry = temp1 / Base;
                    temp1 -= carry * Base;
                    // Сразу же вычесть из U
                    temp2 = uDigits[i + vJ] - temp1 + borrow;
                    if (temp2 < 0)
                    {
                        uDigits[i + vJ] = temp2 + Base;
#if DEBUG
                        if (uDigits[i + vJ] < 0)
                            throw new InvalidOperationException("DEBUG/Div: (uDigits[i + vJ] < 0)");
#endif
                        borrow = -1;
                    }
                    else
                    {
                        uDigits[i + vJ] = temp2;
                        if (uDigits[i + vJ] < 0)
                            throw new Exception("!!!!");
                        borrow = 0;
                    }
                }

                // возможно, умноженое на qGuess число b удлинилось.
                // Если это так, то после умножения остался
                // неиспользованный перенос carry. Вычесть и его тоже.
                temp2 = uDigits[i + vJ] - carry + borrow;
                if (temp2 < 0)
                {
                    uDigits[i + vJ] = temp2 + Base;
#if DEBUG
                    if (uDigits[i + vJ] < 0)
                        throw new InvalidOperationException("DEBUG/Div: (uDigits[i + vJ] < 0)");
#endif
                    borrow = -1;
                }
                else
                {
                    uDigits[i + vJ] = temp2;
#if DEBUG
                    if (uDigits[i + vJ] < 0)
                        throw new InvalidOperationException("DEBUG/Div: (uDigits[i + vJ] < 0)");
#endif
                    borrow = 0;
                }

                // Прошло ли вычитание нормально ?
                if (borrow == 0)
                {
                    // Да, частное угадано правильно
                    qDigits[vJ] = qGuess;
#if DEBUG
                    if (qDigits[vJ] < 0)
                        throw new InvalidOperationException("DEBUG/Div: (qDigits[vJ] < 0)");
#endif
                }
                else
                {
                    // Нет, последний перенос при вычитании borrow = -1,
                    // значит, qGuess на единицу больше истинного частного
                    qDigits[vJ] = qGuess - 1;
                    // добавить одно, вычтенное сверх необходимого b к U
                    carry = 0;
                    for (i = 0; i < n; i++)
                    {
                        temp = uDigits[i + vJ] + bDigits[i] + carry;
                        if (temp >= Base)
                        {
                            uDigits[i + vJ] = temp - Base;
#if DEBUG
                            if (uDigits[i + vJ] < 0)
                                throw new InvalidOperationException("DEBUG/Div: (uDigits[i + vJ] < 0)");
#endif
                            carry = 1;
                        }
                        else
                        {
                            uDigits[i + vJ] = temp;
#if DEBUG
                            if (uDigits[i + vJ] < 0)
                                throw new InvalidOperationException("DEBUG/Div: (uDigits[i + vJ] < 0)");
#endif
                            carry = 0;
                        }
                    }

                    uDigits[i + vJ] = uDigits[i + vJ] + carry - Base;
#if DEBUG
                    if (uDigits[i + vJ] < 0)
                        throw new InvalidOperationException("DEBUG/Div: (uDigits[i + vJ] < 0)");
#endif
                }

                // Обновим размер U, который после вычитания мог уменьшиться
                i = u._length - 1;
                while ((i > 0) && (uDigits[i] == 0))
                {
                    i--;
                }

                u._length = i + 1;
            }
            // Деление завершено !

            // Размер частного равен m+1, но, возможно, первая цифра - ноль.
            while ((m > 0) && (qDigits[m] == 0)) m--;
            q._length = m + 1;
            // Если происходило домножение на нормализующий множитель –
            // разделить на него. То, что осталось от U – остаток.
            if (scale > 1)
            {
                // почему-то остаток junk всегда будет равен нулю!:)
                Div(b, scale, out b, out _);
                Div(u, scale, out r, out _);
            }
            else
            {
                r = u;
            }

            q._isNegative = !isPositive && (!q.IsZero);

            r._isNegative = a.IsNegative && (!r.IsZero);
        }

        /// <summary>
        /// Сдвиг влево
        /// </summary>
        public BigIntegerNumber Shl(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException(nameof(n));

            Init();

            if (IsZero)
            {
                return Zero;
            }

            int length = _length + n;
            var digits = new int[Digits];
            for (int i = n; i < length; i++)
            {
                digits[i] = _digits[i - n];
            }

            return new BigIntegerNumber {_digits = digits, _length = length, _isNegative = _isNegative};
        }

        /// <summary>
        /// Сдвиг вправо
        /// </summary>
        public BigIntegerNumber Shr(int n)
        {
            if (n < 0)
                throw new ArgumentOutOfRangeException(nameof(n));

            Init();

            if ((IsZero) || (_length == n))
            {
                return Zero;
            }

            int length = _length - n;
            var digits = new int[Digits];
            for (int i = n; i < length; i++)
            {
                digits[i - n] = _digits[i];
            }

            return new BigIntegerNumber {_digits = digits, _length = length, _isNegative = _isNegative};
        }

        public static BigIntegerNumber operator +(BigIntegerNumber x, BigIntegerNumber y)
        {
            return x.Add(y);
        }

        public static BigIntegerNumber operator -(BigIntegerNumber x, BigIntegerNumber y)
        {
            return x.Sub(y);
        }

        public static BigIntegerNumber operator -(BigIntegerNumber x)
        {
            return x.Invert();
        }

        public static BigIntegerNumber operator *(BigIntegerNumber x, BigIntegerNumber y)
        {
            return x.Mul(y);
        }

        public static bool operator >(BigIntegerNumber x, BigIntegerNumber y)
        {
            return x.CompareTo(y) > 0;
        }

        public static bool operator <(BigIntegerNumber x, BigIntegerNumber y)
        {
            return x.CompareTo(y) < 0;
        }

        public static bool operator >=(BigIntegerNumber x, BigIntegerNumber y)
        {
            return x.CompareTo(y) >= 0;
        }

        public static bool operator <=(BigIntegerNumber x, BigIntegerNumber y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static bool operator ==(BigIntegerNumber x, BigIntegerNumber y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            return x.CompareTo(y) == 0;
        }

        public static bool operator !=(BigIntegerNumber x, BigIntegerNumber y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            return x.CompareTo(y) != 0;
        }

        public static BigIntegerNumber operator <<(BigIntegerNumber x, int n)
        {
            return x.Shl(n);
        }

        public static BigIntegerNumber operator >>(BigIntegerNumber x, int n)
        {
            return x.Shr(n);
        }

        /// <summary>
        /// Целочисленное деление
        /// </summary>
        public static BigIntegerNumber operator /(BigIntegerNumber x, BigIntegerNumber y)
        {
            Div(x, y, out BigIntegerNumber q, out _);
            return q;
        }

        /// <summary>
        /// Остаток от деления
        /// </summary>
        public static BigIntegerNumber operator %(BigIntegerNumber x, BigIntegerNumber y)
        {
            Div(x, y, out _, out BigIntegerNumber r);
            return r;
        }

        public static implicit operator BigIntegerNumber(int x)
        {
            var big = new BigIntegerNumber();
            big.Parse(x);
            return big;
        }

        public static implicit operator BigIntegerNumber(long x)
        {
            var big = new BigIntegerNumber();
            big.Parse(x);
            return big;
        }

        public static implicit operator BigIntegerNumber(float x)
        {
            return Parse(x.ToString("0"));
        }

        public static explicit operator int(BigIntegerNumber x)
        {
            int r = 0;
            int k = 1;
            for (int i = 0; i < x._length; i++)
            {
                r += k * x._digits[i];
                k *= Base;
            }

            return x.IsNegative ? -r : r;
        }

        public static explicit operator BigIntegerNumber(string value)
        {
            return Parse(value);
        }

        #region IComparable, IEquatable

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null)) return 1;
            if (obj.GetType() != typeof(BigIntegerNumber)) return 1;
            return CompareTo((BigIntegerNumber) obj);
        }

        public int CompareTo(BigIntegerNumber other)
        {
            Init();

            if ((IsPositive) && (other.IsNegative))
            {
                return 1;
            }

            if ((IsNegative) && (other.IsPositive))
            {
                return -1;
            }

            if (_length > other._length)
            {
                return (IsPositive) ? 1 : -1;
            }

            if (_length < other._length)
            {
                return (IsPositive) ? -1 : 1;
            }

            for (int i = _length - 1; i >= 0; i--)
            {
                if (_digits[i] > other._digits[i])
                {
                    return (IsPositive) ? 1 : -1;
                }

                if (_digits[i] < other._digits[i])
                {
                    return (IsPositive) ? -1 : 1;
                }
            }

            return 0;
        }

        public bool Equals(BigIntegerNumber other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Init();
            other.Init();

            return (other._length == _length) && (other._isNegative == _isNegative) && (Equals(other._digits, _digits));
        }

        #endregion

        #region Object

        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            Init();

            unchecked
            {
                int result = (_digits != null ? _digits.GetHashCode() : 0);
                result = (result * 397) ^ _length;
                result = (result * 397) ^ IsNegative.GetHashCode();
                return result;
            }
        }

        public override string ToString()
        {
            Init();
            var sb = new StringBuilder();
            for (int i = _length - 1; i >= 0; i--)
            {
                int digit = _digits[i];
                int pow = Base / 10;
                for (int j = 0; j < BaseDig; j++)
                {
                    int dec = digit / pow;
                    digit -= dec * pow;
                    pow /= 10;
                    sb.Append(dec);
                }
            }

            while ((sb[0] == '0') && (sb.Length > 1))
            {
                sb = sb.Remove(0, 1);
            }

            if (IsNegative)
            {
                sb.Insert(0, "-");
            }

            return sb.ToString();
        }

        #endregion

        public static readonly BigIntegerNumber MinValue;

        public static readonly BigIntegerNumber MaxValue;

        public static readonly BigIntegerNumber IntMinValue = Parse(int.MinValue.ToString());

        public static readonly BigIntegerNumber IntMaxValue = Parse(int.MaxValue.ToString());

        public static readonly BigIntegerNumber LongMinValue = Parse(long.MinValue.ToString());

        public static readonly BigIntegerNumber LongMaxValue = Parse(long.MaxValue.ToString());

        public static readonly BigIntegerNumber Zero;

        #region String Parsing

        public static bool TryParse(string value, out BigIntegerNumber bigInteger)
        {
            bigInteger = new BigIntegerNumber();
            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            if (value[0] == '-')
            {
                bigInteger._isNegative = true;
                value = value.Substring(1);
                if (value.Length == 0)
                {
                    return false;
                }
            }

            if (value[0] == '+')
            {
                value = value.Substring(1);
                if (value.Length == 0)
                {
                    return false;
                }
            }

            int strLength = value.Length;
            int length = strLength % BaseDig == 0 ? strLength / BaseDig : strLength / BaseDig + 1;
            bigInteger._length = length;
            bigInteger._digits = new int[Digits];
            for (int i = 0; i < length; i++)
            {
                int index = strLength - BaseDig * (i + 1);
                int symbols = BaseDig;
                if (index < 0)
                {
                    symbols = BaseDig + index;
                    index = 0;
                }

                string str = value.Substring(index, symbols);
                if (!int.TryParse(str, out int n))
                {
                    return false;
                }

                bigInteger._digits[i] = n;
            }

            return true;
        }

        public static BigIntegerNumber Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (!TryParse(value, out BigIntegerNumber result))
                throw new ArgumentOutOfRangeException(nameof(value), $"Value \"{value}\" cannot be parsed.");

            return result;
        }

        #endregion

        #region IInteger

        //#region Name

        //public NumberName Name
        //{
        //    get { return NumberNameHelper.GetName(this); }
        //}

        //#endregion

        #region Mnemonic/IMathObject

        public void FromMnemonic(string mnemonic)
        {
            // ReSharper disable once BuiltInTypeReferenceStyleForMemberAccess
            Parse(mnemonic);
        }

        public string ToMnemonic()
        {
            return ToString();
        }

        public object GetPrimitive()
        {
            return _digits;
        }

        public IMathObjectType GetMathType()
        {
            return MathObjectType;
        }

        #endregion

        public INumber Add(INumber y)
        {
            return Add((BigIntegerNumber) y);
        }

        public INumber Sub(INumber y)
        {
            return Sub((BigIntegerNumber) y);
        }

        public INumber Mul(INumber y)
        {
            return Mul((BigIntegerNumber) y);
        }

        public INumber Div(INumber y)
        {
            Div(this, (BigIntegerNumber) y, out BigIntegerNumber q, out _);
            return q;
        }

        public bool IsEqual(INumber y)
        {
            return CompareTo(y) == 0;
        }

        public bool IsNotEqual(INumber y)
        {
            return CompareTo(y) != 0;
        }

        public bool IsGreater(INumber y)
        {
            return CompareTo(y) > 0;
        }

        public bool IsGreaterOrEqual(INumber y)
        {
            return CompareTo(y) >= 0;
        }

        public bool IsLess(INumber y)
        {
            return CompareTo(y) < 0;
        }

        public bool IsLessOrEqual(INumber y)
        {
            return CompareTo(y) <= 0;
        }

        #endregion

        #region Math Object Constants

        public const string MenmonicName = nameof(BigIntegerNumber);

        public static readonly MathObjectType MathObjectType = new MathObjectType(MenmonicName, typeof(BigIntegerNumber));

        #endregion
    }
}