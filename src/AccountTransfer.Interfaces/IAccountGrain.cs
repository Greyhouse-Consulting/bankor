﻿using System.Threading.Tasks;
using Orleans;

namespace AccountTransfer.Interfaces
{
    public interface IAccountGrain : IGrainWithGuidKey
    {
        [Transaction(TransactionOption.Required)]
        Task Withdraw(uint amount);

        [Transaction(TransactionOption.Required)]
        Task Deposit(uint amount);

        Task<decimal> GetBalance();

        Task Owner(string userId);
    }
}
