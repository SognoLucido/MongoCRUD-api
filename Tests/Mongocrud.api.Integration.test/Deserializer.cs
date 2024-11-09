
using Mongocrud.api.Integration.test.Model;
using Mongodb.Models.Dto;
using System.Text;
using System.Text.Json;


namespace Mongocrud.api.Integration.test
{
    public class Deserializer
    {

        private readonly string jsonContent;
        private readonly JsonSerializerOptions options;


        public Deserializer()
        {

            var _filepath = Path.Combine(Directory.GetCurrentDirectory(), "Model/postUserdataTemplate.json");

            if (!Path.Exists(_filepath)) throw new FileNotFoundException("postUserdataTemplate.json not found in the PATH={}", _filepath);

            jsonContent = File.ReadAllText(_filepath);

            options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };


        }





        public async Task<StringContent> CreateJsonUsers<T>(List<T> userlist)
        {

            var data = JsonSerializer.Deserialize<PersonDto>(jsonContent, options)
                ?? throw new JsonException("file empty nothing to deserialize");


            var templist = new List<PersonDbModeldto>(userlist.Count);

            foreach (var user in userlist)
            {
                templist.Add(data.Results[0].Clone());
            }


            if (userlist is List<Changeusershort> shortmodel)
            {

                for (int i = 0; i < templist.Count; i++)
                {
                    templist[i].Name.First = shortmodel[i].Firstname;
                    templist[i].Name.Last = shortmodel[i].Lastname;
                    templist[i].Email = shortmodel[i].Email;
                }

            }
            else if (userlist is List<Changeuserage> agemodel)
            {

                for (int i = 0; i < templist.Count; i++)
                {
                    templist[i].Name.First = agemodel[i].Firstname;
                    templist[i].Name.Last = agemodel[i].Lastname;
                    templist[i].Email = agemodel[i].Email;
                    templist[i].Dob.Age = agemodel[i].age;
                }

            }
            else if (userlist is List<Changeuserpatch> patchmodel)
            {
                for (int i = 0; i < templist.Count; i++)
                {
                    templist[i].Name.First = patchmodel[i].Firstname;
                    templist[i].Name.Last = patchmodel[i].Lastname;
                    templist[i].Email = patchmodel[i].Email;

                    if (!Guid.TryParse(patchmodel[i].Uuid, out Guid result)) throw new FormatException("invalid Changeuserpatch uuid");

                    templist[i].Login.Uuid = result;
                }
            }


            data.Results = templist;

            string rawjsonstring = JsonSerializer.Serialize(data);

            return new StringContent(rawjsonstring, Encoding.UTF8, "application/json");
        }



        public async Task<StringContent> SerializeClassToJson<T>(T data) where T : class
        {

            string rawjsonstring = JsonSerializer.Serialize(data);

            return new StringContent(rawjsonstring,Encoding.UTF8, "application/json");
        }



        public async Task<BulkUserModel> CreatebulkBody(List<Changeusershort> data)
        {
          

            return new BulkUserModel()
            {
                Firstname = [data[0].Firstname, data[1].Firstname],
                Lastname = [data[2].Lastname],
                Email = [data[4].Email]
            };


          

        }



    }
}
