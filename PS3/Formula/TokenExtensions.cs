// Luke Ludlow
// CS 3500
// 2019 September

using System;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// TODO comment
    /// </summary>
    internal static class TokenExtensions
    {

        /// <summary>
        /// The allowed symbols are non-negative numbers written using double-precision 
        /// floating-point syntax (without unary preceeding '-' or '+'); 
        /// variables that consist of a letter or underscore followed by 
        /// zero or more letters, underscores, or digits; parentheses; and the four operator 
        /// symbols +, -, *, and /.  
        /// </summary>
        /// <returns>true if the token is one of the valid types</returns>
        public static bool IsValidToken(this string token)
        {
            return (token.IsVariable()
                  || token.IsDouble()
                  || token.IsOperator()
                  || token.IsLeftParen()
                  || token.IsRightParen());
        }

        /// <summary>
        /// initial tokens that are variables must follow this format:
        /// a string consisting of a letter or underscore followed by zero or more letters, digits, or underscores.
        /// (this is checked BEFORE the token makes it to the validateVariable delegate).
        /// </summary>
        /// <returns>true if token is a valid variable format</returns>
        public static bool IsVariable(this string token)
        {
            string variablePattern = @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$";
            return Regex.IsMatch(token, variablePattern, RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// valid double format is anything that can be parsed by Double.Parse,
        /// including scientific notation.
        /// </summary>
        /// <returns>true if token is a valid double format</returns>
        public static bool IsDouble(this string token)
        {
            string doublePattern = @"^(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?$";
            return Regex.IsMatch(token, doublePattern, RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// valid operators are +, -, *, and /
        /// </summary>
        /// <returns>true if token is a valid operator</returns>
        public static bool IsOperator(this string token)
        {
            string operatorPattern = @"^[\+\-*/]$";
            return Regex.IsMatch(token, operatorPattern, RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// check if token is a left parenthesis
        /// </summary>
        /// <returns>true if token is a left parenthesis</returns>
        public static bool IsLeftParen(this string token)
        {
            string leftParenPattern = @"^\($";
            return Regex.IsMatch(token, leftParenPattern, RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// check if token is a right parenthesis
        /// </summary>
        /// <returns>true if token is a right parethesis</returns>
        public static bool IsRightParen(this string token)
        {
            string rightParenPattern = @"^\)$";
            return Regex.IsMatch(token, rightParenPattern, RegexOptions.IgnorePatternWhitespace);
        }

    }
}