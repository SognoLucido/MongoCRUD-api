using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MongoLogic.model.Api
{
    public class MongoId
    {
        [Required]
        [RegularExpression(@"^[0-9A-Fa-f]{24}$")]
        public string MongoObject_Id { get; set; }
    }
}
