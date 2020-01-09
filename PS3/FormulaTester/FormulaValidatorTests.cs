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
    /// tests Formula's validation functionality. 
    /// Formula doesn't have a public validate method, this happens internally 
    /// inside the Formula's constructor.
    /// 
    /// this testing class is NOT testing the internal/private class "FormulaValidator".
    /// it is testing the Formula's PUBLIC Constructor, which performs formula validation. 
    /// these tests are divided into their own testing class for clarity.
    /// 
    /// some of these tests are updated versions of the earlier PS1 FormulaEvaluator grading tests.
    /// i have updated/modified these test cases to sufficiently make them my own.
    /// 
    /// test method names follow this structure:
    /// [MethodName_StateUnderTest_ExpectedBehavior]
    ///</summary>.
    [TestClass]
    public class FormulaValidatorTests
    {

        // After tokenizing, your code should verify that the only tokens are (, ), +, -, *, /, 
        // variables, and decimal real numbers (including scientific notation).
        // TODO wait is it even possible for it to contain unknown tokens after GetTokens parses it?
        [TestMethod]
        public void Constructor_AfterParsingContainsUnknownTokens_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("$"));
        }

        [TestMethod]
        public void Constructor_ParseDoublesOfDifferentFormats_ShouldSucceed()
        {
            Assert.IsNotNull(new Formula("0 + 00.00 + 2 + 2.0000000 + 2e5 + 2e-5 + 1.2345E-56 + 0.00009"));
        }

        [TestMethod]
        public void Constructor_EmptyStringNoTokens_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula(""));
        }

        [TestMethod]
        public void Constructor_ExtraRightParenthesis_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("2+5*7)"));
        }

        [TestMethod]
        public void Constructor_UnbalancedParentheses_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("(2+(5*7)"));
        }

        [TestMethod]
        public void Constructor_JustParentheses_ShouldSucceed()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("((()))"));
        }

        [TestMethod]
        public void Constructor_SingleOperator_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("+"));
        }

        [TestMethod]
        public void Constructor_ExtraOperatorAtEnd_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("2+5*"));
        }

        [TestMethod]
        public void Constructor_OperatorFollowingAnOpeningParenthesis_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("2+(*abc123)"));
        }

        [TestMethod]
        public void Constructor_TwoOperators_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("1++1"));
        }

        [TestMethod]
        public void Constructor_ClosingParenthesisFollowedByNumber_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("5+7+(5)8"));
        }

        [TestMethod]
        public void Constructor_VariableFollowedByAnotherVariable_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("abc123 abc123"));
        }


        // initial tokens that are variables must follow this format:
        // a string consisting of a letter or underscore followed by zero or more letters, digits, or underscores.
        // (this is checked BEFORE the token makes it to the validateVariable delegate):
        [TestMethod]
        public void Constructor_IllegalVariableFormat_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("23x"));
        }

        [TestMethod]
        public void Constructor_PlusIllegalVariable_ShouldThrowException()
        {
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("5+23x"));
        }


        // remember, definition of a variable is: a string consisting of a letter or underscore 
        // followed by zero or more letters, digits, or underscores.
        // so just one simple underscore is a legal variable name. and just one variable is a valid equation.
        [TestMethod]
        public void Constructor_VariableNameIsJustAnUnderscore_ShouldSucceed()
        {
            Assert.IsNotNull(new Formula("_"));
        }

        [TestMethod]
        public void Constructor_VariableNameIsCombinationOfLettersUnderscoresNumbers_ShouldSucceed()
        {
            Assert.IsNotNull(new Formula("abc_123_x_1_5y_z"));
        }

        [TestMethod]
        public void Constructor_ValidatorIsProvided_ShouldValidateAllVariablesAndSucceed()
        {
            Func<string, bool> isValid = CreateValidatorDelegate(("x1", true), ("x2", true), ("x3", true));
            Assert.IsNotNull(new Formula("x1 + x2 + x3", s => s, isValid));
        }

        [TestMethod]
        public void Constructor_ValidatorIsProvidedAndSomeVariablesAreInvalid_ShouldThrowException()
        {
            Func<string, bool> isValid = CreateValidatorDelegate(("x1", true), ("x2", false), ("x3", true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("x1 + x2 + x3", s => s, isValid));
        }

        [TestMethod]
        public void Constructor_NormalizerAndValidatorAreProvided_ShouldValidateTheNormalizedVersions()
        {
            Func<string, string> normalize = s => s.ToUpper();
            Func<string, bool> isValid = CreateValidatorDelegate(("X1", true), ("X2", true), ("X3", true));
            Assert.IsNotNull(new Formula("x1 + x2 + x3", normalize, isValid));
        }

        [TestMethod]
        public void Constructor_SomeVariablesAreInvalid_ShouldThrowException()
        {
            Func<string, string> normalize = s => s.ToUpper();
            Func<string, bool> isValid = CreateValidatorDelegate(("x1", true), ("x2", true), ("x3", true));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("x1 + x2 + x3", normalize, isValid));
        }

        [TestMethod]
        public void Constructor_NormalizedVersionsAreInvalid_ShouldThrowException()
        {
            Func<string, string> normalize = CreateNormalizerDelegate(("x1", "999"));
            Assert.ThrowsException<FormulaFormatException>(() => new Formula("x1", normalize, s => true));
        }

    }
}
