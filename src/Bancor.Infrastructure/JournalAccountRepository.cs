using System;
using Bancor.Core.Events.Account;
using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Core.States.Account;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bancor.Infrastructure
{
    public class JournalAccountRepository : IJournaldAccountRepository
    {
        public Task< KeyValuePair<int, JournaledAccountGrainState>> GetState(long accountId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> ApplyUpdatesToStorage(long accountId, IReadOnlyList<AccountEvent> updates,
            int expectedversion)
        {
            throw new System.NotImplementedException();
        }
    }

    public class JournalAccountRepositoryInMemory : IJournaldAccountRepository
    {
        private readonly IDictionary<long, JournaledAccountGrainStateSnapshot> _stateSnapshots;
        private readonly IDictionary<long, IList<AccountEventLog>> _eventLog;
        private readonly IDictionary<long, int> _versions;


        public JournalAccountRepositoryInMemory()
        {
            _stateSnapshots = new Dictionary<long, JournaledAccountGrainStateSnapshot>();
            _stateSnapshots.Add(2000, new JournaledAccountGrainStateSnapshot());


            _versions = new ConcurrentDictionary<long, int>();
            _versions.Add(2000, 0);

            _eventLog = new ConcurrentDictionary<long, IList<AccountEventLog>>();
            _eventLog.Add(2000, new List<AccountEventLog>());
            _eventLog[2000].Add(new AccountEventLog(2000, new DepositEvent(200), 1 ));
            _eventLog[2000].Add(new AccountEventLog(2000, new WithdrawEvent(100), 2 ));
        }

        public Task<KeyValuePair<int, JournaledAccountGrainState>> GetState(long accountId)
        {
            if (!_stateSnapshots.ContainsKey(accountId))
                _stateSnapshots[accountId] = new JournaledAccountGrainStateSnapshot();

            var version = _versions.ContainsKey(accountId) ? _versions[accountId] : 0;

            if (_eventLog.ContainsKey(accountId))
            {
                var eventsToApply = _eventLog[accountId]
                    .Where(e => e.AccountVersion > version)
                    .OrderBy(e => e.AccountVersion);

                foreach (var accountEventLog in eventsToApply)
                {
                    _stateSnapshots[accountId].Apply(accountEventLog.AccountEvent);
                }
            }

            var latestVersion = _eventLog[accountId].Max(e => e.AccountVersion);

            return Task.FromResult(
                new KeyValuePair<int, JournaledAccountGrainState>(latestVersion, _stateSnapshots[accountId]));
        }

        public Task<bool> ApplyUpdatesToStorage(
            long accountId,
            IReadOnlyList<AccountEvent> updates,
            int expectedversion)
        {
            if(!_eventLog.ContainsKey(accountId))
                _eventLog[accountId] = new List<AccountEventLog>();

            var latestVersion = _eventLog[accountId].Any() ? _eventLog[accountId].Max(e => e.AccountVersion) : 0;

            if (latestVersion + updates.Count - 1 != expectedversion)
                throw new Exception();

            foreach (var update in updates)
            {
                _eventLog[accountId].Add(new AccountEventLog(accountId, update, ++latestVersion));
            }

            var oneYearBack = DateTime.Now.AddYears(-1);

            var snapshot = _stateSnapshots[accountId];
            foreach (var update in updates.Where(e => e.Created < oneYearBack))
            {
                snapshot.Apply(update);
            }

            return Task.FromResult(true);
        }
    }
}