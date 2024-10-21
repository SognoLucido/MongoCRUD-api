using MongoLogic.model.Api;
using Mongodb.Models;


namespace MongoLogic
{
    public interface IManualmapper
    {
        Task<LinkedList<PersonDbModel>> ApitoDbmodel(PersonApiModel Apidatain, bool[] duplicates);
    }
}
