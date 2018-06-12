using System.Threading.Tasks;
using AccountTransfer.Interfaces;
using Orleans;
using Orleans.Providers;

namespace AccountTransfer.Grains
{

    [StorageProvider(ProviderName="DevStore")]
    public class CustomerGrain : Grain<CustomerGrainState>, ICustomerGrain
    {
        public async Task HasNewName(string name)
        {
            this.State.Name = name;
            await this.WriteStateAsync();
        }
    }
}