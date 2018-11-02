using System;
using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Bank.Api.Controllers
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
        public async Task<IActionResult> Post(int customerId, int accountId, [FromBody] TransferRequest request)
        {
            var account = _clusterClient.GetGrain<IAccountGrain>(accountId);

            await account.Deposit(request.Amount);

            return Created(new Uri("/transactions/"), "200");
        }
    }

    public class TransferRequest
    {
        public decimal Amount { get; set; } 
    }
}