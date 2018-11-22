using System.Threading.Tasks;
using Bancor.Core.Grains.Interfaces.Grains;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Bancor.Api.Controllers
{
    [Route("api/users/{id}/accounts")]
    public class UserAccountsController : Controller
    {
        private readonly IClusterClient _clusterClient;

        public UserAccountsController(IClusterClient clusterClient)
        {
            _clusterClient = clusterClient;
        }

        [HttpPost]
        public async Task<IActionResult> Post(string id)
        {
            var accountManger = _clusterClient.GetGrain<ICustomerManagerGrain>(0);

            var userAccount = await accountManger.Create(id);

            return Created("/accounts/", userAccount.GetPrimaryKey());
        }
    }
}
