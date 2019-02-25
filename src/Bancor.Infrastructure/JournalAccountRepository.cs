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
        private readonly IDictionary<long, JournaledAccountGrainStateSnapshot> _stateSnapshots = new Dictionary<long, JournaledAccountGrainStateSnapshot>();
        private readonly IDictionary<long, IList<AccountEventLog>> _eventLog = new ConcurrentDictionary<long, IList<AccountEventLog>>();
        private readonly IDictionary<long, int> _versions = new ConcurrentDictionary<long, int>();

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

            return Task.FromResult(
                new KeyValuePair<int, JournaledAccountGrainState>(version, _stateSnapshots[accountId]));
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

            return Task.FromResult(true);
        }
    }
}