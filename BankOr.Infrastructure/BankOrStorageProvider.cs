using System;
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

namespace BankOr.Infrastructure
{
    public class BankOrStorageProvider : IStorageProvider
    {
        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {

            return Task.CompletedTask;
        }

        public Task Close()
        {
            return Task.CompletedTask;
        }

        public string Name => "DevStore";

        public Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {

            if (grainType == "AccountTransfer.Grains.CustomerGrain")
            {
                var r = grainReference as ICustomerGrain;

                var state = grainState as CustomerGrainState;

                using (IDatabase db = BankorDbFactory.DbFactory.GetDatabase())
                {
                    var account = db.Single<Customer>(r.GetPrimaryKeyString());

                    state.Name = account.Name;
                }
            }

            else if (grainType == "AccountGrain")
            {
                var r = grainReference as IAccountGrain;

                var state = grainState as AccountGrainState;

                using (IDatabase db = BankorDbFactory.DbFactory.GetDatabase())
                {
                    var account = db.Single<Account>(r.GetPrimaryKeyString());

                    state.Balance = account.Balance;
                }
            }

            return Task.CompletedTask;
        }

        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            if (grainType == "AccountGrain")
            {
                var r = grainReference as IAccountGrain;

                var state = grainState as AccountGrainState;

                using (IDatabase db = BankorDbFactory.DbFactory.GetDatabase())
                {
                    var account = await db.SingleAsync<Account>(r.GetPrimaryKeyString());
                    db.StartSnapshot(account);

                    account.Balance = state.Balance;

                    db.Update(account);
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
