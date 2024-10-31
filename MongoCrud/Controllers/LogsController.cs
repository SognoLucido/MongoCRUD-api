using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mongodb;
using MongoLogic.CRUD;
using System.Security.Claims;

namespace MongoCrudPeopleApi.Controllers
{
    [Route("logs")]
    [Tags("z-Logs")]
    [ApiController]
    public class LogsController(IPeopleservice peopleservice) : Controller
    {


        private readonly IPeopleservice dbcall = peopleservice;

        [HttpGet]
        public async Task<IActionResult> Todo()
        {

            await dbcall.TestLog();

            return Ok();
        }


        [HttpGet("readfile")]
        public async Task<IActionResult> Readfilestest()
        {

            await dbcall.TestLogv2();

            return Ok();
        }

    }
}
