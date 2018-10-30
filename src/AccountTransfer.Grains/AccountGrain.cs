using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
using Orleans;
using Orleans.CodeGeneration;
using Orleans.Transactions.Abstractions;
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
        public string Name { get; set; }
    }

    [StorageProvider(ProviderName = "BankOrStorageProvider")]
    public class AccountGrain : Grain<AccountGrainState>, IAccountGrain
    {

        public async Task Deposit(decimal amount)
        {
            State.Balance += amount;

            await WriteStateAsync();
        }

        public async Task Withdraw(decimal amount)
        {
            State.Balance -= amount;

            await WriteStateAsync();
        }

        public Task<decimal> GetBalance()
        {

            return Task.FromResult(State.Balance);
        }

        public async Task Owner(string userId)
        {
            await Task.CompletedTask;
        }

        public async Task SetName(string name)
        {
            State.Name = name;
            await WriteStateAsync();
        }

        public Task<string> GetName()
        {
            return Task.FromResult(State.Name);
        }
    }
}
