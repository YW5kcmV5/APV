using System;
using System.Diagnostics;
using System.Globalization;

namespace APV.Math.MathObjects.Numbers
{
    [DebuggerDisplay("{ToMnemonic()}")]
    public class Int64Number : INumber, IPrimitiveNumber, IEquatable<Int64Number>, IComparable<Int64Number>
    {
        // ReSharper disable once BuiltInTypeReferenceStyle
        private Int64 _value;

        public Int64Number()
        {
        }

        public Int64Number(Int64 value)
        {
            _value = value;
        }

        public void FromMnemonic(string mnemonic)
        {
            // ReSharper disable once BuiltInTypeReferenceStyleForMemberAccess
            _value = Int64.Parse(mnemonic);
        }

        public string ToMnemonic()
        {
            return _value.ToString(CultureInfo.InvariantCulture);
        }

        public object GetPrimitive()
        {
            return _value;
        }

        IMathObjectType IMathObject.GetMathType()
        {
            return GetMathType();
        }

        public MathObjectType GetMathType()
        {
            return MathObjectType;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Int64Number)obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public bool Equals(Int64Number other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            return (_value == other._value);
        }

        public int CompareTo(Int64Number other)
        {
            if (ReferenceEquals(other, null))
            {
                return 1;
            }
            return _value.CompareTo(other._value);
        }

        public static Int64Number operator *(Int64Number x, Int64Number y)
        {
            if (ReferenceEquals(x, null))
                throw new ArgumentNullException("x");
            if (ReferenceEquals(y, null))
                throw new ArgumentNullException("y");

            return new Int64Number(x._value * y._value);
        }

        public static Int64Number operator +(Int64Number x, Int64Number y)
        {
            if (ReferenceEquals(x, null))
                throw new ArgumentNullException("x");
            if (ReferenceEquals(y, null))
                throw new ArgumentNullException("y");

            return new Int64Number(x._value + y._value);
        }

        public static Int64Number operator -(Int64Number x, Int64Number y)
        {
            if (ReferenceEquals(x, null))
                throw new ArgumentNullException("x");
            if (ReferenceEquals(y, null))
                throw new ArgumentNullException("y");

            return new Int64Number(x._value - y._value);
        }

        public static Int64Number operator -(Int64Number x)
        {
            if (ReferenceEquals(x, null))
                throw new ArgumentNullException("x");

            return new Int64Number(-x._value);
        }

        public static Int64Number operator ^(Int64Number x, Int64Number y)
        {
            if (ReferenceEquals(x, null))
                throw new ArgumentNullException("x");
            if (ReferenceEquals(y, null))
                throw new ArgumentNullException("y");

            var value = (long) System.Math.Pow(x._value, y._value);
            return new Int64Number(value);
        }

        public static Int64Number operator /(Int64Number x, Int64Number y)
        {
            if (ReferenceEquals(x, null))
                throw new ArgumentNullException("x");
            if (ReferenceEquals(y, null))
                throw new ArgumentNullException("y");

            return new Int64Number(x._value / y._value);
        }

        public static explicit operator Int64Number(Int64 value)
        {
            return new Int64Number(value);
        }

        public static explicit operator Int64Number(Int32 value)
        {
            return new Int64Number(value);
        }

        public static explicit operator Int64Number(byte value)
        {
            return new Int64Number(value);
        }

        public static bool operator ==(Int64Number x, Int64Number y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if ((ReferenceEquals(x, null)) || (ReferenceEquals(y, null)))
            {
                return false;
            }
            return (x._value == y._value);
        }

        public static bool operator !=(Int64Number x, Int64Number y)
        {
            return !(x == y);
        }

        public const string MenmonicName = @"Int64";

        public static readonly MathObjectType MathObjectType = new MathObjectType(MenmonicName, typeof(Int64Number));
    }
}