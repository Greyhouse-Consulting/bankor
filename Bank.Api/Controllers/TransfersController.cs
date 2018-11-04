using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Bank.Api.Controllers
{

    [Route("api/transfers")]
    //[Route("api/customers/{customerId}/accounts")]
    public class TransfersController : Controller
    {
        private readonly IClusterClient _clusterClient;

        public TransfersController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TransferRequest request)
        {
            var transferManager = _clusterClient.GetGrain<IBankTransferGrain>(0);

            await transferManager.Transfer(request.FromAccount, request.ToAccount, request.Amount);

            return Ok();
        }
    }

    public class TransferRequest
    {
        public long FromAccount { get; set; }
        public long ToAccount { get; set; }

        public decimal Amount { get; set; }
    }
}