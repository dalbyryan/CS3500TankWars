using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    /// <summary>
    /// utility functions for expression tokens
    /// </summary>
    internal static class ExpressionTokenUtils
    {

        public const string ADD = "+";
        public const string SUBTRACT = "-";
        public const string MULTIPLY = "*";
        public const string DIVIDE = "/";
        public const string LEFT_PAREN = "(";
        public const string RIGHT_PAREN = ")";

        /// <summary>
        /// takes the original expression string and splits it using regex. 
        /// trims all whitespace and empty strings.
        /// validates all tokens.
        /// </summary>
        /// <param name="expression">the original expression string to be evaluated</param>
        /// <returns>list of all the expression tokens, ready to be evaluated</returns>
        public static List<string> ParseExpressionTokens(string expression)
        {
            List<string> expressionTokens = (Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)")).ToList();
            expressionTokens = TrimExpressionTokensWhitespace(expressionTokens);
            ValidateAllTokens(expressionTokens);
            return expressionTokens;
        }

        /// <summary>
        /// returns true if given token is a valid variable.
        /// variable format is any assortment of letters followed by any assortment of numbers.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsVariable(this string token)
        {
            return Regex.Match(token, "^[a-zA-Z]+[0-9]+$").Success;
        }

        public static bool IsPositiveInteger(this string token)
        {
            bool canConvertToInteger = int.TryParse(token, out int tokenValue);
            bool isPositive = (tokenValue >= 0);
            return canConvertToInteger && isPositive;
        }

        /// <summary>
        /// returns true if given token is one of the valid operators
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool IsOperator(this string token)
        {
            List<string> operators = new List<string> { ADD, SUBTRACT, MULTIPLY, DIVIDE, LEFT_PAREN, RIGHT_PAREN };
            return operators.Contains(token);
        }

        /// <summary>
        /// after trimming surrounding whitespace, the only valid tokens are 
        /// (, ), +, -, *, /, 
        /// non-negative integers, 
        /// and strings that begin with one or more letters and end with one or more digit
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static bool IsValidToken(this string token)
        {
            return (IsPositiveInteger(token) || IsOperator(token) || IsVariable(token));
        }

        /// <summary>
        /// throws ArgumentException if there is any invalid token in the expression tokens
        /// </summary>
        /// <param name="expressionTokens"></param>
        private static void ValidateAllTokens(List<string> expressionTokens)
        {
            foreach (string s in expressionTokens)
            {
                if (!s.IsValidToken())
                {
                    throw new ArgumentException(String.Format("found invalid token: {0}", s));
                }
            }
        }

        /// <summary>
        /// ignore any empty strings from the original regex split.
        /// remove leading and trailing whitespace from tokens. 
        /// </summary>
        /// <param name="expressionTokens"></param>
        /// <returns></returns>
        private static List<string> TrimExpressionTokensWhitespace(List<string> expressionTokens)
        {
            List<string> trimmedExpressionTokens = new List<string>();
            foreach (string s in expressionTokens)
            {
                // ignore any empty strings
                if (!String.IsNullOrWhiteSpace(s))
                {
                    // ignore leading and trailing whitespace in each token 
                    trimmedExpressionTokens.Add(s.Trim());
                }
            }
            return trimmedExpressionTokens;
        }

    }
}
