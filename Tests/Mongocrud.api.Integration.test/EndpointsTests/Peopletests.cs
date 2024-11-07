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

        ////////////////

        var InsertUserRequest = await client.PostAsync("api/user", jsonbody);

        var getUserbyageRange = await client.GetFromJsonAsync<List<Changeusershort>>("/api/user/byage-nolimit?minage=98&maxage=99");

        ///////////////
        
        Assert.True(getUserbyageRange.Count >= 2);


    }





    //[Fact]
    //public async Task Checkendpoint()
    //{
    //    var des = _factory.Services.GetService<Deserializer>();

    //    string rawjsonstring = await des.CreateJsonUsers();

    //    var rawmanualjson = "{\"results\": [{\"gender\": \"male\",\"name\": {\"title\": \"Mr\",\"first\": \"Brian\",\"last\": \"Day\"},\"location\": {\"street\": {\"number\": 409,\"name\": \"W Campbell Ave\"},\"city\": \"St. Louis\",\"state\": \"Pennsylvania\",\"country\": \"United States\",\"postcode\":31180,\"coordinates\": {\"latitude\": \"83.6369\",\"longitude\": \"-148.3784\"},\"timezone\": {\"offset\": \"-3:30\",\"description\": \"Newfoundland\"}},\"email\": \"brian.day@example.com\",\"login\": {\"uuid\": \"1340edcb-410e-4008-955d-49d3c679c62b\",\"username\": \"goldensnake330\",\"password\": \"emerson\",\"salt\": \"ted6aaGh\",\"md5\": \"c7f1b1780112dc767d4e172807cc8587\",\"sha1\": \"083716ae924691b3664247d55b291b250054e500\",\"sha256\": \"a90dd269a03e3f9345909538882d7a9fefe6923f68fdc7aee80b1a497aa69f78\"},\"dob\": {\"date\": \"1977-07-26T15:04:05.473+00:00\",\"age\": 47},\"registered\": {\"date\": \"2019-10-27T09:51:36.063+00:00\",\"age\": 5},\"phone\": \"(834) 673-1777\",\"cell\": \"(498) 320-2654\",\"id\": {\"name\": \"SSN\",\"value\": \"044-31-2332\"},\"picture\": {\"large\": \"https://randomuser.me/api/portraits/men/24.jpg\",\"medium\": \"https://randomuser.me/api/portraits/med/men/24.jpg\",\"thumbnail\": \"https://randomuser.me/api/portraits/thumb/men/24.jpg\"},\"nat\": \"US\"}]}";

    //   var testpost = "{\"results\":[{\"gender\":\"female\",\"name\":{\"title\":\"Miss\",\"first\":\"Ramya\",\"last\":\"Salian\"},\"location\":{\"street\":{\"number\":6798,\"name\":\"Sao Tome Old Quarter\"},\"city\":\"Sirsa\",\"state\":\"Haryana\",\"country\":\"India\",\"postcode\":31180,\"coordinates\":{\"latitude\":\"5.0193\",\"longitude\":\"24.9719\"},\"timezone\":{\"offset\":\"+11:00\",\"description\":\"Magadan, Solomon Islands, New Caledonia\"}},\"email\":\"ramya.salian@example.com\",\"login\":{\"uuid\":\"9c0fadd5-331e-4968-b4b6-6a37a8652e1a\",\"username\":\"greengorilla704\",\"password\":\"pooter\",\"salt\":\"le3LzH9t\",\"md5\":\"88b6448f36ce9fbf9d5d73ce95568751\",\"sha1\":\"249bace1f1330ce5bac8542dcf6a8f44e59346fb\",\"sha256\":\"8c6d005ce53e9ef705f28d7aedff002206dc9fa587988e9245c361260d1d821c\"},\"dob\":{\"date\":\"1958-09-16T23:59:41.344Z\",\"age\":66},\"registered\":{\"date\":\"2011-06-15T22:40:17.229Z\",\"age\":13},\"phone\":\"8431906523\",\"cell\":\"8281567545\",\"id\":{\"name\":\"UIDAI\",\"value\":\"192847968292\"},\"picture\":{\"large\":\"https://randomuser.me/api/portraits/women/62.jpg\",\"medium\":\"https://randomuser.me/api/portraits/med/women/62.jpg\",\"thumbnail\":\"https://randomuser.me/api/portraits/thumb/women/62.jpg\"},\"nat\":\"IN\"}],\"info\":{\"seed\":\"01a4817b9de3df5a\",\"results\":1,\"page\":1,\"version\":\"1.4\"}}";

    //    var jsonbody = new StringContent(rawjsonstring, Encoding.UTF8, "application/json");

    //    Debug.WriteLine(rawjsonstring);
    //    Debug.WriteLine(testpost);


    //    var InsertUserRequest = await client.PostAsync("api/user/ok", jsonbody);
    //    //var test = await client.PostAsync("api/user",testpost);




    //}






}