using System.Linq;
using System.Threading.Tasks;
using AccountTransfer.Grains;
using BankOr.Core;
using NPoco;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using Orleans.Transactions;

namespace BankOr.Infrastructure
{
    public class AccountsStorageProvider : IStorageProvider
    {
        private readonly IDatabase _database;

        public AccountsStorageProvider(IDatabase database)
        {
            _database = database;
        }

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (grainType == "AccountTransfer.Grains.AccountGrain,AccountTransfer.Grains-transactionalState")
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
            else if (grainType.Contains("AccountTransfer.Grains.AccountGrain"))
            {
                var state = grainState.State as AccountGrainState;

                var accountTransactions = _database.FetchMultiple<Account, Core.Transaction>(
                    "SELECT * FROM ACCOUNTS WHERE ID = '@0'; SELECT * FROM TRANSACTIONS WHERE AccountId = '@0';",
                    grainReference.GetPrimaryKey());

                if (accountTransactions.Item1.Any())
                {
                    state.Name = accountTransactions.Item1.First().Name;
                    state.Created = accountTransactions.Item1.First().Created;
                }
            }

            return Task.CompletedTask;
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (grainType == "AccountTransfer.Grains.AccountGrain,AccountTransfer.Grains-transactionalState")
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
                            Created = true
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
            else if (grainType == "AccountTransfer.Grains.AccountGrain")
            {
                var state = grainState.State as AccountGrainState;

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
                            Name = state.Name,
                            Created = true
                        });
                        state.Created = true;
                    }
                    else
                    {
                        account.Name = state.Name;
                        account.Created = state.Created;
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