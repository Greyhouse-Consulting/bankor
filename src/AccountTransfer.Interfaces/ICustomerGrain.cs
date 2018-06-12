using System.Threading.Tasks;
using Orleans;

namespace AccountTransfer.Interfaces
{
    public interface ICustomerGrain : IGrainWithIntegerKey
    {
        Task HasNewName(string name);
    }
}