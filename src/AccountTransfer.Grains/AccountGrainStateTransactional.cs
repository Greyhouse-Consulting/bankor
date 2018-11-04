using System;
using System.Collections.Generic;
using BankOr.Core;
using Orleans.CodeGeneration;


[assembly: GenerateSerializer(typeof(AccountTransfer.Grains.AccountGrainStateTransactional))]

namespace AccountTransfer.Grains
{
    [Serializable]
    public class AccountGrainStateTransactional
    {

        public AccountGrainStateTransactional()
        {
            Transactions = new List<Transaction>();
        }
        public decimal Balance { get; set; }

        public IList<Transaction> Transactions { get; set; }
    }
}