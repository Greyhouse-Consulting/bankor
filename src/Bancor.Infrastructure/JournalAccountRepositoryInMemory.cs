using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bancor.Core.Events.Account;
using Bancor.Core.Grains.Interfaces.Repository;
using Bancor.Core.States.Account;
using Bancor.Infrastructure.Abstractions;
using MongoDB.Driver;

namespace Bancor.Infrastructure
{
    public class JournalAccountRepositoryInMemory : IJournaldAccountRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        public JournalAccountRepositoryInMemory(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
            //_stateSnapshots = new Dictionary<long, JournaledAccountGrainStateSnapshot>();
            //_stateSnapshots.Add(2000, new JournaledAccountGrainStateSnapshotTest());


            //_versions = new ConcurrentDictionary<long, int>();
            //_versions.Add(2000, 0);

            //_eventLog = new ConcurrentDictionary<long, IList<AccountEventLog>>();
            //_eventLog.Add(2000, new List<AccountEventLog>());
            //_eventLog[2000].Add(new AccountEventLog(2000, new DepositEvent(200), 1 ));
            //_eventLog[2000].Add(new AccountEventLog(2000, new WithdrawEvent(100), 2 ));
        }

        public async Task<KeyValuePair<int, JournaledAccountGrainState>> GetState(Guid accountId)
        {
            try
            {

                var sc = _mongoDatabase.GetCollection<JournaledAccountGrainStateSnapshot>(nameof(JournaledAccountGrainStateSnapshot));
                var snapshot = (await sc.FindAsync(a => a.AccountId == accountId)).FirstOrDefault();

                if(snapshot == null )
                    return new KeyValuePair<int, JournaledAccountGrainState>(0, new JournaledAccountGrainState());

                var ec = _mongoDatabase.GetCollection<AccountEventLog>(nameof(AccountEventLog));

                var sort = Builders<AccountEventLog>.Sort.Descending("LatestVersion");

                var events = await ec.Find(e => e.AccountId == accountId && e.AccountVersion > snapshot.LatestVersion).Sort(sort).ToListAsync();

                if (events.Any())
                {
                    foreach (var accountEventLog in events)
                    {
                        snapshot.Apply(accountEventLog.AccountEvent);
                    }
                }

                var latestVersion = events.Max(e => e.AccountVersion);

                return 
                    new KeyValuePair<int, JournaledAccountGrainState>(latestVersion, snapshot);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<bool> ApplyUpdatesToStorage(
            Guid accountId,
            IReadOnlyList<AccountEvent> updates,
            int expectedversion)
        {

            var sc = _mongoDatabase.GetCollection<JournaledAccountGrainStateSnapshot>(nameof(JournaledAccountGrainStateSnapshot));
            var snapshot = await sc.Find(a => a.AccountId == accountId).FirstOrDefaultAsync();
            
            var ec = _mongoDatabase.GetCollection<AccountEventLog>(nameof(AccountEventLog));
            var events = (await ec.Find(e => e.AccountId == accountId && e.AccountVersion > snapshot.LatestVersion).ToListAsync())
                .OrderByDescending(e => e.AccountVersion);

            var latestVersion = events.Any() ? events.Max(e => e.AccountVersion) : snapshot.LatestVersion;

            if (latestVersion + updates.Count - 1 != expectedversion)
                throw new Exception();

            foreach (var update in updates)
            {
                ec.InsertOne(new AccountEventLog(accountId, update, ++latestVersion));
            }

            var oneYearBack = DateTime.Now.AddYears(-1);

            foreach (var update in updates.Where(e => e.Created < oneYearBack))
            {
                snapshot.Apply(update);
            }

            sc.ReplaceOne(f => f.AccountId == accountId, snapshot);

            return true;
        }
    }
}