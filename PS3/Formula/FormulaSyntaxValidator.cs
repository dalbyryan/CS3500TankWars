// Luke Ludlow
// CS 3500 
// 2019 September

using System;
using System.Collections.Generic;
using static SpreadsheetUtilities.Formula;

namespace SpreadsheetUtilities
{
    internal static class FormulaSyntaxValidator
    {

        /// <summary>
        /// throws FormulaFormatException if formula contains invalid syntax
        /// </summary>
        public static void ValidateSyntax(List<string> tokens)
        {
            try {
                AtLeastOneToken(tokens);
                RightParenthesesRule(tokens);
                BalancedParenthesesRule(tokens);
                StartingTokenRule(tokens);
                EndingTokenRule(tokens);
                ParenthesisOrOperatorFollowingRule(tokens);
                ExtraFollowingRule(tokens);
            } catch (FormulaFormatException e) {
                throw e;
            } catch (Exception e) {
                throw new FormulaFormatException("an unknown error occurred while validating syntax. " + e.Message);
            }
        }

        /// <summary>
        /// There must be at least one token.
        /// </summary>
        private static void AtLeastOneToken(List<string> tokens)
        {
            if (tokens.Count <= 0) {
                throw new FormulaFormatException("formula must contain at least one token");
            }
        }

        /// <summary>
        /// When reading tokens from left to right, at no point should the number of 
        /// closing parentheses seen so far be greater than the number of opening parentheses seen so far.
        /// </summary>
        private static void RightParenthesesRule(List<string> tokens)
        {
            int numLeftParens = 0;
            int numRightParens = 0;
            foreach (string token in tokens) {
                if (token.IsLeftParen()) {
                    numLeftParens++;
                } else if (token.IsRightParen()) {
                    numRightParens++;
                }
                if (numRightParens > numLeftParens) {
                    throw new FormulaFormatException("unmatched right parenthesis");
                }
            }
        }

        /// <summary>
        /// The total number of opening parentheses must equal the total number of closing parentheses.
        /// </summary>
        private static void BalancedParenthesesRule(List<string> tokens)
        {
            int numLeftParens = 0;
            int numRightParens = 0;
            foreach (string token in tokens) {
                if (token.IsLeftParen()) {
                    numLeftParens++;
                } else if (token.IsRightParen()) {
                    numRightParens++;
                }
            }
            if (numLeftParens != numRightParens) {
                throw new FormulaFormatException("unbalanced parentheses");
            }
        }

        /// <summary>
        /// The first token of an expression must be a number, a variable, or an opening parenthesis.
        /// </summary>
        private static void StartingTokenRule(List<string> tokens)
        {
            string firstToken = tokens[0];
            bool firstTokenIsNumberOrVariableOrLeftParen = firstToken.IsDouble()
                                                        || firstToken.IsVariable()
                                                        || firstToken.IsLeftParen();
            if (!firstTokenIsNumberOrVariableOrLeftParen) {
                throw new FormulaFormatException("formula must begin with a number, variable, or opening parenthesis");
            }
        }

        /// <summary>
        /// The last token of an expression must be a number, a variable, or a closing parenthesis.
        /// </summary>
        private static void EndingTokenRule(List<string> tokens)
        {
            string lastToken = tokens[tokens.Count - 1];
            bool lastTokenIsNumberOrVariableOrRightParen = lastToken.IsDouble()
                                                        || lastToken.IsVariable()
                                                        || lastToken.IsRightParen();
            if (!lastTokenIsNumberOrVariableOrRightParen) {
                throw new FormulaFormatException("formula must end with a number, variable, or closing parenthesis");
            }
        }

        /// <summary>
        /// Any token that immediately follows an opening parenthesis or an operator must be 
        /// either a number, a variable, or an opening parenthesis.
        /// </summary>
        private static void ParenthesisOrOperatorFollowingRule(List<string> tokens)
        {
            // last token doesn't matter for this case, just iterate up until i < tokens.Count - 1
            for (int i = 0; i < tokens.Count - 1; i++) {
                if (tokens[i].IsLeftParen() || tokens[i].IsOperator()) {
                    if (tokens[i + 1].IsDouble() || tokens[i + 1].IsVariable() || tokens[i + 1].IsLeftParen()) {
                        // it's okay
                    } else {
                        throw new FormulaFormatException(String.Format("invalid syntax at token \"{0}\": an operator or opening paren must be followed by "
                            + "either a number, variable, or an opening paren.", tokens[i]));
                    }
                }
            }
        }

        /// <summary>
        /// Any token that immediately follows a number, a variable, or a closing parenthesis must be either 
        /// an operator or a closing parenthesis.
        /// </summary>
        private static void ExtraFollowingRule(List<string> tokens)
        {
            // last token doesn't matter for this case, just iterate up until i < tokens.Count - 1
            for (int i = 0; i < tokens.Count - 1; i++) {
                if (tokens[i].IsDouble() || tokens[i].IsVariable() || tokens[i].IsRightParen()) {
                    if (tokens[i + 1].IsOperator() || tokens[i + 1].IsRightParen()) {
                        // it's okay
                    } else {
                        throw new FormulaFormatException("a number, variable, or closing paren must be followed by either an operator or a closing paren");
                    }
                }
            }
        }

    }
}