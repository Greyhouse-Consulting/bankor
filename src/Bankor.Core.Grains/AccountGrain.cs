using System;
using System.Linq;
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

            RegisterTimer(AddInterest, _transactionalState, TimeSpan.FromMinutes(5), TimeSpan.FromDays(1));
        }

        private async Task AddInterest(object o)
        {
            var interestAmount = 200;

            var transactions = await _transactionalState.PerformRead(b => b.Transactions);

            if (!transactions
                .Any(t => t.Type == TransactionType.Interest && t.BookingDate.Date.CompareTo(DateTime.Now.Date) == 0))
            {
                await Deposit(interestAmount, "Dailiy interesst", TransactionType.Interest);
            }
        }

        public async Task Deposit(decimal amount, string dayliyInteresst, TransactionType type = TransactionType.Default)
        {
            if (amount <= 0)
                throw new ArgumentException("amount cannot be less or equal to zero when depositing",
                    nameof(amount));

            if (State.Name == "Krashkonto")
                throw new Exception("Booooom!");

            await EnsureCreated();
            await UpdateBalance(amount, dayliyInteresst, type);
        }

        public async Task Withdraw(decimal amount, string desciption)
        {
            await EnsureCreated();
            await UpdateBalance(-amount, desciption);
        }

        private async Task UpdateBalance(decimal amount, string desciption, TransactionType type = TransactionType.Default)
        {
            await _transactionalState.PerformUpdate(b =>
            {
                b.Balance += amount;
                b.Transactions.Add(CreateTransaction(amount, desciption, type));
            });
        }

        private Transaction CreateTransaction(decimal amount, string desciption, TransactionType type)
        {
            return new Transaction
            {
                Amount = -amount,
                BookingDate = DateTime.Now,
                AccountId = this.GetPrimaryKeyLong(),
                Id = Guid.NewGuid(),
                Type = type,
                Description = desciption
            };
        }
        public async Task<decimal> GetBalance()
        {
            await EnsureCreated();

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
