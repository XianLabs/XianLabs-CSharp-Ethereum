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

namespace Nethereum.CQS.SimpleTokenTransfer
{
    public class Program
    {
        static void Main(string[] args) //use args/cmd line to get recv address + time + amount, add to db
        {

            Console.WriteLine("Checking the balance");
            BalanceAsync().Wait();
            Console.ReadLine();
            Console.WriteLine("Transfering...");
            TransferAsync().Wait();
            Console.ReadLine();
        }

        //This is the contract address of an already deployed smartcontract in the Mainnet
        private static string ContractAddress { get; set; } = "0x6719c6d781ca9a87eef9ca6959431a7da5f5301a";

        //The smart contract deployment message (command) including the Byte code (compiled solidity smart contract) and the Constructor parameters.
        public class StandardTokenDeployment : ContractDeploymentMessage
        {
            public static string BYTECODE = "0x608060405234801561001057600080fd5b50600436106100ea5760003560e01c806395d89b411161008c578063b5931f7c11610066578063b5931f7c1461044b578063d05c78da14610497578063dd62ed3e146104e3578063e6cb90131461055b576100ea565b806395d89b4114610316578063a293d1e814610399578063a9059cbb146103e5576100ea565b806323b872dd116100c857806323b872dd146101f6578063313ce5671461027c5780633eaaf86b146102a057806370a08231146102be576100ea565b806306fdde03146100ef578063095ea7b31461017257806318160ddd146101d8575b600080fd5b6100f76105a7565b6040518080602001828103825283818151815260200191508051906020019080838360005b8381101561013757808201518184015260208101905061011c565b50505050905090810190601f1680156101645780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b6101be6004803603604081101561018857600080fd5b81019080803573ffffffffffffffffffffffffffffffffffffffff16906020019092919080359060200190929190505050610645565b604051808215151515815260200191505060405180910390f35b6101e06106d2565b6040518082815260200191505060405180910390f35b6102626004803603606081101561020c57600080fd5b81019080803573ffffffffffffffffffffffffffffffffffffffff169060200190929190803573ffffffffffffffffffffffffffffffffffffffff1690602001909291908035906020019092919050505061071d565b604051808215151515815260200191505060405180910390f35b610284610948565b604051808260ff1660ff16815260200191505060405180910390f35b6102a861095b565b6040518082815260200191505060405180910390f35b610300600480360360208110156102d457600080fd5b81019080803573ffffffffffffffffffffffffffffffffffffffff169060200190929190505050610961565b6040518082815260200191505060405180910390f35b61031e6109aa565b6040518080602001828103825283818151815260200191508051906020019080838360005b8381101561035e578082015181840152602081019050610343565b50505050905090810190601f16801561038b5780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b6103cf600480360360408110156103af57600080fd5b810190808035906020019092919080359060200190929190505050610a48565b6040518082815260200191505060405180910390f35b610431600480360360408110156103fb57600080fd5b81019080803573ffffffffffffffffffffffffffffffffffffffff16906020019092919080359060200190929190505050610a62565b604051808215151515815260200191505060405180910390f35b6104816004803603604081101561046157600080fd5b810190808035906020019092919080359060200190929190505050610b86565b6040518082815260200191505060405180910390f35b6104cd600480360360408110156104ad57600080fd5b810190808035906020019092919080359060200190929190505050610ba6565b6040518082815260200191505060405180910390f35b610545600480360360408110156104f957600080fd5b81019080803573ffffffffffffffffffffffffffffffffffffffff169060200190929190803573ffffffffffffffffffffffffffffffffffffffff169060200190929190505050610bd3565b6040518082815260200191505060405180910390f35b6105916004803603604081101561057157600080fd5b810190808035906020019092919080359060200190929190505050610c5a565b6040518082815260200191505060405180910390f35b60008054600181600116156101000203166002900480601f01602080910402602001604051908101604052809291908181526020018280546001816001161561010002031660029004801561063d5780601f106106125761010080835404028352916020019161063d565b820191906000526020600020905b81548152906001019060200180831161062057829003601f168201915b505050505081565b600081600560003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020819055506001905092915050565b6000600460008073ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205460035403905090565b6000610768600460008673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205483610a48565b600460008673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002081905550610831600560008673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205483610a48565b600560008673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020819055506108fa600460008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205483610c5a565b600460008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002081905550600190509392505050565b600260009054906101000a900460ff1681565b60035481565b6000600460008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020549050919050565b60018054600181600116156101000203166002900480601f016020809104026020016040519081016040528092919081815260200182805460018160011615610100020316600290048015610a405780601f10610a1557610100808354040283529160200191610a40565b820191906000526020600020905b815481529060010190602001808311610a2357829003601f168201915b505050505081565b600082821115610a5757600080fd5b818303905092915050565b6000610aad600460003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205483610a48565b600460003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002081905550610b39600460008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205483610c5a565b600460008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020819055506001905092915050565b6000808211610b9457600080fd5b818381610b9d57fe5b04905092915050565b600081830290506000831480610bc4575081838281610bc157fe5b04145b610bcd57600080fd5b92915050565b6000600560008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002054905092915050565b6000818301905082811015610c6e57600080fd5b9291505056fea265627a7a72315820ae4eff2b90b2c9bda8b6508392eecc99b40a6d9a211340d64d5e142de2fff10664736f6c63430005110032";

            public StandardTokenDeployment() : base(BYTECODE)
            {

            }

            [Parameter("uint256", "totalSupply")]
            public BigInteger TotalSupply { get; set; }
        }

        //The Deployment of the Standard Token smart contract
       
        public static async Task BalanceAsync()
        {
            //Replace with your own
            var senderAddress = "<send addr>"; 
            var contractAddress = ContractAddress;
            // Note: in this sample, a special INFURA API key is used
            var url = "https://mainnet.infura.io/v3/b2a3829b4de548e38f1f6c79b3d447b9";

            //no private key we are not signing anything (read only mode)
            var web3 = new Web3.Web3(url);

            var balanceOfFunctionMessage = new BalanceOfFunction()
            {
                Owner = senderAddress,
            };

            var balanceHandler = web3.Eth.GetContractQueryHandler<BalanceOfFunction>();
            var balance = await balanceHandler.QueryAsync<BigInteger>(contractAddress, balanceOfFunctionMessage);


            Console.WriteLine("Balance of token: " + balance);

        }

        [Function("balanceOf", "uint256")]
        public class BalanceOfFunction : FunctionMessage
        {

            [Parameter("address", "_owner", 1)]
            public string Owner { get; set; }

        }

        public static async Task TransferAsync()
        {
            //Replace with your own
            var senderAddress = "<send wallet>";
            var receiverAddress = "<recv wallet>";
            var privatekey = "<privkey>";

            // Note: in this sample, a special INFURA API key is used
            var url = "https://mainnet.infura.io/v3/b2a3829b4de548e38f1f6c79b3d447b9";

            var web3 = new Web3.Web3(new Account(privatekey), url); //creates valid/sign account

            var transactionMessage = new TransferFunction()
            {
                FromAddress = senderAddress,
                To = receiverAddress,
                TokenAmount = 100,
                //Set our own price
                Gas = 1000000,
                
            };

            AccountTransactionSigningInterceptor a = new AccountTransactionSigningInterceptor(privatekey, web3.Client);

            var transferHandler = web3.Eth.GetContractTransactionHandler<TransferFunction>();
            var transactionHash = await transferHandler.SendRequestAsync(ContractAddress, transactionMessage);
            Console.WriteLine(transactionHash);
        }


        [Function("transfer", "bool")]
        public class TransferFunction : FunctionMessage
        {
            [Parameter("address", "_to", 1)]
            public string To { get; set; }

            [Parameter("uint256", "_value", 2)]
            public BigInteger TokenAmount { get; set; }
        }
    }
}
