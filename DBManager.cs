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
            public double Gas;
            
            public string initialTxHash;
            public bool initalCompleted;
            
            public string endingTxHash;
            public bool transferCompleted;
        }

        private static string connStr = "yourConn";
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

            string SelectStr = "SELECT * FROM Scramble WHERE (completed = 0 AND txComplete = 0) ORDER BY time DESC;";
            SqlCommand command;
            SqlDataReader dataReader;

            command = new SqlCommand(SelectStr, this.sqlConn);
            dataReader = command.ExecuteReader();

            if(dataReader.HasRows)
            {
                while (dataReader.Read() != false)
                {
                    SE.FromAddr = Convert.ToString(dataReader[2]);
                    SE.ToAddr = dataReader[3].ToString();

                    try
                    {
                        SE.Amount = Convert.ToUInt32(dataReader[4]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to fetch SE.Amount (uint32) - no rows?");
                        return null;
                    }
                    try
                    {
                        SE.Gas = Convert.ToDouble(dataReader[7]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to fetch SE.Amount (uint32) - no rows?");
                        return null;
                    }

                    SE.initialTxHash = Convert.ToString(dataReader[9]);
                }
            }

            dataReader.Close();
            command.Dispose();
            this.sqlConn.Close();
            Console.WriteLine("Closing connection...");

            return SE;
        }

    }
}

