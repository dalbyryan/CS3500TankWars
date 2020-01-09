// Luke Ludlo// Luke Ludlow
// CS 3500
// 2019 Octoberw
// CS 3500
// 2019 October

using System;
using SS;
using SpreadsheetUtilities;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SpreadsheetGUI
{
    /// <summary>
    /// this is the Model in my model view controller architecture.
    /// 
    /// please see the readme in PS6/Resources/README.md for a detailed explanation of 
    /// this component's design and responsibility.
    /// </summary>
    public class Model
    {
        private Spreadsheet spreadsheet;

        private string mostRecentlySavedFileName;


        public Model()
        {
            spreadsheet = new Spreadsheet(IsValid, Normalize, "ps6");
            mostRecentlySavedFileName = null;
        }

        /// <summary>
        /// create the validator delegate for the spreadsheet that verifies all variable names are
        /// valid cell names (one letter A-Z and one number 1-99)
        /// </summary>
        private bool IsValid(string name)
        {
            string validCellNamePattern = @"^[A-Z][1-9][0-9]?$";
            return Regex.IsMatch(name, validCellNamePattern);
        }

        /// <summary>
        /// create the normalizer delegate for the spreadsheet that normalizes cell names to uppercase
        /// </summary>
        private string Normalize(string name)
        {
            return name.ToUpper();
        }

        public bool SpreadsheetHasBeenModifiedSinceLastSave()
        {
            return spreadsheet.Changed;
        }

        public void SaveFile(string filename)
        {
            spreadsheet.Save(filename);
            mostRecentlySavedFileName = filename;
        }

        public void ReadSpreadsheetFromFile(string filename)
        {
            spreadsheet = new Spreadsheet(filename, IsValid, Normalize, "ps6");
            mostRecentlySavedFileName = filename;
        }

        public string GetMostRecentlySavedFileName()
        {
            return mostRecentlySavedFileName;
        }

        public IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            return spreadsheet.GetNamesOfAllNonemptyCells();
        }

        public IList<string> SetContentsOfCell(int col, int row, string contents)
        {
            return spreadsheet.SetContentsOfCell(ConvertColRowToCellName(col, row), contents);
        }

        public string GetCellContents(int col, int row)
        {
            return ConvertCellContentsToString(spreadsheet.GetCellContents(ConvertColRowToCellName(col, row)));
        }
        private string ConvertCellContentsToString(object contents)
        {
            string contentsString = "";
            if (contents.GetType() == typeof(string)) {
                contentsString = (string)contents;
            } else if (contents.GetType() == typeof(double)) {
                contentsString = ((double)contents).ToString();
            } else if (contents.GetType() == typeof(Formula)) {
                contentsString = "=" + ((Formula)contents).ToString();
            }
            return contentsString;
        }

        public string GetCellValue(int col, int row)
        {
            return ConvertCellValueToString(spreadsheet.GetCellValue(ConvertColRowToCellName(col, row)));
        }
        private string ConvertCellValueToString(object value)
        {
            string valueString = "";
            if (value.GetType() == typeof(string)) {
                valueString = (string)value;
            } else if (value.GetType() == typeof(double)) {
                valueString = ((double)value).ToString();
            } else if (value.GetType() == typeof(FormulaError)) {
                valueString = ConvertFormulaErrorToFriendlyErrorMessage((FormulaError)value);
            }
            return valueString;
        }
        private string ConvertFormulaErrorToFriendlyErrorMessage(FormulaError formulaError)
        {
            string errorMessage = formulaError.Reason;
            if (errorMessage.Contains("an unexpected error occurred while evaluating the formula. ")) {
                errorMessage = errorMessage.Replace("an unexpected error occurred while evaluating the formula. ", "");
            }
            if (errorMessage.Contains("lookup delegate ")) {
                errorMessage = errorMessage.Replace("lookup delegate ", "");
            }
            return errorMessage;
        }

        public void ConvertCellNameToColRow(string name, out int col, out int row)
        {
            char letter = name[0];
            col = letter - 'A';
            string digits = name.Substring(1);
            row = int.Parse(digits) - 1;
        }

        public string ConvertColRowToCellName(int col, int row)
        {
            char letter = (char)((int)'A' + col);
            string digits = string.Format("{0}", row + 1);
            return letter + digits;
        }

    }
}
