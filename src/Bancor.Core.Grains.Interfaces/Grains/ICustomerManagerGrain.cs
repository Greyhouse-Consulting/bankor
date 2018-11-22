using System.Threading.Tasks;
using Orleans;

namespace Bancor.Core.Grains.Interfaces.Grains
{
    public interface ICustomerManagerGrain : IGrainWithIntegerKey
    {
        Task<ICustomerGrain> Create(string name);
    }
}