using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bancor.Core.Exceptions;
using Bancor.Core.Grains.Interfaces;
using Bancor.Core.Grains.Interfaces.Grains;
using Bancor.Core.Models;
using Orleans;
using Orleans.Providers;

namespace Bancor.Core.Grains
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
                accountNames.Add(new AccountModel
                {
                    Name = await account.Name(),
                    Id = account.GetPrimaryKey(),
                    Balance = await account.Balance()
                });
            }

            return accountNames;
        }


        public async Task<Guid> CreateAccount(string name)
        {
            EnsureCreated();

            //var item = Math.Abs(Guid.NewGuid().GetHashCode());
            var accountId = Guid.NewGuid();
            var account = GrainFactory.GetGrain<IJournaledAccountGrain>(accountId);

            await account.HasName(name);
            State.AccountGrains.Add(account);

            await NotifyNewAccount(accountId.ToString());

            await WriteStateAsync();

            return accountId;
        }

        private async Task NotifyNewAccount(string accountId)
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<string>(StreamIdGenerator.StreamId, "ACCOUNTID");
            await stream.OnNextAsync(accountId);
        }

        public async Task TryInit(string name)
        {
            await HasNewName(name);
        }

        private void EnsureCreated()
        {
            if (!State.Created)
                throw new GrainDoesNotExistException($"Customer with id '{this.GetPrimaryKey()}' does not exist");
        }
    }
}