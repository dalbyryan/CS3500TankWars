// Luke Ludlow
// CS 3500
// 2019 September

using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using static SpreadsheetTests.SpreadsheetTestUtils;

namespace SpreadsheetTests
{
    /// <summary>
    /// unit tests for the PS5 version of Spreadsheet.
    /// 
    /// please note that a lot of functionality is still tested in the PS4 spreadsheet tests,
    /// so this new test class tries not to re-test those features. 
    /// 
    /// test method names follow this structure:
    /// [MethodName_StateUnderTest_ExpectedBehavior]
    /// </summary>
    [TestClass]
    public class SpreadsheetTests
    { 

        [TestMethod]
        public void Constructor_ZeroAndThreeAsgs_ShouldSucceed()
        {
            if () {

            }
            AbstractSpreadsheet sheetZeroArgs = new Spreadsheet();
            Assert.IsNotNull(sheetZeroArgs);
            AbstractSpreadsheet sheetThreeArgs = new Spreadsheet(s => true, s => s, "v0.1");
            Assert.IsNotNull(sheetThreeArgs);
            // the Four argument constructor is tested heavily in the other tests
        }


        [TestMethod]
        public void GetCellContents_InvalidSpreadsheetVariableName_ShouldThrowException()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.ThrowsException<InvalidNameException>(() => sheet.GetCellContents("A_1"));
        }

        [TestMethod]
        public void GetCellContents_NameIsNotValidAccordingToValidatorDelegate_ShouldThrowException()
        {
            // create a validator where A1 is an invalid name
            Func<string, bool> isValid = s => (s == "A1") ? false : true;
            Spreadsheet sheet = new Spreadsheet(isValid, s => s, "default");
            Assert.ThrowsException<InvalidNameException>(() => sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetCellContents_InputArgumentIsNotNormalized_ShouldNormalizeCellNameBeforeGettingIt()
        {
            Spreadsheet sheet = new Spreadsheet(s => true, s => s.ToUpper(), "default");
            sheet.SetContentsOfCell("A1", "content");
            // calling GetCellContents("a1") will actually look up the cell "A1".
            Assert.AreEqual("content", sheet.GetCellContents("a1"));
        }



        [TestMethod]
        public void SetContentsOfCell_NullContent_ShouldThrowException()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.ThrowsException<ArgumentNullException>(() => sheet.SetContentsOfCell("A1", null));
        }

        [TestMethod]
        public void SetContentsOfCell_InvalidCellNames_ShouldThrowException()
        {
            // a string is a valid cell name if and only if:
            // 1. the string starts with one or more letters and is followed by one or more numbers.
            // 2. the IsValid function returns true for that string
            Spreadsheet sheet = new Spreadsheet();
            Assert.ThrowsException<InvalidNameException>(() => sheet.SetContentsOfCell("A", "content"));
            Assert.ThrowsException<InvalidNameException>(() => sheet.SetContentsOfCell("2", "content"));
            Assert.ThrowsException<InvalidNameException>(() => sheet.SetContentsOfCell("A2B", "content"));

            // create a validator where A1 is an invalid name
            Func<string, bool> isValid = s => (s == "A1") ? false : true;
            sheet = new Spreadsheet(isValid, s => s, "default");
            Assert.ThrowsException<InvalidNameException>(() => sheet.SetContentsOfCell("A1", "content"));
        }

        [TestMethod]
        public void SetContentsOfCell_FormulaContainsInvalidVariableName_ShouldThrowFormulaFormatException()
        {
            Spreadsheet sheet = new Spreadsheet();
            // invalid character
            Assert.ThrowsException<FormulaFormatException>(() => sheet.SetContentsOfCell("A1", "=X$2"));
            // invalid spreadsheet variable name
            Assert.ThrowsException<FormulaFormatException>(() => sheet.SetContentsOfCell("A1", "=A2B"));
            // create a validator where A1 is an invalid name
            Func<string, bool> isValid = s => (s == "A1") ? false : true;
            sheet = new Spreadsheet(isValid, s => s, "default");
            Assert.ThrowsException<FormulaFormatException>(() => sheet.SetContentsOfCell("B2", "=A1"));
        }

        [TestMethod]
        public void SetContentsOfCell_WhitespaceBeforeEqualsSign_ShouldParseAsStringNotFormula()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "    =x1");
            Assert.IsInstanceOfType(sheet.GetCellContents("A1"), typeof(string));
            Assert.AreEqual("    =x1", sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void SetContentsOfCell_WhitespaceAfterEqualsSign_ShouldParseFormula()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=    x1");
            Assert.IsInstanceOfType(sheet.GetCellContents("A1"), typeof(Formula));
            Assert.AreEqual(new Formula("x1"), sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void SetContentsOfCell_Normalizer_ShouldWriteNormalizedCellName()
        {
            Spreadsheet sheet = new Spreadsheet(s => true, s => s.ToUpper(), "default");
            sheet.SetContentsOfCell("a1", "content");
            Assert.IsTrue(sheet.GetNamesOfAllNonemptyCells().Contains("A1"));
            Assert.IsFalse(sheet.GetNamesOfAllNonemptyCells().Contains("a1"));
            Assert.AreEqual("content", sheet.GetCellContents("A1"));
        }




        [TestMethod]
        public void GetCellValue_ContentIsString_ValueShouldBeThatString()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "string");
            Assert.AreEqual("string", sheet.GetCellValue("A1"));
        }

        [TestMethod]
        public void GetCellValue_ContentIsANumber_ValueShouldBeThatNumber()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "420.69");
            Assert.AreEqual(420.69, sheet.GetCellValue("A1"));
        }

        [TestMethod]
        public void GetCellValue_FormulaWithConstantValue_ShouldReturnThatNumberValue()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=420.69");
            Assert.AreEqual(420.69, sheet.GetCellValue("A1"));
        }

        [TestMethod]
        public void GetCellValue_FormulaWithAVariable_ShouldLookupValueThenReturnEvaluatedFormula()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=X2");
            sheet.SetContentsOfCell("X2", "420.69");
            Assert.AreEqual(420.69, sheet.GetCellValue("A1"));
        }

        [TestMethod]
        public void GetCellValue_FormulaWithVariablesDependsOnValuesOfOtherCells_ShouldLookupTheCalculatedValuesThenReturnEvaluatedFormula()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("C3", "=420");
            sheet.SetContentsOfCell("B2", "=0.69 + C3");
            sheet.SetContentsOfCell("A1", "=B2 + C3");
            Assert.AreEqual((double)420, sheet.GetCellValue("C3"));
            Assert.AreEqual(420.69, sheet.GetCellValue("B2"));
            Assert.AreEqual(840.69, sheet.GetCellValue("A1"));
        }

        [TestMethod]
        public void GetCellValue_FormulaThatDependsOnCellThatDoesNotExist_ShouldReturnFormulaError()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B2");
            Assert.IsInstanceOfType(sheet.GetCellValue("A1"), typeof(FormulaError));
        }

        [TestMethod]
        public void GetCellValue_InvalidSpreadsheetVariableName_ShouldThrowException()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.ThrowsException<InvalidNameException>(() => sheet.GetCellValue("A_1"));
        }

        [TestMethod]
        public void GetCellValue_NameIsNotValidAccordingToValidatorDelegate_ShouldThrowException()
        {
            // create a validator where A1 is an invalid name
            Func<string, bool> isValid = s => (s == "A1") ? false : true;
            Spreadsheet sheet = new Spreadsheet(isValid, s => s, "default");
            Assert.ThrowsException<InvalidNameException>(() => sheet.GetCellValue("A1"));
        }

        [TestMethod]
        public void GetCellValue_InputArgumentIsNotNormalized_ShouldNormalizeCellNameBeforeGettingIt()
        {
            Spreadsheet sheet = new Spreadsheet(s => true, s => s.ToUpper(), "default");
            sheet.SetContentsOfCell("A1", "content");
            // calling GetCellContents("a1") will actually look up the cell "A1".
            Assert.AreEqual("content", sheet.GetCellValue("a1"));
        }





        [TestMethod]
        public void Save_EmptySpreadsheet_ShouldCreateFile()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.Save("save.xml");
            Assert.IsTrue(File.Exists("save.xml"));
            // clean up file after we're done
            File.Delete("save.xml");
        }

        [TestMethod]
        public void Save_FileAlreadyExists_ShouldOverwriteThatFile()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.Save("save.xml");
            Assert.IsTrue(File.ReadAllText("save.xml").Contains("version=\"default\""));
            Spreadsheet sheet2 = new Spreadsheet(s => true, s => s, "v1.0");
            sheet2.Save("save.xml");
            Assert.IsTrue(File.ReadAllText("save.xml").Contains("version=\"v1.0\""));
            // clean up file after we're done
            File.Delete("save.xml");
        }

        [TestMethod]
        public void Save_EmptySpreadsheet_ShouldWriteSpreadsheetTagAndVersionButNoCells()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.Save("save.xml");
            string expected =
                @"<?xml version=""1.0"" encoding=""utf-8""?>" + "\n" +
                @"<spreadsheet version=""default"" />";
            System.IO.File.WriteAllText("expected.xml", expected);
            Assert.IsTrue(FilesAreEqual("expected.xml", "save.xml"));
            // delete files after test is done
            File.Delete("save.xml");
            File.Delete("expected.xml");
        }

        [TestMethod]
        public void Save_NonExistentPath_ShouldThrowException()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => sheet.Save("/some/nonsense/path.xml"));
        }

        [TestMethod]
        public void Save_MultipleCells_ShouldWriteSpreadsheetAndCellInfo()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "A1 contents");
            sheet.SetContentsOfCell("B2", "B2 contents");
            sheet.SetContentsOfCell("C3", "C3 contents");
            sheet.Save("save.xml");

            string expected =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>A1 contents</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>B2</name>\n" +
                "        <contents>B2 contents</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>C3</name>\n" +
                "        <contents>C3 contents</contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("expected.xml", expected);

            Assert.IsTrue(FilesAreEqual("expected.xml", "save.xml"));

            // delete files after test is done
            File.Delete("save.xml");
            File.Delete("expected.xml");
        }

        [TestMethod]
        public void Save_SpreadsheetContainsEmptyCell_ShouldNotWriteTheEmptyCell()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "");
            sheet.Save("save.xml");
            string expected =
                @"<?xml version=""1.0"" encoding=""utf-8""?>" + "\n" +
                @"<spreadsheet version=""default"" />";
            System.IO.File.WriteAllText("expected.xml", expected);
            Assert.IsTrue(FilesAreEqual("expected.xml", "save.xml"));
            // delete files after test is done
            File.Delete("save.xml");
            File.Delete("expected.xml");
        }


        [TestMethod]
        public void Save_EveryTypeOfCellContents_ShouldWriteEachContentTypeWithProperFormat()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "string contents");
            sheet.SetContentsOfCell("F1", "=B2");
            sheet.SetContentsOfCell("F2", "=1234");
            sheet.SetContentsOfCell("F3", "=     A1  \n + ( B2 \t\n +   C3) ");  // Formula.ToString() will return "A1+(B2+C3)"
            sheet.SetContentsOfCell("D1", "420.69");
            sheet.SetContentsOfCell("D2", "00420.6900");  // Double.Parse("00420.6900") and then ToString'd will return "420.69"
            sheet.SetContentsOfCell("D3", "1E-7");  // Double.Parse("1E-7") and then ToString'd will return "1E-07"

            sheet.Save("save.xml");

            string expected =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>string contents</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>F1</name>\n" +
                "        <contents>=B2</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>F2</name>\n" +
                "        <contents>=1234</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>F3</name>\n" +
                "        <contents>=A1+(B2+C3)</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>D1</name>\n" +
                "        <contents>420.69</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>D2</name>\n" +
                "        <contents>420.69</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>D3</name>\n" +
                "        <contents>1E-07</contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("expected.xml", expected);

            Assert.IsTrue(FilesAreEqual("expected.xml", "save.xml"));

            // delete files after test is done
            File.Delete("save.xml");
            File.Delete("expected.xml");
        }

        [TestMethod]
        public void Save_SpreadsheetCellsAreChangedThenSavedAgain_FileShouldHaveSameChanges()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "original contents");
            sheet.Save("save.xml");
            string expectedOriginalText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>original contents</contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("expected.xml", expectedOriginalText);
            Assert.IsTrue(FilesAreEqual("expected.xml", "save.xml"));


            sheet.SetContentsOfCell("A1", "new contents");
            sheet.Save("save.xml");
            string expectedChangedText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>new contents</contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("expected.xml", expectedChangedText);
            Assert.IsTrue(FilesAreEqual("expected.xml", "save.xml"));


            sheet.SetContentsOfCell("A1", "");
            sheet.Save("save.xml");
            string expectedEmptyText =
                @"<?xml version=""1.0"" encoding=""utf-8""?>" + "\n" +
                @"<spreadsheet version=""default"" />";
            System.IO.File.WriteAllText("expected.xml", expectedEmptyText);
            Assert.IsTrue(FilesAreEqual("expected.xml", "save.xml"));


            // delete files after test is done
            File.Delete("save.xml");
            File.Delete("expected.xml");
        }



        [TestMethod]
        public void GetSavedVersion_DefaultSpreadsheet_ShouldReturnDefaultVersion()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.Save("saved-spreadsheet.xml");
            string expectedVersionName = "default";
            Assert.AreEqual(expectedVersionName, sheet.GetSavedVersion("saved-spreadsheet.xml"));
            File.Delete("saved-spreadsheet.xml");
        }

        [TestMethod]
        public void GetSavedVersion_CustomSpreadsheetVersionName_ShouldReturnVersion()
        {
            Spreadsheet sheet = new Spreadsheet(s => true, s => s, "v1.0");
            sheet.Save("saved-spreadsheet.xml");
            string expectedVersionName = "v1.0";
            Assert.AreEqual(expectedVersionName, sheet.GetSavedVersion("saved-spreadsheet.xml"));
            File.Delete("saved-spreadsheet.xml");
        }

        [TestMethod]
        public void GetSavedVersion_ExistingSpreadsheetFile_ShouldReturnVersion()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "\n" +
                "<spreadsheet version=\"v1.0\" />";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            string expectedVersionName = "v1.0";
            Assert.AreEqual(expectedVersionName, new Spreadsheet().GetSavedVersion("saved-spreadsheet.xml"));
            File.Delete("saved-spreadsheet.xml");
        }

        [TestMethod]
        public void GetSavedVersion_MissingSpreadsheetTag_ShouldThrowException()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "\n" +
                "< />";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet().GetSavedVersion("saved-spreadsheet.xml"));
            File.Delete("saved-spreadsheet.xml");
        }

        [TestMethod]
        public void GetSavedVersion_StartingElementIsNotSpreadsheet_ShouldThrowException()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<html></html>\n" +
                "<spreadsheet version=\"v1.0\" />";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet().GetSavedVersion("saved-spreadsheet.xml"));
            File.Delete("saved-spreadsheet.xml");
        }

        [TestMethod]
        public void GetSavedVersion_SpreadsheetTagDoesNotContainVersionAttribute_ShouldThrowException()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "\n" +
                "<spreadsheet />";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            SpreadsheetReadWriteException e = Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet().GetSavedVersion("saved-spreadsheet.xml"));
            Assert.AreEqual("version attribute not found", e.Message);
            File.Delete("saved-spreadsheet.xml");
        }




        [TestMethod]
        public void ConstructorReadFromFile_EmptySpreadsheetFile_ShouldCreateEmptySpreadsheetWithSameVersion()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "\n" +
                "<spreadsheet version=\"v6.9\" />";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);

            Spreadsheet sheet = new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "v6.9");
            Assert.AreEqual("v6.9", sheet.Version);
            Assert.IsTrue(sheet.GetNamesOfAllNonemptyCells().IsEmpty());
            File.Delete("saved-spreadsheet.xml");
        }

        [TestMethod]
        public void ConstructorReadFromFile_SavedSpreadsheetVersionDoesNotMatchConstructorVersionParameter_ShouldThrowException()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + "\n" +
                "<spreadsheet version=\"v6.9\" />";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            SpreadsheetReadWriteException e = Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));
            Assert.AreEqual("saved spreadsheet has a different version name", e.Message);
            File.Delete("saved-spreadsheet.xml");
        }

        [TestMethod]
        public void ConstructorReadFromFile_SpreadsheetWithCells_ShouldCreateSpreadsheetWithSameContents()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>string contents</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>F1</name>\n" +
                "        <contents>=B2</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>F2</name>\n" +
                "        <contents>=1234</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>F3</name>\n" +
                "        <contents>=A1+(B2+C3)</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>D1</name>\n" +
                "        <contents>420.69</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>D2</name>\n" +
                "        <contents>420.69</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>D3</name>\n" +
                "        <contents>1E-07</contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);

            Spreadsheet sheet = new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default");

            Assert.AreEqual("default", sheet.Version);
            Assert.AreEqual("string contents", sheet.GetCellContents("A1"));
            Assert.AreEqual(new Formula("B2"), sheet.GetCellContents("F1"));
            Assert.AreEqual(new Formula("1234"), sheet.GetCellContents("F2"));
            Assert.AreEqual(new Formula("A1+(B2+C3)"), sheet.GetCellContents("F3"));
            Assert.AreEqual(420.69, sheet.GetCellContents("D1"));
            Assert.AreEqual(420.69, sheet.GetCellContents("D2"));
            Assert.AreEqual(1E-07, sheet.GetCellContents("D3"));
        }


        [TestMethod]
        public void ConstructorReadFromFile_SavedSpreadsheetContainsInvalidNames_ShouldThrowException()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A_1</name>\n" +
                "        <contents>420</contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));
            File.Delete("saved-spreadsheet.xml");
        }

        [TestMethod]
        public void ConstructorReadFromFile_SavedSpreadsheetContainsCircularDependency_ShouldThrowException()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>=B2</contents>\n" +
                "    </cell>\n" +
                "    <cell>\n" +
                "        <name>B2</name>\n" +
                "        <contents>=A1</contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));
            File.Delete("saved-spreadsheet.xml");
        }

        [TestMethod]
        public void ConstructorReadFromFile_SavedSpreadsheetContainsInvalidFormula_ShouldThrowException()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>=+(+)+</contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));
            File.Delete("saved-spreadsheet.xml");
        }

        [TestMethod]
        public void ConstructorReadFromFile_FileDoesNotExist_ShouldThrowException()
        {
            File.Delete("saved-spreadsheet.xml");  // double check that the file is deleted beforehand
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));
        }

        [TestMethod]
        public void ConstructorReadFromFile_MalformedCells_ShouldThrowException()
        {
            // cell contains multiple tags
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>420</contents>\n" +
                "        <name>A1</name>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));

            // cell is missing a tag
            savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));

            // cell with no elements inside
            savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));

            // cell with null contents
            savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents></contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));


            // cell contains additional unknown tags
            savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>420</contents>\n" +
                "        <color>blue</color>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));

            // another cell opens before current cell is closed
            savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>testing0</name>\n" +
                "        <contents>420</contents>\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>420</contents>\n" +
                "    </cell>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));
        }

        [TestMethod]
        public void ConstructorReadFromFile_InvalidXml_ShouldThrowException()
        {
            // random stuff in between tags
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>420</contents>\n" +
                "    </cell>\n" +
                "aoeu\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));

            // missing closing tag
            savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>420</contents>\n" +
                "    </cell>\n";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));

            // missing version information
            savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet>\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>420</contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));

            // starting element is not a spreadsheet
            savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<html>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>420</contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>\n" +
                "</html>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));

            // unexpected/unknown xml element
            savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>420</contents>\n" +
                "    </cell>\n" +
                "    <person>\n" +
                "        <name>luke</name>\n" +
                "    </person>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Assert.ThrowsException<SpreadsheetReadWriteException>(() => new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default"));

            File.Delete("saved-spreadsheet.xml");
        }

        [TestMethod]
        public void ConstructorReadFromFile_CellElementsAreInDifferentOrder_ShouldSucceed()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <contents>hello</contents>\n" +
                "        <name>A1</name>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Spreadsheet sheet = new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default");
            Assert.AreEqual("hello", sheet.GetCellContents("A1"));
        }


        [TestMethod]
        public void ConstructorReadFromFile_XmlIsAllOnOneLine_ShouldSucceed()
        {
            // NOTE: i created the string on multiple lines in the code for readability,
            // but the actual string is just one line. 
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<spreadsheet version=\"default\">" +
                "<cell>" +
                "<name>A1</name>" +
                "<contents>hello</contents>" +
                "</cell>" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Spreadsheet sheet = new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default");
            Assert.AreEqual("hello", sheet.GetCellContents("A1"));
        }


        // TODO write tests for Changed
        [TestMethod]
        public void Changed_NewSpreadsheet_ShouldReturnFalse()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.IsFalse(sheet.Changed);
        }

        [TestMethod]
        public void Changed_CellIsAdded_ShouldReturnTrue()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "contents");
            Assert.IsTrue(sheet.Changed);
        }


        [TestMethod]
        public void Changed_CellIsAddedThenRemoved_ShouldStillReturnTrue()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "contents");
            sheet.SetContentsOfCell("A1", "");
            Assert.IsTrue(sheet.Changed);
        }


        [TestMethod]
        public void Changed_CellIsAddedThenSheetIsSaved_ShouldReturnFalse()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "contents");
            Assert.IsTrue(sheet.Changed);
            sheet.Save("savefile.xml");
            Assert.IsFalse(sheet.Changed);
            File.Delete("savefile.xml");
        }

        [TestMethod]
        public void Changed_CellIsAddedButCausesCircularException_ShouldReturnFalse()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.ThrowsException<CircularException>(() => sheet.SetContentsOfCell("A1", "=A1"));
            Assert.IsFalse(sheet.Changed);
        }

        [TestMethod]
        public void Changed_CreateFromFile_ShouldReturnFalse()
        {
            string savedSpreadsheetText =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                "<spreadsheet version=\"default\">\n" +
                "    <cell>\n" +
                "        <name>A1</name>\n" +
                "        <contents>hello</contents>\n" +
                "    </cell>\n" +
                "</spreadsheet>";
            System.IO.File.WriteAllText("saved-spreadsheet.xml", savedSpreadsheetText);
            Spreadsheet sheet = new Spreadsheet("saved-spreadsheet.xml", s => true, s => s, "default");
            Assert.IsFalse(sheet.Changed);
        }


    }
}
