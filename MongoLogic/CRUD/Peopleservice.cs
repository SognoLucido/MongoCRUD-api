


using Microsoft.Extensions.Options;
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
       

        public Peopleservice(IOptions<MongoSettings> mongoSettings , IManualmapper manualmapper) 
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


        public async Task<List<PersonDbModel>> Agerange(int minage , int maxage)
        {

            var filter = Builders<PersonDbModel>.Filter.Where(r => r.Dob.Age > minage && r.Dob.Age<maxage);

            return await _peopleCollection.Find(filter).ToListAsync();

        }

        public async Task<PersonDbModel?> Findby_id(string _Id)
        {

            var data = await _peopleCollection.FindAsync(a => a._Id == _Id);
            
            var x  = data.FirstOrDefault();

            return x;
        }


        public async Task<List<PersonDbModel>> FindbyCustomQuary(FindsingleModel data)
        {

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


            try 
            { 
              var result = await _peopleCollection.UpdateOneAsync(filter, ToupdateModel.Combine(updates));

                if (result.ModifiedCount == 1 && result.MatchedCount == 1) return 200;
                else if (result.ModifiedCount == 0 && result.MatchedCount == 1) return 400;
                else if (result.ModifiedCount == 0 && result.MatchedCount == 0) return 404;
            }
            finally
            {
                
            }
            
            
            return 500;
            //await _peopleCollection.ReplaceOneAsync(x => x._Id == id, ToUpdate);
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
                    Console.WriteLine(ex.ToString());
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

    }
}
