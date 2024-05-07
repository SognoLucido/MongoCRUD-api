using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoLogic.model.Api
{
    public class AgerangeModel
    {
        [Required]
        [Range(0, 100)]
        public byte MinAge { get; set; }

        [Required]
        [Range(0, 100)]
        public byte MaxAge { get; set; }


    }
}
