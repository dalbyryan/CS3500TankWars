// Luke Ludlow
// CS 3500
// 2019 October

using System;
using System.Collections.Generic;
using SS;
using System.Text.RegularExpressions;

namespace SpreadsheetGUI
{

    /// <summary>
    /// this is the Controller in my model view controller architecture.
    /// 
    /// please see the readme in PS6/Resources/README.md for a detailed explanation of 
    /// this component's design and responsibility.
    /// </summary>
    public class Controller
    {

        public event ShowCellNameHandler ShowCellNameEvent;
        public event ShowCellContentsHandler ShowCellContentsEvent;
        public event ShowCellValueHandler ShowCellValueEvent;
        public event SelectDefaultStartCellHandler SelectDefaultStartCellEvent;
        public event ShowErrorMessageHandler ShowErrorMessageEvent;
        public event ShowWarningMessageWithYesNoHandler ShowWarningMessageWithYesNoEvent;
        public event ShowSaveFileDialogHandler ShowSaveFileDialogEvent;
        public event ShowOpenFileDialogHandler ShowOpenFileDialogEvent;
        public event ActivateLightModeHandler ActivateLightModeEvent;
        public event ActivateDarkModeHandler ActivateDarkModeEvent;

        private Model model;
        private bool darkModeIsEnabled;

        public Controller()
        {
            model = new Model();
        }

        public void InitializeController()
        {
            SelectDefaultStartCellEvent?.Invoke();
            ActivateLightModeEvent?.Invoke();
            darkModeIsEnabled = false;  
        }

        public void ToggleDarkMode()
        {
            if (darkModeIsEnabled) {
                // if it's currently dark mode, then switch to light mode
                darkModeIsEnabled = false;
                ActivateLightModeEvent?.Invoke();
            } else {
                // if it's currently light mode, then switch to dark mode
                darkModeIsEnabled = true;
                ActivateDarkModeEvent?.Invoke();
            }
        }

        /// <summary>
        /// if an operation would result in the loss of unsaved data,
        /// tell the view to display an warning message, and check if the user clicks okay to continue.
        /// </summary>
        /// <returns></returns>
        public bool WarnUserAndAskToContinue()
        {
            string warningMessage = "are you sure you want to continue? all unsaved changes to the current spreadsheet will be lost";
            string warningTitle = "unsaved changes will be lost";
            bool userChoseToContinue = false;
            ShowWarningMessageWithYesNoEvent?.Invoke(warningMessage, warningTitle, out userChoseToContinue);
            return userChoseToContinue;
        }

        /// <summary>
        /// when the "Open" button is clicked, the view asks the controller to handle opening a new spreadsheet.
        /// this includes checking safety features.
        /// </summary>
        public void HandleOpenFile()
        {
            if (SpreadsheetHasBeenModifiedSinceLastSave()) {
                bool userChoseToContinue = WarnUserAndAskToContinue();
                if (userChoseToContinue) {
                    ShowOpenFileDialogEvent?.Invoke();
                }
            } else {
                ShowOpenFileDialogEvent?.Invoke();
            }
        }

        /// <summary>
        /// read the saved spreadsheet file and fill the model with the read data
        /// </summary>
        public void OpenFile(string filename)
        {
            TryOpenFile(filename);
            FullRefreshView();
        }

        private void TryOpenFile(string filename)
        {
            try {
                model.ReadSpreadsheetFromFile(filename);
            } catch (SpreadsheetReadWriteException e) {
                ShowErrorMessageEvent?.Invoke(e.Message, "unable to open file");
            }
        }

        /// <summary>
        /// tell the model to save the current spreadsheet info to the given file.
        /// if this currently open spreadsheet has already been saved once, or was loaded from an existing file,
        /// then clicking "Save" will go ahead and rewrite to that file. 
        /// 
        /// if the spreadsheet hasn't been saved yet, 
        /// clicking "Save" will follow the same events as if they clicked "Save Aa".
        /// </summary>
        public void SaveFile()
        {
            if (string.IsNullOrEmpty(model.GetMostRecentlySavedFileName())) {
                ShowSaveFileDialogEvent?.Invoke();
            } else {
                TrySaveFile(model.GetMostRecentlySavedFileName());
            }
        }

        /// <summary>
        /// if the given filename ends in .sprd, cool, otherwise append .sprd. 
        /// if the user selects option 2, show all files, then the .sprd extension isn't enforced.
        /// 
        /// save as will always warn when you are about to overwrite a file.
        /// </summary>
        public void SaveFileAs(string filename, int filterIndex)
        {
            if (filename != "") {
                switch (filterIndex) {
                    case 1:
                        string validSprdFilePattern = @"^.*(\.sprd)$";
                        if (!Regex.IsMatch(filename, validSprdFilePattern)) {
                            filename += ".sprd";
                        }
                        break;
                    case 2:
                        break;
                }
                model.SaveFile(filename);
            }
        }

        private void TrySaveFile(string filename)
        {
            try {
                model.SaveFile(filename);
            } catch (SpreadsheetReadWriteException e) {
                ShowErrorMessageEvent?.Invoke(e.Message, "unable to save file");
            }
        }

        public bool SpreadsheetHasARecentlySavedFile()
        {
            return !string.IsNullOrEmpty(model.GetMostRecentlySavedFileName());
        }

        public bool SpreadsheetHasBeenModifiedSinceLastSave()
        {
            return model.SpreadsheetHasBeenModifiedSinceLastSave();
        }

        /// <summary>
        /// tell the view to display an error message to the user
        /// </summary>
        public void ShowErrorMessage(string message, string title)
        {
            ShowErrorMessageEvent?.Invoke(message, title);
        }

        /// <summary>
        /// process a new cell selection input. updates the model and the view.
        /// </summary>
        public void HandleSelectionChanged(int col, int row)
        {
            ShowCellNameEvent?.Invoke(model.ConvertColRowToCellName(col, row));
            ShowCellContentsEvent?.Invoke(model.GetCellContents(col, row));
            ShowCellValueEvent?.Invoke(col, row, model.GetCellValue(col, row));
        }

        /// <summary>
        /// process new cell contents. updates the model and the view.
        /// </summary>
        public void HandleCellContentsChanged(int col, int row, string contents)
        {
            try {
                IList<string> dependents = model.SetContentsOfCell(col, row, contents);
                RefreshDependentCellValues(dependents);
            } catch (CircularException e) {
                ShowErrorMessage("that formula would cause a circular dependency. the change will not be made.", "circular dependency");
            } catch (Exception e) {
                ShowErrorMessage(e.Message, "error");
            }
        }

        /// <summary>
        /// once a cell's contents or value has been changed, ask the model to recalculate the values of the
        /// dependent cells, then tell the view to display the updated values of the dependent cells.
        /// </summary>
        /// <param name="dependents"></param>
        public void RefreshDependentCellValues(IList<string> dependents)
        {
            foreach (string name in dependents) {
                model.ConvertCellNameToColRow(name, out int col, out int row);
                ShowCellValueEvent?.Invoke(col, row, model.GetCellValue(col, row));
            }
        }

        /// <summary>
        /// used when reading from an existing file. iterates through every cell in the spreadsheet and updates
        /// the view based upon the new model. this is useful because it clears any old values from the view too.
        /// </summary>
        public void FullRefreshView()
        {
            for (int col = 0; col < 26; col++) {
                for (int row = 0; row < 99; row++) {
                    ShowCellValueEvent?.Invoke(col, row, model.GetCellValue(col, row));
                }
            }
            SelectDefaultStartCellEvent?.Invoke();
        }

        /// <summary>
        /// process key input. if it's an arrow key, update the current cell selection accordingly.
        /// </summary>
        public bool ProcessArrowKey(string keyName, ref int col, ref int row)
        {
            switch (keyName) {
                case "Left":
                    if (col > 0) {
                        col--;
                    }
                    return true;
                case "Right":
                    if (col < 26) {
                        col++;
                    }
                    return true;
                case "Up":
                    if (row > 0) {
                        row--;
                    }
                    return true;
                case "Down":
                    if (row < 99) {
                        row++;
                    }
                    return true;
                default:
                    return false;
            }
        }

    }
}
