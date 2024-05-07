using MongoLogic.model;
using MongoLogic.model.Api;

namespace MongoLogic
{
    public class Manualmapper : IManualmapper

    {

        public  async Task<LinkedList<PersonDbModel>> ApitoDbmodel(PersonApiModel Apidatain, bool[] duplicates)
        {
           var dbmodels = new LinkedList<PersonDbModel>();


          

            for (var i = 0;i< Apidatain.Results.Length; i++)          
            {
                if (!duplicates[i]) continue;

                PersonDbModel personDB = new()
                {
                    Gender = Apidatain.Results[i].Gender,
                    Name = new model.Name()
                    {
                        Title = Apidatain.Results[i].Name.Title,
                        First = Apidatain.Results[i].Name.First,
                        Last = Apidatain.Results[i].Name.Last
                    },
                    Location = new model.Location()
                    {
                        Street = new model.Street()
                        {
                            Number = Apidatain.Results[i].Location.Street.Number,
                            Name = Apidatain.Results[i].Location.Street.Name
                        },

                        City = Apidatain.Results[i].Location.City,
                        State = Apidatain.Results[i].Location.State,
                        Country = Apidatain.Results[i].Location.Country,
                        Postcode = Apidatain.Results[i].Location.Postcode,

                        Coordinates = new model.Coordinates()
                        {
                            Latitude = Apidatain.Results[i].Location.Coordinates.Latitude,
                            Longitude = Apidatain.Results[i].Location.Coordinates.Longitude
                        },
                        Timezone = new model.Timezone()
                        {
                            Offset = Apidatain.Results[i].Location.Timezone.Offset,
                            Description = Apidatain.Results[i].Location.Timezone.Description
                        }
                    },
                    Email = Apidatain.Results[i].Email,
                    Login = new model.Login() 
                    {
                        Uuid = Apidatain.Results[i].Login.Uuid,
                        Username = Apidatain.Results[i].Login.Username,
                        Password = Apidatain.Results[i].Login.Password,
                        Salt = Apidatain.Results[i].Login.Salt,
                        Md5 = Apidatain.Results[i].Login.Md5,
                        Sha1 = Apidatain.Results[i].Login.Sha1,
                        Sha256 = Apidatain.Results[i].Login.Sha256
                    },
                    Dob = new model.Dob()
                    {
                        Date = Apidatain.Results[i].Dob.Date,
                        Age = Apidatain.Results[i].Dob.Age
                    },
                    Registered = new model.Dob()
                    {
                        Date = Apidatain.Results[i].Registered.Date,
                        Age = Apidatain.Results[i].Registered.Age
                    },
                    Phone = Apidatain.Results[i].Phone,
                    Cell = Apidatain.Results[i].Cell,
                    Id = new model.Id()
                    {
                        Name = Apidatain.Results[i].Id.Name,
                        Value = Apidatain.Results[i].Id.Value,

                    },
                    Picture = new model.Picture()
                    {
                        Large = Apidatain.Results[i].Picture.Large,
                        Medium = Apidatain.Results[i].Picture.Medium,
                        Thumbnail = Apidatain.Results[i].Picture.Thumbnail
                    },
                    Nat = Apidatain.Results[i].Nat
                    
                };

                dbmodels.AddLast(personDB);
            }

            return  dbmodels; 
        }


    }
}
