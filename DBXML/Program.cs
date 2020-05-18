using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBXML
{
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
            Application.Run(new FormMain());
        }

        public static void DBConnect(string userName, string password, string dataSource,
            out bool connectionStatus, out OracleConnection connection)
        {
            OracleConnection con = new OracleConnection();

            OracleConnectionStringBuilder ocsb = new OracleConnectionStringBuilder();
            ocsb.Password = password;
            ocsb.UserID = userName;
            ocsb.DataSource = dataSource;

            con.ConnectionString = ocsb.ConnectionString;
            try
            {
                con.Open();
                connection = con;
                connectionStatus = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd");
                connection = con;
                connectionStatus = false;
            }
        }

    }
    
}
