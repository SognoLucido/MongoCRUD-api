using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Mongodb;
using Serilog;
using System.Diagnostics;
using Testcontainers.MongoDb;

namespace Mongocrud.api.Integration.test;

public class ProgramTestApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{

    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {

        builder.ConfigureServices(services =>
        {
            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(MongoContext));

            if(dbContext is not null)
            services.Remove(dbContext);

            var serilogService = services.FirstOrDefault(s => s.ServiceType == typeof(LoggerConfiguration));
            if (serilogService is not null)
                services.Remove(serilogService);


            services.AddSingleton<MongoContext>(container => new(_mongoDbContainer.GetConnectionString()));

            

            services.AddSingleton(_ => new Deserializer());
          



            //todo serilog mock

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

