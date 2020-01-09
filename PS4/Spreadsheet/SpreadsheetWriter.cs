// Luke Ludlow
// CS 3500
// 2019 September

using System;
using System.Xml;
using SpreadsheetUtilities;

namespace SS
{
    /// <summary>
    /// helper class to read and write spreadsheet xml files
    /// </summary>
    internal class SpreadsheetWriter
    {

        public static void WriteSpreadsheet(Spreadsheet spreadsheet, string filename)
        {
            try {
                using (XmlWriter writer = CreateXmlWriter(filename)) {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("spreadsheet");
                    writer.WriteAttributeString("version", spreadsheet.Version);
                    foreach (Cell cell in spreadsheet.Cells.Values) {
                        cell.WriteAsXml(writer);
                    }
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            } catch (Exception e) {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }


        public static void ReadSpreadsheet(Spreadsheet spreadsheet, string filename)
        {
            if (GetSpreadsheetFileSavedVersionName(filename) != spreadsheet.Version) {
                throw new SpreadsheetReadWriteException("saved spreadsheet has a different version name");
            }
            try {
                using (XmlReader reader = CreateXmlReader(filename)) {
                    while (reader.Read()) {
                        if (reader.IsStartElement()) {
                            ProcessStartElement(spreadsheet, reader);
                        } else {
                            ProcessEndElement(reader);
                        }
                    }
                }
            } catch (Exception e) {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        private static void ProcessStartElement(Spreadsheet spreadsheet, XmlReader reader)
        {
            switch (reader.Name) {
                case "spreadsheet":
                    // just continue i guess
                    break;
                case "cell":
                    ReadCell(spreadsheet, reader);
                    break;
                default:
                    throw new SpreadsheetReadWriteException(string.Format("unexpected xml \"{0}\"", reader.Name));
            }
        }

        private static void ReadCell(Spreadsheet spreadsheet, XmlReader reader)
        {
            string cellName = null;
            string cellContents = null;
            ReadCellNameOrContents(reader, ref cellName, ref cellContents);
            ReadCellNameOrContents(reader, ref cellName, ref cellContents);
            CheckCellNameAndContents(cellName, cellContents);
            ReadEndOfCellTag(reader);
            try {
                spreadsheet.SetContentsOfCell(cellName, cellContents);
            } catch (InvalidNameException e) {
                throw new SpreadsheetReadWriteException(e.Message);
            } catch (FormulaFormatException e) {
                throw new SpreadsheetReadWriteException(e.Message);
            } catch (CircularException e) {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }

        private static void ReadCellNameOrContents(XmlReader reader, ref string cellName, ref string cellContents)
        {
            reader.Read();
            switch (reader.Name) {
                case "name":
                    cellName = reader.ReadString();
                    break;
                case "contents":
                    cellContents = reader.ReadString();
                    break;
            }
        }

        private static void CheckCellNameAndContents(string cellName, string cellContents)
        {
            if (string.IsNullOrEmpty(cellName) || string.IsNullOrEmpty(cellContents)) {
                throw new SpreadsheetReadWriteException("missing cell name and/or contents");
            }
        }

        private static void ReadEndOfCellTag(XmlReader reader)
        {
            reader.Read();
            if (!(reader.Name == "cell" && !reader.IsStartElement())) {
                throw new SpreadsheetReadWriteException("expected end of cell tag");
            }
        }

        private static void ProcessEndElement(XmlReader reader)
        {
            switch (reader.Name) {
                case "spreadsheet":
                    break;
                default:
                    throw new SpreadsheetReadWriteException(string.Format("unexpected xml \"{0}\"", reader.Name));
            }
        }


        public static string GetSpreadsheetFileSavedVersionName(string filename)
        {
            try {
                string foundVersionName = null;
                using (XmlReader reader = CreateXmlReader(filename)) {
                    if (reader.Read()) {
                        if (reader.IsStartElement()) {
                            if (reader.Name == "spreadsheet") {
                                foundVersionName = reader.GetAttribute("version");
                            } else {
                                throw new SpreadsheetReadWriteException("spreadsheet start tag not found");
                            }
                        }
                    }
                }
                if (foundVersionName == null) {
                    throw new SpreadsheetReadWriteException("version attribute not found");
                }
                return foundVersionName;
            } catch (Exception e) {
                throw new SpreadsheetReadWriteException(e.Message);
            }
        }


        private static XmlWriter CreateXmlWriter(string filename)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";    // 4 spaces
            return XmlWriter.Create(filename, settings);
        }

        private static XmlReader CreateXmlReader(string filename)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            return XmlReader.Create(filename, settings);
        }

    }
}