using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace MongoLogic.model;




public class PersonDbModel
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? _Id { get; set; }

    public string Gender { get; set; }
    public Name Name { get; set; } 
    public Location Location { get; set; }
    public string Email { get; set; }
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
    public long Age { get; set; }
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
    public string Postcode { get; set; }
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
    public long Number { get; set; }
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
    public string? First { get; set; }
    public string? Last { get; set; }
}

public class Picture
{
    public Uri Large { get; set; }
    public Uri Medium { get; set; }
    public Uri Thumbnail { get; set; }
}


