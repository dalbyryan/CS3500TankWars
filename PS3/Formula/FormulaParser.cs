// Luke Ludlow
// CS 3500 
// 2019 September

using System;
using System.Text;
using System.Collections.Generic;
using static SpreadsheetUtilities.Formula;

namespace SpreadsheetUtilities
{
    internal static class FormulaParser
    {

        public static string ConvertFormulaTokensToString(List<string> formulaTokens)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string token in formulaTokens) {
                sb.Append(token);
            }
            return sb.ToString();
        }

        public static List<string> ParseFormulaTokens(string formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            try {
                List<string> formulaTokens = new List<string>();
                foreach (string s in GetTokens(formula)) {
                    formulaTokens.Add(ParseToken(s, normalize, isValid));
                }
                return formulaTokens;
            } catch (FormulaFormatException e) {
                throw e;
            } catch (Exception e) {
                throw new FormulaFormatException("an unknown error occurred. " + e.Message);
            }
        }

        private static string ParseToken(string token, Func<string, string> normalize, Func<string, bool> isValid)
        {
            if (token.IsValidToken()) {
                if (token.IsVariable()) {
                    string normalizedVariable = GetNormalizedVariable(token, normalize);
                    string validatedVariable = GetValidatedVariable(normalizedVariable, isValid);
                    return validatedVariable;
                } else if (token.IsDouble()) {
                    return ParseDouble(token);
                } else {
                    return token;
                }
            } else {
                throw new FormulaFormatException(String.Format("\"{0}\" is not a valid token", token));
            }
        }

        /// <summary>
        /// first, the normalizer should not be called on non-variable tokens, so you must check for standard variable
        /// syntax beforehand.
        /// second, the normalizer could potentially make the variable invalid, so after passing it through the normalize
        /// delegate we need to check again that it meets the standard variable syntax.
        /// </summary>
        /// <param name="s">variable token to normalize</param>
        /// <param name="normalize">normalization function delegate</param>
        /// <returns>normalized form of variable</returns>
        private static string GetNormalizedVariable(string s, Func<string, string> normalize)
        {
            string normalizedVariable = normalize(s);
            if (normalizedVariable.IsVariable()) {
                return normalizedVariable;
            } else {
                throw new FormulaFormatException(String.Format("result of normalization was a non-variable token \"0\"", s));
            }
        }

        private static string GetValidatedVariable(string s, Func<string, bool> isValid)
        {
            if (isValid(s)) {
                return s;
            } else {
                throw new FormulaFormatException(String.Format("\"{0}\" is not a valid variable name", s));
            }
        }

        private static string ParseDouble(string s)
        {
            return Double.Parse(s).ToString();
        }

    }
}