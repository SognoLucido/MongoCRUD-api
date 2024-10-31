




using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using MongoCrudPeopleApi.Model;
using Mongodb;
using Mongodb.Models;
using Mongodb.Models.Dto;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoLogic.CRUD;
using System.Data;
using System.Text;
using ZstdSharp;






namespace Mongodb.Services;




public class Peopleservice : IPeopleservice
{
    private readonly IMongoCollection<PersonDbModel> _pplCollection;
    private readonly ILogger<Peopleservice> log;

    public Peopleservice(MongoContext context, ILogger<Peopleservice> logger)
    {


        _pplCollection = context.Peopledb.GetCollection<PersonDbModel>(Collection.User.ToString());

        log = logger;


    }



    public async Task TestLogv2()
    {
        var test = Directory.GetCurrentDirectory();

        var ok = Path.Combine(test, "Logs/log.txt");

        var testx = Path.Exists(ok);

        using (FileStream fileStream = new FileStream(ok, FileMode.Open, FileAccess.Read))
        using (StreamReader reader = new StreamReader(fileStream))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
            }
        }



    }


    public async Task TestLog()
    {

        //try
        //{
        //    throw new NotImplementedException();
        //}
        //catch (Exception ex)
        //{


        log.LogError("This is an error log. {}", "ezzzzzzzzz");
        log.LogWarning("moneez");
        log.LogInformation("mudwad");
        log.LogDebug("lazzzzzzzzz");
        //}
    }


    ///////////////////////////////// 



    public async Task<int> PatchUseritem(PatchUserItemModel identifierdata, UserItemPatchModel bodydata)
    {
        //Builders<PersonDbModel>.Filter.

        var filter = identifierdata.email is null
            ? Builders<PersonDbModel>.Filter.Eq(x => x.Login.Uuid, identifierdata.userid)
            : Builders<PersonDbModel>.Filter.Eq(x => x.Email, identifierdata.email);


        var updatefilterBuilder = Builders<PersonDbModel>.Update;
        var updates = new List<UpdateDefinition<PersonDbModel>>();

        if (bodydata.FirstName is not null) updates.Add(updatefilterBuilder.Set(p => p.Name.First, bodydata.FirstName));
        if (bodydata.LastName is not null) updates.Add(updatefilterBuilder.Set(p => p.Name.Last, bodydata.LastName));
        if (bodydata.Email is not null) updates.Add(updatefilterBuilder.Set(p => p.Email, bodydata.Email));
        if (bodydata.Streetnumb is not null) updates.Add(updatefilterBuilder.Set(p => p.Location.Street.Number, bodydata.Streetnumb));
        if (bodydata.Streetname is not null) updates.Add(updatefilterBuilder.Set(p => p.Location.Street.Name, bodydata.Streetname));
        if (bodydata.City is not null) updates.Add(updatefilterBuilder.Set(p => p.Location.City, bodydata.City));
        if (bodydata.State is not null) updates.Add(updatefilterBuilder.Set(p => p.Location.State, bodydata.State));
        if (bodydata.Country is not null) updates.Add(updatefilterBuilder.Set(p => p.Location.Country, bodydata.Country));
        if (bodydata.Cell is not null) updates.Add(updatefilterBuilder.Set(p => p.Cell, bodydata.Cell));

        if (updates.Count == 0) return 400;


        var update = Builders<PersonDbModel>.Update.Combine(updates);


        int statusC = 200;

        try
        {

            var result = await _pplCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0) statusC = 422;


        }
        catch (Exception ex)
        {
            log.LogError("PATCHuserItemError-{}", ex.Message);

            statusC = 500;
        }


        return statusC;
    }




    public async Task<List<PersonBaseModel>?> BulkSearchUsers(BulkUserModel userdata, Pagesize ps)
    {

        List<PersonBaseModel>? data = [];

        var filterBuilder = Builders<PersonDbModel>.Filter;
        var filters = new List<FilterDefinition<PersonDbModel>>();



        if (userdata.Firstname?.Any() ?? false) filters.Add(filterBuilder.Or(userdata.Firstname.Select(x => Builders<PersonDbModel>.Filter.Regex(d => d.Name.First, new BsonRegularExpression(x))).ToList()));
        if (userdata.Lastname?.Any() ?? false) filters.Add(filterBuilder.Or(userdata.Lastname.Select(x => Builders<PersonDbModel>.Filter.Regex(d => d.Name.Last, new BsonRegularExpression(x))).ToList()));


        if (userdata.State?.Any() ?? false) filters.Add(filterBuilder.Or(userdata.State.Select(x => Builders<PersonDbModel>.Filter.Regex(d => d.Location.State, new BsonRegularExpression(x))).ToList()));
        if (userdata.Country?.Any() ?? false) filters.Add(filterBuilder.Or(userdata.Country.Select(x => Builders<PersonDbModel>.Filter.Regex(d => d.Location.Country, new BsonRegularExpression(x))).ToList()));
        if (userdata.Email?.Any() ?? false) filters.Add(filterBuilder.In(x => x.Email, userdata.Email));
        if (userdata.Uuid?.Any() ?? false) filters.Add(filterBuilder.In(x => x.Login.Uuid, userdata.Uuid));
        if (userdata.Age?.Any() ?? false) filters.Add(filterBuilder.In(x => x.Dob.Age, userdata.Age));
        if (userdata.PhoneNumber?.Any() ?? false) filters.Add(filterBuilder.In(x => x.Cell, userdata.PhoneNumber));


        var filter = filterBuilder.Or(filters);



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
                      cell = x.Cell,

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











    public async Task<List<PersonBaseModel>?> SearchUsers(UsersModel userdata, Pagesize ps)
    {
        List<PersonBaseModel>? data = [];

        var filterBuilder = Builders<PersonDbModel>.Filter;
        var filters = new List<FilterDefinition<PersonDbModel>>();

        if (userdata.Firstname is not null) filters.Add(filterBuilder.Regex(d => d.Name.First, new BsonRegularExpression(userdata.Firstname)));
        if (userdata.Lastname is not null) filters.Add(filterBuilder.Regex(d => d.Name.Last, new BsonRegularExpression(userdata.Lastname)));
        if (userdata.State is not null) filters.Add(filterBuilder.Regex(d => d.Location.State, new BsonRegularExpression(userdata.State)));
        if (userdata.Country is not null) filters.Add(filterBuilder.Regex(d => d.Location.Country, new BsonRegularExpression(userdata.Country)));
        if (userdata.Email is not null) filters.Add(filterBuilder.Regex(d => d.Email, new BsonRegularExpression(userdata.Email)));
        if (userdata.Uuid is not null) filters.Add(filterBuilder.Eq(d => d.Login.Uuid, userdata.Uuid));
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
                      cell = x.Cell,
                      userid = x.Login.Uuid,

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
                    cell = x.Cell,

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
            log.LogError("Database Connection error{}", ex.Message);
        }

        return data.Count == 0 ? null : data;

    }



    public async Task<(int, string?)> Insert(PersonApiModel rawmodel, bool dcheck)
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

                sb.AppendLine($"{rawmodel.Results.Count} item/s inserted");
            }
            catch (Exception ex)
            {
                log.LogError("Insert Item : {}", ex.Message);
                statusCode = 500;
                sb.Clear();
                sb.AppendLine("An error occurred, request aborted ");

            }
        else
        {
            sb.AppendLine("Filtered all duplicates out, nothing to add");
        }


        return (statusCode, sb.ToString());

    }




    public async Task<bool> RemoveItem<T>(T item)
    {

        DeleteResult? result = null;

        if (item is Guid uuid)
        {
            result = await _pplCollection.DeleteOneAsync(x => x.Login.Uuid == uuid);
        }
        else if (item is string email)
        {
            result = await _pplCollection.DeleteOneAsync(x => x.Email == email);
        }

        return result?.DeletedCount > 0;
    }





    public async Task<bool> RemovebyUuidAsync(Guid id)
    {
        var x = await _pplCollection.DeleteOneAsync(x => x.Login.Uuid == id);


        return x.DeletedCount > 0;
    }



    public async Task<bool> RemovebyEmailAsync(string email)
    {
        var x = await _pplCollection.DeleteOneAsync(x => x.Email == email);

        return x.DeletedCount > 0;

    }







}


