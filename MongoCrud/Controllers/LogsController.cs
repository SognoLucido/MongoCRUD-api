using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MongoCrudPeopleApi.Controllers
{
    [Route("logs")]
    [ApiController]
    public class LogsController : Controller
    {


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Todo()
        {
           
          

            return Ok();
        }

    }
}
