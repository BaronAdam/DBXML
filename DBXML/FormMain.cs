using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace DBXML
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private OracleConnection Connection { set; get; }
        private bool ConnectionEstablished { set; get; }

        public void Connect()
        {
            string login = textBoxLogin.Text;
            string password = textBoxPassword.Text;
            string ip = textBoxIP.Text;
            string port = textBoxPort.Text;
            string orcltp = textBoxorcltp.Text;
            string dataSource = ip + ":" + port + "/" + orcltp;

            bool connectionEstablished;
            OracleConnection connection;

            Program.DBConnect(login, password, dataSource, out connectionEstablished, out connection);

            if (connectionEstablished)
            {
                labelConnectionStatus.Text = "Nawiązano połączenie";
                labelConnectionStatus.ForeColor = Color.Green;
                ConnectionEstablished = true;
                Connection = connection;

                SetComboBox();
                PrepareControlls();
            }
            else
            {
                labelConnectionStatus.Text = "Błąd próby połączenia";
                labelConnectionStatus.ForeColor = Color.Red;
                ConnectionEstablished = false;
            }

            GC.Collect();
        }

        private void PrepareControlls()
        {
            comboBoxTableNames.SelectedIndex = 1;
            buttonCreateXMLFile.Enabled = true;
            buttonChooseXMLFile.Enabled = true;
        }

        private void SetComboBox()
        {
            var tableNames = SQL.GetTableNames(Connection);

            foreach (var tableName in tableNames)
            {
                comboBoxTableNames.Items.Add(tableName);
            }
        }

        private void buttonConnect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void buttonCreateXMLFile_Click(object sender, EventArgs e)
        {
            if (ConnectionEstablished)
            {
                string tableName = comboBoxTableNames.Text;
                List<string> columnNames = SQL.GetColumnNames(Connection, tableName);

                XML.PrepareAndCreateXMLFile(Connection, tableName, columnNames);
            }   
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ConnectionEstablished)
            {
                Connection.Close();
            }
        }

        private void buttonChooseXMLFile_Click(object sender, EventArgs e)
        {
            XmlDocument xmlDocument = XML.CheckIfValidXMLFile();
        }
    }
}
