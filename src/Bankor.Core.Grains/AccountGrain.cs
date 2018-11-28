using System;
using System.Threading.Tasks;
using Bancor.Core.Exceptions;
using Bancor.Core.Grains.Interfaces.Grains;
using Orleans;
using Orleans.Providers;
using Orleans.Transactions.Abstractions;

namespace Bancor.Core.Grains
{
    [StorageProvider(ProviderName = "AccountsStorageProvider")]
    public class AccountGrain : Grain<AccountGrainState>, IAccountGrain
    {
        private readonly ITransactionalState<AccountGrainStateTransactional> _transactionalState;

        public AccountGrain([TransactionalState("transactionalState")] ITransactionalState<AccountGrainStateTransactional> transactionalState)
        {
            _transactionalState = transactionalState ?? throw new ArgumentNullException(nameof(transactionalState));
        }

        public async Task Deposit(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("amount cannot be less or equal to zero when doing a deposit",
                    nameof(amount));

            if (State.Name == "Krashkonto")
                throw new Exception("Booooom!");

            await EnsureCreated();
            await UpdateBalance(amount);
        }

        public async Task Withdraw(decimal amount)
        {
            await EnsureCreated();
            await UpdateBalance(-amount); 
        }

        private async Task UpdateBalance(decimal amount)
        {
            await _transactionalState.PerformUpdate(b =>
            {
                b.Balance += amount;
                b.Transactions.Add( CreateTransaction(amount));
            });
        }

        private Transaction CreateTransaction(decimal amount)
        {
            return new Transaction
            {
                Amount = -amount,
                BookingDate = DateTime.Now,
                AccountId = this.GetPrimaryKeyLong(),
                Id = Guid.NewGuid()
            };
        }
        public async Task<decimal> GetBalance()
        {
            return await _transactionalState.PerformRead(r => r.Balance);
        }

        public async Task HasNewName(string name)
        {
            State.Name = name;
            await WriteStateAsync();
        }

        public async Task<string> GetName()
        {
            await EnsureCreated();
            return State.Name;
        }

        private Task EnsureCreated()
        {
            if (!State.Created)
                throw new GrainDoesNotExistException($"Customer with id '{this.GetPrimaryKeyLong()}' does not exist");

            return Task.CompletedTask;
        }
    }
}
