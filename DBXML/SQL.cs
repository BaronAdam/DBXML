using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBXML
{
    static class SQL
    {
        public static List<string> GetTableNames(OracleConnection con)
        {
            string sql = "select table_name from tabs";
            OracleCommand cmd = new OracleCommand(sql, con);

            List<string> tableNames = new List<string>();

            using (OracleDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    tableNames.Add(dr.GetString(0));
                }
            }

            return tableNames;
        }
        
        public static List<string> GetColumnNames(OracleConnection con, string tableName)
        {
            string sql = "SELECT column_name FROM all_tab_cols WHERE table_name = '" + tableName + "'";
            OracleCommand cmd = new OracleCommand(sql, con);

            List<string> columnNames = new List<string>();

            using (OracleDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    columnNames.Add(dr.GetString(0));
                }
            }

            return columnNames;
        }

        public static List<ArrayList> GetAllDataFromTable(OracleConnection con, string tableName, int noOfColumns)
        {
            string sql = "SELECT * FROM " + tableName;
            OracleCommand cmd = new OracleCommand(sql, con);

            List<ArrayList> data = new List<ArrayList>();

            using (OracleDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    ArrayList arrayList = new ArrayList();

                    for (int i = 0; i < noOfColumns; i++)
                    {
                        arrayList.Add(dr.GetValue(i));
                    }

                    data.Add(arrayList);
                }
            }

            return data;
        }

        public static void SendData(OracleConnection con, List<List<string>> data)
        {
            string tableName = data[0][0];

            List<List<string>> localData = new List<List<string>>(data);

            localData.RemoveAt(0);
            try
            {
                foreach (var row in localData)
                {
                    StringBuilder sql = new StringBuilder("INSERT INTO ");
                    sql.Append(tableName.ToLower());
                    sql.Append(" VALUES (");
                    int total = row.Count();
                    for (int i = 0; i < total; i++)
                    {
                        sql.Append(row[i]);
                        if (i != total - 1)
                        {
                            sql.Append(", ");
                        }
                    }

                    sql.Append(")");

                    OracleCommand cmd = new OracleCommand(sql.ToString(), con);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Błąd");
            }
        }
    }
}
