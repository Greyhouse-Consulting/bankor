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


    [StorageProvider(ProviderName="DevStore")]
    public class AccountGrain : Grain<AccountGrainState>, IAccountGrain
    {
        private readonly ITransactionalState<AccountGrainState> _transactionalState;

        public AccountGrain(
            [TransactionalState("transactionalState")] ITransactionalState<AccountGrainState> balance)
        {
            this._transactionalState = balance ?? throw new ArgumentNullException(nameof(balance));
        }

        Task IAccountGrain.Deposit(uint amount)
        {
            this._transactionalState.State.Balance += amount;
            this._transactionalState.Save();
            return Task.CompletedTask;
        }

        Task IAccountGrain.Withdraw(uint amount)
        {
            this._transactionalState.State.Balance -= amount;
            this._transactionalState.Save();
            return Task.CompletedTask;
        }

        Task<decimal> IAccountGrain.GetBalance()
        {
            return Task.FromResult(this._transactionalState.State.Balance);
        }

        public Task Owner(string userId)
        {
            return Task.CompletedTask;
        }
    }
}
