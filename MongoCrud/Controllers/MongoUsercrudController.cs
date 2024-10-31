using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoCrudPeopleApi.Model;
using Mongodb;
using Mongodb.Models;
using Mongodb.Models.Dto;
using MongoLogic.CRUD;
using Serilog.Core;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
namespace MongoCrudPeopleApi.Controllers;


[Route("api/user")]
[Tags("User")]
[ApiController]
public class MongoUsercrudController(IPeopleservice peopleservice) : ControllerBase
{


    private readonly IPeopleservice dbcall = peopleservice;



    /// <summary>
    /// 
    /// </summary>
    /// <param name="Pagesize"> default = 100 items </param>
    /// <returns></returns>
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PersonBaseModel>))]
    public async Task<IActionResult> Getusersearch([FromQuery] UsersModel search, [FromQuery] Pagesize Pagesize = Pagesize.i100)
    {
        var data = await dbcall.SearchUsers(search, Pagesize);


       


        return data is null ? NotFound() : Ok(data);


        
    }





    /// <summary>
    /// </summary>
    /// <remarks>
    ///Swagger can't handle the load (+1000 return items), so the result of this endpoint will be truncated       
    ///If you want to check the real limits, make a GET request directly in the browser or with Postman, etc.        
    /// like : /api/user/byage-nolimit?minage=1&amp;maxage=100      
    /// </remarks>
    /// <returns></returns>
    [HttpGet("byage-limit")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PersonBaseModel>))]
    public async Task<IActionResult> GetbyAgeRange(
    [FromQuery][Required][Range(0, 150)] int minage,
    [FromQuery][Required][Range(0, 150)] int maxage) // add page with nitems hardcoded 
    {

        if (minage > maxage) return BadRequest("minAge > maxAge");


        var Result = await dbcall.GetAgerangeUserItem(minage, maxage);


        if (Result is null) return NotFound();
        else
        {

            // we can also truncate with .limit() in the DB query to save resources 
            // like await dbcall.GetAgerangeUserItem(minage, maxage ,  /*  truncate-option : bool/enum */)

            if (Result.Count < 100) return Ok(Result);

            var truncList = Result.Take(100).ToList();

            return Ok(truncList);
        }



    }






    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("byage-nolimit")]
    public async Task<IActionResult> GetbyAgeRange_nolimit(
       [FromQuery][Required][Range(0, 150)] int minage,
       [FromQuery][Required][Range(0, 150)] int maxage)
    {

        if (minage > maxage) return BadRequest("minAge > maxAge");


        var Result = await dbcall.GetAgerangeUserItem(minage, maxage);


        return Result is null ? NotFound() : Ok(Result);



    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// userid(uuid) OR email
    /// </remarks>
    /// <param name="patchdata"> Optional parameters; at least one is required </param>
    /// <returns></returns>
    [HttpPatch]
    public async Task<IActionResult> PatchUserItem([FromQuery] PatchUserItemModel request, [FromBody][Required] UserItemPatchModel patchdata)
    {

        if (request.userid is null && request.email is null) return BadRequest();
        if (request.userid is not null && request.email is not null) return BadRequest("choose one ");

        var statuscode = await dbcall.PatchUseritem(request, patchdata);


        return StatusCode(statuscode);



    }



    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// likethis : { "firstname": [ "marice", "mario" ], "lastname": [ "mortensen", "fortin" ] ,"age": [40,41,42,43]}
    /// </remarks>
    /// <param name="Pagesize">default = 100</param>
    /// <returns></returns>
    [HttpPost("bulk-search")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PersonBaseModel>))]
    public async Task<IActionResult> Bulksearch(BulkUserModel search, [FromQuery] Pagesize Pagesize = Pagesize.i100)
    {

        var data = await dbcall.BulkSearchUsers(search, Pagesize);

        return data is null ? NotFound() : Ok(data);
    }










    /// <summary>
    ///  insert User data 1:1 from https://randomuser.me/api?results=3 , copy and paste in the post here . database is empty by default
    /// </summary>
    /// <param name="dupecheck"> check duplicates before insert in the database (true,false) , This does not remove existing records in the database</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> PostUserItemModel([FromBody][Required] PersonApiModel Value, bool dupecheck = true)
    {

        if (Value.Results.Count == 0) return BadRequest();
        else if (Value.Results.Count > 1000)
        {
            return BadRequest("POST too big");
        }


        var (code, messg) = await dbcall.Insert(Value, dupecheck);


        return StatusCode(code, messg);
    }






    /// <summary>
    /// deleteby uuid or email  --> uuid : guid , email : string
    /// </summary>
    /// <param name="uuid_email"></param>
    /// <returns></returns>
    [HttpDelete("{uuid_email}")]
    public async Task<IActionResult> Delete(string uuid_email)
    {
        bool status;

        if (Guid.TryParse(uuid_email, out var uuid))
        {
            status = await dbcall.RemoveItem(uuid);
        }
        else if (Regex.IsMatch(uuid_email, emailregex))
        {
            status = await dbcall.RemoveItem(uuid_email);
        }
        else return BadRequest();


        return status ? Ok() : NotFound();


    }

   const string emailregex = @"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,}$";



   

}
