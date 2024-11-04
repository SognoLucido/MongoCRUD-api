using System.ComponentModel.DataAnnotations;

namespace Logger.Model.Dto
{
    public class DateDtomodel
    {
       
        public Startdate Startdate { get; set; }

        public Enddate? Enddate { get; set; }

    }



    public class Startdate
    {
        [Required]
        public int Year { get; set; }
        public int? Month { get; set; } 
        public int? Day { get; set; } 



        public int? Hour { get; set; }
        public int? Minute { get; set; } 

        public int? Second { get; set; } 

        public override string ToString()
        {
            
            return $"{Year}-{Month ?? 0}-{Day ?? 0} {Hour ?? 0}:{Minute ?? 0}:{Second ?? 0}";
        }

    }

    public class Enddate
    {
        [Required]
        public int Year { get; set; }
      
        public int Month { get; set; }
     
        public int Day { get; set; }



        public int? Hour { get; set; }
        public int? Minute { get; set; }

        public int? Second { get; set; }

        public override string ToString()
        {

            return $"{Year}-{Month}-{Day} {Hour ?? 0}:{Minute ?? 0}:{Second ?? 0}";
        }
    }


}
