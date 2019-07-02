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
    public class JournalAccountRepository : IJournaldAccountRepository
    {
        private readonly IMongoDatabase _mongoDatabase;

        public JournalAccountRepository(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
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

            try
            {
                var sc = _mongoDatabase.GetCollection<JournaledAccountGrainStateSnapshot>(nameof(JournaledAccountGrainStateSnapshot));
                var snapshot = await sc.Find(a => a.AccountId == accountId).FirstOrDefaultAsync();

                if (snapshot == null)
                {
                    snapshot = new JournaledAccountGrainStateSnapshot{ AccountId = accountId, Id = Guid.NewGuid()};
                    sc.InsertOne(snapshot);
                }

                var ec = _mongoDatabase.GetCollection<AccountEventLog>(nameof(AccountEventLog));
            
                var events = (await ec.Find(e => e.AccountId == accountId && e.AccountVersion > snapshot.LatestVersion).ToListAsync())
                    .OrderByDescending(e => e.AccountVersion)
                    .Where(e => updates.All(u => u.Id != e.Id));

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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}