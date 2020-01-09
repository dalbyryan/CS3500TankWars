// Luke Ludlow
// CS 3500 
// 2019 September 

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace SpreadsheetTests
{
    /// <summary>
    /// utility functions used specifically for testing. 
    /// </summary>
    internal static class SpreadsheetTestUtils
    {

        public static bool FilesAreEqual(string filename1, string filename2)
        {
            // read the files to compare 
            string text1 = File.ReadAllText(filename1);
            string text2 = File.ReadAllText(filename2);

            // have to do a bunch of weird tricks so that this test will work on windows too 
            // (windows file formats and whitespace stuff is annoying)
            // (i code on mac and autograder uses linux)

            // replace whitespace around xml nodes 
            string replaceRight = ">\\s+";
            string replaceLeft = "\\s+<";
            text1 = Regex.Replace(text1, replaceRight, ">");
            text1 = Regex.Replace(text1, replaceLeft, "<");
            text2 = Regex.Replace(text2, replaceRight, ">");
            text2 = Regex.Replace(text2, replaceLeft, "<");

            // now compare
            return text1 == text2;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        public static int GetCount<T>(this IEnumerable<T> enumerable)
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

    }
}
