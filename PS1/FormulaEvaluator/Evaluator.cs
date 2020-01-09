using System;
using System.Collections.Generic;
using static FormulaEvaluator.StackExtensions;
using static FormulaEvaluator.ExpressionTokenUtils;

namespace FormulaEvaluator
{
    /// <summary>
    /// expression evaluator -
    /// evaluates integer arithmetic expressions written using standard infix notation
    /// </summary>
    public static class Evaluator
    {

        /// <summary>
        /// delegate to look up the value of a variable
        /// </summary>
        /// <param name="variable">variable name</param>
        /// <returns>integer value of that variable</returns>
        public delegate int Lookup(string variable);

        /// <summary>
        /// 
        /// evaluate an integer arithmetic expressions written using standard infix notation.
        /// respects the usual precedence rules and integer arithmetic rules.
        /// 
        /// the only legal tokens are the four operator symbols: + - * /, left parentheses, right parentheses, 
        /// non-negative integers, whitespace, and variables consisting of one or more letters followed by one or more digits. 
        /// letters can be lowercase or uppercase.
        /// 
        /// </summary>
        /// <param name="expression">expression to be evaluated</param>
        /// <param name="variableEvaluator">delegate to look up the value of a variable</param>
        /// <returns>returns value of the expression if it is valid and has a value, otherwise it throws an ArgumentException</returns>
        public static int Evaluate(string expression, Lookup variableEvaluator)
        {
            try
            {
                Stack<int> valueStack = new Stack<int>();
                Stack<string> operatorStack = new Stack<string>();
                List<string> expressionTokens = ParseExpressionTokens(expression);

                foreach (string token in expressionTokens)
                {
                    if (token.IsPositiveInteger())
                    {
                        HandleInteger(int.Parse(token), valueStack, operatorStack);
                    }
                    else if (token.IsVariable())
                    {
                        int variableValue = LookupVariable(token, variableEvaluator);
                        HandleInteger(variableValue, valueStack, operatorStack);
                    }
                    else if (token.IsOperator())
                    {
                        HandleOperator(token, valueStack, operatorStack);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format("token {0} is of unknown type", token));
                    }
                }

                int finalResult = HandleEndCase(valueStack, operatorStack);
                return finalResult;
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException(string.Format("evaluation error. {0}", e.Message));
            }
            catch (Exception e)
            {
                throw new ArgumentException(string.Format("an unexpected exception occurred. {0}", e.Message));
            }
        }


        private static int LookupVariable(string variableName, Lookup variableEvaluator)
        {
            try
            {
                int value = variableEvaluator(variableName);
                return value;
            }
            catch (Exception e)
            {
                throw new ArgumentException(
                    string.Format("lookup delegate could not find value for variable {0}. {1}", variableName, e.Message));
            }
        }

        private static int ApplyOperation(int leftVal, int rightVal, string operatorVal)
        {
            int result;
            switch (operatorVal)
            {
                case ADD:
                    result = leftVal + rightVal;
                    break;
                case SUBTRACT:
                    result = leftVal - rightVal;
                    break;
                case MULTIPLY:
                    result = leftVal * rightVal;
                    break;
                case DIVIDE:
                    if (rightVal == 0)
                    {
                        throw new ArgumentException("division by 0");
                    }
                    result = leftVal / rightVal;
                    break;
                default:
                    throw new ArgumentException
                        (string.Format("could not apply operation due to unknown operand {0}", operatorVal));
            }
            return result;
        }

        private static void PerformMultiplyOrDivide(Stack<int> valueStack, Stack<string> operatorStack)
        {
            // leftVal and rightVal are set in this order so that division works properly
            int leftVal = valueStack.Pop();
            int rightVal = valueStack.Pop();
            string operatorVal = operatorStack.Pop();
            int result = ApplyOperation(leftVal, rightVal, operatorVal);
            valueStack.Push(result);
        }

        private static void PerformAddOrSubtract(Stack<int> valueStack, Stack<string> operatorStack)
        {
            // leftVal and rightVal are set in this order so that subtraction works properly
            int rightVal = valueStack.Pop();
            int leftVal = valueStack.Pop();
            string operatorVal = operatorStack.Pop();
            int result = ApplyOperation(leftVal, rightVal, operatorVal);
            valueStack.Push(result);
        }

        private static void HandleInteger(int integerToken, Stack<int> valueStack, Stack<string> operatorStack)
        {
            if (operatorStack.NextOperatorIsMultiply() || operatorStack.NextOperatorIsDivide())
            {
                // this case is slightly different than HandleMultiplyDivide because we have to use integerToken immediately
                int leftVal = valueStack.Pop();
                int rightVal = integerToken;
                string operatorVal = operatorStack.Pop();
                int result = ApplyOperation(leftVal, rightVal, operatorVal);
                valueStack.Push(result);
            }
            else
            {
                valueStack.Push(integerToken);
            }
        }

        private static void HandleOperator(string operatorToken, Stack<int> valueStack, Stack<string> operatorStack)
        {
            if (operatorToken == ADD || operatorToken == SUBTRACT)
            {
                if (operatorStack.NextOperatorIsAdd() || operatorStack.NextOperatorIsSubtract())
                {
                    PerformAddOrSubtract(valueStack, operatorStack);
                }
                operatorStack.Push(operatorToken);
            }
            else if (operatorToken == MULTIPLY || operatorToken == DIVIDE)
            {
                operatorStack.Push(operatorToken);
            }
            else if (operatorToken == LEFT_PAREN)
            {
                operatorStack.Push(operatorToken);
            }
            else if (operatorToken == RIGHT_PAREN)
            {
                if (operatorStack.NextOperatorIsAdd() || operatorStack.NextOperatorIsSubtract())
                {
                    PerformAddOrSubtract(valueStack, operatorStack);
                }
                if (operatorStack.Peek() == LEFT_PAREN)
                {
                    operatorStack.Pop();
                }
                else
                {
                    throw new ArgumentException(("did not find matching left parenthesis"));
                }
                if (operatorStack.NextOperatorIsMultiply() || operatorStack.NextOperatorIsDivide())
                {
                    PerformMultiplyOrDivide(valueStack, operatorStack);
                }
            }
            else
            {
                throw new ArgumentException(string.Format("operator token {0} is of unknown type", operatorToken));
            }
        }

        private static int HandleEndCase(Stack<int> valueStack, Stack<string> operatorStack)
        {
            if (operatorStack.IsEmpty())
            {
                if (valueStack.Count == 1)
                {
                    return valueStack.Pop();
                }
                else
                {
                    throw new ArgumentException(string.Format("operator stack is empty but {0} values remain", valueStack.Count));
                }
            }
            else
            {
                if (valueStack.Count == 2 
                    && operatorStack.Count == 1 
                    && (operatorStack.NextOperatorIsAdd() || operatorStack.NextOperatorIsSubtract()))
                {
                    PerformAddOrSubtract(valueStack, operatorStack);
                    return valueStack.Pop();
                }
                else
                {
                    throw new ArgumentException("the last remaining operator must be either + or -, " +
                        "and there should be exactly two values remaining");
                }
            }
        }

    }
}
