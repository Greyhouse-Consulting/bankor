using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans;

namespace AccountTransfer.Interfaces.Grains
{
    public interface ICustomerGrain : IGrainWithIntegerKey
    {
        Task HasNewName(string name);
        Task<IList<string>> GetAccounts();

        Task CreateAccount(string name);
        Task TryInit(string name);
    }
}