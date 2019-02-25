using System.Threading.Tasks;
using Bancor.Core.Grains;
using Bancor.Core.Grains.EventStores;
using Bancor.Core.States.Account;
using MongoDB.Driver;

namespace Bancor.Infrastructure
{
    public class AccountEventStore : IAccountEventStore
    {
        private readonly IMongoCollection<JournaledAccountGrainStateSnapshot> _accountCollection;

        public AccountEventStore()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");

            var database = mongoClient.GetDatabase("eventStore");

             _accountCollection = database.GetCollection<JournaledAccountGrainStateSnapshot>("accountEventStoreCollection");

        }

        public async Task<JournaledAccountGrainState> GetAccountState(int id)
        {
            var state = await _accountCollection.FindAsync(s => s.AccountId == id);

            return await state.FirstOrDefaultAsync();
        }
    }
}