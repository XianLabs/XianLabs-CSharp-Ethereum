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
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using NethTest;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.Transactions;
using Nethereum.RPC;
using System.Reflection;
using System.Net.Http;
using Newtonsoft.Json;
using Nethereum.RPC.Infrastructure;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.Filters;
using static NethTest.DBManager;
using System.Threading;

namespace NethTest
{
    public class ERC20Sender
    {
        //This is the contract address of an already deployed smartcontract in the Mainnet
        private static string ContractAddress { get; set; } = "CONTRACT"; //stays static for CHIMERA

        private string FromAddr { get; set; }
        private string FromPrivKey { get; set; }
        private string ToAddr { get; set; }
        private uint Amount { get; set; }

        private uint Gas { get; set; }

        private DBManager DBM { get; set; }

        public ERC20Sender(string From, string FromPvtKey, string To, uint Qty, uint Gas)
        {
            this.FromAddr = From;
            this.FromPrivKey = FromPvtKey;
            this.ToAddr = To;
            this.Amount = Qty;
            this.Gas = Gas;
        }

        public void QueryForSubmissions()
        {
            DBM = new DBManager();
            bool isLooping = true;
            ScramblerEntry SE = null;

            while (isLooping == true)
            {
                try
                {
                    SE = DBM.GetOldestEntry();
                    
                    if (SE == null)
                    {
                        Console.WriteLine("No current inputs...");
                        continue;
                    }
                    else
                    {
                        Console.WriteLine(SE.FromAddr + " " + SE.ToAddr + " " + SE.Amount);


                        //check if initial TX was successful

                        //if successful, check the current balance, verify it's larger or equal to amount recv'd

                        //send out payment to other middle wallet or end user

                        //verify the end transaction succeeded


                    }
                }
                catch
                {
                    Console.WriteLine("Failed to fetch SE or no current inputs...");
                }

                Thread.Sleep(3000);
            }
        }


        public async Task BalanceAsync()
        {
            //Replace with your own
            var senderAddress = this.FromAddr;
            var contractAddress = ContractAddress;
            // Note: in this sample, a special INFURA API key is used
            var url = "https://mainnet.infura.io/v3/b2a3829b4de548e38f1f6c79b3d447b9";

            //no private key we are not signing anything (read only mode)
            var web3 = new Web3(url);

            var balanceOfFunctionMessage = new BalanceOfFunction()
            {
                Owner = senderAddress,
            };


            var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
            var balance = await balanceHandler.QueryAsync<BigInteger>(contractAddress, balanceOfFunctionMessage);
     
            Console.WriteLine("Balance of token: " + balance);
        }

        public async Task TransferAsync(string to, uint amount)
        {
            //Replace with your own
            var senderAddress = this.FromAddr;
            var privatekey = this.FromPrivKey;

            // Note: in this sample, a special INFURA API key is used
            var url = "https://mainnet.infura.io/v3/b2a3829b4de548e38f1f6c79b3d447b9";

            var web3 = new Web3(new Account(privatekey), url); //creates valid/sign account

            var transactionMessage = new TransferFunction()
            {
                FromAddress = senderAddress,
                To = to,
                TokenAmount = amount,
                //Set our own price
                Gas = 1000000,

            };

            AccountTransactionSigningInterceptor a = new AccountTransactionSigningInterceptor(privatekey, web3.Client);
          
            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
            var transactionHash = await transferHandler.SendRequestAsync(ContractAddress, transactionMessage);
            Console.WriteLine("Transfer txHash: " + transactionHash);
            
        }


        public bool DidTransactionSucceed(string txid)
        {
            // Note: in this sample, a special INFURA API key is used
            var url = "https://mainnet.infura.io/v3/b2a3829b4de548e38f1f6c79b3d447b9";

            var web3 = new Web3(new Account(this.FromPrivKey), url); //creates valid/sign account

            var rcpt = web3.TransactionManager.TransactionReceiptService.PollForReceiptAsync(txid).Result;

            Console.WriteLine("Success: " + rcpt.Succeeded());

            return rcpt.Succeeded();
        }

        internal async Task<ulong> GetTransactionAmount(string txid)
        {
            // Note: in this sample, a special INFURA API key is used
            var url = "https://mainnet.infura.io/v3/b2a3829b4de548e38f1f6c79b3d447b9";

            var web3 = new Web3(new Account(this.FromPrivKey), url); //creates valid/sign account
            var result = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txid);

            string newInput = result.Input.Remove(0, 2); //remove '0x'

            string strVal = newInput.Substring(104, 32);
            ulong amountSent = 0;

            try
            {
                amountSent = Convert.ToUInt64(strVal, 16);

            }
            catch
            {
                Console.WriteLine("Could not convert sent amount in txid: " + txid);
            }

            Console.WriteLine("Amount sent: " + amountSent);

            var rcpt = web3.TransactionManager.TransactionReceiptService.PollForReceiptAsync(txid).Result;

            Console.WriteLine("Success: " + rcpt.Succeeded());

            return amountSent;
        }

        /// <summary>
        /// Attempt to get the raw hex directly from the geth node
        /// </summary>
        /// <param name="txid"></param>
        /// <returns></returns>
        internal async Task<string> GetTransaction(string txid)
        {
            // Note: in this sample, a special INFURA API key is used
            var url = "https://mainnet.infura.io/v3/b2a3829b4de548e38f1f6c79b3d447b9";

            var web3 = new Web3(new Account(this.FromPrivKey), url); //creates valid/sign account
            var result = await web3.Eth.Transactions.GetTransactionByHash.SendRequestAsync(txid);
           
            
            string newInput = result.Input.Remove(0, 2); //remove '0x'

            string strVal = newInput.Substring(104, 32);
            ulong amountSent = 0;

            try
            {
                amountSent = Convert.ToUInt64(strVal, 16);
            }
            catch
            {
                Console.WriteLine("Could not convert sent amount in txid: " + txid);
            }

            Console.WriteLine("From: " + result.From + "  " + "To: " + result.To + " Gas: " + result.Gas + " Amount sent: " + amountSent);

            var rcpt = web3.TransactionManager.TransactionReceiptService.PollForReceiptAsync(txid).Result;

            Console.WriteLine("Success: " + rcpt.Succeeded());

            return result.Input;
        }

        [Function("transfer", "bool")]
        public class TransferFunction : FunctionMessage
        {
            [Parameter("address", "_to", 1)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 2)]
            public BigInteger TokenAmount { get; set; }
        }

        [Function("balanceOf", "uint256")]
        public class BalanceOfFunction : FunctionMessage
        {

            [Parameter("address", "_owner", 1)]
            public string Owner { get; set; }

        }
    }
}
