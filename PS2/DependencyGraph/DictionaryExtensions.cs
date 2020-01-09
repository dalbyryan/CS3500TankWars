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
  /// utility functions that extend Dictionary. 
  /// </summary>
  internal static class DictionaryExtensions
  {

    /// <summary>
    /// gets the key's value from the dictionary if it exists.
    /// otherwise, returns an empty set instead of null or throwing an exception.
    /// </summary>
    /// <returns>the key's value if it exists, otherwise returns an empty set</returns>
    public static HashSet<string> GetValue(this Dictionary<string, HashSet<string>> dictionary, string key)
    {
      if (dictionary.TryGetValue(key, out HashSet<string> value)) {
        return value;
      } else {
        return new HashSet<string>();
      }
    }

    /// <summary>
    /// if the dictionary contains the key and the value is an empty set,
    /// remove the entry from the dictionary.
    /// </summary>
    public static void RemoveEntryIfValueIsEmpty(this Dictionary<string, HashSet<string>> dictionary, string key)
    {
      if (dictionary.GetValue(key).Count == 0) {
        dictionary.Remove(key);
      }
    }

  }
}
