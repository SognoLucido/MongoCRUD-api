
using Microsoft.Extensions.DependencyInjection;
using Mongodb;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoLogic;
using MongoLogic.Crud;
using MongoLogic.CRUD;



var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

//builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration["INIT_VAR"];

builder.Services.AddSingleton<MongoContext>(_ => new (connectionString));
//builder.Services.AddSingleton(new MongoContext(connectionString));

//builder.Services.Configure<MongoSettings>(builder.Configuration.GetValue<string>("INIT_VAR");

builder.Services.AddScoped<IPeopleservice,Peopleservice>();
//builder.Services.AddSingleton<IManualmapper,Manualmapper>();



var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

//app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
