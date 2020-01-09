// Luke Ludlow
// CS 3500
// 2019 September

using System;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// handles all the non-trivial logic of creating cells and figuring out their content and value.
    /// </summary>
    internal class CellFactory
    {

        private Func<string, double> lookup;

        public CellFactory(Func<string, double> lookup)
        {
            this.lookup = lookup;
        }

        public Cell CreateNewCell(string name, object content)
        {
            return new Cell(name, content, CalculateCellValue(content));
        }

        private object CalculateCellValue(object contents)
        {
            object result = null;
            if (contents.GetType() == typeof(string)) {
                result = contents;
            } else if (contents.GetType() == typeof(double)) {
                result = contents;
            } else if (contents.GetType() == typeof(Formula)) {
                Formula f = (Formula)contents;
                result = f.Evaluate(lookup); 
            } 
            return result;
        }

        public void RefreshCellValue(Cell cell)
        {
            if (cell.Contents.GetType() == typeof(Formula)) {
                cell.Value = CalculateCellValue(cell.Contents);
            } 
        }

    }
}