using System;
using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
using Orleans;
using Orleans.CodeGeneration;
using Orleans.Transactions.Abstractions;
using BankOr.Core;
using BankOr.Core.Exceptions;
using Orleans.Providers;



namespace AccountTransfer.Grains
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
        //    State.Transactions.Add(CreateTransaction(amount));
        //    await WriteStateAsync();
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

        public async Task Owner(string userId)
        {
            await EnsureCreated();
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
