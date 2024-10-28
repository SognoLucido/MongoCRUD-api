using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mongodb.Models.Dto
{
    public class PatchUserItemModel
    {
        public Guid? userid { get; set; }

        [EmailAddress]
        public string? email { get; set; }
    }



    public class UserItemPatchModel 
    { 
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Email { get; set; }

        public int? Streetnumb { get; set; }
        public string? Streetname { get; set; }

        public string? City { get; set; }
        public string? State { get; set; }

        public string? Country {  get; set; }

        [RegularExpression("^[0-9]{1,20}$")]
        public string? Cell {  get; set; }



    }


}
