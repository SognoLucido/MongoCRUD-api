
# MongoCrud
CRUD api Based on randomuser.me/api model    



**Tools and Technologies** :
- ASP.NET Core Web API
- .net8
- serilog(appsettings base config) console & file
- Mvc Controllers
- Minimal api (check branch)
- apikey auth 
- xunit  
- Testcontainers   
- SwaggerUI
- docker / compose
- Mongodb
- MongoDB.Driver

> [!NOTE]
> The database is empty by default. Copy the body from https://randomuser.me/api/?results=1; it's a 1:1 ,   
> and then post it to /api/user to fill the database .  
> You can also POST multiple results ( ?results=x )
> 
> ![Screenshot 2024-05-09 122011](https://github.com/SognoLucido/MongoCRUD-api/assets/123832236/bdea874a-3297-4a9f-b274-3b30deec3ecb)

The POST endpoint includes duplicate protection before inserting data into the database

## TODO

- [x] Optimize queries
- [x] Logging to a file and retrieving log information using apikey
- [x] Unit/Integ testing
- [x] Docker implementation
- [ ]  .net9
- [ ]  Aspire
- [ ] ms Openapi 

