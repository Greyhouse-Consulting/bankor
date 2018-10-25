using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;
using Orleans.CodeGeneration;
using Orleans.Transactions.Abstractions;
using AccountTransfer.Interfaces;
using BankOr.Core;
using Orleans.Providers;

[assembly: GenerateSerializer(typeof(AccountTransfer.Grains.AccountGrainState))]

namespace AccountTransfer.Grains
{
    [Serializable]
    public class AccountGrainState
    {
        public decimal Balance { get; set; }

        public IList<Transaction> Transactions { get; set; }
    }

    [StorageProvider(ProviderName = "BankOrStorageProvider")]
    public class AccountGrain : Grain<AccountGrainState>, IAccountGrain
    {
        private readonly ITransactionalState<AccountGrainState> _transactionalState;
        private string _name;

        public AccountGrain(
            [TransactionalState("transactionalState")] ITransactionalState<AccountGrainState> balance)
        {
            _transactionalState = balance ?? throw new ArgumentNullException(nameof(balance));
        }

        public async Task Deposit(uint amount)
        {
            await _transactionalState.PerformUpdate(x => x.Balance += amount);
        }

        public async Task Withdraw(uint amount)
        {
            await _transactionalState.PerformUpdate(x => x.Balance - amount);
        }

        public async Task<decimal> GetBalance()
        {
            return await _transactionalState.PerformRead(x => x.Balance);
        }

        public async Task Owner(string userId)
        {
            await Task.CompletedTask;
        }

        public Task SetName(string name)
        {
            _name = name;


            return Task.CompletedTask;
        }

        public Task<string> GetName()
        {
            return Task.FromResult(_name);
        }
    }
}
