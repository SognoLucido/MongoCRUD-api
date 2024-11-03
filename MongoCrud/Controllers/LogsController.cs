using Logger;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace MongoCrudPeopleApi.Controllers
{
    [Route("logs")]
    [Tags("z-Logs")]
    [ApiController]
    public class LogsController(LogService logservice) : Controller
    {

        private readonly LogService  logdata = logservice;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="body"> max lenght 20 : message - string </param>
        /// <param name="ex">enabling this will throw an exception</param>
        /// <returns></returns>
        [HttpPost]  
        public async Task<IActionResult> Writetologger([FromBody][MaxLength(50)]string? body , LogLevelError level , [FromQuery(Name = "throw-exception")] bool ex = false)
        {


            await logdata.WriteLog(body,level,ex);

            return Ok();
        }

        /// <summary>
        /// retrieve logs based on filter criteria
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="start"></param>
        /// <param name="end">optional</param>
        /// <returns></returns>
        [HttpGet("readfile")]
        public async Task<IActionResult> Readfile(DateTime start/*, DateTime? end*/)
        {




            var data = await logdata.ReadLog(start);

            return Ok(data);

            //return Ok(start);
        }

    }
}
