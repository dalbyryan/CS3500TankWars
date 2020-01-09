// Luke Ludlow
// CS 3500
// 2019 September

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;

namespace SpreadsheetTests
{
    /// <summary>
    /// these are the spreadsheet unit tests from PS4. 
    /// i updated them to work with the new public methods in AbstractSpreadsheet,
    /// but they just test the same PS4 functionality. 
    /// </summary>
    [TestClass]
    public class PS4SpreadsheetTests
    {

        [TestMethod]
        public void Constructor_AssignToAbstractSpreadsheet_ShouldSucceed()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Assert.IsNotNull(sheet);
        }


        [TestMethod]
        public void GetNamesOfAllNonemptyCells_EmptySheet_ShouldReturnEmptyEnumerable()
        {
            Assert.IsTrue(new Spreadsheet().GetNamesOfAllNonemptyCells().IsEmpty());
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_AddCells_ShouldReturnAllCellNames()
        {
            Spreadsheet sheet = new Spreadsheet();
            string cellName = "name1";
            string cellName2 = "name2";
            string cellName3 = "name3";
            sheet.SetContentsOfCell(cellName, "content");
            sheet.SetContentsOfCell(cellName2, "content");
            sheet.SetContentsOfCell(cellName3, "content");
            Assert.AreEqual(3, sheet.GetNamesOfAllNonemptyCells().GetCount());
            Assert.IsTrue(sheet.GetNamesOfAllNonemptyCells().Contains(cellName));
            Assert.IsTrue(sheet.GetNamesOfAllNonemptyCells().Contains(cellName2));
            Assert.IsTrue(sheet.GetNamesOfAllNonemptyCells().Contains(cellName3));
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_AddCellsWithSameName_ShouldReturnOneName()
        {
            Spreadsheet sheet = new Spreadsheet();
            string cellName = "name1";
            sheet.SetContentsOfCell(cellName, "content");
            sheet.SetContentsOfCell(cellName, "42");
            sheet.SetContentsOfCell(cellName, "=x1");
            Assert.AreEqual(1, sheet.GetNamesOfAllNonemptyCells().GetCount());
            Assert.IsTrue(sheet.GetNamesOfAllNonemptyCells().Contains(cellName));
        }

        [TestMethod]
        public void GetNamesOfAllNonemptyCells_AddThenRemoveCell_ShouldReturnUpdatedEnumerable()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("name1", "content");
            Assert.AreEqual(1, sheet.GetNamesOfAllNonemptyCells().GetCount());
            sheet.SetContentsOfCell("name1", "");
            Assert.AreEqual(0, sheet.GetNamesOfAllNonemptyCells().GetCount());
        }



        [TestMethod]
        public void GetCellContents_CellDoesNotExistYet_ShouldReturnEmptyString()
        {
            Spreadsheet sheet = new Spreadsheet();
            // don't use string.IsNullOrEmpty because should GetCellContents should NOT return null! 
            // an empty cell contents are the empty string "", so compare to string.Empty
            Assert.AreEqual(string.Empty, sheet.GetCellContents("name1"));
        }

        [TestMethod]
        public void GetCellContents_ContentsAreString_ShouldReturnString()
        {
            Spreadsheet sheet = new Spreadsheet();
            string cellName = "name1";
            string cellContents = "contents";
            sheet.SetContentsOfCell(cellName, cellContents);
            Assert.IsInstanceOfType(sheet.GetCellContents(cellName), typeof(string));
            Assert.AreEqual(cellContents, sheet.GetCellContents(cellName));
        }

        [TestMethod]
        public void GetCellContents_ContentsAreDouble_ShouldReturnDouble()
        {
            Spreadsheet sheet = new Spreadsheet();
            string cellName = "name1";
            double cellContents = 420.69;
            sheet.SetContentsOfCell(cellName, cellContents.ToString());
            Assert.IsInstanceOfType(sheet.GetCellContents(cellName), typeof(double));
            Assert.AreEqual(cellContents, sheet.GetCellContents(cellName));
        }

        [TestMethod]
        public void GetCellContents_ContentsAreFormula_ShouldReturnFormula()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=x1 + x2");
            Assert.IsInstanceOfType(sheet.GetCellContents("A1"), typeof(Formula));
            Assert.AreEqual(new Formula("x1 + x2"), sheet.GetCellContents("A1"));
        }

        [TestMethod]
        public void GetCellContents_NameIsNullOrInvalid_ShouldThrowInvalidNameException()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.ThrowsException<InvalidNameException>(() => sheet.GetCellContents(null));
            Assert.ThrowsException<InvalidNameException>(() => sheet.GetCellContents(""));
            Assert.ThrowsException<InvalidNameException>(() => sheet.GetCellContents("23x"));
            Assert.ThrowsException<InvalidNameException>(() => sheet.GetCellContents("&"));
            Assert.ThrowsException<InvalidNameException>(() => sheet.GetCellContents("abc1$2"));
        }



        [TestMethod]
        public void SetContentsOfCell_SetMultipleCells_NonEmptyCellCountShouldIncrease()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("name1", "cellContents1");
            sheet.SetContentsOfCell("name2", "cellContents2");
            sheet.SetContentsOfCell("name3", "cellContents3");
            Assert.AreEqual(3, sheet.GetNamesOfAllNonemptyCells().GetCount());
        }

        [TestMethod]
        public void SetContentsOfCell_SetCellToEmptyString_NonEmptyCellCountShouldDecrease()
        {
            Spreadsheet sheet = new Spreadsheet();
            string cellName = "name1";
            sheet.SetContentsOfCell(cellName, "cellContents");
            Assert.AreEqual(1, sheet.GetNamesOfAllNonemptyCells().GetCount());
            sheet.SetContentsOfCell(cellName, "");
            Assert.AreEqual(0, sheet.GetNamesOfAllNonemptyCells().GetCount());
        }

        [TestMethod]
        public void SetContentsOfCell_CallAgainWithSameNameDifferentContent_ShouldUpdateCellContent()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "string");
            Assert.AreEqual("string", sheet.GetCellContents("A1"));
            sheet.SetContentsOfCell("A1", "42");
            Assert.AreEqual((double)42, sheet.GetCellContents("A1"));
            sheet.SetContentsOfCell("A1", "=x1");
            Assert.AreEqual(new Formula("x1"), sheet.GetCellContents("A1"));
            // make sure that there is only one cell in the spreadsheet, 
            // we should be overwriting the same cell, not creating new ones
            Assert.AreEqual(1, sheet.GetNamesOfAllNonemptyCells().GetCount());
        }

        [TestMethod]
        public void SetContentsOfCell_InvalidName_ShouldThrowInvalidNameException()
        {
            Spreadsheet sheet = new Spreadsheet();
            string invalidCellName = "invalid$name";
            Assert.ThrowsException<InvalidNameException>(() => sheet.SetContentsOfCell(invalidCellName, "42"));
            Assert.ThrowsException<InvalidNameException>(() => sheet.SetContentsOfCell(invalidCellName, "string"));
            Assert.ThrowsException<InvalidNameException>(() => sheet.SetContentsOfCell(invalidCellName, "=x1"));
        }

        [TestMethod]
        public void SetContentsOfCell_ContentIsNull_ShouldThrowArgumentNullException()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.ThrowsException<ArgumentNullException>(() => sheet.SetContentsOfCell("cellName", (string)null));
        }

        [TestMethod]
        public void SetContentsOfCell_DoublesInDifferentFormats_ShouldSucceed()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("name1", "42");
            sheet.SetContentsOfCell("name2", "5.2e-9");
            sheet.SetContentsOfCell("name3", "0.00000000000010");
            sheet.SetContentsOfCell("name4", "(1.2 / .9)");  // FIXME idk if this will work
            Assert.AreEqual(4, sheet.GetNamesOfAllNonemptyCells().GetCount());
        }

        [TestMethod]
        public void SetContentsOfCell_ZeroDependents_ShouldReturnListContainingItself()
        {
            Spreadsheet sheet = new Spreadsheet();
            IList<string> dependents = sheet.SetContentsOfCell("A1", "42");
            Assert.AreEqual(1, dependents.Count);
            Assert.IsTrue(dependents.Contains("A1"));
        }

        [TestMethod]
        public void SetContentsOfCell_DirectDependents_ShouldReturnListContainingItselfAndDirectDependents()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "=A1");
            sheet.SetContentsOfCell("C1", "=A1");
            sheet.SetContentsOfCell("D1", "=A1");
            IList<string> dependents = sheet.SetContentsOfCell("A1", "=42");
            Assert.AreEqual(4, dependents.Count);
            // when the cells are all direct dependents, they can be in any order, so we just need to verify that
            // the set contains each of the direct dependent cells. however, the node itself is always first.
            Assert.AreEqual("A1", dependents[0]);
            Assert.IsTrue(dependents.Contains("B1"));
            Assert.IsTrue(dependents.Contains("C1"));
            Assert.IsTrue(dependents.Contains("D1"));
        }

        [TestMethod]
        public void SetContentsOfCell_IndirectDependents_ShouldReturnListContainingItselfAndDirectAndIndirectDependents()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "=A1");
            sheet.SetContentsOfCell("C1", "=B1");
            sheet.SetContentsOfCell("D1", "=C1");
            IList<string> dependents = sheet.SetContentsOfCell("A1", "42");
            Assert.AreEqual(4, dependents.Count);
            // dependent order matters!
            Assert.AreEqual("A1", dependents[0]);
            Assert.AreEqual("B1", dependents[1]);
            Assert.AreEqual("C1", dependents[2]);
            Assert.AreEqual("D1", dependents[3]);
        }

        [TestMethod]
        public void SetContentsOfCell_NewlyAddedCellHasDependees_ShouldReturnListThatContainsDependentsNotDependees()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "=A1 + (X5 / C3)");
            IList<string> dependents = sheet.SetContentsOfCell("A1", "X5 / C3");
            // X5 and C3 are not dependents of A1
            Assert.AreEqual(2, dependents.Count);
            Assert.IsTrue(dependents.Contains("A1"));
            Assert.IsTrue(dependents.Contains("B1"));
        }

        [TestMethod]
        public void SetContentsOfCell_CellContentsAreStringNotActuallyAFormula_ShouldNotHaveAnyDependents()
        {
            Spreadsheet sheet = new Spreadsheet();
            IList<string> dependents = sheet.SetContentsOfCell("A1", "A1 + B1 + C1");
            Assert.AreEqual(1, dependents.Count);
            Assert.IsTrue(dependents.Contains("A1"));
        }

        [TestMethod]
        public void SetContentsOfCell_CellDependsOnItself_ShouldThrowCircularException()
        {
            Spreadsheet sheet = new Spreadsheet();
            Assert.ThrowsException<CircularException>(() => sheet.SetContentsOfCell("A1", "=A1 * 2"));
        }

        [TestMethod]
        public void SetContentsOfCell_DirectCircularDependency_ShouldThrowCircularException()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1");
            Assert.ThrowsException<CircularException>(() => sheet.SetContentsOfCell("B1", "=A1"));
        }

        [TestMethod]
        public void SetContentsOfCell_IndirectCircularDependency_ShouldThrowCircularException()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "=B1");
            sheet.SetContentsOfCell("B1", "=C1");
            Assert.ThrowsException<CircularException>(() => sheet.SetContentsOfCell("C1", "=A1"));

            sheet = new Spreadsheet();
            sheet.SetContentsOfCell("B1", "=A1 * 1");
            sheet.SetContentsOfCell("C1", "=A1 * 2");
            sheet.SetContentsOfCell("D1", "=A1 * 3");
            Assert.ThrowsException<CircularException>(() => sheet.SetContentsOfCell("A1", "=B1 + C1 + D1"));
        }

        [TestMethod]
        public void SetContentsOfCell_NewCellHasDependeesButNoDependents_ShouldReturnListContainingOnlyItself()
        {
            Spreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "42");
            sheet.SetContentsOfCell("B1", "=A1");
            IList<string> dependents = sheet.SetContentsOfCell("A2", "=B1");
            Assert.AreEqual(1, dependents.Count);
            Assert.AreEqual("A2", dependents[0]);
        }


        [TestMethod]
        public void SetContentsOfCell_RemoveDirectDependency_ShouldReturnUpdatedListThatNoLongerHasTheIndirectDependency()
        {
            Spreadsheet sheet = new Spreadsheet();

            // B1 is the "middle node" that links C1 to A1. 
            // A1 <- B1 <- C1
            sheet.SetContentsOfCell("B1", "=A1");
            sheet.SetContentsOfCell("C1", "=B1");
            IList<string> dependents = sheet.SetContentsOfCell("A1", "42");
            Assert.AreEqual(3, dependents.Count);
            Assert.AreEqual("A1", dependents[0]);
            Assert.AreEqual("B1", dependents[1]);
            Assert.AreEqual("C1", dependents[2]);

            // now remove B1.
            // removing the direct dependency B1 also breaks the indirect link between A1 and C1. now the graph would
            // look like:
            // A1 (alone)    and then    D1 <- B1 <- C1
            // so now A1 will have no dependents
            sheet.SetContentsOfCell("B1", "=D2");
            dependents = sheet.SetContentsOfCell("A1", "42");
            Assert.AreEqual(1, dependents.Count);
            Assert.AreEqual("A1", dependents[0]);
        }


    }
}
