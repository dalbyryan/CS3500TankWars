// Luke Ludlow
// CS 3500
// 2019 October

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    /// <summary>
    /// the SpreadsheetGUIEvents is the "glue" that loosely connects the view and controller.
    /// 
    /// please see the readme in PS6/Resources/README.md for a detailed explanation of 
    /// this component's design and responsibility.
    /// </summary>
namespace SpreadsheetGUI
{

    public delegate void ShowCellNameHandler(string cellName);

    public delegate void ShowCellContentsHandler(string cellContents);

    public delegate void ShowCellValueHandler(int col, int row, string cellValue);

    public delegate void SaveFileAsHandler();

    public delegate void SelectDefaultStartCellHandler();

    public delegate void ShowErrorMessageHandler(string errorMessage, string title);

    public delegate void ShowWarningMessageWithYesNoHandler(string warningMessage, string title, out bool userChoseToContinue);

    public delegate void ShowSaveFileDialogHandler();

    public delegate void ShowOpenFileDialogHandler();

    public delegate void ActivateLightModeHandler();
    
    public delegate void ActivateDarkModeHandler();

}
