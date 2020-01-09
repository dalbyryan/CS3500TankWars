// Luke Ludlow
// CS 3500 
// 2019 September

using System;
using System.Collections.Generic;

namespace SpreadsheetUtilities
{
    internal static class FormulaEvaluator
    {
        // this Evaluate does throw exceptions!
        // and then the higher Formula.Evaluate catches and handles and returns a FormulaError object. 
        // but this FormulaEvaluator simply returns a double result or throws. 
        public static double Evaluate(List<string> formulaTokens, Func<string, double> lookup)
        {
            try {
                Stack<double> valueStack = new Stack<double>();
                Stack<string> operatorStack = new Stack<string>();

                foreach (string token in formulaTokens) {
                    if (token.IsDouble()) {
                        HandleNumber(Double.Parse(token), valueStack, operatorStack);
                    } else if (token.IsVariable()) {
                        double variableValue = LookupVariable(token, lookup);
                        HandleNumber(variableValue, valueStack, operatorStack);
                    } else if (token.IsOperator() || token.IsLeftParen() || token.IsRightParen()) {
                        operatorStack.Push(token);
                        HandleOperator(valueStack, operatorStack);
                    }
                }

                double finalResult = HandleEndCase(valueStack, operatorStack);
                return finalResult;

            } catch (FormulaEvaluationException e) {
                throw e;
            } catch (Exception e) {
                throw new FormulaEvaluationException("an unexpected error occurred while evaluating the formula. " + e.Message);
            }
        }

        private static double ApplyOperation(double leftVal, double rightVal, string operatorVal)
        {
            double result = 0;
            switch (operatorVal) {
                case "+":
                    result = leftVal + rightVal;
                    break;
                case "-":
                    result = leftVal - rightVal;
                    break;
                case "*":
                    result = leftVal * rightVal;
                    break;
                case "/":
                    if (rightVal == 0) {
                        throw new FormulaEvaluationException("division by zero");
                    }
                    result = leftVal / rightVal;
                    break;
            }
            return result;
        }

        private static void PerformMultiplyOrDivide(Stack<double> valueStack, Stack<string> operatorStack)
        {
            // leftVal and rightVal are set in this order so that division works properly
            double rightVal = valueStack.Pop();
            double leftVal = valueStack.Pop();
            double result = ApplyOperation(leftVal, rightVal, operatorStack.Pop());
            valueStack.Push(result);
        }

        private static void PerformAddOrSubtract(Stack<double> valueStack, Stack<string> operatorStack)
        {
            // leftVal and rightVal are set in this order so that subtraction works properly
            double rightVal = valueStack.Pop();
            double leftVal = valueStack.Pop();
            double result = ApplyOperation(leftVal, rightVal, operatorStack.Pop());
            valueStack.Push(result);
        }

        private static void HandleNumber(double number, Stack<double> valueStack, Stack<string> operatorStack)
        {
            if (operatorStack.OnTop("*", "/")) {
                // this case is slightly different than PerformMultiplyDivide because we have to use integerToken immediately
                double result = ApplyOperation(valueStack.Pop(), number, operatorStack.Pop());
                valueStack.Push(result);
            } else {
                valueStack.Push(number);
            }
        }

        private static void HandleOperator(Stack<double> valueStack, Stack<string> operatorStack)
        {
            if (operatorStack.OnTop("+", "-")) {
                string nextOperator = operatorStack.Pop();
                if (operatorStack.OnTop("+", "-")) {
                    PerformAddOrSubtract(valueStack, operatorStack);
                }
                operatorStack.Push(nextOperator);
            } else if (operatorStack.OnTop(")")) {
                operatorStack.Pop();
                if (operatorStack.OnTop("+", "-")) {
                    PerformAddOrSubtract(valueStack, operatorStack);
                }
                if (operatorStack.OnTop("(")) {
                    operatorStack.Pop();
                }
                if (operatorStack.OnTop("*", "/")) {
                    PerformMultiplyOrDivide(valueStack, operatorStack);
                }
            }
        }

        private static double HandleEndCase(Stack<double> valueStack, Stack<string> operatorStack)
        {
            if (operatorStack.IsNotEmpty() && operatorStack.OnTop("+", "-")) {
                PerformAddOrSubtract(valueStack, operatorStack);
            }
            return valueStack.Pop();
        }


        private static double LookupVariable(string variableName, Func<string, double> lookup)
        {
            try {
                double value = lookup(variableName);
                return value;
            } catch (Exception e) {
                throw new FormulaFormatException(String.Format("lookup delegate could not find value for variable {0}. {1}", variableName, e.Message));
            }
        }

    }
}