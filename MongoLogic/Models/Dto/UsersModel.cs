
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;


namespace Mongodb.Models.Dto
{
    public class UsersModel
    {
        [Length(3,15)]
        public string? Firstname {  get; set; }

        [Length(3,15)]
        public string? Lastname { get; set; }

        [Length(3,20)]
        public string? State { get; set; }

        [Length(3,20)]
        public string? Country { get; set; }

        public string? Email { get; set; }

        //[BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid? Uuid { get; set; }

        public int? Age { get; set; }

        public string? PhoneNumber {  get; set; }



    }


    public class BulkUserModel
    {
        //[Length(3, 15)]
        public IEnumerable<string?>? Firstname { get; set; }

        //[Length(3, 15)]
        public IEnumerable<string?>? Lastname { get; set; }

        //[Length(3, 20)]
        public IEnumerable<string?>? State { get; set; }

        //[Length(3, 20)]
        public IEnumerable<string?>? Country { get; set; }

        public IEnumerable<string?>? Email { get; set; }

        public IEnumerable<Guid?>? Uuid { get; set; }

        public IEnumerable<int?>? Age { get; set; }

        public IEnumerable<string?>? PhoneNumber { get; set; }

    }


}
