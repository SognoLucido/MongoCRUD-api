using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace Mongodb.Models;




public class PersonApiModel
{
    
    public List<PersonDbModel> Results { get; set; }

}




public class PersonDbModel
{

    [JsonIgnore]
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId _id { get; set; }

    public string Gender { get; set; }
    public Name Name { get; set; }
    public Location Location { get; set; }

    private string _email;
    public string Email { get=>_email; set=> _email = value.ToLower(); }

    public Login Login { get; set; }
    public Dob Dob { get; set; }
    public Dob Registered { get; set; }
    public string Phone { get; set; }
    public string Cell { get; set; }
    public Id Id { get; set; }
    public Picture Picture { get; set; }
    public string Nat { get; set; }
}

public class Dob
{
    public DateTimeOffset Date { get; set; }
    public int Age { get; set; }
}

public class Id
{
    public string Name { get; set; }
    public string? Value { get; set; }
}

public class Location
{
    public Street Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Country { get; set; }


    [JsonIgnore]
    [BsonElement("Postcode")]
    public string FormattedPostcode { get; set; }


    [Required]
    [BsonIgnore]
    public object Postcode
    {
        //get => FPostcode ;
        set
        {

            if (value is JsonElement jsonElement)
            {

                switch (jsonElement.ValueKind)
                {
                    case JsonValueKind.String: FormattedPostcode = jsonElement.GetString(); break;
                    default: FormattedPostcode = jsonElement.GetRawText(); break;
                }

            }

        }
    }





    public Coordinates Coordinates { get; set; }
    public Timezone Timezone { get; set; }
}

public class Coordinates
{
    public string Latitude { get; set; }
    public string Longitude { get; set; }
}

public class Street
{
    public int Number { get; set; }
    public string Name { get; set; }
}

public class Timezone
{
    public string Offset { get; set; }
    public string Description { get; set; }
}

public partial class Login
{
    public Guid Uuid { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
    public string Md5 { get; set; }
    public string Sha1 { get; set; }
    public string Sha256 { get; set; }
}

public class Name
{
    public string? Title { get; set; }
    
    public string First { get; set; }
    public string Last { get; set; }
}

public class Picture
{
    public Uri Large { get; set; }
    public Uri Medium { get; set; }
    public Uri Thumbnail { get; set; }
}


