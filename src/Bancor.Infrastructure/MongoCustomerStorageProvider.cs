using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bancor.Core;
using Bancor.Core.Grains;
using Bancor.Core.Grains.Interfaces;
using Bancor.Core.Grains.Interfaces.Grains;
using Bancor.Infrastructure.MongoEntites;
using MongoDB.Bson;
using MongoDB.Driver;
using Orleans;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;

namespace Bancor.Infrastructure
{
    public class MongoCustomerStorageProvider : IStorageProvider
    {
        private readonly IMongoDatabase _database;
        private readonly IGrainFactory _grainFactory;

        public MongoCustomerStorageProvider(IMongoDatabase database, IGrainFactory grainFactory)
        {
            _database = database;
            _grainFactory = grainFactory;
        }

        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (grainType == typeof(CustomerGrain).FullName)
            {
                var state = grainState.State as CustomerGrainState;
                if(state == null)
                    throw new Exception("Invalid state in MongoCustomerStorageProvider::ReadStateAsync");

                var customer = (await _database.GetCollection<MongoCustomer>("Customers")
                    .FindAsync(a => a.Id == grainReference.GetPrimaryKey())).FirstOrDefault();

                if (customer != null)
                {
                    state.Name = customer.Customer.Name;
                    state.Created = true;

                    foreach (var customerAccountId in customer.Customer.AccountsIds)
                    {
                        state.AccountGrains.Add(_grainFactory.GetGrain<IJournaledAccountGrain>(customerAccountId));
                    }
                }
            }
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {

            if (grainType == typeof(CustomerGrain).FullName)
            {
                var customerCollection = _database.GetCollection<MongoCustomer>("Customers");
                var customer = (await customerCollection
                    .FindAsync(a => a.Id == grainReference.GetPrimaryKey())).FirstOrDefault();

                var state = grainState.State as CustomerGrainState;
                if(state == null)
                    throw new Exception("Invalid state in MongoCustomerStorageProvider::WriteStateAsync");

                var customerAccounts = customer?.Customer?.AccountsIds ?? new List<Guid>();
                var newAccounts = state.AccountGrains.Select(a => a.GetPrimaryKey()).Except(customerAccounts);
                var removedAccounts = customerAccounts.Except(state.AccountGrains.Select(a => a.GetPrimaryKey()));


                if (customer == null)
                {
                    var customerId = Guid.NewGuid();
                    await customerCollection.InsertOneAsync(
                        new MongoCustomer
                        {
                            Id = customerId,
                            Customer = new Customer
                            {
                                Id =  customerId,
                                Name = state.Name,
                                AccountsIds = state.AccountGrains.Select(a => a.GetPrimaryKey()).ToArray()
                            }
                        });

                    state.Created = true;
                }
                else
                {

                    foreach (var removedAccount in removedAccounts)
                    {
                        customer.Customer.AccountsIds.RemoveAt(customer.Customer.AccountsIds.IndexOf(removedAccount));
                    }

                    foreach (var newAccount in newAccounts)
                    {
                        customer.Customer.AccountsIds.Add(newAccount);
                    }

                    await customerCollection.FindOneAndReplaceAsync(c => c.Id == customer.Id, customer);
                }
            }
        }

        public Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            throw new System.NotImplementedException();
        }

        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            throw new System.NotImplementedException();
        }

        public Task Close()
        {
            throw new System.NotImplementedException();
        }

        public string Name => "MongoCustomerStorageProvider";
    }
}