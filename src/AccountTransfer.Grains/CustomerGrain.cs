using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountTransfer.Interfaces;
using BankOr.Core;
using Orleans;
using Orleans.Providers;

namespace AccountTransfer.Grains
{

    [StorageProvider(ProviderName="BankOrStorageProvider")]
    public class CustomerGrain : Grain<CustomerGrainState>, ICustomerGrain
    {
        public async Task HasNewName(string name)
        {
            this.State.Name = name;
            await this.WriteStateAsync();
        }



        public async Task<IList<string>> GetAccounts()
        {
            var accountNames = new List<string>();
            foreach (var accountId in State.AccountIds)
            {
                var account = GrainFactory.GetGrain<IAccountGrain>(accountId);

                accountNames.Add(await account.GetName());
            }

            return accountNames;
        }

        public async Task CreateAccount(string name)
        {
            var account = GrainFactory.GetGrain<IAccountGrain>(Guid.NewGuid());

            await account.SetName(name);

            State.AccountIds.Add(account.GetPrimaryKey());
        }
    }
}