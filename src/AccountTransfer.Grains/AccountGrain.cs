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

    [StorageProvider(ProviderName="BankOrStorageProvider")]
    public class AccountGrain : Grain<AccountGrainState>, IAccountGrain
    {
        private readonly ITransactionalState<AccountGrainState> _transactionalState;

        public AccountGrain(
            [TransactionalState("transactionalState")] ITransactionalState<AccountGrainState> balance)
        {
            this._transactionalState = balance ?? throw new ArgumentNullException(nameof(balance));
        }

        async Task IAccountGrain.Deposit(uint amount)
        {
            await this._transactionalState.PerformUpdate(x => x.Balance += amount);
        }

        Task IAccountGrain.Withdraw(uint amount)
        {
            this._transactionalState.PerformUpdate(x => x.Balance - amount);
            return Task.CompletedTask;
        }

        Task<decimal> IAccountGrain.GetBalance()
        {
            return  this._transactionalState.PerformRead(x => x.Balance);
        }

        public async Task Owner(string userId)
        {
            await Task.CompletedTask;
        }
    }
}
