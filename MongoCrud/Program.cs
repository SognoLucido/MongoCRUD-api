

using MongoLogic;
using MongoLogic.Crud;
using MongoLogic.CRUD;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<MongoSettings>(builder.Configuration.GetSection("PeopleDb"));
builder.Services.AddSingleton<IPeopleservice,Peopleservice>();

builder.Services.AddSingleton<IManualmapper,Manualmapper>();



var app = builder.Build();


//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
