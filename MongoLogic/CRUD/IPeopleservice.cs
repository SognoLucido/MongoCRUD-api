using MongoLogic.model;
using MongoLogic.model.Api;


namespace MongoLogic.CRUD
{
    public interface IPeopleservice
    {
        Task<(PersonDbModel?,short)> Findby_id(string _Id);
        Task<(List<PersonDbModel>,short)> Agerange(int minage, int maxage);
        Task<string> InsertDupcheck(PersonApiModel model);
        Task<List<PersonDbModel>?> FindbyCustomQuary(FindsingleModel data);
        Task<byte> RemoveAsync(string id);
        Task<short> UpdateAsync(string id, PutPersonmodel ToUpdate);

        Task<short> PutAsyncFullModel(string id, PersonApiModel data);

        Task<long?> GetTotalItems();

        Task<List<PersonDbModel>?> GetXitems(int numb);

    }
}
