using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountTransfer.Grains;
using AccountTransfer.Interfaces;
using BankOr.Core;
using Microsoft.Extensions.Logging;
using NPoco;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using Orleans.Transactions;
using Transaction = BankOr.Core.Transaction;

namespace BankOr.Infrastructure
{
    public class BankOrStorageProvider : IStorageProvider
    {
        private readonly IDatabase _database;

        public BankOrStorageProvider(IDatabase database)
        {
            _database = database;
        }
        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {

            return Task.CompletedTask;
        }

        public Task Close()
        {
            return Task.CompletedTask;
        }

        public string Name => "BankOrStorageProvider";

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {

            if (grainType == "AccountTransfer.Grains.CustomerGrain")
            {
                var state = grainState.State as CustomerGrainState;

                using (IDatabase db = BankorDbFactory.DbFactory.GetDatabase())
                {
                    var account = db.SingleOrDefault<Customer>(grainReference.GetPrimaryKeyString());

                    if (account != null)
                    {
                        state.Name = account.Name;
                    }
                    else
                    {
                        state.AccountIds = new List<Guid>();
                    }
                }
            }

            else if (grainType.Contains("AccountTransfer.Grains.AccountGrain,AccountTransfer.Grains-transactionalState"))
            {
                var state = grainState.State as TransactionalStateRecord<AccountGrainState>;
            }
            else if (grainType.Contains("AccountTransfer.Grains.AccountGrain"))
            {
                var state = grainState.State as AccountGrainState;

                using (IDatabase db = BankorDbFactory.DbFactory.GetDatabase())
                {
                    var accountTransactions = db.FetchMultiple<Account, Transaction>(
                        "SELECT * FROM ACCOUNTS WHERE ID = '@0'; SELECT * FROM TRANSACTIONS WHERE AccountId = '@0';",
                        grainReference.GetPrimaryKey());

                    state.Balance = !accountTransactions.Item1.Any() ? 0 : accountTransactions.Item1.First().Balance;
                    state.Transactions = accountTransactions.Item2.Any() ? accountTransactions.Item2 : new List<Transaction>();
                }
            }

            return Task.CompletedTask;
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (grainType == "AccountTransfer.Grains.AccountGrain")
            {
                var r = grainReference as IAccountGrain;

                var state = grainState.State as AccountGrainState;

                using (IDatabase db = BankorDbFactory.DbFactory.GetDatabase())
                {
                    var accountTransactions = db.FetchMultiple<Account, Transaction>(
                        "SELECT * FROM ACCOUNTS WHERE ID = @0; SELECT * FROM TRANSACTIONS WHERE AccountId = @0;",
                        r.GetPrimaryKeyLong());

                    var account = accountTransactions.Item1.First();

                    account.Balance = state.Balance;

                    await db.UpdateAsync(account);

                    foreach (var stateTransaction in state.Transactions.Where(t => t.Id < 0))
                    {
                        await db.InsertAsync(stateTransaction);
                    }
                }
            }

            if (grainType == "AccountTransfer.Grains.CustomerGrain")
            {

                var state = grainState.State as CustomerGrainState;
                using (IDatabase db = BankorDbFactory.DbFactory.GetDatabase())
                {
                    var customerAccounts = db.FetchMultiple<Customer, Guid>(
                        "SELECT * FROM CUSTOMERS WHERE ID = @0; SELECT AccountId FROM CUSTOMERS_ACCOUNTS WHERE CustomerId = @0;",
                        grainReference.GetPrimaryKeyLong());

                    //var customer = customerAccounts.Item1.First();


                    var newAccounts = customerAccounts.Item2.Except(state.AccountIds);

                    //foreach (var newAccount in newAccounts)
                    //{
                        db.Insert<CustomerAccount>(new CustomerAccount
                        {
                            AccountId = 3001,
                            CustomerId = 3000
                        });
                    //}
                }
            }
        }

        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            return Task.CompletedTask;
        }

        public Logger<object> Log { get; }
    }
}
