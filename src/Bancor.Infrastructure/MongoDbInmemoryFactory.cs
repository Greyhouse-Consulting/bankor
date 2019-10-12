using System;
using Bancor.Core.Events.Account;
using Bancor.Core.States.Account;
using Bancor.Infrastructure.Abstractions;
using Mongo2Go;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Bancor.Infrastructure
{
    public class MongoDbFactory : IMongoDbFactory
    {
        public IMongoDatabase Create(string connectionString = "mongodb://db:27017")
        {


            var client = new MongoClient(new MongoUrl(connectionString));

            return client.GetDatabase("Bancor");
        }
    }

    public class MongoDbInmemoryFactory : IMongoDbFactory, IDisposable
    {
        private MongoDbRunner _runner;
        internal static string _databaseName = "BancorInmemory";

        public IMongoDatabase Create(string connectionString = "")
        {
            _runner = MongoDbRunner.Start(singleNodeReplSet: false);

            var client = new MongoClient(_runner.ConnectionString);
            IMongoDatabase mongoDatabase = client.GetDatabase(_databaseName);

            Seed(mongoDatabase);
            return mongoDatabase;
        }

        private void Seed(IMongoDatabase mongoDatabase)
        {
            var snapshotCollection = mongoDatabase.GetCollection<JournaledAccountGrainStateSnapshot>(nameof(JournaledAccountGrainStateSnapshot));

            var accountId = Guid.Parse("EBD09F9C-4A99-4B8D-A581-3C93764D24B1");
            //       snapshotCollection.InsertOne(new JournaledAccountGrainStateSnapshotTest { LatestVersion = 0, AccountId = accountId });

            var accountEventlogCollection
                = mongoDatabase.GetCollection<AccountEventLog>(nameof(AccountEventLog));

            accountEventlogCollection.InsertOne(new AccountEventLog(accountId, new AccountNameEvent("Sparkonto", "Sparkonto"), 1));
            accountEventlogCollection.InsertOne(new AccountEventLog(accountId, new DepositEvent(200, "Depositing"), 2));
            accountEventlogCollection.InsertOne(new AccountEventLog(accountId, new WithdrawEvent(100, "Withdrawal"), 3));
        }

        public void Dispose()
        {
            _runner?.Dispose();
        }
    }
}