

namespace Mongocrud.api.Integration.test.Model
{
    public record Changeusershort(string Firstname, string Lastname, string Email);
    public record ChangeuserLong(string Firstname, string Lastname, string email,string? streetnumb , string? streetname, string? country,string? age );

    public record Changeuserage(string Firstname,string Lastname,string Email,int age);
}
