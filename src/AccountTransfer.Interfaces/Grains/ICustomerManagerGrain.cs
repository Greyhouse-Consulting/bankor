using System.Threading.Tasks;
using Orleans;

namespace AccountTransfer.Interfaces.Grains
{
    public interface ICustomerManagerGrain : IGrainWithIntegerKey
    {
        Task<ICustomerGrain> Create(string name);
    }
}