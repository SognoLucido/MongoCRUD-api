using Mongodb.Models;
using Mongodb.Models.Dto;
using MongoLogic.model.Api;


namespace MongoLogic.CRUD
{
    public interface IPeopleservice
    {

        Task<List<PersonDbModel>?> SearchUsers(Usersearch userdata);
        Task TestLog();
        Task<(PersonDbModel?,short)> Findby_id(string _Id);
        Task<List<PersonDbModel>?> GetAgerangeUserItem(int minage, int maxage);
        Task<(int,string?)> Insert(PersonApiModel model, bool duplicatecheck);
        Task<List<PersonDbModel>?> FindbyCustomQuary(FindsingleModel data);
        Task<byte> RemoveAsync(string id);
        Task<short> UpdateAsync(string id, PutPersonmodel ToUpdate);

        Task<short> PutAsyncFullModel(string id, PersonApiModel data);

        Task<long?> GetTotalItems();

        Task<List<PersonDbModel>?> GetXitems(int numb);

    }
}
