
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoCrudPeopleApi.Auth;

using Mongodb;
using MongoLogic.Crud;
using MongoLogic.CRUD;
using System.Reflection;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


builder.Services.AddAuthentication("ApiKey")
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
