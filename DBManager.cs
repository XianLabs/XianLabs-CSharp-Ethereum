using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NethTest
{
    public class DBManager
    {
        public class ScramblerEntry
        {
            public string ToAddr;
            public string FromAddr;
            public uint Amount;
        }

        private static string connStr = "connetionString=Data Source=localhost\\SQLEXPRESS;Initial Catalog=WalletScrambler;";
        private SqlConnection sqlConn;

        public bool ConnectToDatabase()
        {
            sqlConn = new SqlConnection(connStr);
            try
            {
                sqlConn.Open();
                Console.WriteLine("Connection Open!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can not open connection: " + ex);
                return false;
            }

            return true;
        }

        public ScramblerEntry GetOldestEntry()
        {
            ScramblerEntry SE = new ScramblerEntry();

            this.ConnectToDatabase();

            string SelectStr = "SELECT 1 FROM Submissions ORDER BY time DESC;";
            SqlCommand command;
            SqlDataReader dataReader;

            command = new SqlCommand(SelectStr, this.sqlConn);
            dataReader = command.ExecuteReader();

            if(dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    Console.WriteLine(dataReader.GetValue(0) + " - " + dataReader.GetValue(1) + " - " + dataReader.GetValue(2));
                    SE.FromAddr = dataReader.GetValue(0).ToString();
                    SE.ToAddr = dataReader.GetValue(1).ToString();
                    try
                    {
                        SE.Amount = Convert.ToUInt32(dataReader.GetValue(2));
                    }
                    catch
                    {
                        Console.WriteLine("Failed to fetch SE.Amount (uint32) - no rows?");
                        return null;
                    }
                }
            }

            dataReader.Close();
            command.Dispose();
            this.sqlConn.Close();

            return SE;
        }

    }
}
