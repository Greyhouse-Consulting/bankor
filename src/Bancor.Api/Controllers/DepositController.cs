using System.Threading.Tasks;
using Bancor.Core.Grains.Interfaces.Grains;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Bancor.Api.Controllers
{

    [Route("api/customers/{customerId}/accounts/{accountId}/transfer")]

    public class DepositController : Controller
    {
        private readonly IClusterClient _clusterClient;

        public DepositController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpPost]
        public async Task<IActionResult> Post(int customerId, int accountId, [FromBody] DepositRequest request)
        {
            var account = _clusterClient.GetGrain<IAccountGrain>(accountId);

            await account.Deposit(request.Amount);

            return Created("/transactions/", "2020202");
        }
    }

    public class DepositRequest
    {
        public decimal Amount { get; set; } 
    }
}