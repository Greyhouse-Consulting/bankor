using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
using BankOr.Core.Exceptions;
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

            var item = Math.Abs(Guid.NewGuid().GetHashCode());  
            State.AccountIds.Add(item);
            var account = GrainFactory.GetGrain<IAccountGrain>(item);
            State.AccountGrains.Add(account);
            await WriteStateAsync();

        }

        public async Task TryInit(string name)
        {
            await HasNewName(name);
        }

        private void EnsureCreated()
        {
            if (!State.Created)
                throw new GrainDoesNotExistException($"Customer with id '{this.GetPrimaryKeyLong()}' does not exist");
        }
    }
}