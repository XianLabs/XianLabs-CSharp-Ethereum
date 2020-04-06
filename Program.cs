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

namespace Nethereum.CQS.SimpleTokenTransfer
{
    public class Program
    {
        static void Main(string[] args) //use args/cmd line to get recv address + time + amount, add to db
        {        
            string FromAddr = args[0];
            string ToAddr = args[1];
            int Amount = Convert.ToInt32(args[2]);

            ERC20Sender Manager = new ERC20Sender(FromAddr, "key",  ToAddr, Amount);

            Console.WriteLine("Checking the balance");
            Manager.BalanceAsync().Wait();
            Console.WriteLine("Transfering...");
            Manager.TransferAsync().Wait();
            Console.ReadLine();
        }
    }
}
