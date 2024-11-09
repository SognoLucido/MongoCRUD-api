using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Reflection.Emit;
using System;
using Logger;
using System.Net;
using System.Net.Http.Headers;


namespace Mongocrud.api.Integration.test.EndpointsTests
{



   


    public class Loggertests(ProgramTestApplicationFactory factory) : IClassFixture<ProgramTestApplicationFactory>
    {

        private readonly HttpClient client = factory.CreateClient();
        private readonly Loggermock logger = factory.Services.GetService<Loggermock>();
        private readonly string jez = factory.Services.GetRequiredService<IConfiguration>()["API_KEY"]!;
        private readonly IConfiguration zzz = factory.Services.GetRequiredService<IConfiguration>();


        [Fact]
        public async Task TestLogLevel()
        {


            ///////////////


            var Unauthorized = await client.PostAsync($"/logs?level={LogLevelError.Warning}", null);

            client.DefaultRequestHeaders.Add("x-api-key",jez);
            var test3 = await client.PostAsync($"/logs?level={LogLevelError.Critical}", null);

            //////////////


            Assert.Equal(HttpStatusCode.Unauthorized, Unauthorized.StatusCode);

            Console.WriteLine();
        }
    }




}
