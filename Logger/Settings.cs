using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Logger
{
    public static class Settings
    {



        //public static void  LoggerInit(this IServiceCollection builder)
        //{


        //    var configuration = new ConfigurationBuilder()
        //   .SetBasePath(Directory.GetCurrentDirectory())
        //   .AddJsonFile("appsettings.json")
        //   .Build();

        //    var logger = new LoggerConfiguration()
        //  .ReadFrom.Configuration(configuration)
        //  .CreateLogger();

        //    builder.AddLogging(opt =>
        //    {
        //        opt.AddSerilog();
        //    });
        //    //builder.AddSerilog();

        //    logger.Information("Hello, world!");
        //    logger.Warning("wtfwaring");
          
        //}
    }
}
