using System;
using System.Threading.Tasks;
using Bancor.Core.Grains.Interfaces.Grains;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Bancor.Api.Controllers
{
    [Route("customers")]
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

            var x = _clusterClient.GetGrain<IAccoutCreatedObserverGrain>(0);
            await x.StartSubscribe();
            return Ok(customer.GetPrimaryKey());
        }


        // GET /customers/
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var customerManager = _clusterClient.GetGrain<ICustomerManagerGrain>(0);

            return Ok(await customerManager.GetAll());
        }

        // GET /customers/{id}
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var customer = _clusterClient.GetGrain<ICustomerGrain>(id);

            return Ok(customer.GetPrimaryKey());
        }
    }

    public class CreateCutomerRequest
    {
        public string Name { get; set; }
    }
}