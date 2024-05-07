using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoLogic.model.Api
{
    public class FindsingleModel
    {

        public string? Name { get; set; }    
        public string? LastName { get; set; }
        public Guid? LoginUuid { get; set; }
    }
}
