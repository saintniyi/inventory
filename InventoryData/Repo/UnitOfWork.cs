using InventoryData.Data;
using InventoryData.IRepo;


namespace InventoryData.Repo
{
    public class UnitOfWork : IUnitOfWork
    {
        public IProductRepo Product { get; private set; }

        public ISupplierRepo Supplier { get; private set; }

        public IAppUserRepo AppUser { get; private set; }


        private AppDbContext _context;


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Product = new ProductRepo(_context);
            Supplier = new SupplierRepo(_context);
            AppUser = new AppUserRepo(_context);

        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }



    }
}
