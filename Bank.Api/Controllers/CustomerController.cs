using System;
using System.Threading.Tasks;
using AccountTransfer.Interfaces.Grains;
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

        // POST

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateCutomerRequest request)
        {
            var customerManager = _clusterClient.GetGrain<ICustomerManagerGrain>(0);

            var customer = await customerManager.Create(request.Name);

            return Ok(customer.GetPrimaryKeyLong());
        }

        // GET
        [HttpGet]
        public IActionResult Get()
        {
            var customer = _clusterClient.GetGrain<ICustomerGrain>(200);

            return Ok(customer.GetPrimaryKey());
        }
    }

    public class CreateCutomerRequest
    {
        public string Name { get; set; }
    }
}