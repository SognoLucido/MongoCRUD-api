

namespace Mongocrud.api.Integration.test.Model
{
    public record Changeusershort(string Firstname, string Lastname, string Email);
    public record ChangeuserLong(string Firstname, string Lastname, string email,string? streetnumb , string? streetname, string? country,string? age );

    public record Changeuserage(string Firstname,string Lastname,string Email,int age);

    public record Changeuserpatch(string Firstname, string Lastname, string Email,string Uuid);

    public record PatchFirstnameNLast(string Firstname,string Lastname);
    public record PatchLastname(string Lastname);
    public record PatchEmail(string Email);
}
