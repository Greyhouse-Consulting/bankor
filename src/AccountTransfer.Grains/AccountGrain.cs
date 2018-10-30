using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
using Orleans;
using Orleans.CodeGeneration;
using Orleans.Transactions.Abstractions;
using BankOr.Core;
using BankOr.Core.Exceptions;
using Orleans.Providers;

[assembly: GenerateSerializer(typeof(AccountTransfer.Grains.AccountGrainState))]

namespace AccountTransfer.Grains
{
    [Serializable]
    public class AccountGrainState
    {

        public AccountGrainState()
        {
            Transactions = new List<Transaction>();
        }
        public decimal Balance { get; set; }

        public IList<Transaction> Transactions { get; set; }
        public string Name { get; set; }

        public bool Created { get; set; }
    }

    [StorageProvider(ProviderName = "BankOrStorageProvider")]
    public class AccountGrain : Grain<AccountGrainState>, IAccountGrain
    {

        public async Task Deposit(decimal amount)
        {
            EnsureCreated();
            
            State.Balance += amount;

            await WriteStateAsync();
        }

        public async Task Withdraw(decimal amount)
        {
            EnsureCreated();
            State.Balance -= amount;

            await WriteStateAsync();
        }

        public Task<decimal> GetBalance()
        {
            EnsureCreated();

            return Task.FromResult(State.Balance);
        }

        public async Task Owner(string userId)
        {
            EnsureCreated();
            await Task.CompletedTask;
        }

        public async Task TryInit(string name)
        {
            await HasNewName(name);
        }

        public async Task HasNewName(string name)
        {
            State.Name = name;
            await WriteStateAsync();
        }

        public Task<string> GetName()
        {
            EnsureCreated();
            return Task.FromResult(State.Name);
        }

        private void EnsureCreated()
        {
            if (!State.Created)
                throw new GrainDoesNotExistException($"Customer with id '{this.GetPrimaryKeyLong()}' does not exist");
        }
    }
}
