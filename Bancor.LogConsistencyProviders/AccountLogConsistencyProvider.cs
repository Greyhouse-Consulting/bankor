using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bancor.Core.Grains;
using Bancor.Core.Grains.Events.Account;
using MongoDB.Driver;
using Orleans.EventSourcing.CustomStorage;

namespace Bancor.LogConsistencyProviders
{
    public class AccountLogConsistencyProvider : ICustomStorageInterface<JournaledAccountGrainState, AccountEvent>
    {
        private readonly IMongoClient _mongoClient;

        public AccountLogConsistencyProvider(IMongoClient mongoClient)
        {
            _mongoClient = mongoClient;
        }
        public async Task<KeyValuePair<int, JournaledAccountGrainState>> ReadStateFromStorage()
        {
            var db  = _mongoClient.GetDatabase("Journals");

            var accountStates  = db.GetCollection<JournaledAccountGrainState>("accountStates");
            var a = await accountStates.FindAsync(a=> a.);
            throw new NotImplementedException();
        }

        public Task<bool> ApplyUpdatesToStorage(IReadOnlyList<AccountEvent> updates, int expectedversion)
        {
            throw new NotImplementedException();
        }
    }
}