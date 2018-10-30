using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountTransfer.Grains;
using AccountTransfer.Interfaces.Grains;
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
        private readonly IGrainFactory _grainFactory;

        public BankOrStorageProvider(IDatabase database, IGrainFactory grainFactory)
        {
            _database = database;
            _grainFactory = grainFactory;
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

                        var customerAccounts = db.Fetch<CustomerAccount>("SELECT * FROM Customers_Accounts WHERE CustomerId = '@0'", grainReference.GetPrimaryKeyLong());

                        foreach (var customerAccount in customerAccounts)
                        {
                            state.AccountGrains.Add(_grainFactory.GetGrain<IAccountGrain>(customerAccount.AccountId));
                        }

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

                    if(accountTransactions.Item1.Any())
                    {
                        state.Balance = accountTransactions.Item1.First().Balance;
                        state.Transactions = accountTransactions.Item2;
                        state.Name = accountTransactions.Item1.First().Name;
                        state.Created = accountTransactions.Item1.First().Created;
                    }


                }
            }

            return Task.CompletedTask;
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (grainType == "AccountTransfer.Grains.AccountGrain")
            {
                var state = grainState.State as AccountGrainState;

                using (IDatabase db = BankorDbFactory.DbFactory.GetDatabase())
                {
                    var accountTransactions = db.FetchMultiple<Account, Transaction>(
                        "SELECT * FROM ACCOUNTS WHERE ID = @0; SELECT * FROM TRANSACTIONS WHERE AccountId = @0;",
                        grainReference.GetPrimaryKeyLong());

                    var account = accountTransactions.Item1.FirstOrDefault();

                    if (account == null)
                    {
                        db.Insert(new Account
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
                        account.Balance = state.Balance;
                        account.Name = state.Name;
                        account.Created = state.Created;

                        await db.UpdateAsync(account);

                        foreach (var stateTransaction in state.Transactions.Where(t => t.Id < 0))
                        {
                            await db.InsertAsync(stateTransaction);
                        }
                    }
                }
            }

            if (grainType == "AccountTransfer.Grains.CustomerGrain")
            {

                var state = grainState.State as CustomerGrainState;
                using (IDatabase db = BankorDbFactory.DbFactory.GetDatabase())
                {
                    var customerAccounts = db.FetchMultiple<Customer, long>(
                        "SELECT * FROM CUSTOMERS WHERE ID = @0; SELECT AccountId FROM CUSTOMERS_ACCOUNTS WHERE CustomerId = @0;",
                        grainReference.GetPrimaryKeyLong());

                    //var customer = customerAccounts.Item1.First();




                    if (!customerAccounts.Item1.Any())

                    {
                        try
                        {
                            db.Save<Customer>(new Customer
                            {
                                Name = state.Name,
                                Created = true,
                                Id = grainReference.GetPrimaryKeyLong()
                            });
                            state.Created = true;
                        }
                        catch (Exception ex)
                        {

                            state.Created = false;
                            throw;
                        }
                    }
                    else
                    {
                        var newAccounts = state
                            .AccountGrains
                            .Select(ag => ag.GetPrimaryKeyLong())
                            .ToList()
                            .Except(customerAccounts.Item2);

                        foreach (var newAccount in newAccounts)
                        {
                            db.Insert(new CustomerAccount
                            {
                                AccountId = newAccount,
                                CustomerId = grainReference.GetPrimaryKeyLong()
                            });
                        }
                    }
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
