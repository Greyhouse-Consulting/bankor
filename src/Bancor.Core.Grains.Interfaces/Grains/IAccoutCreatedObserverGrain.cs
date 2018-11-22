using System.Threading.Tasks;
using Orleans;

namespace Bancor.Core.Grains.Interfaces.Grains
{
    public interface IAccoutCreatedObserverGrain : IGrainWithIntegerKey
    {
        Task StartSubscribe();
    }
}
