using System;

namespace APV.Common
{
    public static class Comparator
    {
        #region Private

        private static void Add(int[] values, int[] limits)
        {
            int length = values.Length;
            int index = 0;
            do
            {
                int value = values[index];
                value++;
                if (value <= limits[index])
                {
                    values[index] = value;
                    break;
                }
                values[index] = 0;
                index++;
                if (index >= length)
                {
                    break;
                }
            } while (true);
        }

        #endregion

        public static bool Equals(this byte[] x, byte[] y)
        {
            if (x == y)
            {
                return true;
            }
            if ((x == null) || (y == null) || (x.Length != y.Length))
            {
                return false;
            }

            int length = x.Length;
            for (int i = 0; i < length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Equals(this int[] x, int[] y)
        {
            if (x == y)
            {
                return true;
            }
            if ((x == null) || (y == null) || (x.Length != y.Length))
            {
                return false;
            }

            int length = x.Length;
            for (int i = 0; i < length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Equals(this long[] x, long[] y)
        {
            if (x == y)
            {
                return true;
            }
            if ((x == null) || (y == null) || (x.Length != y.Length))
            {
                return false;
            }

            int length = x.Length;
            for (int i = 0; i < length; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }
            return true;
        }

        public static bool Equals(this Array x, Array y)
        {
            if (x == y)
            {
                return true;
            }
            if ((x == null) || (y == null) || (x.GetType() != y.GetType()) ||
                (x.GetType().GetElementType() != y.GetType().GetElementType()) ||
                (x.Rank != y.Rank) || (x.LongLength != y.LongLength))
            {
                return false;
            }

            int rank = x.Rank;
            var limits = new int[rank];
            for (int dimension = 0; dimension < rank; dimension++)
            {
                int dimensionLength = x.GetLength(dimension);
                if (dimensionLength != y.GetLength(dimension))
                {
                    return false;
                }
                limits[dimension] = dimensionLength - 1;
            }

            long length = x.LongLength;
            var indexes = new int[rank];
            for (long index = 0; index < length; index++)
            {
                object xValue = x.GetValue(indexes);
                object yValue = y.GetValue(indexes);
                if (!Equals(xValue, yValue))
                {
                    return false;
                }

                Add(indexes, limits);
            }

            return true;
        }

        public static bool Equals(DateTime x, DateTime y)
        {
            if (x == y)
            {
                return true;
            }
            string xValue = x.ToUniversalTime().ToString("o");
            string yValue = y.ToUniversalTime().ToString("o");
            return (xValue == yValue);
        }

        public new static bool Equals(object x, object y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if ((ReferenceEquals(x, null)) || (ReferenceEquals(y, null)) || (x.GetType() != y.GetType()))
            {
                return false;
            }
            Type type = x.GetType();
            if (type == typeof(bool))
            {
                return (bool)x == (bool)y;
            }
            if (type == typeof(bool?))
            {
                return (bool?)x == (bool?)y;
            }
            if (type == typeof(byte))
            {
                return (byte)x == (byte)y;
            }
            if (type == typeof(byte?))
            {
                return (byte?)x == (byte?)y;
            }
            if (type == typeof(int))
            {
                return (int)x == (int)y;
            }
            if (type == typeof(int?))
            {
                return (int?)x == (int?)y;
            }
            if (type == typeof(long))
            {
                return (long)x == (long)y;
            }
            if (type == typeof(long?))
            {
                return (long?)x == (long?)y;
            }
            if (type == typeof(byte[]))
            {
                return Equals((byte[])x, (byte[])y);
            }
            if (type == typeof(int[]))
            {
                return Equals((int[])x, (int[])y);
            }
            if (type == typeof(long[]))
            {
                return Equals((long[])x, (long[])y);
            }
            if (type.IsArray)
            {
                return Equals((Array)x, (Array)y);
            }
            if (type == typeof(DateTime))
            {
                return Equals((DateTime)x, (DateTime)y);
            }
            if (type == typeof(DateTime?))
            {
                return Equals(((DateTime?)x).Value, ((DateTime?)y).Value);
            }
            if (type.IsEnum)
            {
                Type underlyingType = Enum.GetUnderlyingType(type);
                if (underlyingType == typeof(byte))
                {
                    return (byte)x == (byte)y;
                }
                if (underlyingType == typeof(int))
                {
                    return (int)x == (int)y;
                }
                if (underlyingType == typeof(long))
                {
                    return (long)x == (long)y;
                }
            }

            return object.Equals(x, y);
        }
    }
}