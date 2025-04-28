using InventoryModel.Models;


namespace InventoryData.IRepo
{
    public interface ISupplierRepo : IRepo<Supplier>
    {
        void Update(Supplier supplier);
    }
}
