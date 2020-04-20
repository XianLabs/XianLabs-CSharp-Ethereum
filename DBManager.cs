using System;
using System.Data.SqlClient;

namespace NethTest
{
    public class DBManager
    {
        public class ScramblerEntry
        {
            public int id;

            public string FromAddr;
            public string ToAddr;
            
            public string ContractAddress;
            public string TokenSymbol;

            public uint Amount;
            public double Gas;

            public string InitialTxHash;
            public bool InitialCompleted;
            public bool InitialFailed;

            public string EndingTxHash;
            public bool nCompleted;
            public bool nFailed;
        }

        private static string connStr = "Server=localhost\\SQLEXPRESS; Data Source=ALEX-PC\\CRYPTOTRACK;Initial Catalog=WalletScrambler;Trusted_Connection=true";
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

            string SelectStr = "SELECT * FROM Entries WHERE (iTxComplete = 1 AND iTxFailed = 0 AND nTxComplete = 0) ORDER BY Time DESC;";
            SqlCommand command;
            SqlDataReader dataReader;

            command = new SqlCommand(SelectStr, this.sqlConn);
            dataReader = command.ExecuteReader();

            if(dataReader.HasRows)
            {
                while (dataReader.Read() != false)
                {
                    //todo, put set of tries into some nested form

                    SE.id = Convert.ToInt32(dataReader[0]);
                    SE.FromAddr = Convert.ToString(dataReader[2]);
                    SE.ToAddr = Convert.ToString(dataReader[3]);

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
                        SE.Gas = Convert.ToDouble(dataReader[12]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to fetch SE.Amount (uint32) - no rows?");
                        return null;
                    }

                    try
                    {
                        SE.ContractAddress = Convert.ToString(dataReader[1]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to get contract address from: " + SE.FromAddr);
                    }

                    try
                    {
                        SE.InitialTxHash = Convert.ToString(dataReader[6]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to get iTxHash address from: " + SE.FromAddr);
                    }
                    
                    try
                    {
                        SE.EndingTxHash = Convert.ToString(dataReader[9]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to get iTxHash address from: " + SE.FromAddr);
                    }
                    //check for success on initial
                    SE.nFailed = false;
                    SE.InitialCompleted = false;
                    SE.nCompleted = false;         
                    
                }
            }

            dataReader.Close();
            command.Dispose();
            this.sqlConn.Close();
            Console.WriteLine("Closing connection...");

            return SE;
        }

        public ScramblerEntry GetUnsentEntry()
        {
            ScramblerEntry SE = new ScramblerEntry();

            this.ConnectToDatabase();

            string SelectStr = "SELECT * FROM Entries WHERE (iTxComplete = 0 AND iTxFailed = 0 AND nTxComplete = 0) ORDER BY Time DESC;";
            SqlCommand command;
            SqlDataReader dataReader;

            command = new SqlCommand(SelectStr, this.sqlConn);
            dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read() != false)
                {
                    //todo, put set of tries into some nested form

                    SE.id = Convert.ToInt32(dataReader[0]);
                    SE.FromAddr = Convert.ToString(dataReader[2]);
                    SE.ToAddr = Convert.ToString(dataReader[3]);

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
                        SE.Gas = Convert.ToDouble(dataReader[12]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to fetch SE.Amount (uint32) - no rows?");
                        return null;
                    }

                    try
                    {
                        SE.ContractAddress = Convert.ToString(dataReader[1]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to get contract address from: " + SE.FromAddr);
                    }

                    try
                    {
                        SE.InitialTxHash = Convert.ToString(dataReader[6]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to get iTxHash address from: " + SE.FromAddr);
                    }

                    try
                    {
                        SE.EndingTxHash = Convert.ToString(dataReader[9]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to get iTxHash address from: " + SE.FromAddr);
                    }
                    //check for success on initial
                    SE.nFailed = false;
                    SE.InitialCompleted = false;
                    SE.nCompleted = false;

                }
            }

            dataReader.Close();
            command.Dispose();
            this.sqlConn.Close();
            Console.WriteLine("Closing connection...");

            return SE;
        }

        //clean this shit up
        public bool AlterRecord(ScramblerEntry SE)
        {
            if(this.ConnectToDatabase())
            {
                string SelectStr = "UPDATE Entries SET iTxFailed=" + Convert.ToInt32(SE.InitialFailed) + ",iTxComplete=" + Convert.ToInt32(SE.InitialCompleted) + ", nTxFailed =" + Convert.ToInt32(SE.nFailed) + ", nTxComplete =" + Convert.ToInt32(SE.nCompleted) + ", nTxID = '" + SE.EndingTxHash + "' WHERE id = " + SE.id;
                
                SqlCommand command;

                command = new SqlCommand(SelectStr, this.sqlConn);
                int rows = command.ExecuteNonQuery();
            
                if (rows > 0)
                {
                    Console.WriteLine("Modified DB Accordingly");
                    return true;
                }
                else if (rows <= 0)
                {
                    Console.WriteLine("Somehow did not find entry in db anymore... under attack or code error.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Failed to connect to db in AlterRecord!");
                return false;
            }

            return false;
        }
    }
}

