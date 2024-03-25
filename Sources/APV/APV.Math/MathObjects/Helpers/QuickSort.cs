using System;
using System.Collections.Generic;

namespace APV.Math.MathObjects.Helpers
{
    [Serializable]
    public class QuickSort<T>
    {
        [Serializable]
        public delegate void SwapDelegate(int index1, int index2);

        private static void SwapIfGreaterWithItems(T[] keys, IComparer<T> comparer, SwapDelegate swaper, int a, int b)
        {
            if ((a != b) && (comparer.Compare(keys[a], keys[b]) > 0))
            {
                swaper(a, b);
            }
        }

        private static void Sort(T[] keys, int left, int right, IComparer<T> comparer, SwapDelegate swaper)
        {
            do
            {
                int a = left;
                int b = right;
                int num3 = a + ((b - a) >> 1);
                SwapIfGreaterWithItems(keys, comparer, swaper, a, num3);
                SwapIfGreaterWithItems(keys, comparer, swaper, a, b);
                SwapIfGreaterWithItems(keys, comparer, swaper, num3, b);
                T y = keys[num3];
                do
                {
                    while (comparer.Compare(keys[a], y) < 0)
                    {
                        a++;
                    }
                    while (comparer.Compare(y, keys[b]) < 0)
                    {
                        b--;
                    }
                    if (a > b)
                    {
                        break;
                    }
                    if (a < b)
                    {
                        swaper(a, b);
                    }
                    a++;
                    b--;
                }
                while (a <= b);
                if ((b - left) <= (right - a))
                {
                    if (left < b)
                    {
                        Sort(keys, left, b, comparer, swaper);
                    }
                    left = a;
                }
                else
                {
                    if (a < right)
                    {
                        Sort(keys, a, right, comparer, swaper);
                    }
                    right = b;
                }
            }
            while (left < right);
        }

        public static void Sort(T[] keys, IComparer<T> comparer, SwapDelegate swaper)
        {
            if (comparer == null)
            {
                comparer = Comparer<T>.Default;
            }
            Sort(keys, 0, keys.Length - 1, comparer, swaper);
        }

        public static void Sort(T[] keys, SwapDelegate swaper)
        {
            Sort(keys, null, swaper);
        }
    }
}
