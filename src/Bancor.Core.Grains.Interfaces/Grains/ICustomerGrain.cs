using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace Bancor.Core.Grains.Interfaces.Grains
{
    public interface ICustomerGrain : IGrainWithGuidKey
    {
        Task HasNewName(string name);
        Task<IList<Models.AccountModel>> GetAccounts();

        Task<Guid> CreateAccount(string name);
        Task TryInit(string name);
    }
}