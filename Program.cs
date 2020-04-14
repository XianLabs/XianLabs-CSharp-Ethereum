using System;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts.CQS;
using Nethereum.Util;
using Nethereum.Web3.Accounts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Contracts;
using Nethereum.Web3.Accounts.Managed;
using NethTest;
using static NethTest.DBManager;
using Nethereum.JsonRpc.Client;
using System.Threading;

namespace NethTest
{
    public class Program
    {
        static void Main(string[] args) //use args/cmd line to get recv address + time + amount, add to db
        {

            string FromAddr = args[0];
            string ToAddr = args[1];
            uint Amount = 0;
            uint Gas = 0;

            try
            {
                Amount = Convert.ToUInt32(args[2]);
            }
            catch
            {
                Console.WriteLine("invalid command line argument: Amount [2]");
                return;
            }
      
            try
            {
                Gas = Convert.ToUInt32(args[3]);
            }
            catch
            {
                Console.WriteLine("invalid command line argument: Gas [3]");
                return;         
            }
            
            ERC20Sender Manager = new ERC20Sender(FromAddr, args[4], ToAddr, Amount, Gas);

            Thread QueryThread = new Thread(Manager.QueryForSubmissions);
            QueryThread.Start();
        }
    }
}
