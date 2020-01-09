// Luke Ludlow
// CS 3500 
// 2019 September 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{
  /// <summary>
  /// utility functions that extend IEnumerable. 
  /// </summary>
  internal static class EnumerableExtensions
  {

    /// <summary>
    /// count the number of elements in the enumerable
    /// </summary>
    public static int Count<T>(this IEnumerable<T> enumerable)
    {
      int count = 0;
      foreach (T item in enumerable) {
        count++;
      }
      return count;
    }

    /// <summary>
    /// provides a copy of the original collection for us to safely iterate over.
    /// </summary>
    public static IEnumerable<string> Clone(this IEnumerable<string> enumerable)
    {
      return enumerable.ToList();
    }

  }
}
