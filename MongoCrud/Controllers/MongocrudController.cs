using Microsoft.AspNetCore.Mvc;
using MongoCrudPeopleApi.Apimodels;
using Mongodb;
using Mongodb.Models;
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



[Route("api")]
[ApiController]
public class MongocrudController : ControllerBase
{

    
    private readonly IPeopleservice _peopleservice;
    //private readonly IManualmapper _mapper;
    //private readonly MongoContext _context;



    public MongocrudController(IPeopleservice peopleservice,/* IManualmapper mapper,*/MongoContext context )
    {
        _peopleservice = peopleservice;
     

    }




    //[HttpGet("YOZ")]
    //public async Task<IActionResult> TestEndpoint() 
    //{


    //   ////var database = _context.GetDatabase(Database.Peopledb.ToString());
    //   //var collection = _context.GetCollection<User>("Users");


    //   // var usertest = new User()
    //   // {
    //   //     Email = "fra@test.com",
    //   //     Name = "fra"
    //   // };


    //   //await  collection.InsertOneAsync(usertest);


    //   // return Ok();
    //}












    //[ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet("byage")]  
    public async Task<IActionResult> GetbyAgeRange([FromQuery] AgerangeModel age)
    {


        if (ModelState.IsValid)
        {
            if (age.MinAge > age.MaxAge || age.MinAge == age.MaxAge) return BadRequest("minAge < maxAge");
            //else if ((age.MaxAge - age.MinAge) > 10) return BadRequest("max range cap 10 maxAge-minAge");
        }
        else
        {
            return BadRequest("invalid Query");
        }

        var Resultcode = await _peopleservice.Agerange(age.MinAge, age.MaxAge);



        if (Resultcode.Item2 == 404) return NotFound();
        else if (Resultcode.Item2 == 200) return Ok(Resultcode.Item1);
        else return StatusCode(500, "ops server gone");


    }




    [HttpGet("{Mongo_Id}")]
    public async Task<IActionResult> GetOne(MongoId Mongo_Id)
    {
        var data = await _peopleservice.Findby_id(Mongo_Id.MongoObject_Id);

        if (data.Item2 == 200 && data.Item1 is not null)
        {
            return Ok(data.Item1);
        }
        else if (data.Item1 is null)
        {
            return NotFound();
        }
        else
        {
            return StatusCode(500, "server error");
        }



    }



    [HttpGet]
    public async Task<IActionResult> GetFromQuerypara([FromQuery] FindsingleModel findbyone)
    {

        if (ModelState.IsValid)
        {
            if (findbyone.Name is null && findbyone.LastName is null && findbyone.LoginUuid is null)
            {
                return BadRequest("At least one parameter is required");
            }


            //if((findbyone.Name is null || findbyone.LastName is null) && findbyone.LoginId is null)
            //{
            //    return BadRequest("First name and Last Name Both required");
            //}

        }


        var datafromdb = await _peopleservice.FindbyCustomQuary(findbyone);


        if (datafromdb is null)
        {
            return StatusCode(500, "server error");
        }
        if (datafromdb.Count == 0)
        {
            return StatusCode(404, "record Notfound");
        }
        else
        {
            return Ok(datafromdb);
        }


    }


    //Task<long?> GetTotalItems()
    [HttpGet("totalitems")]
    public async Task<IActionResult> GetCountitems()
    {
       
        long? count = await _peopleservice.GetTotalItems();


        if (count is null)
        {
            return StatusCode(500, "An internal server error occurred");
        }
        else return Ok(count);


    }

    
    [HttpGet("{Randomppl}")]
    public async Task<IActionResult> GetrandomPerson([Range(1,50)]int Randomppl)
    {




      List<PersonDbModel>? result =  await  _peopleservice.GetXitems(Randomppl);

        if (result is null)
        {
            return StatusCode(500, "An internal server error occurred");
        }
        else return Ok(result);



    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] PersonApiModel Value,bool dupecheck = true)
    {
        if(Value.Results.Count > 1000)
        {
            return BadRequest("POST too big");
        }

        string result = await _peopleservice.Insert(Value,dupecheck);

        //if (result == "500")
        //{
        //    return StatusCode(500, "An internal server error occurred");
        //}
        //else
        //{
        //    return Ok(result);
        //}

        return Ok(result);
    }





    [HttpPatch("{Mongo_Id}")]
    public async Task<IActionResult> Patch(MongoId Mongo_Id, [FromBody] PutPersonmodel model  )
    {



        short result = await _peopleservice.UpdateAsync(Mongo_Id.MongoObject_Id, model);

        if (result == 200)
        {
            return Ok("Successfully updated");
        }
        else if (result == 400)
        {
            return BadRequest("No changes detected.");
        }
        else if (result == 404)
        {
            return NotFound();
        }
        else
        {
            return StatusCode(500, "Server on fire");
        }

    }



    [HttpPut("{Mongo_Id}")]
    public async Task<IActionResult> Put(MongoId Mongo_Id, [FromBody] PersonApiModel model)
    {

        if (model.Results.Count != 1)
        {
            return BadRequest("1 PUT record at time");
        }



        short result = await _peopleservice.PutAsyncFullModel(Mongo_Id.MongoObject_Id, model);



        if (result == 200)
        {
            return Ok("Successfully updated");
        }
        else if (result == 400)
        {
            return BadRequest("No changes detected.");
        }
        else if (result == 404)
        {
            return NotFound();
        }
        else
        {
            return StatusCode(500, "An internal server error occurred");
        }

    }




    [HttpDelete("{Mongo_Id}")]
    public async Task<IActionResult> Delete(MongoId Mongo_Id)
    {
        byte result = await _peopleservice.RemoveAsync(Mongo_Id.MongoObject_Id);

        if (result > 0)
        {
            return Ok();
        }

        return StatusCode(404, "record Notfound");


    }







}
