using InventoryModel.Models;


namespace InventoryData.IRepo
{
    public interface IAppUserRepo : IRepo<AppUser>
    {
        void Update(AppUser user);
    }
}
