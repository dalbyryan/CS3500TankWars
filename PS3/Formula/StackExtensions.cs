using System;
using System.Collections.Generic;
using System.Text;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// utility functions that extend stack functionality. 
    /// these methods make the Evaluator code more readable and DRY.
    /// </summary>
    internal static class StackExtensions
    {

        /// <summary>
        /// checks if any of the given strings is on top of the stack
        /// </summary>
        public static bool OnTop(this Stack<string> stack, params string[] searchValues)
        {
            foreach (string s in searchValues) {
                if (stack.IsNotEmpty() && stack.Peek() == s) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsNotEmpty<T>(this Stack<T> stack)
        {
            return stack.Count > 0;
        }

    }
}
