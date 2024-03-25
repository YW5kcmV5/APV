using System.Collections.Generic;

namespace RealtyStorage.Common.RealtySystem.Collections
{
    public class AscLongComparer : IComparer<long>
    {
        public int Compare(long x, long y)
        {
            return x.CompareTo(y);
        }

        public static AscLongComparer Default = new AscLongComparer();
    }

    public class DescLongComparer : IComparer<long>
    {
        public int Compare(long x, long y)
        {
            return y.CompareTo(x);
        }

        public static DescLongComparer Default = new DescLongComparer();
    }
}