// Luke Ludlow
// CS 3500
// 2019 September

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;

namespace FormulaTester
{
    /// <summary>
    /// most of the "business logic" tests are contained in FormulaValidatorTests and FormulaEvaluatorTests.
    /// this testing class focuses on the rest of the public methods: 
    /// GetVariables, ToString, Equals, ==, !=, and GetHashCode.
    ///   
    /// side note: this class is named "FormulaTester" because the lab spec told me to.
    /// my usual naming scheme would be "FormulaTests".
    /// 
    /// test method names follow this structure:
    /// [MethodName_StateUnderTest_ExpectedBehavior]
    /// </summary>
    [TestClass]
    public class FormulaTester
    {

        [TestMethod]
        public void ToString_ExtraWhitespace_ShouldReturnStringWithNoWhitespace()
        {
            Formula formula = new Formula("x1     +      x2  \n \n  * x3\n");
            string expected = "x1+x2*x3";
            Assert.AreEqual(expected, formula.ToString());
        }

        [TestMethod]
        public void ToString_DifferentVariableFormats_ShouldReturnStringWithNormalizedVariables()
        {
            Func<string, string> normalize = s => s.ToUpper();
            Formula formula = new Formula("x1 + X2 + aBc123", normalize, s => true);
            string expected = "X1+X2+ABC123";
            Assert.AreEqual(expected, formula.ToString());
        }

        [TestMethod]
        public void ToString_DoublesWithDifferentFormats_ShouldReturnStringWithNormalizedDoubles()
        {
            Formula formula = new Formula("2 + 2.0 + .02e2 + 0.02E2");
            string expected = "2+2+2+2";
            Assert.AreEqual(expected, formula.ToString());
        }

        [TestMethod]
        public void ToString_DoubleWithTooMuchPrecision_ShouldReturnStringWithParsedDoubleValue()
        {
            Formula formula = new Formula("2.000000000000005");
            string expected = "2";
            Assert.AreEqual(expected, formula.ToString());
        }

        [TestMethod]
        public void GetVariables_VariablesNeedToBeNormalized_ShouldReturnNormalizedVariables()
        {
            Formula formula = new Formula("x1 + X2 + aBc123", s => s.ToUpper(), s => true);
            Assert.AreEqual(3, formula.GetVariables().Count());
            Assert.IsTrue(formula.GetVariables().Contains("X1"));
            Assert.IsTrue(formula.GetVariables().Contains("X2"));
            Assert.IsTrue(formula.GetVariables().Contains("ABC123"));
        }

        [TestMethod]
        public void GetVariables_NoVariablesInFormula_ShouldReturnEmptyEnumerable()
        {
            Formula formula = new Formula("1 + 2 + 3");
            Assert.AreEqual(0, formula.GetVariables().Count());
        }

        [TestMethod]
        public void GetVariables_DuplicateVariable_ShouldReturnOnlyOneVariable()
        {
            Formula formula = new Formula("x1 + x1 * x1");
            Assert.AreEqual(1, formula.GetVariables().Count());
            Assert.IsTrue(formula.GetVariables().Contains("x1"));

            Formula formulaWithNormalizer = new Formula("abc123 + ABC123", s => s.ToUpper(), s => true);
            Assert.AreEqual(1, formulaWithNormalizer.GetVariables().Count());
            Assert.IsTrue(formulaWithNormalizer.GetVariables().Contains("ABC123"));
        }

        [TestMethod]
        public void Equals_ObjIsNull_ShouldReturnFalse()
        {
            Formula formula = new Formula("a + b + c");
            Assert.IsFalse(formula.Equals(null));
        }

        [TestMethod]
        public void Equals_ObjIsNotAFormula_ShouldReturnFalse()
        {
            Formula formula = new Formula("a + b + c");
            Stack<string> randomObject = new Stack<string>();
            Assert.IsFalse(formula.Equals(randomObject));
        }

        [TestMethod]
        public void Equals_ObjIsItself_ShouldReturnTrue()
        {
            Formula formula = new Formula("a + b + c");
            Assert.IsTrue(formula.Equals(formula));
        }


        [TestMethod]
        public void Equals_SameTokensButDifferentOrder_ShouldReturnFalse()
        {
            Formula formula = new Formula("abc123+2");
            Formula formulaDifferentOrder = new Formula("2+abc123");
            Assert.IsFalse(formula.Equals(formulaDifferentOrder));
        }

        [TestMethod]
        public void Equals_NormalizedAndNotNormalizedFormula_ShouldReturnTrue()
        {
            Formula formula = new Formula("x1+x2/abc123");
            Formula formulaWithNormalizer = new Formula("x1     +   X2/aBc123", s => s.ToLower(), s => true);
            Assert.IsTrue(formula.Equals(formulaWithNormalizer));
        }

        public void Equals_OneFormulaIsNotGivenANormalizer_ShouldReturnFalse()
        {
            Formula formula = new Formula("x1 + x2");
            Formula formulaWithNormalizer = new Formula("x1 + x2", s => s.ToUpper(), s => true);
            Assert.IsFalse(formula.Equals(formulaWithNormalizer));
        }

        [TestMethod]
        public void Equals_SameDoubleButDifferentFormats_ShouldReturnTrue()
        {
            Formula formula = new Formula("1.1 * 5.06");
            Formula formulaDifferentDoubleFormat = new Formula("11e-1 * 05.06000");
            Assert.IsTrue(formula.Equals(formulaDifferentDoubleFormat));
        }

        [TestMethod]
        public void Equals_DoublePrecisionIsTooFarAndWillBeDropped_ShouldReturnTrue()
        {
            Formula formula = new Formula("2");
            Formula formulaWithTooMuchPrecision = new Formula("2.000000000000005");
            Assert.IsTrue(formula.Equals(formulaWithTooMuchPrecision));
        }

        [TestMethod]
        public void EqualsOperator_BothAreNull_ShouldReturnTrue()
        {
            Assert.IsTrue(null == null);
        }

        [TestMethod]
        public void EqualsOperator_OneIsNull_ShouldReturnFalse()
        {
            Formula formula = new Formula("a + b + c");
            Assert.IsFalse(formula == null);
            Assert.IsFalse(null == formula);
        }

        [TestMethod]
        public void EqualsOperator_EqualAfterNormalization_ShouldReturnTrue()
        {
            Formula formula = new Formula("x1+x2/abc123");
            Formula formula2 = new Formula("x1+x2/abc123");
            Formula formulaWithNormalizer = new Formula("x1     +   X2/aBc123", s => s.ToLower(), s => true);
            Assert.IsTrue(formula == formulaWithNormalizer);
        }

        [TestMethod]
        public void NotEqualsOperator_BothAreNull_ShouldReturnFalse()
        {
            Assert.IsFalse(null != null);
        }

        [TestMethod]
        public void NotEqualsOperator_OneIsNull_ShouldReturnTrue()
        {
            Formula formula = new Formula("a + b + c");
            Assert.IsTrue(formula != null);
            Assert.IsTrue(null != formula);
        }

        [TestMethod]
        public void NotEqualsOperator_EqualAfterNormalization_ShouldReturnFalse()
        {
            Formula formula = new Formula("x1+x2/abc123");
            Formula formula2 = new Formula("x1+x2/abc123");
            Formula formulaWithNormalizer = new Formula("x1     +   X2/aBc123", s => s.ToLower(), s => true);
            Assert.IsFalse(formula != formulaWithNormalizer);
        }

        [TestMethod]
        public void GetHashCode_FormulasAreEqual_HashcodesShouldBeEqual()
        {
            Formula f1 = new Formula("abc123 * 3");
            Formula f2 = new Formula("abc123 * 3");
            Assert.IsTrue(f1.Equals(f2));
            Assert.IsTrue(f1.GetHashCode() == f2.GetHashCode());

            Formula formula = new Formula("x1+x2/abc123");
            Formula formulaWithNormalizer = new Formula("x1     +   X2/aBc123", s => s.ToLower(), s => true);
            Assert.IsTrue(formula.Equals(formulaWithNormalizer));
            Assert.IsTrue(formula.GetHashCode() == formulaWithNormalizer.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_FormulasAreNotEqual_HashcodesShouldNotBeEqual()
        {
            Formula f1 = new Formula("1 + 2");
            Formula f2 = new Formula("1 - 2");
            Assert.IsFalse(f1.Equals(f2));
            Assert.IsFalse(f1.GetHashCode() == f2.GetHashCode());
        }

    }
}
