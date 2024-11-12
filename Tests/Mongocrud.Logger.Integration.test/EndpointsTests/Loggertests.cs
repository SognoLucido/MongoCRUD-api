using Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mongocrud.Logger.Integration.test;
using Mongocrud.Logger.Integration.test.Models;
using System.Net;
using System.Net.Http.Json;


namespace Mongocrud.api.Integration.test.EndpointsTests;



[CollectionDefinition("logcollection", DisableParallelization = true)]
public class Logger : IClassFixture<ProgramTestApplicationFactory>
{

}



[Collection("logcollection")]
public class LoggerReadtests(ProgramTestApplicationFactory factory)
{

    private readonly HttpClient client = factory.CreateClient();
    private readonly FileUtils filez = factory.Services.GetRequiredService<FileUtils>();
    private readonly string key = factory.Services.GetRequiredService<IConfiguration>()["API_KEY"]!;



    [Fact]
    public async Task TestReadFile()
    {

  

        //////////


        client.DefaultRequestHeaders.Add("x-api-key", key);
        await client.PostAsync($"/logs?level={LogLevelError.Verbose}", null);
        await Task.Delay(2000);
        await client.PostAsync($"/logs?level={LogLevelError.Error}", null);
        await Task.Delay(2000);
        await client.PostAsync($"/logs?level={LogLevelError.Debug}", null);

        var Option1data = await client.PostAsJsonAsync("/logs/readfile", new RDate(new Ryear(DateTime.Now.Year)));
        var dataOption1 = await Option1data.Content.ReadAsStringAsync();

        var rangebody = await filez.RangeCraftResposte(dataOption1);
        var Option2data = await client.PostAsJsonAsync("/logs/readfile", rangebody);
        var dataOption2 = await Option2data.Content.ReadAsStringAsync();

        int linesAKAnumbLog1 = dataOption1.Count(c => c == '\n');
        int linesAKAnumbLog2 = dataOption2.Count(c => c == '\n');


        ////////////////

       
        Assert.Equal(3, linesAKAnumbLog1);
        Assert.Equal(2, linesAKAnumbLog2);


    }
}







[Collection("logcollection")]
public class Loggertests(ProgramTestApplicationFactory factory)
{

    private readonly HttpClient client = factory.CreateClient();
    private readonly FileUtils filez = factory.Services.GetRequiredService<FileUtils>();
    private readonly string key = factory.Services.GetRequiredService<IConfiguration>()["API_KEY"]!;





    [Fact]
    public async Task TestLogLevelFile()
    {


        object[] errors = [LogLevelError.Warning, LogLevelError.Error, LogLevelError.Critical, "Exception"];


        ///////////////


        var Unauthorized = await client.PostAsync($"/logs?level={LogLevelError.Warning}", null);

        client.DefaultRequestHeaders.Add("x-api-key", key);
        await client.PostAsync($"/logs?level={errors[0]}", null);
        await client.PostAsync($"/logs?level={errors[1]}", null);
        await client.PostAsync($"/logs?level={errors[2]}&throw-exception=true", null);




        // check message send Error message and check the message


        //try
        //{
        //    using FileStream fileStream = new FileStream("Logs/log.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //    using StreamReader reader = new StreamReader(fileStream);
        //}
        //catch (Exception zzz)
        //{
        //    Debug.WriteLine(zzz.Message);
        //}



        var ex = await filez.Findmatch(errors);
        //////////////


        Assert.Equal(HttpStatusCode.Unauthorized, Unauthorized.StatusCode);
        Assert.True(ex.All(p => p), "");

    }



}
