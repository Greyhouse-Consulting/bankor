using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountTransfer.Interfaces;
using BankOr.Core;
using Orleans;

namespace AccountTransfer.Grains
{
    public class AccountManagerGrain : Grain, IAccountManagerGrain
    {
        public async  Task<IAccountGrain> Create(string userId)
        {
                var bankAccountGrain = GrainFactory.GetGrain<IAccountGrain>(Guid.NewGuid());
                await bankAccountGrain.Owner(userId);

                return bankAccountGrain;
            }

        public Task<IList<Account>> GetAccounts(string customerId)
        {
            throw new NotImplementedException();
        }

    }
}