using MongoCrudPeopleApi.Model;
using Mongodb.Models;
using Mongodb.Models.Dto;



namespace MongoLogic.CRUD
{
    public interface IPeopleservice
    {

        Task<int> PatchUseritem(PatchUserItemModel identifierdata, UserItemPatchModel bodydata);
        Task<List<PersonBaseModel>?> BulkSearchUsers(BulkUserModel userdata, Pagesize pagesize);
        Task<List<PersonBaseModel>?> SearchUsers(UsersModel userdata,Pagesize pagesize);
        Task<List<PersonBaseModel>?> GetAgerangeUserItem(int minage, int maxage);
        Task<(int, string?)> Insert(PersonApiModel model, bool duplicatecheck);
        Task<bool> RemoveItem<T>(T item);

        //Task testex();

    }
}
