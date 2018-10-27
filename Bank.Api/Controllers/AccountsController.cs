using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Bank.Api.Controllers
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
        public IActionResult Get(int customerId)
        {
            var customers = _clusterClient.GetGrain<ICustomerGrain>(customerId);

            var accounts = customers.GetAccounts();

            return Ok(accounts);
        }

        [HttpPost]
        public async Task<IActionResult> Post(int customerId, [FromBody]string name)
        {
            var customer = _clusterClient.GetGrain<ICustomerGrain>(customerId);

            await customer.CreateAccount(name);

            return Ok();
        }
    }
}
