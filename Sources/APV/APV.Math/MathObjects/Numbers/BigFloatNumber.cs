using System;

namespace APV.Math.MathObjects.Numbers
{
    /// <summary>
    /// Простая дробь
    /// </summary>
    //[NumberName("BigFloatNumber", "Большое вещественное число")]
    [Serializable]
    public class BigFloatNumber : IFloat, ICloneable, IComparable, IComparable<BigFloatNumber>, IEquatable<BigFloatNumber>
    {
        /// <summary>
        /// Показывает, что дробь уже сокращена
        /// </summary>
        public bool IsGrow { get; private set; }

        /// <summary>
        /// Числитель дроби
        /// </summary>
        public BigIntegerNumber Numerator { get; private set; }

        /// <summary>
        /// Знаменатель дроби
        /// </summary>
        public BigIntegerNumber Term { get; private set; }

        /// <summary>
        /// Положительное
        /// </summary>
        public bool IsNegative { get { return Numerator.IsNegative; } }

        /// <summary>
        /// Отрицательное
        /// </summary>
        public bool IsPositive { get { return !IsNegative; } }

        /// <summary>
        /// Ноль
        /// </summary>
        public bool IsZero { get { return (Numerator.IsZero); } }

        private void Normalize()
        {
            //Sign
            if (Term.IsNegative)
            {
                Term = Term.Invert();
                Numerator = Numerator.Invert();
            }
            //Grow
            Grow();
        }

        /// <summary>
        /// Смена знака (инвертирование)
        /// </summary>
        /// <returns></returns>
        public BigFloatNumber Invert()
        {
            return new BigFloatNumber { Numerator = Numerator.Invert(), Term = (BigIntegerNumber)Term.Clone() };
        }

        /// <summary>
        /// Модуль
        /// </summary>
        public BigFloatNumber Abs()
        {
            return new BigFloatNumber { Numerator = Numerator.Abs(), Term = (BigIntegerNumber)Term.Clone() };
        }

        /// <summary>
		/// Сократить дробь
		/// Нахождение и деление на общий множитель.
		/// Алгоритм Евклида нахождения наибольшего общего делителя чисел
		/// После того как разделим числа на наибольший общий делитель - получим несократимую рациональную дробь
		/// </summary>
		public void Grow()
        {
            if (!IsGrow)
            {
                IsGrow = true;

                if (IsZero)
                {
                    Term = 1;
                }
                else if (Numerator == Term)
                {
                    Numerator = 1;
                    Term = 1;
                }
                else
                {
                    BigIntegerNumber m = Numerator;
                    BigIntegerNumber n = Term;

                    while (true)
                    {
                        BigIntegerNumber r = m % n;
                        if (r == 0)
                        {
                            break;
                        }
                        m = n;
                        n = r;
                    }

                    n = n.Abs();
                    Numerator = Numerator / n;
                    Term = Term / n;
                }
            }
        }

        public void Assign(BigFloatNumber bigFloat)
        {
            Numerator = bigFloat.Numerator;
            Term = bigFloat.Term;
        }

        public object Clone()
        {
            var result = new BigFloatNumber();
            result.Assign(this);
            return result;
        }

        public static BigFloatNumber operator +(BigFloatNumber x, BigFloatNumber y)
        {
            BigIntegerNumber n = x.Numerator * y.Term + x.Term * y.Numerator;
            BigIntegerNumber t = x.Term * y.Term;
            var result = new BigFloatNumber
            {
                Numerator = n,
                Term = t
            };
            result.Normalize();
            return result;
        }

        public static BigFloatNumber operator -(BigFloatNumber x, BigFloatNumber y)
        {
            BigIntegerNumber n = x.Numerator * y.Term - x.Term * y.Numerator;
            BigIntegerNumber t = x.Term * y.Term;
            var result = new BigFloatNumber
            {
                Numerator = n,
                Term = t
            };
            result.Normalize();
            return result;
        }

        public static BigFloatNumber operator *(BigFloatNumber x, BigFloatNumber y)
        {
            BigIntegerNumber n = x.Numerator * y.Numerator;
            BigIntegerNumber t = x.Term * y.Term;
            var result = new BigFloatNumber
            {
                Numerator = n,
                Term = t
            };
            result.Normalize();
            return result;
        }

        public static BigFloatNumber operator /(BigFloatNumber x, BigFloatNumber y)
        {
            BigIntegerNumber n = x.Numerator * y.Term;
            BigIntegerNumber t = x.Term * y.Numerator;
            var result = new BigFloatNumber
            {
                Numerator = n,
                Term = t
            };
            result.Normalize();
            return result;
        }

        public static bool operator ==(BigFloatNumber x, BigFloatNumber y)
        {
            return x.CompareTo(y) == 0;
        }

        public static bool operator !=(BigFloatNumber x, BigFloatNumber y)
        {
            return x.CompareTo(y) != 0;
        }

        public static bool operator >(BigFloatNumber x, BigFloatNumber y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            return x.CompareTo(y) > 0;
        }

        public static bool operator >=(BigFloatNumber x, BigFloatNumber y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            return x.CompareTo(y) >= 0;
        }

        public static bool operator <(BigFloatNumber x, BigFloatNumber y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));

            return x.CompareTo(y) < 0;
        }

        public static bool operator <=(BigFloatNumber x, BigFloatNumber y)
        {
            return x.CompareTo(y) <= 0;
        }

        public static BigFloatNumber operator -(BigFloatNumber x)
        {
            return x.Invert();
        }

        public static implicit operator BigFloatNumber(float x)
        {
            int counter = 0;
            //Находим показатель степени десятки такой
            while (((int)(x % 1) != 0) && (counter < 6)) //float precision - 7 digits, последнюю цифру отбрасываем
            {
                x *= 10;
                counter++;
            }

            //В этом месте имеем
            BigIntegerNumber numerator = x;
            BigIntegerNumber term = (int)System.Math.Pow(10, counter);

            //Нужно быть внимательным и учитывать ошибки округления: если изначально имели дробь 5/3
            //то в компьютере она представляется приближенной дробью 1.66666666666667
            //если проведем обратное преобразование в рациональную, то уже не получим 5/3, а получим близкую к нему
            //рациональную дробь 166666666666667/100000000000000
            var result = new BigFloatNumber
            {
                Numerator = numerator,
                Term = term
            };
            result.Grow();
            return result;
        }

        public static implicit operator BigFloatNumber(int x)
        {
            return new BigFloatNumber { Numerator = x, Term = 1, IsGrow = true };
        }

        public static implicit operator BigFloatNumber(BigIntegerNumber x)
        {
            return new BigFloatNumber { Numerator = x, Term = 1, IsGrow = true };
        }

        public static explicit operator float(BigFloatNumber x)
        {
            BigIntegerNumber.Div(x.Numerator, x.Term, out BigIntegerNumber q, out _);
            var intPart = (float)q;
            return intPart;
        }

        #region IComparable, IEquatable

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return 1;
            }

            if (obj.GetType() != typeof(BigFloatNumber))
            {
                return 1;
            }

            return CompareTo((BigFloatNumber)obj);
        }

        public int CompareTo(BigFloatNumber other)
        {
            if ((IsZero) && (other.IsZero))
            {
                return 0;
            }

            if ((IsPositive) && (other.IsNegative))
            {
                return 1;
            }

            if ((IsNegative) && (other.IsPositive))
            {
                return -1;
            }

            if ((IsZero) && (!other.IsZero) && (other.IsPositive))
            {
                return -1;
            }

            if ((IsZero) && (!other.IsZero) && (other.IsNegative))
            {
                return 1;
            }

            if ((other.IsZero) && (!IsZero) && (IsPositive))
            {
                return 1;
            }

            if ((other.IsZero) && (!IsZero) && (IsNegative))
            {
                return -1;
            }

            return (Numerator * other.Term).CompareTo(Term * other.Numerator);
        }

        public bool Equals(BigFloatNumber other)
        {
            return (other != null) && (other.IsGrow.Equals(IsGrow)) && (other.Numerator.Equals(Numerator)) && (other.Term.Equals(Term));
        }

        #endregion

        #region INumber

        //#region Name

        //public NumberName Name
        //{
        //    get { return NumberNameHelper.GetName(this); }
        //}

        //#endregion

        #region Mnemonic/IMathObject

        public void FromMnemonic(string mnemonic)
        {
            //Parse(mnemonic);
            throw new NotImplementedException();
        }

        public string ToMnemonic()
        {
            return ToString();
        }

        public object GetPrimitive()
        {
            throw new NotSupportedException();
        }

        public IMathObjectType GetMathType()
        {
            return MathObjectType;
        }

        #endregion

        public INumber Div(INumber y)
        {
            return this / (BigFloatNumber)y;
        }

        public INumber Add(INumber y)
        {
            return this + (BigFloatNumber)y;
        }

        public INumber Sub(INumber y)
        {
            return this - (BigFloatNumber)y;
        }

        public INumber Mul(INumber y)
        {
            return this * (BigFloatNumber)y;
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

        #region Object

        public override bool Equals(object obj)
        {
            return CompareTo(obj) == 0;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = IsGrow.GetHashCode();
                result = (result * 397) ^ Numerator.GetHashCode();
                result = (result * 397) ^ Term.GetHashCode();
                return result;
            }
        }

        public override string ToString()
        {
            return $"{Numerator}/{Term}";
        }

        #endregion

        public bool IsNull
        {
            get
            {
                return (Term == 0);
            }
        }

        public bool IsNatural
        {
            get { return (this > 0); }
        }

        public static BigFloatNumber Zero
        {
            get { return new BigFloatNumber { Numerator = 0, Term = 1, IsGrow = true }; }
        }

        public static BigFloatNumber Create(long numerator, int term)
        {
            if (term == 0)
                throw new ArgumentOutOfRangeException(nameof(term), "(term == 0)");

            if (numerator == 0)
            {
                return new BigFloatNumber
                {
                    Numerator = 0,
                    Term = 1,
                };
            }
            bool isPositive = ((numerator > 0) && (term > 0)) || ((numerator < 0) && (term < 0));
            var result = new BigFloatNumber
            {
                Numerator = isPositive ? System.Math.Abs(numerator) : -System.Math.Abs(numerator),
                Term = System.Math.Abs(term),
            };
            result.Normalize();
            return result;
        }

        #region Math Object Constants

        public const string MenmonicName = nameof(BigFloatNumber);

        public static readonly MathObjectType MathObjectType = new MathObjectType(MenmonicName, typeof(BigFloatNumber));

        #endregion
    }
}
