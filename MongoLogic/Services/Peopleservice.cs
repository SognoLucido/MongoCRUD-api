




using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoCrudPeopleApi.Model;
using Mongodb;
using Mongodb.Models;
using Mongodb.Models.Dto;
using MongoDB.Bson;
using MongoDB.Driver;

using MongoLogic.CRUD;
using MongoLogic.model.Api;
using System.Data;

using System.Text;





namespace MongoLogic.Crud;




public class Peopleservice : IPeopleservice
{
    private readonly IMongoCollection<PersonDbModel> _pplCollection;
    private readonly ILogger<Peopleservice> log;

    public Peopleservice(MongoContext context, ILogger<Peopleservice> logger)
    {


        _pplCollection = context.Peopledb.GetCollection<PersonDbModel>(Collection.User.ToString());

        log = logger;


    }


    //public async Task TestLog()
    //{

    //    try
    //    {
    //        throw new NotImplementedException();
    //    }
    //    catch (Exception ex) 
    //    {
    //        log.LogError("This is an error log. {}",ex.Message);
    //    }
    //}


    ///////////////////////////////// 



    //public List<List<BsonRegularExpression>> BsonExpressionBuilder(BulkUserModel data)
    //{

    //   // var list = new List<List<BsonRegularExpression>?>();

    //   // if (data.Firstname.Any())
    //   // {

    //   // }

    //   //return list;
    //}

    



    public async Task<List<PersonBaseModel>?> BulkSearchUsers(BulkUserModel userdata, Pagesize ps)
    {

        List<PersonBaseModel>? data = [];

        var filterBuilder = Builders<PersonDbModel>.Filter;
        var filters = new List<FilterDefinition<PersonDbModel>>();

     

        if (userdata.Firstname.Any()) filters.Add(filterBuilder.Or(userdata.Firstname.Select(x=>Builders<PersonDbModel>.Filter.Regex(d => d.Name.First, new BsonRegularExpression(x))).ToList()));
        if (userdata.Lastname.Any()) filters.Add(filterBuilder.Or(userdata.Lastname.Select(x => Builders<PersonDbModel>.Filter.Regex(d => d.Name.Last, new BsonRegularExpression(x))).ToList()));


        if (userdata.State.Any()) filters.Add(filterBuilder.Or(userdata.State.Select(x => Builders<PersonDbModel>.Filter.Regex(d => d.Location.State, new BsonRegularExpression(x))).ToList()));
        if (userdata.Country.Any()) filters.Add(filterBuilder.Or(userdata.Country.Select(x => Builders<PersonDbModel>.Filter.Regex(d => d.Location.Country, new BsonRegularExpression(x))).ToList()));
        if (userdata.Email.Any()) filters.Add(filterBuilder.In(x => x.Email, userdata.Email));
        if (userdata.Uuid.Any()) filters.Add(filterBuilder.In(x => x.Login.Uuid , userdata.Uuid));
        if (userdata.Age.Any()) filters.Add(filterBuilder.In(x => x.Dob.Age, userdata.Age));
        if (userdata.PhoneNumber.Any()) filters.Add(filterBuilder.In(x => x.Cell, userdata.PhoneNumber));
      

        var filter = filterBuilder.And(filters);
       


        try
        {


            data = await _pplCollection
                  .Find(filter)
                  .Project(x => new PersonBaseModel
                  {
                      gender = x.Gender,
                      Firstname = x.Name.First,
                      Lastname = x.Name.Last,
                      email = x.Email,
                      age = x.Dob.Age,
                      phone = x.Phone,

                      Address = new Address
                      (
                          x.Location.Street.Name,
                          x.Location.Street.Number,
                          x.Location.City,
                          x.Location.State,
                          x.Location.Country
                      )

                  })
                  .Limit((int)ps)
                  .ToListAsync();
        }
        catch (MongoConnectionException ex)
        {
            log.LogError("Database Connection error{}", ex.Message);
        }
        catch (Exception ex)
        {
            log.LogError("{}", ex.Message);
        }


        return data.Count == 0 ? null : data;

    }











        public async Task<List<PersonBaseModel>?> SearchUsers(UsersModel userdata,Pagesize ps)
    {
        List<PersonBaseModel>? data = [];

        var filterBuilder = Builders<PersonDbModel>.Filter;
        var filters = new List<FilterDefinition<PersonDbModel>>();

        if (userdata.Firstname is not null) filters.Add(filterBuilder.Regex(d =>d.Name.First, new BsonRegularExpression(userdata.Firstname)));
        if (userdata.Lastname is not null) filters.Add(filterBuilder.Regex(d => d.Name.Last, new BsonRegularExpression(userdata.Lastname)));
        if (userdata.State is not null) filters.Add(filterBuilder.Regex(d => d.Location.State, new BsonRegularExpression(userdata.State)));
        if (userdata.Country is not null) filters.Add(filterBuilder.Regex(d => d.Location.Country, new BsonRegularExpression(userdata.Country)));
        if (userdata.Email is not null) filters.Add(filterBuilder.Regex(d => d.Email, new BsonRegularExpression(userdata.Email)));
        if (userdata.Uuid is not null) filters.Add(filterBuilder.Eq(d => d.Login.Uuid,userdata.Uuid));
        if (userdata.Age is not null) filters.Add(filterBuilder.Eq(d => d.Dob.Age, (int)userdata.Age));
        if (userdata.PhoneNumber is not null) filters.Add(filterBuilder.Regex(d => d.Phone, new BsonRegularExpression(userdata.PhoneNumber)));


        var filter = filterBuilder.And(filters);

        try
        {


          data = await _pplCollection
                .Find(filter)
                .Project(x => new PersonBaseModel
                {
                    gender = x.Gender,
                    Firstname = x.Name.First,
                    Lastname = x.Name.Last,
                    email = x.Email,
                    age = x.Dob.Age,
                    phone = x.Phone,

                    Address = new Address
                    (
                        x.Location.Street.Name,
                        x.Location.Street.Number,
                        x.Location.City,
                        x.Location.State,
                        x.Location.Country
                    )
                    
                })
                .Limit((int)ps)
                .ToListAsync();
        }
        catch (MongoConnectionException ex)
        {
            log.LogError("Database Connection error{}", ex.Message);
        }
        catch (Exception ex)
        {
            log.LogError("{}", ex.Message);
        }
      

        return data.Count == 0 ? null : data;

       
    }






    public async Task<List<PersonBaseModel>?> GetAgerangeUserItem(int minage, int maxage)
    {

        List<PersonBaseModel>? data = [];

        var filter = Builders<PersonDbModel>.Filter.Gte(p => p.Dob.Age, minage) &
                Builders<PersonDbModel>.Filter.Lte(p => p.Dob.Age, maxage);


        try
        {
            data = await _pplCollection.Find(filter)
                .Project(x => new PersonBaseModel
                {
                    gender = x.Gender,
                    Firstname = x.Name.First,
                    Lastname = x.Name.Last,
                    email = x.Email,
                    age = x.Dob.Age,
                    phone = x.Phone,

                    Address = new Address
                    (
                        x.Location.Street.Name,
                        x.Location.Street.Number,
                        x.Location.City,
                        x.Location.State,
                        x.Location.Country
                    )

                })
                .ToListAsync();
        }
        catch (MongoConnectionException ex)
        {
           log.LogError("Database Connection error{}",ex.Message);
        }

       return data.Count == 0 ? null : data;
      
    }



    public Task<List<PersonDbModel>?> FindbyCustomQuary(FindsingleModel data)
    {
        throw new NotImplementedException();
    }

    public Task<(PersonDbModel?, short)> Findby_id(string _Id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<PersonDbModel>> GetAsync() =>
    await _pplCollection.Find(_ => true).Limit(50).ToListAsync();

    public Task<long?> GetTotalItems()
    {
        throw new NotImplementedException();
    }

    public Task<List<PersonDbModel>?> GetXitems(int numb)
    {
        throw new NotImplementedException();
    }


  
    

    public async Task<(int,string?)> Insert(PersonApiModel rawmodel, bool dcheck)   
    {

        StringBuilder sb = new(); 
        int statusCode = 201;
        int RawItemscount = rawmodel.Results.Count;
        int duplecoun = 0;


        sb.AppendLine("Total items : " + RawItemscount);



        rawmodel.Results = rawmodel.Results.DistinctBy(z => z.Email).ToList();

        if (RawItemscount != rawmodel.Results.Count)
        {
            duplecoun += RawItemscount - rawmodel.Results.Count;
            sb.AppendLine($"{duplecoun} Duplicate/s  found in the POST request and removed");
          
            RawItemscount = rawmodel.Results.Count;
        }

        if (dcheck)
        {

            var EmailTocheck = rawmodel.Results.Select(z => z.Email).ToArray();
            var filter = Builders<PersonDbModel>.Filter.In(p => p.Email, EmailTocheck);

            var Getclones = await _pplCollection
                .Distinct(x => x.Email, filter)
                .ToListAsync();

            if (Getclones.Count > 0)
            {
               

                rawmodel.Results.RemoveAll(x => Getclones.Contains(x.Email));

                duplecoun += RawItemscount - rawmodel.Results.Count;

                sb.AppendLine($"{RawItemscount - rawmodel.Results.Count} Duplicate/s  found in db");
                statusCode = 409;
            }
        }


        if (rawmodel.Results.Count > 0)
            try
            {

                foreach (var item in rawmodel.Results)
                {
                    item.Name.First = item.Name.First.ToLower();
                    item.Name.Last = item.Name.Last.ToLower();
                    item.Location.State = item.Location.State.ToLower();
                    item.Location.Country = item.Location.Country.ToLower();

                }

                await _pplCollection.InsertManyAsync(rawmodel.Results);

                if (statusCode == 409) 
                {
                    sb.AppendLine("Duplicates filtered, the remaining records were inserted ");
                    sb.AppendLine($"{duplecoun} duplicate/s removed");
                };

                sb.AppendLine($"{rawmodel.Results.Count} item/s inserted" );
            }
            catch (Exception ex)
            {
                log.LogError("Insert Item : {}" ,ex.Message);
                statusCode = 500;
                sb.Clear();
                sb.AppendLine("An error occurred, request aborted ");

            }
        else 
        {
            sb.AppendLine("Filtered all duplicates out, nothing to add");
        }


        return (statusCode,sb.ToString());

    }







    private record Email (string email);

    public Task<short> PutAsyncFullModel(string id, PersonApiModel data)
    {
        throw new NotImplementedException();
    }

    public Task<byte> RemoveAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<short> UpdateAsync(string id, PutPersonmodel ToUpdate)
    {
        throw new NotImplementedException();
    }

    

    //public async Task<PersonDbModel?> GetAsync(string id) =>
    //    await _peopleCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    //public async Task CreateAsync(PersonDbModel newPerson) =>
    //    await _peopleCollection.InsertOneAsync(newPerson);




    //public async Task<List<PersonDbModel>?> GetXitems(int numb)
    //{
    //    if (await IsServerNotAlive())  return null; ;

    //    // var pipeline = new BsonDocument
    //    //{
    //    //    { "$sample" ,new BsonDocument("size", 2)}

    //    // };

    //    var pipeline = new List<BsonDocument>
    //    {
    //        new BsonDocument("$sample", new BsonDocument("size", numb)),

    //    };


    //    var result = _pplCollection.Aggregate<PersonDbModel>(pipeline).ToListAsync();


    //    return result.Result;

    //}




    //public async Task<long?> GetTotalItems() 
    //{
    //    if (await IsServerNotAlive()) return null;


    //     return await _pplCollection.CountDocumentsAsync(FilterDefinition<PersonDbModel>.Empty);
    //}



    //public async Task<(List<PersonDbModel>?, short)> Agerange(int minage, int maxage)
    //{

    //    if (await IsServerNotAlive()) return (null, 500);


    //    var filter = Builders<PersonDbModel>.Filter.Where(r => r.Dob.Age > minage && r.Dob.Age<maxage);



    //      var personDbModels = await _pplCollection.Find(filter).ToListAsync();

    //    if(personDbModels is null)return (null, 500);
    //    else if (personDbModels.Count == 0) return (null, 404);
    //    else return (personDbModels, 200);



    //}



    //public async Task<(PersonDbModel?, short)> Findby_id(string _Id)
    //{
    //    if (await IsServerNotAlive()) return (null, 500);

    //    var data = await _pplCollection.FindAsync(a => a._Id == _Id);

    //    var x  = data.FirstOrDefault();

    //    return (x,200);
    //}




    //public async Task<List<PersonDbModel>?> FindbyCustomQuary(FindsingleModel data)
    //{

    //    if (await IsServerNotAlive()) return null;

    //    var builder = Builders<PersonDbModel>.Filter;

    //    var filter = builder.Empty;


    //    if (data.Name != null)
    //    {
    //        filter &= builder.Eq(a=> a.Name.First, data.Name);
    //    }

    //    if(data.LastName != null)
    //    {
    //        filter &= builder.Eq(a => a.Name.Last, data.LastName);
    //    }

    //    if(data.LoginUuid != null)
    //    {
    //        filter &= builder.Eq(a => a.Login.Uuid, data.LoginUuid);
    //    }



    //    return await _pplCollection.Find(filter).ToListAsync();

    //}




    //public async Task<short> UpdateAsync(string id, PutPersonmodel ToUpdate)  //v1
    //{

    //    if (await IsServerNotAlive()) return 500;
    //    //if (await IsServerNotAlive()) throw new TimeoutException("MongoDB server is unreachable");


    //    var filter = Builders<PersonDbModel>.Filter.Eq(p => p._Id, id);
    //    var ToupdateModel = Builders<PersonDbModel>.Update;

    //    var updates = new List<UpdateDefinition<PersonDbModel>>();


    //    PropertyInfo[] properties = typeof(PutPersonmodel).GetProperties();

    //    foreach (var property in properties)
    //    {
    //        if(property.GetValue(ToUpdate) == null) continue;

    //        switch (property.Name)
    //        {
    //            case "City": updates.Add(ToupdateModel.Set(a => a.Location.City, property.GetValue(ToUpdate))); break;
    //            case "State": updates.Add(ToupdateModel.Set(a => a.Location.State, property.GetValue(ToUpdate))); break;
    //            case "Country": updates.Add(ToupdateModel.Set(a => a.Location.Country, property.GetValue(ToUpdate))); break;
    //            case "Username": updates.Add(ToupdateModel.Set(a => a.Login.Username, property.GetValue(ToUpdate))); break;
    //            case "Phone": updates.Add(ToupdateModel.Set(a => a.Phone, property.GetValue(ToUpdate))); break;
    //            case "Cell": updates.Add(ToupdateModel.Set(a => a.Cell, property.GetValue(ToUpdate))); break;
    //            case "Email": updates.Add(ToupdateModel.Set(a => a.Email, property.GetValue(ToUpdate))); break;

    //        }

    //    }



    //      var result = await _pplCollection.UpdateOneAsync(filter, ToupdateModel.Combine(updates));

    //        if (result.ModifiedCount == 1 && result.MatchedCount == 1) return 200;
    //        else if (result.ModifiedCount == 0 && result.MatchedCount == 1) return 400;
    //        else if (result.ModifiedCount == 0 && result.MatchedCount == 0) return 404;



    //    return 500;

    //}




    //public async Task<short> PutAsyncFullModel(string id, PersonApiModel data)
    //{

    //    if (await IsServerNotAlive()) return 500;

    //    //if (await IsServerNotAlive()) throw new TimeoutException("MongoDB server is unreachable") ;

    //    bool[] matrix = [true];

    //   var resultmodel = await _manualmapper.ApitoDbmodel(data, matrix);

    //    resultmodel.First.ValueRef._Id = id;

    //    var filter = Builders<PersonDbModel>.Filter.Eq(p => p._Id, id);





    //        var result = await _pplCollection.ReplaceOneAsync(filter, resultmodel.First.ValueRef);
    //        resultmodel = null;

    //        if (result.ModifiedCount == 1 && result.MatchedCount == 1) return 200;
    //        else if (result.ModifiedCount == 0 && result.MatchedCount == 1) return 400;
    //        else if (result.ModifiedCount == 0 && result.MatchedCount == 0) return 404;


    //    return 500;
    //}








    //public async Task<byte> RemoveAsync(string id) 
    //{
    //   var  x =  await _pplCollection.DeleteOneAsync(x => x._Id == id);

    //    return (byte)x.DeletedCount;

    //}


    //private async Task<bool[]> CheckifExistCustom(PersonApiModel model) // string FirstName ,string LastName , Guid Loginguid
    //{

    //    bool[] check = new bool[model.Results.Length];

    //    for (int i = 0; i < model.Results.Length; i++)
    //    {

    //        //var checkdata = await _peopleCollection.FindAsync(a => a.Name.First == model.Results[i].Name.First && a.Name.Last == model.Results[i].Name.Last && a.Login.Uuid == model.Results[i].Login.Uuid);

    //        //var x = checkdata.FirstOrDefault();

    //        //if(x != null) check[i] = true;

    //        var checkdata = await _pplCollection.CountDocumentsAsync(a => a.Name.First == model.Results[i].Name.First && a.Name.Last == model.Results[i].Name.Last && a.Login.Uuid == model.Results[i].Login.Uuid);

    //        if (checkdata == 0) check[i] = true;

    //        //var x = checkdata.FirstOrDefault();

    //            //if (x != null) check[i] = true;


    //    }


    //    //var x = await _peopleCollection.FindAsync
    //    //(
    //    //a => a.Name.First == FirstName 
    //    //&& a.Name.Last == LastName
    //    //&& a.Login.Uuid == Loginguid
    //    //);



    //    //WasFirstBatchEmpty = true


    //    return check;
    //}



    //public async Task<string> InsertDupcheck(PersonApiModel model)
    //{

    //    if (await IsServerNotAlive()) return "500";

    //    bool[] matrix = await CheckifExistCustom(model);


    //   LinkedList<PersonDbModel> axzer = await _manualmapper.ApitoDbmodel(model,matrix);

    //    if (axzer.Count != 0)
    //    {
    //        try
    //        {
    //            await _pplCollection.InsertManyAsync(axzer);
    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex.Message);
    //            return "500";
    //        }
    //    }

    //   StringBuilder duplicates = new StringBuilder();
    //    duplicates.AppendLine("Duplicates:");

    //   StringBuilder Successfulinserted = new StringBuilder();
    //    Successfulinserted.AppendLine("Successfulinserted:");




    //    for (var i = 0; i < model.Results.Length; i++)
    //    {
    //        switch (matrix[i])
    //        {
    //            case false: { duplicates.AppendLine($" \t FirstName: {model.Results[i].Name.First} - LastName: {model.Results[i].Name.Last} - LoginId: {model.Results[i].Login.Uuid}"); break; }
    //            case true: { Successfulinserted.AppendLine($" \t FirstName: {model.Results[i].Name.First} - LastName: {model.Results[i].Name.Last} - LoginId: {model.Results[i].Login.Uuid}"  ); break; }

    //        }

    //    }

    //    Successfulinserted.AppendLine(duplicates.ToString());

    //    return Successfulinserted.ToString();

    //}




    //private async Task<bool> IsServerNotAlive() =>  _pplCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected ? true : false;  








}


