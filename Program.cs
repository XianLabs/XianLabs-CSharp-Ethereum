using System;
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
                Gas = Convert.ToUInt32(args[3]);
            }
            catch
            {
                Console.WriteLine("invalid command line argument: Amount [2]");
                return;
            }
      
            ERC20Sender Manager = new ERC20Sender(FromAddr, "<privkey>", ToAddr, Amount, Gas);

            Thread QueryThread = new Thread(Manager.QueryForSubmissions);
            QueryThread.Start();
        }
    }
}
