using System;
using AccountTransfer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Bank.Api.Controllers
{
    [Route("api/customers")]
    public class CustomerController : Controller
    {
        private readonly IClusterClient _clusterClient;

        public CustomerController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }
        // GET

        [HttpGet]
        public IActionResult Get()
        {
            var customer = _clusterClient.GetGrain<ICustomerGrain>(200);

            customer.HasNewName("Kalle");
            return Ok(customer.GetPrimaryKey());
        }
    }
}