using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.CodeGeneration;
using Orleans.Transactions.Abstractions;
using AccountTransfer.Interfaces;
using Orleans.Providers;

[assembly: GenerateSerializer(typeof(AccountTransfer.Grains.AccountGrainState))]

namespace AccountTransfer.Grains
{
    [Serializable]
    public class AccountGrainState
    {
        public uint Balance { get; set; }
    }

    [StorageProvider(ProviderName="BankOrStorageProvider")]
    public class AccountGrain : Grain<AccountGrainState>, IAccountGrain
    {
        private readonly ITransactionalState<AccountGrainState> transactionalState;

        public AccountGrain(
            [TransactionalState("transactionalState")] ITransactionalState<AccountGrainState> balance)
        {
            this.transactionalState = balance ?? throw new ArgumentNullException(nameof(balance));
        }

        Task IAccountGrain.Deposit(uint amount)
        {
            this.transactionalState.State.Balance += amount;
            this.transactionalState.Save();
            return Task.CompletedTask;
        }

        Task IAccountGrain.Withdraw(uint amount)
        {
            this.transactionalState.State.Balance -= amount;
            this.transactionalState.Save();
            return Task.CompletedTask;
        }

        Task<uint> IAccountGrain.GetBalance()
        {
            return Task.FromResult(this.transactionalState.State.Balance);
        }
    }
}
