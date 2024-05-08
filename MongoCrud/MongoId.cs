
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MongoLogic.model.Api
{
    public class MongoId
    {

        [Required]
        [FromRoute(Name = "Mongo_Id")]
        [RegularExpression(@"^[0-9A-Fa-f]{24}$")]
        public string MongoObject_Id { get; set; }
    }
}
