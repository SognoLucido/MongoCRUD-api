﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MongoLogic;
using MongoLogic.CRUD;
using MongoLogic.model;
using MongoLogic.model.Api;
using System.Diagnostics.Eventing.Reader;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MongoCrudPeopleApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class mongocrudController : ControllerBase
    {


        public readonly IPeopleservice _peopleservice;
        public readonly IManualmapper _mapper;

        public mongocrudController(IPeopleservice peopleservice, IManualmapper mapper)
        {
            _peopleservice = peopleservice;
            _mapper = mapper;
        }



        [HttpGet("byage")]
        public async Task<IActionResult> GetbyAgeRange([FromQuery] AgerangeModel age)
        {           

            if(ModelState.IsValid)
            {
                if (age.MinAge > age.MaxAge || age.MinAge == age.MaxAge) return BadRequest("minAge < maxAge");

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




        [HttpGet("firstordefault")]
        public async Task<IActionResult> GetOne([FromQuery] MongoId id)
        {
            var data = await _peopleservice.Findby_id(id.MongoObject_Id);

            if (data.Item2 == 200 && data.Item1 is not null)
            {
                return Ok(data.Item1);
            }
            else if(data.Item1 is null)
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
                if ( findbyone.Name is null && findbyone.LastName is null && findbyone.LoginUuid is null)
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
                return StatusCode(404,"record Notfound");
            }
            else
            {
                return Ok(datafromdb);
            }
            


          
        }




        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PersonApiModel Value)
        {

            string result = await _peopleservice.InsertDupcheck(Value);

            if (result == "500")
            {
                return StatusCode(500, "An internal server error occurred");
            }
            else
            {
                return Ok(result);
            }

        }



      
        
        [HttpPatch("{Mongo_Id}")]
        public async Task<IActionResult> Patch(MongoId Mongo_Id, [FromBody] PutPersonmodel model)
        {

             

          short result = await _peopleservice.UpdateAsync(Mongo_Id.MongoObject_Id, model);

            if(result == 200 )
            {
                return Ok("Successfully updated");
            }
            else if (result == 400)
            {
                return BadRequest("No changes detected.");
            }
            else if(result == 404)
            {
                return NotFound();
            }
            else
            {
                return StatusCode(500,"Server on fire");
            }

        }

        [HttpPut("{Mongo_Id}")]

        public async Task<IActionResult> Put(MongoId Mongo_Id , [FromBody] PersonApiModel model)
        {

            if (model.Results.Length != 1) 
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




        [HttpDelete("{Mongo_id}")]
        public async Task<IActionResult> Delete(MongoId Mongo_Id)
        {
           byte result = await _peopleservice.RemoveAsync(Mongo_Id.MongoObject_Id);

            if(result > 0)
            {
                return Ok();
            }
           
                return StatusCode(404, "record Notfound");
          

            

        }
    }
}
