using System;
using System.Collections.Generic;

namespace RealtyStorage.Common.RealtySystem.Collections
{
    public static class ListExtender
    {
        public static List<T> Clone<T>(this List<T> original)
        {
            if (original == null)
                throw new ArgumentNullException(nameof(original));

            return new List<T>(original);
        }
      
        public static void Assign<T>(this List<T> to, List<T> from)
        {
            if (to == null)
                throw new ArgumentNullException(nameof(to));
            if (from == null)
                throw new ArgumentNullException(nameof(from));

            to.Clear();
            to.AddRange(from);
        }
    }
}