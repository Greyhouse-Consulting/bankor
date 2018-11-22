using System.Threading.Tasks;
using Bancor.Core.Exceptions;
using Bancor.Core.Grains.Interfaces.Grains;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Bancor.Api.Controllers
{
    [Route("api/customers/{customerId}/accounts")]
    public class AccountsController : Controller
    {
        private readonly IClusterClient _clusterClient;

        public AccountsController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get(int customerId)
        {
            var customers = _clusterClient.GetGrain<ICustomerGrain>(customerId);

            var accounts = await customers.GetAccounts();

            return Ok(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> Post(int customerId, [FromBody]CreateAccountRequest request)
        {
            var customer = _clusterClient.GetGrain<ICustomerGrain>(customerId);

            try
            {
                await customer.CreateAccount(request.Name);
            }
            catch (GrainDoesNotExistException e)
            {
                return NotFound(customerId);
            }

            return Ok();
        }
    }

    public class CreateAccountRequest
    {
        public string Name { get; set; }
    }
}
