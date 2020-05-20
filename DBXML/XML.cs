using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace DBXML
{
    static class XML
    {
        public static void PrepareAndCreateXMLFile(OracleConnection con, string tableName, List<string> columnNames)
        {
            int noOfColumns = columnNames.Count();

            var data = SQL.GetAllDataFromTable(con, tableName, noOfColumns);

            string path = "";

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = tableName;
                saveFileDialog.Filter = "XML File | *.xml";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    path = saveFileDialog.FileName;
                }
            }

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "    "
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(path, settings))
            {
                xmlWriter.WriteStartDocument();

                xmlWriter.WriteStartElement("table");

                xmlWriter.WriteStartElement("table-name");
                xmlWriter.WriteString(tableName);
                xmlWriter.WriteEndElement();

                foreach (var row in data)
                {
                    xmlWriter.WriteStartElement("row");

                    for (int i = 0; i < noOfColumns; i++)
                    {
                        xmlWriter.WriteStartElement(columnNames[i].ToLower());
                        xmlWriter.WriteString(row[i].ToString());
                        xmlWriter.WriteEndElement();
                    }

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();

                xmlWriter.WriteEndDocument();
            }                    
        }

        public static XmlDocument CheckIfValidXMLFile(OracleConnection con, out string fileName)
        {
            XmlDocument xmlDocument = OpenXMLFile(out fileName);
            var tableNames = SQL.GetTableNames(con);

            string tableName = "";

            if (CheckIfXMLDocumentIsEmpty(xmlDocument))
            {
                return new XmlDocument();
            }
            
            try
            {
                tableName = xmlDocument.SelectSingleNode("/table/table-name").InnerText;
            }
            catch (Exception) { }

            if (!CheckIfIsValidTableName(tableName, tableNames))
            {
                return new XmlDocument();
            }

            var columnNames = SQL.GetColumnNames(con, tableName);

            foreach (var name in columnNames)
            {
                if (xmlDocument.SelectSingleNode("/table/row/" + name.ToLower()) == null)
                {
                    return new XmlDocument();
                }
            }

            int noOfColumns = columnNames.Count();

            XmlNodeList allRows = xmlDocument.SelectNodes("/table/row");

            foreach (XmlNode row in allRows)
            {
                if (row.ChildNodes.Count != noOfColumns)
                {
                    return new XmlDocument();
                }
            }

            return xmlDocument;
        }

        public static XmlDocument OpenXMLFile(out string fileName)
        {
            fileName = "";

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "XML File | *.xml";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = openFileDialog.FileName;

                }
            }

            XmlDocument xmlDocument = new XmlDocument();

            try
            {
                xmlDocument.Load(fileName);
            }
            catch (Exception) { }
            
            return xmlDocument;
        }

        private static bool CheckIfIsValidTableName(string tableName, List<string> tableNames)
        {
            foreach (var name in tableNames)
            {
                if (name == tableName)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckIfXMLDocumentIsEmpty(XmlDocument xmlDocument)
        {
            if (xmlDocument.ChildNodes.Count <= 1)
            {
                return true;
            }
            return false;
        }

        public static List<List<string>> GetDataFromXML(XmlDocument xmlDocument, OracleConnection con)
        {
            string tableName = xmlDocument.SelectSingleNode("/table/table-name").InnerText;

            List<List<string>> data = new List<List<string>>();

            List<string> tabName = new List<string>();
            tabName.Add(tableName);
            data.Add(tabName);

            string patternDate = @"[0-9]{2}\.[0-9]{2}\.[0-9]{4}";

            string patternString = @"[a-zA-Z]+";

            var columnNames = SQL.GetColumnNames(con, tableName);

            XmlNodeList allRows = xmlDocument.SelectNodes("/table/row");

            foreach (XmlNode row in allRows)
            {
                List<string> dataRow = new List<string>();

                foreach (var columnName in columnNames)
                {
                    string path = columnName.ToLower();
                    string dataCell = row.SelectSingleNode(path).InnerText;

                    if (Regex.IsMatch(dataCell, patternDate))
                    {
                        dataCell = dataCell.Replace('.', '/');
                        dataCell = "TO_DATE('" + dataCell + "', 'DD/MM/YYYY HH24:MI:SS')";
                    }

                    if (Regex.IsMatch(dataCell, patternString))
                    {
                        dataCell = "'" + dataCell + "'";
                    }

                    if (dataCell == String.Empty)
                    {
                        dataCell = "null";
                    }

                    dataRow.Add(dataCell);
                }

                data.Add(dataRow);
            }

            return data;
        }
    }
}
