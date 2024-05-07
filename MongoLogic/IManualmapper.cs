using MongoLogic.model.Api;
using MongoLogic.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoLogic
{
    public interface IManualmapper
    {
        Task<LinkedList<PersonDbModel>> ApitoDbmodel(PersonApiModel Apidatain, bool[] duplicates);
    }
}
