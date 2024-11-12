
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace Mongocrud.Logger.Integration.test
{
    public class ProgramTestApplicationFactory : WebApplicationFactory<Program> , IAsyncLifetime
    {

     

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {

          
            


            builder.ConfigureServices(services =>
            {
                services.AddSingleton<FileUtils>(
                    
                    );
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

            var test = await FIlefinder.LogFilepath();


            if(File.Exists(test))
            await File.WriteAllTextAsync(test, string.Empty);
           

        }

        public new async Task DisposeAsync()
        {
           
        }

    }
}
