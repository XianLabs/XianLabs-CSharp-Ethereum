using Org.BouncyCastle.Math;
using System;
using System.Data.SqlClient;
using System.Text;

namespace Scram
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
            public int Decimals;
           
            public double FloatAmount;

            public uint Amount;
            public double Gas;

            public string InitialTxHash;
            public bool InitialCompleted;
            public bool InitialFailed;

            public string EndingTxHash;
            public bool nCompleted;
            public bool nFailed;

            public int Hops;
        }

        private static string connStr = "Server=localhost\\; Data Source=WIN-XXXX\\SCRAMBLEZ;Initial Catalog=WS;Trusted_Connection=true";
        private SqlConnection sqlConn;

        public bool ConnectToDatabase()
        {    
            sqlConn = new SqlConnection(connStr);

            try
            {
                sqlConn.Open();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot open connection: " + ex);
                return false;
            }

            return true;
        }

        public bool SetMiddleWalletStatus(bool busy, string middleWallet)
        {
            if (!this.ConnectToDatabase())
            {
                Console.WriteLine("Couldn't connecto to DB @ EncodeRecord.");
                return false;
            }
            else
            {
                string SelectStr = "UPDATE MiddleWallets SET busy=" + Convert.ToInt32(busy) + " WHERE(pubKey='" + middleWallet + "')";

                SqlCommand command = new SqlCommand(SelectStr, this.sqlConn);
                int rows = command.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.WriteLine("Updated middle wallet busy status: " + middleWallet + " " + busy);
                    return true;
                }
                else if (rows <= 0)
                {
                    Console.WriteLine("Somehow did not find entry in db @ Encode");
                    return false;
                }
            }

            return false;
        }

        public ScramblerEntry GetEntryByID(int id)
        {
            ScramblerEntry SE = new ScramblerEntry();

            this.ConnectToDatabase();

            string SelectStr = "SELECT * FROM Entries WHERE (id=" + id + ");";
            SqlCommand command;
            SqlDataReader dataReader;

            command = new SqlCommand(SelectStr, this.sqlConn);
            dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read() != false)
                {
                    //todo, cleanup exception handling
                    try
                    {
                        SE.id = Convert.ToInt32(dataReader[0]);
                        SE.FromAddr = Convert.ToString(dataReader[2]);
                        SE.ToAddr = Convert.ToString(dataReader[3]);
                        SE.Amount = Convert.ToUInt32(dataReader[4]);
                        SE.FloatAmount = Convert.ToDouble(dataReader[4]);
                        SE.Gas = Convert.ToDouble(dataReader[12]);
                        SE.ContractAddress = Convert.ToString(dataReader[1]);
                        SE.InitialTxHash = Convert.ToString(dataReader[6]);
                        SE.EndingTxHash = Convert.ToString(dataReader[9]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to fetch data, possibly corrupt or no rows");
                        return null;
                    }

                    //check for success on initial
                    SE.nFailed = false;
                    SE.InitialCompleted = false;
                    SE.nCompleted = false;
                    SE.Decimals = Convert.ToInt32(dataReader[14]);
                    SE.Hops = Convert.ToInt32(dataReader[13]);
                }

                dataReader.Close();
                command.Dispose();
                this.sqlConn.Close();

                return SE;
            }

            this.sqlConn.Close();
            return null;
        }

        public ScramblerEntry GetOldestEntry()
        {
            ScramblerEntry SE = new ScramblerEntry();

            this.ConnectToDatabase();

            string SelectStr = "SELECT * FROM Entries WHERE (iTxComplete = 0 AND iTxFailed = 0 AND nTxComplete = 0) ORDER BY Time DESC;";
            SqlCommand command;
            SqlDataReader dataReader;

            command = new SqlCommand(SelectStr, this.sqlConn);
            dataReader = command.ExecuteReader();

            if(dataReader.HasRows)
            {
                while (dataReader.Read() != false)
                {
                    //todo, put set of tries into some nested form

                    //todo, cleanup exception handling
                    try
                    {
                        SE.id = Convert.ToInt32(dataReader[0]);
                        SE.FromAddr = Convert.ToString(dataReader[2]);
                        SE.ToAddr = Convert.ToString(dataReader[3]);
                        SE.Amount = Convert.ToUInt32(dataReader[4]);
                        SE.FloatAmount = Convert.ToDouble(dataReader[4]);
                        SE.Gas = Convert.ToDouble(dataReader[12]);
                        SE.ContractAddress = Convert.ToString(dataReader[1]);
                        SE.InitialTxHash = Convert.ToString(dataReader[6]);
                        SE.EndingTxHash = Convert.ToString(dataReader[9]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to fetch data, possibly corrupt or no rows");
                        return null;
                    }

                    //check for success on initial
                    SE.nFailed = false;
                    SE.InitialCompleted = false;
                    SE.nCompleted = false;
                    SE.Decimals = Convert.ToInt32(dataReader[14]);
                    SE.Hops = Convert.ToInt32(dataReader[13]);
                }
            }

            dataReader.Close();
            command.Dispose();
            this.sqlConn.Close();

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
                    //todo, cleanup exception handling
                    try
                    {
                        SE.id = Convert.ToInt32(dataReader[0]);
                        SE.FromAddr = Convert.ToString(dataReader[2]);
                        SE.ToAddr = Convert.ToString(dataReader[3]);
                        SE.Amount = Convert.ToUInt32(dataReader[4]);
                        SE.FloatAmount = Convert.ToDouble(dataReader[4]);
                        SE.Gas = Convert.ToDouble(dataReader[12]);
                        SE.ContractAddress = Convert.ToString(dataReader[1]);
                        SE.InitialTxHash = Convert.ToString(dataReader[6]);
                        SE.EndingTxHash = Convert.ToString(dataReader[9]);
                    }
                    catch
                    {
                        Console.WriteLine("Failed to fetch data, possibly corrupt or no rows");
                        return null;
                    }

                    //check for success on initial
                    SE.nFailed = false;
                    SE.InitialCompleted = false;
                    SE.nCompleted = false;
                    SE.Decimals = Convert.ToInt32(dataReader[14]);
                    SE.Hops = Convert.ToInt32(dataReader[13]);
                }
            }

            dataReader.Close();
            command.Dispose();
            this.sqlConn.Close();

            return SE;
        }

        public string GetIdleMiddleWallet()
        {
            this.ConnectToDatabase();

            string SelectStr = "SELECT * FROM MiddleWallets WHERE (busy = 0);";
            SqlCommand command;
            SqlDataReader dataReader;

            command = new SqlCommand(SelectStr, this.sqlConn);
            dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read() != false)
                {
                    string MiddleWallet = Convert.ToString(dataReader[1]);
                    this.sqlConn.Close();
                    return MiddleWallet;
                }
            }

            this.sqlConn.Close();
            return null;
        }

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public bool EncodeRecord(ScramblerEntry SE)
        {
            if (!this.ConnectToDatabase())
            {
                Console.WriteLine("Couldn't connecto to DB @ EncodeRecord.");
                return false;
            }
            else
            {
                const uint valueXor = 0x93377339;
                string md5_From = CreateMD5(SE.FromAddr);
                string md5_To = CreateMD5(SE.ToAddr);

                uint EncodedAmount = SE.Amount * valueXor;

                string md5_iTxID = CreateMD5(SE.InitialTxHash);
                string md5_nTxID = CreateMD5(SE.EndingTxHash);

                string SelectStr = "UPDATE Entries SET FromAddr='" + md5_From +
                    "',ToAddr='" + md5_To + 
                    "', iTxID ='" + md5_iTxID + 
                    "', nTxID = '" + md5_nTxID
                    + "' WHERE id = " + SE.id;

                SqlCommand command = new SqlCommand(SelectStr, this.sqlConn);
                int rows = command.ExecuteNonQuery();

                if (rows > 0)
                {
                    Console.WriteLine("Encoded record accordingly");
                    return true;
                }
                else if (rows <= 0)
                {
                    Console.WriteLine("Somehow did not find entry in db @ Encode");
                    return false;
                }

            }

            return true;
        }

        public bool AlterRecord(ScramblerEntry SE)
        {
            if(this.ConnectToDatabase())
            {
                string SelectStr = "UPDATE Entries SET iTxFailed=" + Convert.ToInt32(SE.InitialFailed) + ",iTxComplete=" + Convert.ToInt32(SE.InitialCompleted) + ", nTxFailed =" + Convert.ToInt32(SE.nFailed) + ", nTxComplete =" + Convert.ToInt32(SE.nCompleted) + ", nTxID = '" + SE.EndingTxHash + "', Hops=" + SE.Hops + " WHERE id = " + SE.id;
                
                SqlCommand command;

                command = new SqlCommand(SelectStr, this.sqlConn);
                int rows = command.ExecuteNonQuery();
            
                if (rows > 0)
                {
                    Console.WriteLine("Modified DB Accordingly");
                    this.sqlConn.Close();
                    return true;
                }
                else if (rows <= 0)
                {
                    Console.WriteLine("No rows to update..");
                    this.sqlConn.Close();
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
