using MongoLogic.model.Api;
using MongoLogic.model;


namespace MongoLogic
{
    public interface IManualmapper
    {
        Task<LinkedList<PersonDbModel>> ApitoDbmodel(PersonApiModel Apidatain, bool[] duplicates);
    }
}
