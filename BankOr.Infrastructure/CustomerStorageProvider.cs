using System;
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

namespace BankOr.Infrastructure
{
    public class CustomerStorageProvider : IStorageProvider
    {
        private readonly IDatabase _database;
        private readonly IGrainFactory _grainFactory;

        public CustomerStorageProvider(IDatabase database, IGrainFactory grainFactory)
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

        public string Name => "CustomerStorageProvider";

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {

            if (grainType == "AccountTransfer.Grains.CustomerGrain")
            {
                var state = grainState.State as CustomerGrainState;

                var account = _database.SingleOrDefault<Customer>(grainReference.GetPrimaryKeyString());

                if (account != null)
                {
                    state.Name = account.Name;

                    var customerAccounts = _database.Fetch<CustomerAccount>("SELECT * FROM Customers_Accounts WHERE CustomerId = '@0'", grainReference.GetPrimaryKeyLong());

                    foreach (var customerAccount in customerAccounts)
                    {
                        state.AccountGrains.Add(_grainFactory.GetGrain<IAccountGrain>(customerAccount.AccountId));
                    }

                }
            }

            return Task.CompletedTask;
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {

            if (grainType == "AccountTransfer.Grains.CustomerGrain")
            {

                var state = grainState.State as CustomerGrainState;
                var customerAccounts = _database.FetchMultiple<Customer, long>(
                    "SELECT * FROM CUSTOMERS WHERE ID = @0; SELECT AccountId FROM CUSTOMERS_ACCOUNTS WHERE CustomerId = @0;",
                    grainReference.GetPrimaryKeyLong());


                if (!customerAccounts.Item1.Any())

                {
                    try
                    {
                        _database.Save<Customer>(new Customer
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
                        _database.Insert(new CustomerAccount
                        {
                            AccountId = newAccount,
                            CustomerId = grainReference.GetPrimaryKeyLong()
                        });
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
