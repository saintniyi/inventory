using InventoryData.Data;
using InventoryData.IRepo;
using InventoryModel.Models;


namespace InventoryData.Repo
{
    public class AppUserRepo : Repo<AppUser>, IAppUserRepo
    {
        private AppDbContext _context;

        public AppUserRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(AppUser appUser)
        {
            var appUserFromDb = _context.AppUsers.FirstOrDefault(x => x.Id == appUser.Id);
            if (appUserFromDb != null)
            {
                appUserFromDb.Email = appUser.Email;
                appUserFromDb.PhoneNumber = appUser.PhoneNumber;
                appUserFromDb.FirstName = appUser.FirstName;
                appUserFromDb.LastName = appUser.LastName;
            }


        }
    }
}
