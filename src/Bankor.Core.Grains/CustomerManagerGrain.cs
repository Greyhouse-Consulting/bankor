using System;
using System.Threading.Tasks;
using Bancor.Core;
using Bancor.Core.Grains.Interfaces.Grains;
using Bancor.Core.Grains.Interfaces.Repository;
using Orleans;

namespace Bankor.Core.Grains
{
    public class CustomerManagerGrain : Grain, ICustomerManagerGrain
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerManagerGrain(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<ICustomerGrain> Create(string name)
        {
            var customer = new Customer
            {
                Name = name
            };

            var customerId = Math.Abs(Guid.NewGuid().GetHashCode());

            //var customerId = await _customerRepository.Insert(customer);

            var customerGrain = GrainFactory.GetGrain<ICustomerGrain>(customerId);

            await customerGrain.TryInit(name);

            return customerGrain;
        }

    }
}