using System;
using System.Linq;
using System.Threading.Tasks;
using Bancor.Core;
using Bancor.Core.Exceptions;
using Bancor.Core.Grains.Interfaces;
using Bancor.Core.Grains.Interfaces.Grains;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Serilog;

namespace Bancor.Api.Controllers
{
    [Route("customers/{customerId}/accounts")]
    public class CustomerAccountsController : Controller
    {
        private readonly IClusterClient _clusterClient;

        public CustomerAccountsController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid customerId)
        {
            //var customers = _clusterClient.GetGrain<IJournaledAccountGrain>(Guid.Parse("EBD09F9C-4A99-4B8D-A581-3C93764D24B1"));
            var customer = _clusterClient.GetGrain<ICustomerGrain>(customerId);

            return Ok((await customer.GetAccounts()).Select(a => a.Id));
        }

        [HttpPost]
        public async Task<IActionResult> Post(Guid customerId, [FromBody]CreateAccountRequest request)
        {
            var customer = _clusterClient.GetGrain<ICustomerGrain>(customerId);

            try
            {
                return Ok(await customer.CreateAccount(request.Name));
            }
            catch (GrainDoesNotExistException e)
            {
                Log.Warning(e, "Customer {grainId} does not exists", customerId);
                return NotFound(customerId);
            }

            return Ok();
        }

        [HttpPost("{accountId}/transactions")]
        public async Task<IActionResult> Post(Guid customerId, Guid accountId, TransactionModel transaction)
        {
            var account = _clusterClient.GetGrain<IJournaledAccountGrain>(accountId);
            
            await account.AddTransaction(new Transaction
            {
                Amount = transaction.Amount,
                BookingDate = transaction.BookingDate
            });

            return Ok();
        }
    }

    public class TransactionModel
    {
        public decimal Amount { get; set; }
        public DateTime BookingDate { get; set; }
    }

    public class CreateAccountRequest
    {
        public string Name { get; set; }
    }
}
