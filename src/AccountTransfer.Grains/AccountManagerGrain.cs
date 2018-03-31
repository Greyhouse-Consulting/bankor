using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using AccountTransfer.Interfaces;
using BankOr.Core;
using Orleans;

namespace AccountTransfer.Grains
{
    public class AccountManagerGrain : Grain, IAccontManagerGrain
    {
        public Task<IBankAccountGrain> Create(string userId)
        {
            var bankAccountGrain = GrainFactory.GetGrain<IBankAccountGrain>(Guid.NewGuid());
            bankAccountGrain.Owner(userId);

            return Task.FromResult(bankAccountGrain);
        }

        public Task<IList<BankAccountModel>> GetAccounts()
        {
            throw new NotImplementedException();
        }
    }
}