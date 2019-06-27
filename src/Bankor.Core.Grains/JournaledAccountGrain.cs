using System.Collections.Generic;
using System.Threading.Tasks;
using Bancor.Core.Events.Account;
using Bancor.Core.Exceptions;
using Bancor.Core.Grains.Interfaces;
using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Core.States.Account;
using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;
using Orleans.Providers;

namespace Bancor.Core.Grains
{
    [LogConsistencyProvider(ProviderName = "CustomStorage")]
    public class JournaledAccountGrain : JournaledGrain<JournaledAccountGrainState, AccountEvent>,
        IJournaledAccountGrain,
        ICustomStorageInterface<JournaledAccountGrainState, AccountEvent>
    {
        private readonly IJournaldAccountRepository _journaldAccountRepository;


        public JournaledAccountGrain(IJournaldAccountRepository journaldAccountRepository)
        {
            _journaldAccountRepository = journaldAccountRepository;
        }

        public Task Deposit(decimal amount)
        {
            EnsureCreated();

            RaiseEvent(new DepositEvent(amount));

            return ConfirmEvents();
        }

        public Task Withdraw(decimal amount)
        {
            EnsureCreated();

            RaiseEvent(new WithdrawEvent(amount));

            return ConfirmEvents();
        }

        public Task HasNewName(string name)
        {
            RaiseEvent(new AccountNameEvent(name));

            return ConfirmEvents();
        }

        public Task<decimal> Balance()
        {
            EnsureCreated();

            return Task.FromResult(State.Balance);
        }

        public Task HasName(string name)
        {
            RaiseEvent(new AccountNameEvent(name));

            return ConfirmEvents();
        }

        public Task<string> Name()
        {
            return Task.FromResult(State.Name);
        }

        private void EnsureCreated()
        {
            if (!State.Created)
                throw new GrainDoesNotExistException($"Customer with id '{this.GetPrimaryKeyLong()}' does not exist");
        }

        public async Task<KeyValuePair<int, JournaledAccountGrainState>> ReadStateFromStorage()
        {
            return await _journaldAccountRepository.GetState(this.GetPrimaryKey());
        }

        public async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<AccountEvent> updates, int expectedversion)
        {
            return await _journaldAccountRepository.ApplyUpdatesToStorage(this.GetPrimaryKey(), updates, expectedversion);
        }
    }
}
