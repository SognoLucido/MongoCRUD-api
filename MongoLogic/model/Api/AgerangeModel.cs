
using System.ComponentModel.DataAnnotations;


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
