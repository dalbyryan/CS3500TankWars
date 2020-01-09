// Luke Ludlow
// CS 3500
// 2019 September

using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// Cell is an immutable type that holds a cell's name, content, and value.
    /// </summary>
    internal class Cell
    {

        /// <summary>
        /// the cell's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// get the cell content as a generic object. 
        /// </summary>
        public object Contents { get; }

        /// <summary>
        /// get the cell value as a generic object. provides a set method because cell values
        /// can change and be recalculated.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// create a cell with the given name, content, and value. 
        /// cells are immutable, the value cannot be changed once constructed.
        /// </summary>
        /// <param name="name">string name of cell</param>
        /// <param name="contents">content must be either a string, double, or Formula</param>
        /// <param name="value">value must be either a string, double, or FormulaError</param>
        public Cell(string name, object contents, object value)
        {
            this.Name = name;
            this.Contents = contents;
            this.Value = value;
        }

        /// <summary>
        /// serialize the cell into xml
        /// </summary>
        public void WriteAsXml(XmlWriter writer)
        {
            writer.WriteStartElement("cell");
            writer.WriteElementString("name", this.Name);
            writer.WriteElementString("contents", GetContentAsString());
            writer.WriteEndElement();
        }

        private string GetContentAsString()
        {
            string contentsString = "";
            if (this.Contents.GetType() == typeof(string)) {
                contentsString = (string)this.Contents;
            } else if (this.Contents.GetType() == typeof(double)) {
                contentsString = ((double)this.Contents).ToString();
            } else if (this.Contents.GetType() == typeof(Formula)) {
                contentsString = "=" + ((Formula)this.Contents).ToString();
            } 
            return contentsString;
        }

    }
}
