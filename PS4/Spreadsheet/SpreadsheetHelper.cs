// Luke Ludlow
// CS 3500
// 2019 September

using System;
using System.Collections.Generic;
using SpreadsheetUtilities;
using System.Text.RegularExpressions;

namespace SS
{

    /// <summary>
    /// contains useful helper methods for the Spreadsheet.
    /// accesses and updates the Spreadsheet via dependency injection.
    /// </summary>
    internal class SpreadsheetHelper
    {

        private Spreadsheet spreadsheet;
        private CellFactory cellFactory;
        private Func<string, bool> isValidName;

        public SpreadsheetHelper(Spreadsheet spreadsheet)
        {
            this.spreadsheet = spreadsheet;
            // combine the provided IsValid delegate with the spreadsheet variable rules to create a more
            // fool-proof validator for us to use and give to formulas.
            this.isValidName = s => (IsValidSpreadsheetCellName(s) && spreadsheet.IsValid(s));
            this.cellFactory = new CellFactory(LookupCellValue);
        }


        /// <summary>
        /// create the lookup delegate that will be given to Formula for Evaluate method.
        /// it gets the value of the cell that corresponds to the given variable name.
        /// </summary>
        public double LookupCellValue(string name)
        {
            object cellValue = spreadsheet.GetCellValue(name);
            if (cellValue.GetType() != typeof(double)) {
                throw new ArgumentException(string.Format("the value of cell {0} is not a number", name));
            } else {
                return (double)cellValue;
            }
        }


        /// <summary>
        /// if the formula cannot be parsed, its constructor will throw a FormulaFormatException,
        /// which will propogate upwards from this method.
        /// 
        /// additionally, the Spreadsheet must pass its IsValid and Normalize delegates 
        /// into any Formula object that it creates, which is done in this method.
        /// </summary>
        public bool TryParseFormula(string s, out Formula formula)
        {
            if (!string.IsNullOrEmpty(s) && s[0] == '=') {
                string formulaString = s.Substring(1);
                formula = new Formula(formulaString, spreadsheet.Normalize, this.isValidName);
                return true;
            } else {
                formula = null;
                return false;
            }
        }

        public void RefreshCellValue(string name)
        {
            if (SpreadsheetContainsCell(name)) {
                cellFactory.RefreshCellValue(spreadsheet.Cells[name]);
            }
        }


        /// <summary>
        /// add cell replaces whatever exists at the current cell name, 
        /// or adds the cell if it doesn't exist yet.
        /// note that this method "refreshes" the dependency graph.
        /// </summary>
        public void AddCell(string name, object content)
        {
            if (SpreadsheetContainsCell(name)) {
                RemoveCell(name);
            }
            spreadsheet.Cells[name] = cellFactory.CreateNewCell(name, content);
            AddCellToDependencyGraph(name, content);
        }

        public void RemoveCell(string name)
        {
            RemoveCellFromDependencyGraph(name);
            spreadsheet.Cells.Remove(name);
        }

        public void RestoreCell(string name, object originalContent)
        {
            // remove the "failed" version of the cell
            RemoveCell(name);
            // re-add the original cell content 
            AddCell(name, originalContent);
        }

        public void AddCellToDependencyGraph(string name, object content)
        {
            if (CellContentsAreFormula(name)) {
                foreach (string variableName in GetCellFormulaVariables(name)) {
                    spreadsheet.DependencyGraph.AddDependency(variableName, name);
                }
            }
        }

        public void RemoveCellFromDependencyGraph(string name)
        {
            if (CellContentsAreFormula(name)) {
                foreach (string variableName in GetCellFormulaVariables(name)) {
                    spreadsheet.DependencyGraph.RemoveDependency(variableName, name);
                }
            }
        }

        public bool SpreadsheetContainsCell(string name)
        {
            return spreadsheet.Cells.ContainsKey(name);
        }

        public bool CellContentsAreFormula(string name)
        {
            return spreadsheet.Cells[name].Contents.GetType() == typeof(Formula);
        }

        public IEnumerable<string> GetCellFormulaVariables(string name)
        {
            return ((Formula)spreadsheet.Cells[name].Contents).GetVariables();
        }


        /// <summary>
        /// validates cell name by applying the spreadsheet's variable name conditions and applying 
        /// the IsValid delegate, then returns the normalized version of the name by calling the Normalize delegate.
        /// </summary>
        public string SanitizeCellName(string name)
        {
            ValidateCellName(name);
            string normalizedName = spreadsheet.Normalize(name);
            ValidateCellName(normalizedName);
            return normalizedName;
        }


        /// <summary>
        /// Variables for a Spreadsheet are only valid if they are one or more letters 
        /// followed by one or more digits (numbers).
        /// 
        /// specifically, a string is a valid cell name if and only if:
        /// 1. the string starts with one or more letters and is followed by one or more numbers.
        /// 2. the IsValid function returns true for that string, 
        ///        and IsValid should be called only for variable strings that are valid first by (1) above.
        /// 
        /// For example, "x", "_", "x2", "y_15", are all valid cell  names, but
        /// "25", "2x", "_A1", "A_1", and "&" are not.  Cell names are case sensitive, so "x" and "X" are
        /// different cell names.
        /// 
        /// a variable must first pass the loose tokenizer definition of a variable, 
        /// then pass the spreadsheet variable test,
        /// then pass the "outside developer's" IsValid test.
        /// 
        /// if any of the conditions fail, throws an InvalidNameException.
        /// </summary>
        public void ValidateCellName(string name)
        {
            bool isValidCellName = !string.IsNullOrEmpty(name) && isValidName(name);
            if (!isValidCellName) {
                throw new InvalidNameException();
            }
        }

        private bool IsValidSpreadsheetCellName(string name)
        {
            string validCellNamePattern = @"^[a-zA-Z]+\d+$";
            return Regex.IsMatch(name, validCellNamePattern, RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// throws ArgumentNullException if cell content is a null value
        /// </summary>
        public void ValidateCellContent(object content)
        {
            if (content is null) {
                throw new ArgumentNullException();
            }
        }


    }
}