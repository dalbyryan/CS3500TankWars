// Luke Ludlow
// CS 3500
// 2019 September

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using static FormulaTester.FormulaTesterUtils;

namespace FormulaTester
{
    /// <summary>
    /// tests Formula's Evaluate functionality. 
    /// 
    /// this testing class is NOT testing the internal/private class "FormulaEvaluator".
    /// it is testing the Formula's PUBLIC Evaluate method. 
    /// these tests are divided into their own testing class for clarity.
    /// 
    /// some of these tests are updated versions of the earlier PS1 FormulaEvaluator grading tests.
    /// i have updated/modified these test cases to sufficiently make them my own.
    /// 
    /// test method names follow this structure:
    /// [MethodName_StateUnderTest_ExpectedBehavior]
    ///</summary>
    [TestClass]
    public class FormulaEvaluatorTests
    {

        [TestMethod]
        public void Evaluate_NormalizerConvertsVariable_ShouldEvaluateWithCorrectValue()
        {
            Func<string, string> normalize = s => s.ToUpper();
            Func<string, double> lookup = CreateLookupDelegate(("x", 2), ("X", 4));
            // TODO do i need to cast the Evaluate result to a double? and do i need to add delta?
            Assert.AreEqual((double)9, new Formula("x+7").Evaluate(lookup));
            Assert.AreEqual((double)11, new Formula("x+7", normalize, s => true).Evaluate(lookup));
        }

        [TestMethod]
        public void Evaluate_LookupFails_ShouldReturnFormulaError()
        {
            object obj = new Formula("2+abc123").Evaluate(s => { throw new ArgumentException("unknown variable"); });
            Assert.IsInstanceOfType(obj, typeof(FormulaError));
        }

        [TestMethod]
        public void Evaluate_DivisionByZero_ShouldReturnFormulaError()
        {
            Func<string, double> lookup = CreateLookupDelegate(("x", 0));
            object obj = new Formula("2/x").Evaluate(lookup);
            Assert.IsInstanceOfType(obj, typeof(FormulaError));
        }

        [TestMethod]
        public void Evaluate_DoubleArithmeticGivesWeirdResult_ShouldReturnResult()
        {
            // because we're using doubles, this will NOT be a divide be zero error, 
            // double arithmetic will just give a weird result.
            // to verify, this is what Microsoft Excel does: 5.5 / (3.3 - 2.2 - 1.1) = -1.23849e+16
            double expected = 5.5 / (3.3 - 2.2 - 1.1);
            double precision = 1e-9;
            Assert.AreEqual(expected, (double)new Formula("5.5 / (3.3 - 2.2 - 1.1)").Evaluate(s => 0), precision);
        }

        [TestMethod]
        public void Evaluate_AllVariablesMustBeNormalized_ShouldEvaluateWithCorrectValues()
        {
            Func<string, string> normalize = s => s.ToUpper();
            Func<string, double> lookup = CreateLookupDelegate(("ABC123", 1), ("X7", 2), ("ZZZ", 3));
            Assert.AreEqual((double)6, new Formula("abc123 + x7 + zzz", normalize, s => true).Evaluate(lookup));
        }

        [TestMethod]
        public void Evaluate_NormalizeTheSameVariableThatIsInDifferentFormats_ShouldEvaluateWithCorrectValues()
        {
            Func<string, string> normalize = s => s.ToLower();
            Func<string, double> lookup = CreateLookupDelegate(("abc123", 5));
            Assert.AreEqual((double)20, new Formula("abc123 + aBc123 + AbC123 + ABC123", normalize, s => true).Evaluate(lookup));
        }

        [TestMethod]
        public void Evaluate_SingleNumber_ShouldReturnItself()
        {
            Assert.AreEqual((double)5, new Formula("5").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_SingleVariable_ShouldReturnItself()
        {
            Assert.AreEqual((double)13, new Formula("X5").Evaluate(s => 13));
        }

        [TestMethod]
        public void Evaluate_Addition_ShouldReturnResult()
        {
            Assert.AreEqual((double)8, new Formula("5+3").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_AdditionWithDoublesOfDifferentFormats_ShouldReturnResult()
        {
            double precision = 1e-9;
            Assert.AreEqual(6.6, (double)new Formula("2.2 + 2.200000000000000000000 + 0.022e+2").Evaluate(s => 0), precision);
        }

        [TestMethod]
        public void Evaluate_Subtraction_ShouldReturnResult()
        {
            Assert.AreEqual((double)8, new Formula("18-10").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_SubtractionWithDoublesOfDifferentFormats_ShouldReturnResult()
        {
            Assert.AreEqual((double)8, new Formula("18000.0e-3-10.0").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_Multiplication_ShouldReturnResult()
        {
            Assert.AreEqual((double)8, new Formula("2*4").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_MultiplicationWithDoubles_ShouldReturnResult()
        {
            Assert.AreEqual(8.8, new Formula("2*4.4").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_Division_ShouldReturnResult()
        {
            Assert.AreEqual((double)8, new Formula("16/2").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_DivisionWithDoubles_ShouldReturnResult()
        {
            double expected = 16.1 / 2.1000000;
            Assert.AreEqual(expected, new Formula("16.1/2.1000000").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_ArithmeticWithVariable_ShouldReturnResult()
        {
            Assert.AreEqual((double)6, new Formula("2+X1").Evaluate(s => 4));
        }

        [TestMethod]
        public void Evaluate_UnknownVariable_ShouldReturnFormulaError()
        {
            object obj = new Formula("2+X1").Evaluate(s => { throw new ArgumentException("unknown variable"); });
            Assert.IsInstanceOfType(obj, typeof(FormulaError));
        }

        [TestMethod]
        public void Evaluate_OrderOfOperationsGoesLeftToRight_ShouldEvaluateUsingPrecedenceRules()
        {
            Assert.AreEqual((double)15, new Formula("2*6+3").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_OrderOfOperationsGoesRightToLeft_ShouldEvaluateUsingPrecedenceRules()
        {
            Assert.AreEqual((double)20, new Formula("2+6*3").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_ParenthesesBeforeMultiplication_ShouldEvaluateUsingPrecedenceRules()
        {
            Assert.AreEqual((double)24, new Formula("(2+6)*3").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_MultiplicationBeforeParentheses_ShouldEvaluateUsingPrecedenceRules()
        {
            Assert.AreEqual((double)16, new Formula("2*(3+5)").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_AdditionBeforeParentheses_ShouldEvaluateUsingPrecedenceRules()
        {
            Assert.AreEqual((double)5, new Formula("2+(6/2)").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_ComplexOrderOfOperationsInsideParentheses_ShouldEvaluateUsingPrecedenceRules()
        {
            Assert.AreEqual((double)50, new Formula("2+(3+5*9)").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_OperatorImmediatelyAfterParentheses_ShouldReturnResult()
        {
            Assert.AreEqual((double)0, new Formula("(1*1)-2/2").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_ComplexOrderOfOperationsOutsideParentheses_ShouldEvaluateUsingPrecedenceRules()
        {
            Assert.AreEqual((double)26, new Formula("2+3*(3+5)").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_ComplexOrderOfOperationsInsideAndOutsideParentheses_ShouldEvaluateUsingPrecedenceRules()
        {
            Assert.AreEqual((double)194, new Formula("2+3*5+(3+4*8)*5+2").Evaluate(s => 0));
        }

        [TestMethod]
        public void Evaluate_DivideByZero_ShouldReturnFormulaError()
        {
            object obj = new Formula("5/0").Evaluate(s => 0);
            Assert.IsInstanceOfType(obj, typeof(FormulaError));
            Assert.AreEqual("division by zero", ((FormulaError)obj).Reason);
        }

        [TestMethod]
        public void Evaluate_MultipleVariables_ShouldEvaluateUsingLookupValues()
        {
            double expected = -2;
            double precision = 1e-9;
            Assert.AreEqual(expected, (double)new Formula("y1*3-8/2+4*(8-9*2)/4*x7").Evaluate(s => (s == "x7") ? 1 : 4), precision);
        }

        [TestMethod]
        public void Evaluate_NestedParenthesesRight_ShouldEvaluateInParenthesesOrder()
        {
            Assert.AreEqual((double)6, new Formula("x1+(x2+(x3+(x4+(x5+x6))))").Evaluate(s => 1));
        }

        [TestMethod]
        public void Evaluate_NestedParenthesesLeft_ShouldEvaluateInParenthesesOrder()
        {
            Assert.AreEqual((double)12, new Formula("((((x1+x2)+x3)+x4)+x5)+x6").Evaluate(s => 2));
        }

        [TestMethod]
        public void Evaluate_RepeatedVariable_ShouldEvaluateUsingLookupValue()
        {
            Assert.AreEqual((double)0, new Formula("a4-a4*a4/a4").Evaluate(s => 3));
        }

    }
}
