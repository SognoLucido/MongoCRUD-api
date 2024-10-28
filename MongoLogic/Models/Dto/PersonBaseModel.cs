using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mongodb.Models.Dto
{
    public class PersonBaseModel
    {
        public  string gender {  get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public Address Address { get; set; }    
        public string email { get; set; }
        public int age { get; set; }
        public string cell { get; set; }

        public Guid? userid { get; set; }

    }


    public record Address (
        string street,
        int number ,
        string city,
        string state,
        string country
        );
}
