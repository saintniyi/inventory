using InventoryData.Data;
using InventoryData.IRepo;
using InventoryModel.Models;


namespace InventoryData.Repo
{
    public class SupplierRepo : Repo<Supplier>, ISupplierRepo
    {
        private AppDbContext _context;

        public SupplierRepo(AppDbContext context) : base(context)
        {
            _context = context;
        }


        public void Update(Supplier supplier)
        {
            var supplierFromDb = _context.Suppliers.FirstOrDefault(x => x.Id == supplier.Id);
            if (supplierFromDb != null)
            {
                supplierFromDb.Name = supplier.Name;
                supplierFromDb.Phone = supplier.Phone;
                supplierFromDb.Email = supplier.Email;
                
            }
        }




    }
}
