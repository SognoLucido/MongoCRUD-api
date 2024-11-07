using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace Mongodb;


public enum Database
{
    Peopledb
}

public enum Collection
{
    User
}

public class MongoContext
{

    public IMongoDatabase Peopledb { get; }

    public MongoContext(string conn)
    {
       



        Peopledb =  new MongoClient(conn).GetDatabase(Database.Peopledb.ToString());
    }

}
