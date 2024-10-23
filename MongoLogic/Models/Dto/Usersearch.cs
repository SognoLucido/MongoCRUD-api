
using System.ComponentModel.DataAnnotations;


namespace Mongodb.Models.Dto
{
    public class Usersearch
    {
        [Length(4,15)]
        public string? Firstname {  get; set; }

        [Length(4,15)]
        public string? Lastname { get; set; }

        [Length(3,20)]
        public string? State { get; set; }

        [Length(3,20)]
        public string? Country { get; set; }

        public string? Email { get; set; }

        public Guid? Uuid { get; set; }

        public int? Age { get; set; }

        public string? PhoneNumber {  get; set; }



    }



    public class BulkUserSearch
    {
        public List<Firstname> Firstname { get; set; }
        public List<Lastname> Lastname { get; set; }

    }

    public record Firstname([Length(3, 15)] string firstname );

    public record Lastname([Length(3, 15)] string lastname);



}
