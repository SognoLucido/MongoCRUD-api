using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MongoLogic;
using MongoLogic.CRUD;
using MongoLogic.model;
using MongoLogic.model.Api;


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
        public async Task<IActionResult> Get([FromQuery] AgerangeModel age)
        {           

            if(ModelState.IsValid)
            {
                if (age.MinAge > age.MaxAge) return BadRequest("min < max");

            }
            else
            {
                return BadRequest("invalid Query");
            }


          return Ok(await _peopleservice.Agerange(age.MinAge,age.MaxAge));
        }




        [HttpGet("firstordefault")]
        public async Task<IActionResult> Get(string mongo_id)
        {
            var data = await _peopleservice.Findby_id(mongo_id);

            if(data == null)
            {
                return StatusCode(404, "record Notfound");
            }
            else
            {
                return Ok(data);
            }
   
            
        }



        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] FindsingleModel findbyone)
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
        public async Task<IActionResult> Put(string Mongo_Id, [FromBody] PutPersonmodel model)
        {

             

          short result = await _peopleservice.UpdateAsync(Mongo_Id, model);

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


        [HttpDelete("{Mongo_id}")]
        public async Task<IActionResult> Delete(string Mongo_id)
        {
           byte result = await _peopleservice.RemoveAsync(Mongo_id);

            if(result > 0)
            {
                return Ok();
            }
            else if(result == 0)
            {
                return StatusCode(404, "record Notfound");
            }
            else 
            {
                return StatusCode(500, "it's over");
            }

            

        }
    }
}
