using Logger;
using Logger.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace MongoCrudPeopleApi.Controllers
{
    [Authorize]
    [Route("logs")]
    [Tags("z-Logs")]
    [ApiController]
    public class LogsController(LogService logservice) : Controller
    {

        private readonly LogService logdata = logservice;

        /// <summary>
        ///  api key : 32B66F391C7142F994974A99C509817B    
        ///  header : x-api-key
        /// </summary>
        /// <remarks>
        /// (time saved as UTC)    
        /// you can always manually edit the dates in the file; just make sure to respect the timestamp       
        /// verbose and debug level logs are saved only to file     
        /// 
        /// </remarks>
        /// <param name="body"> max lenght 50 : message - string </param>
        /// <param name="ex">enabling this will throw an exception</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Writetologger([FromBody][MaxLength(50)] string? body, LogLevelError level, [FromQuery(Name = "throw-exception")] bool ex = false)
        {


            await logdata.WriteLog(body, level, ex);

            return Ok();
        }

        /// <summary>
        /// retrieve logs based on filter criteria 
        /// </summary>
        /// <remarks>
        ///      
        /// if you are using the filter with hour/day parameters and your current time differs from UTC,
        /// convert your machine's local time to UTC since the logs are saved in UTC.
        ///        
        /// howtouse :   
        ///  - option1 -     
        ///         [singledate] startdate only  , year param is required ;the rest is optional    
        ///         {"startdate": {"year": 2024,"month": 1}}  --> all records from january year 2024.     
        ///         {"startdate": {"year": 2024,"day": 1,"hour": 12}}  ---> all records from the first day of each month that match midday (time is UTC).     
        ///         {"startdate": {"year": 2024,"second":21}} ---> all logs that start at the 21st second     
        ///         etc..    
        ///  - option2 -        
        ///         [rangedate] startdate and enddate ,  year-month-day parameters required .Enddate must be greater than startdate.(hour,min,sec are OPTIONal) if you are using hour-min-sec, hour must not be null.     
        ///         {"startdate": {"year": 2024,"month": 11,"day": 4},"enddate": {"year": 2024,"month": 11,"day": 30}}  ---> leaving timeonly unspecified will default to midnight : h00:m00:s00 .mean endate = day30-hour00-min00-sec00                           
        ///         {"startdate": {"year": 2024,"month": 11,"day": 4,"hour":14},"enddate": {"year": 2024,"month": 11,"day": 5 } }  ---> range : from y-m-day4-hour14 to y-m-day5-hour00-min00-sec00      
        ///         {"startdate": {"year": 2024,"month": 11, "day": 4,"hour": 14},"enddate": {"year": 2024,"month":11, "day": 5,"hour": 8 } }     
        /// </remarks>
        /// <param name="seek">Reads the last x lines from the file. Use only for a quick seek with option1.    
        /// Seek logic is triggered before the StreamRead line-by-line date matching (open the file, navigate to the last x lines, then pass the seek position to the StreamReader ) </param>
        /// <returns></returns>
        [HttpPost("readfile")]
        public async Task<IActionResult> Readfile([FromBody] DateDtomodel dateinput,uint? seek) // todo find logs by logerror + seek last x line
        {



            if (dateinput.Enddate is not null)
            {
                if (dateinput.Startdate.Hour is null && ( dateinput.Startdate.Minute is not null || dateinput.Startdate.Second is not null)) return BadRequest("hour param is null");

                if (dateinput.Enddate.Hour is null && (dateinput.Enddate.Minute is not null || dateinput.Enddate.Second is not null)) return BadRequest("hour param is null");

                if (!DateTime.TryParse(dateinput.Enddate.ToString(), out var end)) return BadRequest("invalid enddate ");

                if (!DateTime.TryParse(dateinput.Startdate.ToString(), out var start)) return BadRequest("invalid startdate ");

                if (start >= end) return BadRequest("enddate must be later then startdate");
            }

        

            var data = await logdata.ReadLog(dateinput, seek);

            return Ok(data);

          
        }

       


    }
}
