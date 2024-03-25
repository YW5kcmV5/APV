using System;
using APV.Common;
using APV.TransControl.Common;

namespace APV.TransControl.Core.DataLayer
{
    public static class TransConverter
    {
        public static bool IsNull(object value)
        {
            if ((value == null) || (value is DBNull))
            {
                return true;
            }
            return false;
        }

        public static int ToInt(object value)
        {
            if (!IsNull(value))
            {
                if (value is decimal)
                {
                    return decimal.ToInt32((decimal)value);
                }
                if (value is long)
                {
                    return (int)(long)value;
                }
                if (value is int)
                {
                    return (int)value;
                }
            }
            throw new InvalidCastException(string.Format("Can not convert value to Int32 from type='{0}', value='{1}'", ((value != null) ? value.GetType().ToString() : "null"), value));
        }

        public static OperationResult ToOperationResult(object value)
        {
            return (OperationResult) ToInt(value);
        }

        public static double ToDouble(object value)
        {
            if (!IsNull(value))
            {
                if (value is double)
                {
                    return (double)value;
                }
                if (value is decimal)
                {
                    return decimal.ToDouble((decimal)value);
                }
                if (value is Int16)
                {
                    return (Int16)value;
                }
                if (value is Int32)
                {
                    return (Int32)value;
                }
                if (value is Single)
                {
                    return (Single)value;
                }
            }
            throw new InvalidCastException(string.Format("Can not convert value to Double from type='{0}', value='{1}'", ((value != null) ? value.GetType().ToString() : "null"), value));
        }

        public static string ToString(object value)
        {
            if (IsNull(value))
                throw new InvalidCastException(string.Format("Can not convert value to String from type='{0}', value='{1}'", ((value != null) ? value.GetType().ToString() : "null"), value));

            return ToNullableString(value);
        }

        public static string ToNullableString(object value)
        {
            if (!IsNull(value))
            {
                if (value is string)
                {
                    return (string)value;
                }

                if (value is DateTime)
                {
                    return ((DateTime) value).ToString("u");
                }

                if (value is byte[])
                {
                    return ((byte[])value).ToHexString();
                }
            }
            return null;
        }

        public static byte[] ToByteArray(object value)
        {
            if (IsNull(value))
                throw new InvalidCastException(string.Format("Can not convert value to String from type='{0}', value='{1}'", ((value != null) ? value.GetType().ToString() : "null"), value));

            return ToNullableByteArray(value);
        }

        public static byte[] ToNullableByteArray(object value)
        {
            if (!IsNull(value))
            {
                return (byte[])value;
            }
            return null;
        }

        public static DateTime ToDateTime(object value)
        {
            if (!IsNull(value))
            {
                if (value is DateTime)
                {
                    return (DateTime)value;
                }
            }
            throw new InvalidCastException(string.Format("Can not convert value to DateTime from type='{0}', value='{1}'", ((value != null) ? value.GetType().ToString() : "null"), value));
        }

        public static bool ToBoolean(object value)
        {
            if (!IsNull(value))
            {
                if (value is bool)
                {
                    return (bool)value;
                }
            }
            throw new InvalidCastException(string.Format("Can not convert value to bool from type='{0}', value='{1}'", ((value != null) ? value.GetType().ToString() : "null"), value));
        }

        public static VehicleType ToVehicleType(object value)
        {
            string str = ToString(value).Trim();
            return (string.Compare(str, "A", StringComparison.InvariantCultureIgnoreCase) == 0)
                       ? VehicleType.Avto
                       : VehicleType.DST;
        }
    }
}
