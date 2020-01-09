// Luke Ludlow
// CS 3500
// 2019 October

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpreadsheetGUI
{

    /// <summary>
    /// the SpreadsheetGUIApplicationContext handles running multiple gui windows.
    /// </summary>
    class SpreadsheetGUIApplicationContext : ApplicationContext
    {
        private int numberOfOpenForms = 0;
        // singleton ApplicationContext
        private static SpreadsheetGUIApplicationContext appContext;
        // private constructor for singleton pattern
        private SpreadsheetGUIApplicationContext() { }
        public static SpreadsheetGUIApplicationContext GetAppContext()
        {
            if (appContext == null) {
                appContext = new SpreadsheetGUIApplicationContext();
            }
            return appContext;
        }
        public void RunForm(SpreadsheetGUIView form)
        {
            // one more form is running
            numberOfOpenForms++;
            // when this form closes, we want to know
            form.FormClosed += (o, e) => { if (--numberOfOpenForms <= 0) ExitThread(); };
            // run the form
            form.Show();
        }
    }


    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SpreadsheetGUIApplicationContext appContext = SpreadsheetGUIApplicationContext.GetAppContext();
            appContext.RunForm(new SpreadsheetGUIView());
            Application.Run(appContext);
        }
    }
}
