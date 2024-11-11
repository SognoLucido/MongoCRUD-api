
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace Mongocrud.Logger.Integration.test
{
    public class ProgramTestApplicationFactory : WebApplicationFactory<Program> , IAsyncLifetime
    {

        //private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder().Build();
        //private readonly Loggermock _loggermock;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

          
            


            builder.ConfigureServices(services =>
            {
                services.AddSingleton<FileUtils>();
            });


            builder.ConfigureAppConfiguration((context, config) =>
            {
                var settings = new Dictionary<string, string>
                {
                    { "API_KEY", "hello" }
                };

                config.AddInMemoryCollection(settings);
            });


           


        }


        public async Task InitializeAsync()
        {
            // FileUtils filez = Services.GetRequiredService<FileUtils>();

            //await filez.Clearfile();


            try
            {
                await File.WriteAllTextAsync("Logs/log.txt", string.Empty);
            }
            catch
            {

            }



        }

        public new async Task DisposeAsync()
        {
           
        }

    }
}
