﻿



using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

using MongoLogic.CRUD;

using MongoLogic.model;
using MongoLogic.model.Api;

using System.Reflection;
using System.Text;



namespace MongoLogic.Crud
{
    public class Peopleservice : IPeopleservice
    {
        private readonly IMongoCollection<PersonDbModel> _peopleCollection;


        private readonly IManualmapper _manualmapper;


        public Peopleservice(IOptions<MongoSettings> mongoSettings, IManualmapper manualmapper)
        {


            _manualmapper = manualmapper;

            var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);

            _peopleCollection = mongoDatabase.GetCollection<PersonDbModel>(mongoSettings.Value.CollectionName);



        }

        public async Task<List<PersonDbModel>> GetAsync() =>
        await _peopleCollection.Find(_ => true).Limit(50).ToListAsync();

        //public async Task<PersonDbModel?> GetAsync(string id) =>
        //    await _peopleCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        //public async Task CreateAsync(PersonDbModel newPerson) =>
        //    await _peopleCollection.InsertOneAsync(newPerson);




        public async Task<List<PersonDbModel>?> GetXitems(int numb)
        {
            if (await IsServerNotAlive())  return null; ;

            // var pipeline = new BsonDocument
            //{
            //    { "$sample" ,new BsonDocument("size", 2)}

            // };

            var pipeline = new List<BsonDocument>
            {
                new BsonDocument("$sample", new BsonDocument("size", numb)),
               
            };


            var result = _peopleCollection.Aggregate<PersonDbModel>(pipeline).ToListAsync();


            return result.Result;

        }




        public async Task<long?> GetTotalItems() 
        {
            if (await IsServerNotAlive()) return null;


             return await _peopleCollection.CountDocumentsAsync(FilterDefinition<PersonDbModel>.Empty);
        }



        public async Task<(List<PersonDbModel>?, short)> Agerange(int minage, int maxage)
        {

            if (await IsServerNotAlive()) return (null, 500);


            var filter = Builders<PersonDbModel>.Filter.Where(r => r.Dob.Age > minage && r.Dob.Age<maxage);

          
           
              var personDbModels = await _peopleCollection.Find(filter).ToListAsync();

            if(personDbModels is null)return (null, 500);
            else if (personDbModels.Count == 0) return (null, 404);
            else return (personDbModels, 200);
           
  

        }



        public async Task<(PersonDbModel?, short)> Findby_id(string _Id)
        {
            if (await IsServerNotAlive()) return (null, 500);

            var data = await _peopleCollection.FindAsync(a => a._Id == _Id);
            
            var x  = data.FirstOrDefault();

            return (x,200);
        }




        public async Task<List<PersonDbModel>?> FindbyCustomQuary(FindsingleModel data)
        {

            if (await IsServerNotAlive()) return null;

            var builder = Builders<PersonDbModel>.Filter;

            var filter = builder.Empty;


            if (data.Name != null)
            {
                filter &= builder.Eq(a=> a.Name.First, data.Name);
            }
            
            if(data.LastName != null)
            {
                filter &= builder.Eq(a => a.Name.Last, data.LastName);
            }

            if(data.LoginUuid != null)
            {
                filter &= builder.Eq(a => a.Login.Uuid, data.LoginUuid);
            }

          

            return await _peopleCollection.Find(filter).ToListAsync();
   
        }




        public async Task<short> UpdateAsync(string id, PutPersonmodel ToUpdate)  //v1
        {

            if (await IsServerNotAlive()) return 500;
            //if (await IsServerNotAlive()) throw new TimeoutException("MongoDB server is unreachable");


            var filter = Builders<PersonDbModel>.Filter.Eq(p => p._Id, id);
            var ToupdateModel = Builders<PersonDbModel>.Update;

            var updates = new List<UpdateDefinition<PersonDbModel>>();
         

            PropertyInfo[] properties = typeof(PutPersonmodel).GetProperties();

            foreach (var property in properties)
            {
                if(property.GetValue(ToUpdate) == null) continue;

                switch (property.Name)
                {
                    case "City": updates.Add(ToupdateModel.Set(a => a.Location.City, property.GetValue(ToUpdate))); break;
                    case "State": updates.Add(ToupdateModel.Set(a => a.Location.State, property.GetValue(ToUpdate))); break;
                    case "Country": updates.Add(ToupdateModel.Set(a => a.Location.Country, property.GetValue(ToUpdate))); break;
                    case "Username": updates.Add(ToupdateModel.Set(a => a.Login.Username, property.GetValue(ToUpdate))); break;
                    case "Phone": updates.Add(ToupdateModel.Set(a => a.Phone, property.GetValue(ToUpdate))); break;
                    case "Cell": updates.Add(ToupdateModel.Set(a => a.Cell, property.GetValue(ToUpdate))); break;
                    case "Email": updates.Add(ToupdateModel.Set(a => a.Email, property.GetValue(ToUpdate))); break;

                }

            }


            
              var result = await _peopleCollection.UpdateOneAsync(filter, ToupdateModel.Combine(updates));

                if (result.ModifiedCount == 1 && result.MatchedCount == 1) return 200;
                else if (result.ModifiedCount == 0 && result.MatchedCount == 1) return 400;
                else if (result.ModifiedCount == 0 && result.MatchedCount == 0) return 404;
            
           

            return 500;

        }




        public async Task<short> PutAsyncFullModel(string id, PersonApiModel data)
        {

            if (await IsServerNotAlive()) return 500;

            //if (await IsServerNotAlive()) throw new TimeoutException("MongoDB server is unreachable") ;

            bool[] matrix = [true];

           var resultmodel = await _manualmapper.ApitoDbmodel(data, matrix);

            resultmodel.First.ValueRef._Id = id;

            var filter = Builders<PersonDbModel>.Filter.Eq(p => p._Id, id);


          

         
                var result = await _peopleCollection.ReplaceOneAsync(filter, resultmodel.First.ValueRef);
                resultmodel = null;

                if (result.ModifiedCount == 1 && result.MatchedCount == 1) return 200;
                else if (result.ModifiedCount == 0 && result.MatchedCount == 1) return 400;
                else if (result.ModifiedCount == 0 && result.MatchedCount == 0) return 404;


            return 500;
        }








        public async Task<byte> RemoveAsync(string id) 
        {
           var  x =  await _peopleCollection.DeleteOneAsync(x => x._Id == id);

            return (byte)x.DeletedCount;

        }


        private async Task<bool[]> CheckifExistCustom(PersonApiModel model) // string FirstName ,string LastName , Guid Loginguid
        {

            bool[] check = new bool[model.Results.Length];

            for (int i = 0; i < model.Results.Length; i++)
            {

                //var checkdata = await _peopleCollection.FindAsync(a => a.Name.First == model.Results[i].Name.First && a.Name.Last == model.Results[i].Name.Last && a.Login.Uuid == model.Results[i].Login.Uuid);

                //var x = checkdata.FirstOrDefault();

                //if(x != null) check[i] = true;

                var checkdata = await _peopleCollection.CountDocumentsAsync(a => a.Name.First == model.Results[i].Name.First && a.Name.Last == model.Results[i].Name.Last && a.Login.Uuid == model.Results[i].Login.Uuid);

                if (checkdata == 0) check[i] = true;

                //var x = checkdata.FirstOrDefault();

                    //if (x != null) check[i] = true;


            }


            //var x = await _peopleCollection.FindAsync
            //(
            //a => a.Name.First == FirstName 
            //&& a.Name.Last == LastName
            //&& a.Login.Uuid == Loginguid
            //);



            //WasFirstBatchEmpty = true


            return check;
        }



        public async Task<string> InsertDupcheck(PersonApiModel model)
        {

            if (await IsServerNotAlive()) return "500";

            bool[] matrix = await CheckifExistCustom(model);


           LinkedList<PersonDbModel> axzer = await _manualmapper.ApitoDbmodel(model,matrix);

            if (axzer.Count != 0)
            {
                try
                {
                    await _peopleCollection.InsertManyAsync(axzer);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return "500";
                }
            }

           StringBuilder duplicates = new StringBuilder();
            duplicates.AppendLine("Duplicates:");

           StringBuilder Successfulinserted = new StringBuilder();
            Successfulinserted.AppendLine("Successfulinserted:");




            for (var i = 0; i < model.Results.Length; i++)
            {
                switch (matrix[i])
                {
                    case false: { duplicates.AppendLine($" \t FirstName: {model.Results[i].Name.First} - LastName: {model.Results[i].Name.Last} - LoginId: {model.Results[i].Login.Uuid}"); break; }
                    case true: { Successfulinserted.AppendLine($" \t FirstName: {model.Results[i].Name.First} - LastName: {model.Results[i].Name.Last} - LoginId: {model.Results[i].Login.Uuid}"  ); break; }
                   
                }

            }

            Successfulinserted.AppendLine(duplicates.ToString());

            return Successfulinserted.ToString();
          
        }




        private async Task<bool> IsServerNotAlive() =>  _peopleCollection.Database.Client.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Disconnected ? true : false;  
         
       






    }
}
