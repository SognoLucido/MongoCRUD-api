
using Microsoft.AspNetCore.Mvc;
using MongoCrudPeopleApi.Model;
using Mongodb.Models;
using Mongodb.Models.Dto;
using MongoLogic.CRUD;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


namespace MongoCrudPeopleApi.MinimalEndpoints;

public static class UserEndpoints
{

    public static async void UseUserEndpoints(this IEndpointRouteBuilder app)
    {

        var apiGroup = app.MapGroup("api/user").WithTags("User");

        apiGroup.MapGet("search", Getusersearch);
        //     .Produces<List<PersonBaseModel>>(200);

        apiGroup.MapGet("byage-limit", GetbyAgeRange)
            .Produces<List<PersonBaseModel>>(200);

        apiGroup.MapGet("byage-nolimit", GetbyAgeRange_nolimit)
            .ExcludeFromDescription();

        apiGroup.MapPatch("/", PatchUserItem);

        apiGroup.MapPost("bulk-search", Bulksearch)
            .Produces<List<PersonBaseModel>>(200);

        apiGroup.MapPost("", PostUserItemModel);

        apiGroup.MapDelete("{uuid_email}", Delete)
            .Produces(400)
            .Produces(404)
            .Produces(200);


    }






    /// <summary>
    /// 
    /// </summary>
    /// <param name="Pagesize"> default = 100 items </param>
    /// <returns></returns>
    private static async Task<IResult> Getusersearch(
           
           //[FromQuery]UsersModel search,
           [Length(3, 20)] string? firstname,
           [Length(3, 20)] string? lastname,
           [Length(3, 20)] string? state,
           [Length(3, 20)] string? country,
           [EmailAddress] string? email,
           Guid? uuid,
           [Range(1,150)]int? age,
           string? phonenumber,
          IPeopleservice dbcall,
          Pagesize Pagesize = Pagesize.i100
          
        )
    {

        




        var data = await dbcall.SearchUsers(new()
        {
            firstname = firstname,
            lastname = lastname,
            state = state,
            country = country,
            email = email,
            uuid = uuid,
            age = age,
            phoneNumber = phonenumber

        }, Pagesize);

        return data is null ? Results.NotFound() : Results.Ok(data);

    }








    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// userid(uuid) OR email    
    /// </remarks>
    /// <param name="patchdata"> Optional parameters; at least one is required </param>
    /// <returns></returns>
    private static async Task<IResult> PatchUserItem(
        Guid? userid,
        [EmailAddress]string? email,
        [FromBody]UserItemPatchModel patchdata,
        //[FromQuery] PatchUserItemModel request,
        IPeopleservice dbcall
        )
    {




        if (userid is null && email is null) return Results.BadRequest();
        if (userid is not null && email is not null) return Results.BadRequest("choose one ");


        //Guid tempid = new();

        //if(userid is not null )
        //if(!Guid.TryParse(userid, out tempid)) return Results.BadRequest("invalid userid");

        //if(!Regex.IsMatch(email, emailregex)) return Results.BadRequest("invalid email");


        //Guid? RequestUserID;

        var request = new PatchUserItemModel
        {
            userid = userid,
            email = email,
        };


        var statuscode = await dbcall.PatchUseritem(request, patchdata);


        return Results.StatusCode(statuscode);

        //return Results.Ok();
    }








    /// <summary>
    /// </summary>
    /// <remarks>
    ///Swagger can't handle the load (+1000 return items), so the result of this endpoint will be truncated       
    ///If you want to check the real limits, make a GET request directly in the browser or with Postman, etc.        
    /// like : /api/user/byage-nolimit?minage=1&amp;maxage=100      
    /// </remarks>
    /// <returns></returns>
    private static async Task<IResult> GetbyAgeRange(
         [Range(0, 150)] int minage,
         [Range(0, 150)] int maxage,
         IPeopleservice dbcall
        )
    {
        if (minage > maxage) return Results.BadRequest("minAge > maxAge");


        var Result = await dbcall.GetAgerangeUserItem(minage, maxage);


        if (Result is null) return Results.NotFound();
        else
        {

            if (Result.Count < 100) return Results.Ok(Result);

            var truncList = Result.Take(100).ToList();

            return Results.Ok(truncList);
        }

    }





    private static async Task<IResult> GetbyAgeRange_nolimit(
        [Range(0, 150)] int minage,
         [Range(0, 150)] int maxage,
         IPeopleservice dbcall
        )
    {
        if (minage > maxage) return Results.BadRequest("minAge > maxAge");


        var Result = await dbcall.GetAgerangeUserItem(minage, maxage);


        return Result is null ? Results.NotFound() : Results.Ok(Result);
    }





  



    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// likethis : { "firstname": [ "marice", "mario" ], "lastname": [ "mortensen", "fortin" ] ,"age": [40,41,42,43]}
    /// </remarks>
    /// <param name="Pagesize">default = 100</param>
    /// <returns></returns>
    private static async Task<IResult> Bulksearch(
        BulkUserModel search,
         IPeopleservice dbcall,
        [FromQuery] Pagesize Pagesize = Pagesize.i100
        )
    {
        var data = await dbcall.BulkSearchUsers(search, Pagesize);

        return data is null ? Results.NotFound() : Results.Ok(data);
    }


    /// <summary>
    ///  insert User data 1:1 from https://randomuser.me/api?results=3 , copy and paste in the post here . database is empty by default
    /// </summary>
    /// <param name="dupecheck"> check duplicates before insert in the database (true,false) , This does not remove existing records in the database</param>
    /// <returns></returns>
    private static async Task<IResult> PostUserItemModel(
        [FromBody]PersonApiModel Value,
         IPeopleservice dbcall,
        bool dupecheck = true
        )
    {
        if (Value.Results.Count == 0) return Results.BadRequest();
        else if (Value.Results.Count > 1000)
        {
            return Results.BadRequest("POST too big");
        }


        var (code, messg) = await dbcall.Insert(Value, dupecheck);

     

        return Results.Json( messg,statusCode: code);
    }




    /// <summary>
    /// deleteby uuid or email  --> uuid : guid , email : string
    /// </summary>
    /// <param name="uuid_email"></param>
    /// <returns></returns>
    private static async Task<IResult> Delete(
        [FromRoute]string uuid_email,
        IPeopleservice dbcall
        )
    {

        bool status;

        if (Guid.TryParse(uuid_email, out var uuid))
        {
            status = await dbcall.RemoveItem(uuid);
        }
        else if (Regex.IsMatch(uuid_email, emailregex))
        {
            status = await dbcall.RemoveItem(uuid_email);
        }
        else return Results.BadRequest();


        return status ? Results.Ok() : Results.NotFound();
    }

    const string emailregex = @"^[\w\-\.]+@([\w-]+\.)+[\w-]{2,}$";







}