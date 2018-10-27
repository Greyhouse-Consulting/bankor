using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
using Orleans;
using Orleans.Providers;

namespace AccountTransfer.Grains
{

    [StorageProvider(ProviderName = "BankOrStorageProvider")]
    public class CustomerGrain : Grain<CustomerGrainState>, ICustomerGrain
    {
        public async Task HasNewName(string name)
        {

                State.Name = name;
            await WriteStateAsync();
        }

        public async Task<IList<string>> GetAccounts()
        {
            EnsureCreated();

            var accountNames = new List<string>();
            foreach (var accountId in State.AccountIds)
            {
                var account = GrainFactory.GetGrain<IAccountGrain>(accountId);

                accountNames.Add(await account.GetName());
            }

            return accountNames;
        }


        //[Transaction(TransactionOption.Supported)]
        public async Task CreateAccount(string name)
        {
            EnsureCreated();
            //var account = GrainFactory.GetGrain<IAccountGrain>(Guid.NewGuid());

            //await account.SetName(name);

            State.AccountIds.Add(Guid.NewGuid().GetHashCode());

            await WriteStateAsync();

        }

        public async Task TryInit(string name)
        {
            await HasNewName(name);
        }

        private void EnsureCreated()
        {
            if (!State.Created)
                throw new Exception($"Customer with id '{this.GetPrimaryKeyLong()}'");
        }
    }
}