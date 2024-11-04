
# MongoCrud
CRUD api Based on randomuser.me/api model    
in progress


**Tools and Technologies** :
- ASP.NET Core Web API
- .net8
- serilog(appsettings base config) console & file
- Mvc Controllers
- Minimal api (todo)
- apikey auth (todo)
- xunit (todo)
- Testcontainers (todo)
- SwaggerUI
- docker / compose
- Mongodb
- MongoDB.Driver

> [!NOTE]
> If you want to utilize the POST endpoint, you can copy the body from https://randomuser.me/api/?results=1 ;it's 1:1 .  
> You can also POST multiple results ( ?results=x )
> 
> ![Screenshot 2024-05-09 122011](https://github.com/SognoLucido/MongoCRUD-api/assets/123832236/bdea874a-3297-4a9f-b274-3b30deec3ecb)

The POST endpoint includes duplicate protection before inserting data into the database

## TODO

- [x] Optimize queries
- [ ] Logging to a file and retrieving log information using apikey
- [ ] Unit/Integ testing
- [x] Docker implementation



## Live Demo
soon
