/*
 *  CHIMERA SCRAMBLER
 *  STARTED APRIL 6 2020
 *  COPYRIGHT CHIMERA DIGITAL INCORPORATED
 */

using System;
using System.Threading;

namespace NethTest
{
    public class Program
    {
        static void Main(string[] args) 
        {
            if(args.Length >= 1)
            {
                if(args[0] == "/quicksend")
                {
                    ERC20Sender QuickManager = new ERC20Sender("P", "K");
                    QuickManager.GetAndSendIDSubmission(Convert.ToInt32(args[1]));
                }
            }
            else
            {
                //looping variant
                ERC20Sender Manager = new ERC20Sender("P", "K");
                Thread QueryThread = new Thread(Manager.QueryForSubmissions);
                QueryThread.Start();
            }
        }
    }
}
