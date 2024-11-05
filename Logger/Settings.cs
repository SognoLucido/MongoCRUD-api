using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Diagnostics;

namespace Logger
{
    public static class Settings
    {

        public static void Logger(this IServiceCollection builder)
        {


            builder.AddSerilog(opt =>
            {
                var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path: "appsettings.json", optional: false)
                .Build();

                opt.ReadFrom.Configuration(configuration);

                

            });



        }
    }
}
