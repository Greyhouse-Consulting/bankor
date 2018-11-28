using System;
using System.Collections.Generic;
using Bancor.Core.Grains;
using Orleans.CodeGeneration;

[assembly: GenerateSerializer(typeof(AccountGrainStateTransactional))]

namespace Bancor.Core.Grains
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