using System.Linq;
using System.Threading.Tasks;
using Bancor.Core;
using Bancor.Core.Grains;
using NPoco;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using Orleans.Transactions;

namespace Bancor.Infrastructure
{
    public class AccountsStorageProvider : IStorageProvider
    {
        private readonly IDatabase _database;

        public AccountsStorageProvider(IDatabase database)
        {
            _database = database;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (grainType == "Bancor.Core.Grains.AccountGrain,Bancor.Core.Grains-transactionalState")
            {
                var state = grainState.State as TransactionalStateRecord<AccountGrainStateTransactional>;

                var accountTransactions = _database.FetchMultiple<Account, Core.Transaction>(
                    "SELECT * FROM ACCOUNTS WHERE ID = '@0'; SELECT * FROM TRANSACTIONS WHERE AccountId = '@0';",
                    grainReference.GetPrimaryKey());

                if (accountTransactions.Item1.Any())
                {
                    state.CommittedState.Balance = accountTransactions.Item1.First().Balance;
                    state.CommittedState.Transactions = accountTransactions.Item2;
                }
            }
            else if (grainType.Contains("Bancor.Core.Grains.AccountGrain"))
            {
                var state = grainState.State as AccountGrainState;

                try
                {
                    var account = await _database.SingleOrDefaultByIdAsync<Account>(
                                grainReference.GetPrimaryKeyLong());

                    if (account != null)
                    {
                        state.Name = account.Name;
                        state.Created = true;
                    }
                }
                catch (System.Exception ex)
                {

                    throw;
                }
            }
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (grainType == "Bancor.Core.Grains.AccountGrain,Bancor.Core.Grains-transactionalState")
            {

                var state = grainState.State as TransactionalStateRecord<AccountGrainStateTransactional>;

                var accountTransactions = _database.FetchMultiple<Account, Core.Transaction>(
                    "SELECT * FROM ACCOUNTS WHERE ID = @0; SELECT * FROM TRANSACTIONS WHERE AccountId = @0;",
                    grainReference.GetPrimaryKeyLong());

                var account = accountTransactions.Item1.FirstOrDefault();

                if (account == null)
                {
                    _database.Insert(new Account
                    {
                        Id = grainReference.GetPrimaryKeyLong(),
                        Balance = 0,
                    });
                }
                else
                {
                    account.Balance = state.CommittedState.Balance;

                    await _database.UpdateAsync(account);

                    foreach (var stateTransaction in state.CommittedState.Transactions.Where(t => t == null))
                    {
                        await _database.InsertAsync(stateTransaction);
                    }
                }
            }
            else if (grainType == "Bancor.Core.Grains.AccountGrain")
            {
                var state = grainState.State as AccountGrainState;

                var accountTransactions = _database.FetchMultiple<Account, Core.Transaction>(
                    "SELECT * FROM ACCOUNTS WHERE ID = @0; SELECT * FROM TRANSACTIONS WHERE AccountId = @0;",
                    grainReference.GetPrimaryKeyLong());

                var account = accountTransactions.Item1.FirstOrDefault();

                if (account == null)
                {
                    try
                    {
                        _database.Insert(new Account
                        {
                            Id = grainReference.GetPrimaryKeyLong(),
                            Balance = 0,
                            Name = state.Name,
                        });
                    }
                    catch (System.Exception ex)
                    {

                        throw;
                    }
                    state.Created = true;
                }
                else
                {
                    account.Name = state.Name;
                    await _database.UpdateAsync(account);
                }
            }
        }

        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new System.NotImplementedException();
        }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            return Task.CompletedTask;
        }

        public Task Close()
        {
            return Task.CompletedTask;
        }

        public string Name => "AccountsStorageProvider";
    }
}