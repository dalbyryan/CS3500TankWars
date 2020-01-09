// Luke Ludlow
// CS 3500 
// 2019 September 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormulaTester
{
    /// <summary>
    /// enumerably utility functions used specifically for testing. 
    /// this class has some additional methods that are used for testing purposes.
    /// </summary>
    internal static class EnumerableTestingUtils
    {

        public static int Count<T>(this IEnumerable<T> enumerable)
        {
            int count = 0;
            foreach (T item in enumerable) {
                count++;
            }
            return count;
        }

        public static bool Contains(this IEnumerable<string> enumerable, string s)
        {
            foreach (string item in enumerable) {
                if (s == item) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

    }
}
