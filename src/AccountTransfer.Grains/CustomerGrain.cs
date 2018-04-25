using AccountTransfer.Interfaces;
using Orleans;
using Orleans.Providers;

namespace AccountTransfer.Grains
{

    [StorageProvider(ProviderName="DevStore")]
    public class CustomerGrain : Grain<UserGrainState>, ICustomerGrain
    {
        
    }

    public class UserGrainState
    {

    }
}