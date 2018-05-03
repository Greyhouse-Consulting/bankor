using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountTransfer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Bank.Api.Controllers
{
    [Route("api/accounts")]
    public class AccountsController : Controller
    {
        private readonly IClusterClient _clusterClient;

        public AccountsController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var accountManager = _clusterClient.GetGrain<IAccountManagerGrain>(0);

            var account = await accountManager.GetAccounts();
            return Ok();
        }
    }
}
