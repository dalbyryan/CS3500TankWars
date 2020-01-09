// Luke Ludlow
// CS 3500
// 2019 September

using System;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace SS
{

    /// <summary>
    /// A spreadsheet consists of an infinite number of named cells.
    /// 
    /// A string is a cell name if and only if it consists of one or more letters,
    /// followed by one or more digits AND it satisfies the predicate IsValid.
    /// For example, "A15", "a15", "XY032", and "BC7" are cell names so long as they
    /// satisfy IsValid.  On the other hand, "Z", "X_", and "hello" are not cell names,
    /// regardless of IsValid.
    /// 
    /// Any valid incoming cell name, whether passed as a parameter or embedded in a formula,
    /// must be normalized with the Normalize method before it is used by or saved in 
    /// this spreadsheet.  For example, if Normalize is s => s.ToUpper(), then
    /// the Formula "x3+a5" should be converted to "X3+A5" before use.
    /// 
    /// A spreadsheet contains a cell corresponding to every possible cell name.  
    /// In addition to a name, each cell has a contents and a value.  The distinction is
    /// important.
    /// 
    /// The contents of a cell can be (1) a string, (2) a double, or (3) a Formula.  If the
    /// contents is an empty string, we say that the cell is empty.  (By analogy, the contents
    /// of a cell in Excel is what is displayed on the editing line when the cell is selected.)
    /// 
    /// In a new spreadsheet, the contents of every cell is the empty string.
    ///  
    /// The value of a cell can be (1) a string, (2) a double, or (3) a FormulaError.  
    /// (By analogy, the value of an Excel cell is what is displayed in that cell's position
    /// in the grid.)
    /// 
    /// If a cell's contents is a string, its value is that string.
    /// 
    /// If a cell's contents is a double, its value is that double.
    /// 
    /// If a cell's contents is a Formula, its value is either a double or a FormulaError,
    /// as reported by the Evaluate method of the Formula class.  The value of a Formula,
    /// of course, can depend on the values of variables.  The value of a variable is the 
    /// value of the spreadsheet cell it names (if that cell's value is a double) or 
    /// is undefined (otherwise).
    /// 
    /// Spreadsheets are never allowed to contain a combination of Formulas that establish
    /// a circular dependency.  A circular dependency exists when a cell depends on itself.
    /// For example, suppose that A1 contains B1*2, B1 contains C1*2, and C1 contains A1*2.
    /// A1 depends on B1, which depends on C1, which depends on A1.  That's a circular
    /// dependency.
    /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {

        private SpreadsheetHelper helper;

        /// <summary>
        /// spreadsheet cells dictionary 
        /// internal access is provided for the helper to use
        /// </summary>
        internal Dictionary<string, Cell> Cells { get; private set; }

        /// <summary>
        /// cells dependency graph
        /// internal access is provided for the helper to use
        /// </summary>
        internal DependencyGraph DependencyGraph { get; private set; }

        /// <summary>
        /// true if this spreadsheet has been modified since it was created or saved                  
        /// (whichever happened most recently), false otherwise.
        /// </summary>
        public override bool Changed { get; protected set; }



        /// <summary>
        /// creates an empty spreadsheet with the user specified validity delegate, 
        /// normalization delegate, and version name.
        /// this is the "main" constructor, the other two spreadsheet constructors call this one.
        /// </summary>
        /// <param name="isValid">validity delegate</param>
        /// <param name="normalize">normalization delegate</param>
        /// <param name="version">version name</param>
        public Spreadsheet(Func<string, bool> isValid, Func<string, string> normalize, string version)
            : base(isValid, normalize, version)
        {
            this.Cells = new Dictionary<string, Cell>();
            this.DependencyGraph = new DependencyGraph();
            this.helper = new SpreadsheetHelper(this);
            this.Changed = false;
        }

        /// <summary>
        /// creates an empty spreadsheet and allows the user to provide a string representing a path to a file, 
        /// a validity delegate, a normalization delegate, and a version. 
        /// it should read a saved spreadsheet from the file and use it to construct a new spreadsheet. 
        /// if anything goes wrong when reading the file, throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        /// <param name="filename">file path to a saved spreadsheet file</param>
        /// <param name="isValid">validity delegate</param>
        /// <param name="normalize">normalization delegate</param>
        /// <param name="version">version name</param>
        public Spreadsheet(string filename, Func<string, bool> isValid, Func<string, string> normalize, string version)
            : this(isValid, normalize, version)
        {
            ReadSavedSpreadsheet(filename);
            this.Changed = false;
        }

        /// <summary>
        /// zero-argument constructor creates an empty spreadsheet that imposes no extra validity conditions,
        /// normalizes every cell name to itself, and has version "default"
        /// </summary>
        public Spreadsheet()
            : this(s => true, s => s, "default")
        {
        }


        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return Cells.Keys;
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// 
        /// should use the internal IsValid delegate to check the name first.
        /// </summary>
        public override object GetCellContents(string name)
        {
            name = helper.SanitizeCellName(name);
            if (helper.SpreadsheetContainsCell(name)) {
                return Cells[name].Contents;
            } else {
                return string.Empty;
            }
        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a SpreadsheetUtilities.FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            name = helper.SanitizeCellName(name);
            if (helper.SpreadsheetContainsCell(name)) {
                return Cells[name].Value;
            } else {
                return string.Empty;
            }
        }


        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        /// 
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor.  There are then three possibilities:
        /// 
        ///   (1) If the remainder of content cannot be parsed into a Formula, a 
        ///       SpreadsheetUtilities.FormulaFormatException is thrown.
        ///       
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown,
        ///       and no change is made to the spreadsheet.
        ///       
        ///   (3) Otherwise, the contents of the named cell becomes f.
        /// 
        /// Otherwise, the contents of the named cell becomes just the string content.
        /// 
        /// If an exception is not thrown, the method returns a list consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell. The order of the list should be any
        /// order such that if cells are re-evaluated in that order, their dependencies 
        /// are satisfied by the time they are evaluated.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// list {A1, B1, C1} is returned.
        /// </summary>
        public override IList<string> SetContentsOfCell(string name, string content)
        {
            helper.ValidateCellContent(content);
            name = helper.SanitizeCellName(name);
            IList<string> result;
            if (double.TryParse(content, out double number)) {
                result = SetCellContents(name, number);
            } else if (helper.TryParseFormula(content, out Formula formula)) {
                result = SetCellContents(name, formula);
            } else {
                result = SetCellContents(name, content);
            }
            this.Changed = true;
            RecalculateDependentCellValues(result);
            return result;
        }

        private void RecalculateDependentCellValues(IList<string> dependentCellNames)
        {
            foreach (string name in dependentCellNames) {
                helper.RefreshCellValue(name);
            }
        }

        /// <summary>
        /// the contents of the named cell becomes number.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// </summary>
        protected override IList<string> SetCellContents(string name, double number)
        {
            helper.AddCell(name, number);
            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// the contents of the named cell becomes text.  The method returns a
        /// list consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// </summary>
        protected override IList<string> SetCellContents(string name, string text)
        {
            helper.AddCell(name, text);
            if (text == string.Empty) {
                helper.RemoveCell(name);
            }
            return GetCellsToRecalculate(name).ToList();
        }

        /// <summary>
        /// the contents of the named cell becomes formula.  
        /// 
        /// the Spreadsheet passes its IsValid and Normalize delegates into any Formula object that it creates.
        /// (note: this is performed when the spreadsheet parses the formula.)
        /// 
        /// if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException, and no change is made to the spreadsheet.
        /// 
        /// The method returns a list consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// </summary>
        protected override IList<string> SetCellContents(string name, Formula formula)
        {
            // save original content, if adding the cell results in a circular dependency we can restore the previous
            // contents that were valid
            object originalContents = GetCellContents(name);
            helper.AddCell(name, formula);
            try {
                return GetCellsToRecalculate(name).ToList();
            } catch (CircularException e) {
                helper.RestoreCell(name, originalContents);
                throw e;
            }
        }

        /// <summary>
        /// returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string cellName)
        {
            return DependencyGraph.GetDependents(cellName);
        }


        /// <summary>
        /// Writes the contents of this spreadsheet to the named file using an XML format.
        /// The XML elements should be structured as follows:
        /// 
        /// <spreadsheet version="version information goes here">
        /// 
        /// <cell>
        /// <name>
        /// cell name goes here
        /// </name>
        /// <contents>
        /// cell contents goes here
        /// </contents>    
        /// </cell>
        /// 
        /// </spreadsheet>
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.  
        /// If the cell contains a string, it should be written as the contents.  
        /// If the cell contains a double d, d.Tostring() should be written as the contents.  
        /// If the cell contains a Formula f, f.Tostring() with "=" prepended should be written as the contents.
        /// 
        /// If there are any problems opening, writing, or closing the file, the method should throw a
        /// SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override void Save(string filename)
        {
            SpreadsheetWriter.WriteSpreadsheet(this, filename);
            this.Changed = false;
        }

        /// <summary>
        /// Returns the version information of the spreadsheet saved in the named file.
        /// If there are any problems opening, reading, or closing the file, the method
        /// should throw a SpreadsheetReadWriteException with an explanatory message.
        /// </summary>
        public override string GetSavedVersion(string filename)
        {
            return SpreadsheetWriter.GetSpreadsheetFileSavedVersionName(filename);
        }

        /// <summary>
        /// reads the spreadsheet contained in the saved file.
        /// if there are any problems opening, reading, or closing the file, 
        /// throws a SpreadsheetReadWriteException. 
        /// </summary>
        /// <param name="filename">file path to a saved spreadsheet file</param>
        /// <returns>new constructed spreadsheet object</returns>
        private void ReadSavedSpreadsheet(string filename)
        {
            SpreadsheetWriter.ReadSpreadsheet(this, filename);
        }


    }
}