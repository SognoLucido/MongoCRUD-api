using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoLogic.model.Api
{
    public class PutPersonmodel
    {
        [MaxLength(40)]
        public string? City { get; set; }

        [MaxLength(40)]
        public string? State { get; set; }

        [MaxLength(40)]
        public string? Country { get; set; }

        [MaxLength(20)]
        public string? Username { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(20)]
        public string? Cell { get; set; }


        [EmailAddress]
        public string? Email { get; set; }
    }
}
