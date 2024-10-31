

using Logger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoCrudPeopleApi.Auth;

using Mongodb;
using Mongodb.Services;
using MongoLogic.CRUD;
using Serilog;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);



builder.Services.AddSerilog(opt =>
{
    var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(path: "appsettings.json", optional: false)
    .Build();

    opt.ReadFrom.Configuration(configuration);


});

//Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));


//builder.Services.LoggerInit();

builder.Services.AddAuthentication()
.AddScheme<ApiOptions, ApiKeyAuthenticationHandler>("ApiKey", options => { });


builder.Services.AddControllers()
    .AddJsonOptions(opt =>
{
    opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
}); ;


//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{

    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "MongoCRUD API:User Profile Management_DEMO",
        Description = "ASP.NET Core Web API",
        Contact = new OpenApiContact
        {
            Name = "Francesco Barbano",
            Url = new Uri("https://github.com/SognoLucido/MongoCRUD-api")
        }

    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var connectionString = builder.Configuration["INIT_VAR"];
builder.Services.AddSingleton<MongoContext>(_ => new(connectionString));
builder.Services.AddScoped<IPeopleservice, Peopleservice>();


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();


app.MapControllers();

app.Run();
