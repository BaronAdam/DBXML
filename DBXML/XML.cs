using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        public static XmlDocument CheckIfValidXMLFile()
        {
            XmlDocument xmlDocument = OpenXMLFile();

            string 
        }

        public static XmlDocument OpenXMLFile()
        {
            string fileName = "";

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
            catch (Exception)
            {

            }
            
            return xmlDocument;
        }
    }
}
