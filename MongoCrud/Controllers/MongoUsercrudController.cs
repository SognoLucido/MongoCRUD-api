using Microsoft.AspNetCore.Mvc;
using MongoCrudPeopleApi.Apimodels;
using Mongodb;
using Mongodb.Models;
using Mongodb.Models.Dto;
using MongoDB.Bson;
using MongoLogic.CRUD;
using MongoLogic.model.Api;
using System.ComponentModel.DataAnnotations;



namespace MongoCrudPeopleApi.Controllers;



public class User
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}



[Route("api/user")]
[ApiController]
public class MongoUsercrudController : ControllerBase
{

    
    private readonly IPeopleservice dbcall;


    public MongoUsercrudController(IPeopleservice peopleservice,MongoContext context )
    {
        dbcall = peopleservice;

    }




    [HttpGet("search")]
    public async Task<IActionResult> Getusersearch([FromQuery]Usersearch search)
    {
        var data = await dbcall.SearchUsers(search);

        return data is null ? NotFound(): Ok(data);
    }






    /// <summary>
    /// </summary>
    /// <remarks>
    ///Swagger can't handle the load, so the result of this endpoint will be truncated       
    ///If you want to check the real limits, make a GET request directly in the browser or with Postman, etc.        
    /// like : /api/user/byage-nolimit?minage=1&amp;maxage=100      
    /// </remarks>
    /// <returns></returns>
    [HttpGet("byage-limit")]
    public async Task<IActionResult> GetbyAgeRange(
        [FromQuery][Required][Range(0, 150)] int minage,
        [FromQuery][Required][Range(0, 150)] int maxage)
    {

        if (minage > maxage) return BadRequest("minAge > maxAge");


        var Result = await dbcall.GetAgerangeUserItem(minage, maxage);


        if (Result is null) return NotFound();
        else
        {
            if (Result.Count < 100) return Ok(Result);

            var truncList = Result.Take(100).ToList();

            return Ok(truncList);
        }



    }



    //[HttpGet("YOZ")]
    //public async Task<IActionResult> TestEndpoint()
    //{

    //   await dbcall.TestLog();

    //     return Ok();
    //}


    //---------------------- LOG ENDPOINT = READFILE AND FILTER IT BY ENDPOINT/QUERY/ERR-WARNING-info/ LIMIT last 5 last 20 


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








    //[HttpGet]
    //public async Task<IActionResult> GetOne([FromRoute]MongoId Mongo_Id1)
    //{
    //    var data = await dbcall.Findby_id(Mongo_Id1.MongoObject_Id);

    //    if (data.Item2 == 200 && data.Item1 is not null)
    //    {
    //        return Ok(data.Item1);
    //    }
    //    else if (data.Item1 is null)
    //    {
    //        return NotFound();
    //    }
    //    else
    //    {
    //        return StatusCode(500, "server error");
    //    }



    //}



    //[HttpGet]
    //public async Task<IActionResult> GetFromQuerypara([FromQuery] FindsingleModel findbyone)
    //{

    //    if (ModelState.IsValid)
    //    {
    //        if (findbyone.Name is null && findbyone.LastName is null && findbyone.LoginUuid is null)
    //        {
    //            return BadRequest("At least one parameter is required");
    //        }


    //        //if((findbyone.Name is null || findbyone.LastName is null) && findbyone.LoginId is null)
    //        //{
    //        //    return BadRequest("First name and Last Name Both required");
    //        //}

    //    }


    //    var datafromdb = await dbcall.FindbyCustomQuary(findbyone);


    //    if (datafromdb is null)
    //    {
    //        return StatusCode(500, "server error");
    //    }
    //    if (datafromdb.Count == 0)
    //    {
    //        return StatusCode(404, "record Notfound");
    //    }
    //    else
    //    {
    //        return Ok(datafromdb);
    //    }


    //}


    ////Task<long?> GetTotalItems()
    //[HttpGet("totalitems")]
    //public async Task<IActionResult> GetCountitems()
    //{
       
    //    long? count = await dbcall.GetTotalItems();


    //    if (count is null)
    //    {
    //        return StatusCode(500, "An internal server error occurred");
    //    }
    //    else return Ok(count);


    //}

    
    //[HttpGet("{Randomppl}")]
    //public async Task<IActionResult> GetrandomPerson([Range(1,50)]int Randomppl)
    //{


    //  List<PersonDbModel>? result =  await dbcall.GetXitems(Randomppl);

    //    if (result is null)
    //    {
    //        return StatusCode(500, "An internal server error occurred");
    //    }
    //    else return Ok(result);



    //}

    /// <summary>
    ///  insert User data 1:1 from https://randomuser.me/api?results=3 , copy and paste in the post here
    /// </summary>
    /// <param name="dupecheck"> check duplicates before insert in the database (true,false) , This does not remove existing records in the database</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> PostUserItemModel([FromBody][Required] PersonApiModel Value,bool dupecheck = true)
    {

        if (Value.Results.Count == 0) return BadRequest();
        else if (Value.Results.Count > 1000)
        {
            return BadRequest("POST too big");
        }
       

        var (code,messg) = await dbcall.Insert(Value,dupecheck);


        return StatusCode(code,messg);
    }





    //[HttpPatch("{Mongo_Id}")]
    //public async Task<IActionResult> Patch(MongoId Mongo_Id, [FromBody] PutPersonmodel model  )
    //{



    //    short result = await dbcall.UpdateAsync(Mongo_Id.MongoObject_Id, model);

    //    if (result == 200)
    //    {
    //        return Ok("Successfully updated");
    //    }
    //    else if (result == 400)
    //    {
    //        return BadRequest("No changes detected.");
    //    }
    //    else if (result == 404)
    //    {
    //        return NotFound();
    //    }
    //    else
    //    {
    //        return StatusCode(500, "Server on fire");
    //    }

    //}



    //[HttpPut("{Mongo_Id}")]
    //public async Task<IActionResult> Put(MongoId Mongo_Id, [FromBody] PersonApiModel model)
    //{

    //    if (model.Results.Count != 1)
    //    {
    //        return BadRequest("1 PUT record at time");
    //    }



    //    short result = await dbcall.PutAsyncFullModel(Mongo_Id.MongoObject_Id, model);



    //    if (result == 200)
    //    {
    //        return Ok("Successfully updated");
    //    }
    //    else if (result == 400)
    //    {
    //        return BadRequest("No changes detected.");
    //    }
    //    else if (result == 404)
    //    {
    //        return NotFound();
    //    }
    //    else
    //    {
    //        return StatusCode(500, "An internal server error occurred");
    //    }

    //}




    //[HttpDelete("{Mongo_Id}")]
    //public async Task<IActionResult> Delete(MongoId Mongo_Id)
    //{
    //    byte result = await dbcall.RemoveAsync(Mongo_Id.MongoObject_Id);

    //    if (result > 0)
    //    {
    //        return Ok();
    //    }

    //    return StatusCode(404, "record Notfound");


    //}







}
