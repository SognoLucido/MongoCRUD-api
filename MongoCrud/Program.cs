using Bookstore_backend;
using Logger;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MongoCrudPeopleApi.Auth;
using MongoCrudPeopleApi.MinimalEndpoints;
using Mongodb;
using Mongodb.Services;
using MongoLogic.CRUD;
using System.Reflection;
using System.Text.Json.Serialization;



var builder = WebApplication.CreateBuilder(args);


//Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));


builder.Services.Logger();

builder.Services.AddAuthentication()
.AddScheme<ApiOptions, ApiKeyAuthenticationHandler>("ApiKey", options => 
{
    options.ApiKeyHeaderName= "x-api-key";
    options.ApiKey = builder.Configuration.GetValue<string>("API_KEY")!;

});

builder.Services.AddAuthorization();

builder.Services.Configure<RouteHandlerOptions>(o => o.ThrowOnBadRequest = false);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{

    opt.MapType<LogLevelError>(() => new OpenApiSchema
    {
        Type = "string",
        Enum = Enum.GetNames(typeof(LogLevelError))
            .Select(name => new OpenApiString(name))
            .ToList<IOpenApiAny>()
    });

    opt.AddSecurityDefinition("apikey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Login using Apikey",
        Name = "x-api-key",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "x-api-key"


    });


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

  
    opt.OperationFilter<AuthResponsesOperationFilter>();


    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var connectionString = builder.Configuration["INIT_VAR"];
builder.Services.AddSingleton<MongoContext>(_ => new(connectionString));
builder.Services.AddScoped<IPeopleservice, Peopleservice>();
builder.Services.AddScoped<LogService>();


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

//app.UseHttpsRedirection();
app.UseUserEndpoints();
app.UseLogEndpoints();


app.Run();



public partial class Program { }