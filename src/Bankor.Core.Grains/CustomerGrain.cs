using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bancor.Core;
using Bancor.Core.Exceptions;
using Bancor.Core.Grains.Interfaces.Grains;
using Bancor.Core.Models;
using Orleans;
using Orleans.Providers;

namespace Bankor.Core.Grains
{

    [StorageProvider(ProviderName = "CustomerStorageProvider")]
    public class CustomerGrain : Grain<CustomerGrainState>, ICustomerGrain
    {
        public async Task HasNewName(string name)
        {

            State.Name = name;
            await WriteStateAsync();
        }

        public async Task<IList<AccountModel>> GetAccounts()
        {
            EnsureCreated();

            var accountNames = new List<AccountModel>();
            foreach (var account in State.AccountGrains)
            {
                accountNames.Add(new AccountModel{
                   Name = await account.GetName(),
                    Id = account.GetPrimaryKeyLong(),
                    Balance = await account.GetBalance()
                });
            }

            return accountNames;
        }


        public async Task CreateAccount(string name)
        {
            EnsureCreated();

            var item = Math.Abs(Guid.NewGuid().GetHashCode());  
            var account = GrainFactory.GetGrain<IAccountGrain>(item);

            await account.HasNewName(name);
            State.AccountGrains.Add(account);

            await NotifyNewAccount(item);

            await WriteStateAsync();

        }

        private async Task NotifyNewAccount(int item)
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<int>(StreamIdGenerator.StreamId, "ACCOUNTID");
            await stream.OnNextAsync(item);
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