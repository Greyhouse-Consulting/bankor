using System;
using System.Threading.Tasks;
using Bancor.Core.Grains.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Bancor.Api.Controllers
{
    [Route("accounts")]
    public class AccountsController : Controller
    {
        private readonly IClusterClient _clusterClient;

        public AccountsController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }
        [HttpGet]
        public async Task<ActionResult> Get(Guid accountId)
        {
            var account = _clusterClient.GetGrain<IJournaledAccountGrain>(accountId);

            var accountContract = new AccountContract
            {
                Balance = await account.Balance(),
                Name = await account.Name(),
                Id = accountId
            };

            return Ok(accountContract);
        }

    }

    public class AccountContract
    {
        public decimal Balance { get; set; }
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}