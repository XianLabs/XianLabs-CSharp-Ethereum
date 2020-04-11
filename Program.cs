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

namespace Nethereum.CQS.SimpleTokenTransfer
{
    public class Program
    {
        static void Main(string[] args) //use args/cmd line to get recv address + time + amount, add to db
        {

            string FromAddr = args[0];
            string ToAddr = args[1];
            uint Amount = 0;

            try
            {
                Amount = Convert.ToUInt32(args[2]);
            }
            catch
            {
                Console.WriteLine("invalid command line argument: Amount [2]");
                return;
            }

            DBManager DB = new DBManager();
      
            ScramblerEntry SE = DB.GetOldestEntry();

            if(SE.FromAddr == String.Empty)
            {
                Console.WriteLine("Couldn't fetch DB entry!");
            }

            Console.WriteLine(SE.FromAddr + " " + SE.ToAddr + " " + SE.Amount + " " + SE.Gas);


            ERC20Sender Manager = new ERC20Sender(FromAddr, "GoAwayFromMyKeySour", ToAddr, Amount);

            //todo: get complete transaction before sending off to next wallet
            Console.WriteLine("Checking the balance");
            Manager.BalanceAsync().Wait();

            //Console.WriteLine("Transfering...");
            Manager.TransferAsync(ToAddr, Amount).Wait();
            Console.ReadLine();
        }
    }
}
