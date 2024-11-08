using Microsoft.Extensions.DependencyInjection;
using Mongocrud.api.Integration.test.Model;
using System.Net;
using System.Net.Http.Json;

namespace Mongocrud.api.Integration.test.EndpointsTests;

public class Peopletests(ProgramTestApplicationFactory factory) : IClassFixture<ProgramTestApplicationFactory>
{

    private readonly HttpClient client = factory.CreateClient();
    private readonly Deserializer des = factory.Services.GetService<Deserializer>();

    [Fact]
    public async Task PostUser_Searchuser()
    {


        List<Changeusershort> userToinsert =
            [
               new("marco","utez","marco.utez@example.com"),
               new("luigi","bianco","luigi.bianco@example.com"),
               new("mandarino","bello","mandarino.bello@example.com")
            ];


        var jsonbody = await des.CreateJsonUsers(userToinsert);


        /////////////


        var InsertUserRequest = await client.PostAsync("api/user", jsonbody);


        var findMarco = await client.GetFromJsonAsync<List<Changeusershort>>($"api/user/search?email={userToinsert[0].Email}");
        var findluigi = await client.GetFromJsonAsync<List<Changeusershort>>($"api/user/search?email={userToinsert[1].Email}");
        var findmandarino = await client.GetFromJsonAsync<List<Changeusershort>>($"api/user/search?email={userToinsert[2].Email}");



        /////////////


        Assert.Equal(HttpStatusCode.Created, InsertUserRequest.StatusCode);
        Assert.Equal(userToinsert[0].Firstname.ToLower(), findMarco[0].Firstname);
        Assert.Equal(userToinsert[1].Lastname.ToLower(), findluigi[0].Lastname);
        Assert.Equal(userToinsert[2].Email.ToLower(), findmandarino[0].Email);


    }






    [Fact]
    public async Task GetAgerange()
    {

        List<Changeuserage> userToinsert =
           [
              new("AgerangeFirstname","AgerangeLastname","agerange@example.com",99),
                new("AgerangeFirstnamev2","AgerangeLastnamev2","agerangev2@example.com",98),
           ];


        var jsonbody = await des.CreateJsonUsers(userToinsert);
        var InsertUserRequest = await client.PostAsync("api/user", jsonbody);

        ////////////////

        var getUserbyageRange = await client.GetFromJsonAsync<List<Changeusershort>>("/api/user/byage-nolimit?minage=98&maxage=99");

        ///////////////

        Assert.True(getUserbyageRange.Count >= 2);


    }



    [Fact]
    public async Task PatchUser()
    {

        List<Changeuserpatch> userToinsert =
          [
             new("oldn","oldln","oldemail@example.com","62C3EBA570C94063BA26234E075E1E2C"),
              
          ];

        var newNameNLastName = new PatchFirstnameNLast("newn" , "newln");
        var newEmail = new PatchEmail("newemail@example.com");

        var patchNameNLastName = await des.SerializeClassToJson(newNameNLastName);
        var patchEmail = await des.SerializeClassToJson(newEmail);

        var jsonbody = await des.CreateJsonUsers(userToinsert);
        var InsertUserRequest = await client.PostAsync("api/user", jsonbody);

        //////////


      var test =  await client.PatchAsync($"api/user?email={userToinsert[0].Email}", patchNameNLastName);



       var pep = await client.PatchAsync($"api/user?uuid={userToinsert[0].Uuid}", patchEmail);



        var Getpatcheduser = await client.GetFromJsonAsync<List<Changeusershort>>($"api/user/search?uuid={userToinsert[0].Uuid}");



        //////////


        Assert.Equal(newNameNLastName.Firstname, Getpatcheduser[0].Firstname);
        Assert.Equal(newNameNLastName.Lastname, Getpatcheduser[0].Lastname);
        Assert.Equal(newEmail.Email, Getpatcheduser[0].Email);


    }



}