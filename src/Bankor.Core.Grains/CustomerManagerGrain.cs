using System;
using System.Threading.Tasks;
using Bancor.Core.Grains.Interfaces.Grains;
using Orleans;

namespace Bancor.Core.Grains
{
    public class CustomerManagerGrain : Grain, ICustomerManagerGrain
    {
        public async Task<ICustomerGrain> Create(string name)
        {
            var customer = new Customer
            {
                Name = name
            };

            var customerId = Guid.NewGuid();

            var customerGrain = GrainFactory.GetGrain<ICustomerGrain>(customerId);

            await customerGrain.TryInit(name);

            return customerGrain;
        }

    }
}