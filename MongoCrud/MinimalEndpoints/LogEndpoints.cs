using Logger;
using Logger.Model.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace MongoCrudPeopleApi.MinimalEndpoints
{
    [Authorize]
    public static class LogEndpoints
    {
        public static async void UseLogEndpoints(this IEndpointRouteBuilder app)
        {

            var apiGroup = app.MapGroup("/logs").WithTags("Z-log").RequireAuthorization();

            apiGroup.MapPost("", Writetologger);
            apiGroup.MapPost("readfile", Readfile);

        }


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
        private static async Task<IResult> Writetologger(
            [FromBody][MaxLength(50)] string? body,
            LogLevelError level,
            LogService logdata,
            [FromQuery(Name = "throw-exception")] bool ex = false)
        {

            await logdata.WriteLog(body, level, ex);

            return Results.Ok();
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
        private static async Task<IResult> Readfile(
            [FromBody] DateDtomodel dateinput,
            uint? seek,
            LogService logdata
            )
        {

            if (dateinput.Enddate is not null)
            {
                if (dateinput.Startdate.Hour is null && (dateinput.Startdate.Minute is not null || dateinput.Startdate.Second is not null)) return Results.BadRequest("hour param is null");

                if (dateinput.Enddate.Hour is null && (dateinput.Enddate.Minute is not null || dateinput.Enddate.Second is not null)) return Results.BadRequest("hour param is null");

                if (!DateTime.TryParse(dateinput.Enddate.ToString(), out var end)) return Results.BadRequest("invalid enddate ");

                if (!DateTime.TryParse(dateinput.Startdate.ToString(), out var start)) return Results.BadRequest("invalid startdate ");

                if (start >= end) return Results.BadRequest("enddate must be later then startdate");
            }



            var data = await logdata.ReadLog(dateinput, seek);

            return Results.Text(data);
        }






    }
}
