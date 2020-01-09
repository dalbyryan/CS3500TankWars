using System;
using System.Collections.Generic;
using System.Text;
using static FormulaEvaluator.ExpressionTokenUtils;

namespace FormulaEvaluator
{
    /// <summary>
    /// utility functions that extend stack functionality. 
    /// these methods make the Evaluator code more readable and DRY.
    /// </summary>
    internal static class StackExtensions
    {

        public static bool IsNotEmpty<T>(this Stack<T> stack)
        {
            return stack.Count > 0;
        }

        public static bool IsEmpty<T>(this Stack<T> stack)
        {
            return stack.Count == 0;
        }

        public static bool NextOperatorIsAdd(this Stack<string> stack)
        {
            return stack.Count > 0 && stack.Peek() == ADD;
        }

        public static bool NextOperatorIsSubtract(this Stack<string> stack)
        {
            return stack.Count > 0 && stack.Peek() == SUBTRACT;
        }

        public static bool NextOperatorIsMultiply(this Stack<string> stack)
        {
            return stack.Count > 0 && stack.Peek() == MULTIPLY;
        }

        public static bool NextOperatorIsDivide(this Stack<string> stack)
        {
            return stack.Count > 0 && stack.Peek() == DIVIDE;
        }

    }
}
