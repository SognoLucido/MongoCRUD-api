
using System.ComponentModel.DataAnnotations;


namespace Mongodb.Models.Dto
{
    public class UsersModel
    {
        [Length(3, 15)]
        public string? firstname { get; set; }

        [Length(3, 15)]
        public string? lastname { get; set; }

        [Length(3, 20)]
        public string? state { get; set; }

        [Length(3, 20)]
        public string? country { get; set; }

        public string? email { get; set; }

        //[BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid? uuid { get; set; }

        public int? age { get; set; }

        public string? phoneNumber { get; set; }



        public static bool TryParse(string value, out UsersModel? result) 
        { 

            result = default;


            return true;
        }





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
