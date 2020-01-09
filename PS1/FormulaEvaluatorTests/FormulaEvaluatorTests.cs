using Microsoft.VisualStudio.TestTools.UnitTesting;
using FormulaEvaluator;
using System;

namespace FormulaEvaluatorTests
{
    /// <summary>
    ///   test method names follow this structure:
    ///   [MethodName_StateUnderTest_ExpectedBehavior]
    ///   https://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html
    /// </summary>
    [TestClass]
    public class FormulaEvaluatorTests
    {

        // positive test cases //

        [TestMethod]
        public void Evaluate_NoVariables_ShouldCalculateValue()
        {
            string expression = "(2 + 3) * 5 + 2";
            int expected = 27;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_NoSpacesBetweenTokens_ShouldCalculateValue()
        {
            string expression = "(2+3)*5+2";
            int expected = 27;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_OneVariable_ShouldCalculateValue()
        {
            string expression = "(2 + A6) * 5 + 2";
            int mockLookup(string s)
            {
                if (s == "A6")
                {
                    return 5;
                }
                else
                {
                    throw new Exception();
                }
            }
            int expected = 37;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, mockLookup));
        }

        [TestMethod]
        public void Evaluate_PrecedenceMultiplyDivide_ShouldCalculateValue()
        {
            string expression = "8 / 2 * 5 * 9 / 3";
            int expected = 60;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_PrecedenceAddSubtract_ShouldCalculateValue()
        {
            string expression = "1 - 3 + 2 * 7 - 9 + 4";
            int expected = 7;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_AllVariables_ShouldCalculateValue()
        {
            string expression = "a1 + b2 + abc123";
            int mockLookup(string s)
            {
                if (s == "a1")
                {
                    return 2;
                }
                else if (s == "b2")
                {
                    return 3;
                }
                else if (s == "abc123")
                {
                    return 4;
                }
                else
                {
                    throw new Exception();
                }
            };
            int expected = 9;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, mockLookup));
        }

        [TestMethod]
        public void Evaluate_MultipleOfTheSameVariable_ShouldCalculateValue()
        {
            string expression = "abc123 + abc123 + abc123";
            int mockLookup(string s)
            {
                if (s == "abc123")
                {
                    return 5;
                }
                else
                {
                    throw new Exception();
                }
            };
            int expected = 15;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, mockLookup));
        }

        [TestMethod]
        public void Evaluate_SingleInteger_ShouldReturnValue()
        {
            string expression = "9";
            int expected = 9;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
            string expressionWithBalancedParentheses = "(9)";
            Assert.AreEqual(expected, Evaluator.Evaluate(expressionWithBalancedParentheses, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_SingeVariable_ShouldReturnValue()
        {
            string expression = "abc123";
            int mockLookup(string s) => 9;
            int expected = 9;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, mockLookup));
            string expressionWithBalancedParentheses = "(abc123)";
            Assert.AreEqual(expected, Evaluator.Evaluate(expressionWithBalancedParentheses, mockLookup));
        }

        [TestMethod]
        public void Evaluate_IntegersWithLeadingZerosAreStillValid_ShouldCalculateValue()
        {
            string expression = "01 + 002 + 0003";
            int expected = 6;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_IgnoreLotsOfWhitespace_ShouldCalculateValue()
        {
            string expression = "\n 1  +  ( 5 \n\n *  2 ) \t - 2 \n";
            int expected = 9;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_EmptyParentheses_ShouldIgnoreAndContinueCalculation()
        {
            string expression = "5 + () 6";
            int expected = 11;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_NestedParentheses_ShouldCalculateValue()
        {
            string expression = "((1 + (2 * (3 * (4)))) * (5))";
            int expected = 125;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_LookupReturnsNegativeVariable_ShouldCalculateValue()
        {
            string expression = "abc123 - 5";
            int mockLookup(string s) => -20;
            int expected = -25;
            Assert.AreEqual(expected, Evaluator.Evaluate(expression, mockLookup));
        }


        // negative test cases //

        [TestMethod]
        public void Evaluate_UnbalancedParenthesis_ShouldThrowArgumentException()
        {
            string expression = "1 + (((2 + 3))";
            Assert.ThrowsException<System.ArgumentException>(() => Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_NegativeIntegerNotAllowed_ShouldThrowArgumentException()
        {
            string expression = "(-2 + 3)";
            Assert.ThrowsException<System.ArgumentException>(() => Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_MissingOperator_ShouldThrowArgumentException()
        {
            string expression = "5 6";
            Assert.ThrowsException<System.ArgumentException>(() => Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_DivisionByZero_ShouldThrowArgumentException()
        {
            string expression = "2 / 0";
            Assert.ThrowsException<System.ArgumentException>(() => Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_VariableNotFound_ShouldThrowArgumentException()
        {
            string expression = "1 + 2 - badVariable01";
            int mockLookup(string s) => throw new Exception();
            Assert.ThrowsException<System.ArgumentException>(() => Evaluator.Evaluate(expression, mockLookup));
        }

        [TestMethod]
        public void Evaluate_EmptyExpression_ShouldThrowArgumentException()
        {
            string expression = "";
            Assert.ThrowsException<System.ArgumentException>(() => Evaluator.Evaluate(expression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_IllegalTokens_ShouldThrowArgumentException()
        {
            string illegalOperatorExpression = "1 * 2 ^ 3";
            Assert.ThrowsException<System.ArgumentException>(() => Evaluator.Evaluate(illegalOperatorExpression, MockLookupThatShouldNotBeCalled));
        }

        [TestMethod]
        public void Evaluate_InvalidVariableName_ShouldThrowArgumentException()
        {
            string invalidVariableNameExpression = "1 + 2 + badVariableBecauseItDoesntHaveANumber";
            Assert.ThrowsException<System.ArgumentException>(() => Evaluator.Evaluate(invalidVariableNameExpression, MockLookupThatShouldNotBeCalled));
        }


        private static int MockLookupThatShouldNotBeCalled(string variable)
        {
            throw new Exception("the mock lookup delegate should not be called in this test");
        }
    }
}
