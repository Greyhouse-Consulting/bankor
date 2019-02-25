using System.Collections.Generic;
using System.Threading.Tasks;
using Bancor.Core.Events.Account;
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
            RaiseEvent(new DepositEvent(amount));

            return ConfirmEvents();
        }

        public Task<decimal> Balance()
        {
            return Task.FromResult(State.Balance);
        }

        public async Task<KeyValuePair<int, JournaledAccountGrainState>> ReadStateFromStorage()
        {
            return await _journaldAccountRepository.GetState(this.GetPrimaryKeyLong());
        }

        public async  Task<bool> ApplyUpdatesToStorage(IReadOnlyList<AccountEvent> updates, int expectedversion)
        {
            return await _journaldAccountRepository.ApplyUpdatesToStorage(this.GetPrimaryKeyLong(), updates, expectedversion);
        }
    }
}
