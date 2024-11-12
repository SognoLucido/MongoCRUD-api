
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mongodb;
using Testcontainers.MongoDb;

namespace Mongocrud.api.Integration.test;

public class ProgramTestApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{

    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder().Build();
    //private readonly Loggermock _loggermock;


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.ConfigureServices(services =>
        {
            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(MongoContext));

            if(dbContext is not null)
            services.Remove(dbContext);


            builder.ConfigureLogging(opt =>
            {
                opt.ClearProviders();
            });



            services.AddSingleton<MongoContext>(container => new(_mongoDbContainer.GetConnectionString()));


            services.AddSingleton<Deserializer>();
          

        });



    }



    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mongoDbContainer.DisposeAsync();    
    }
}

