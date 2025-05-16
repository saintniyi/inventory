using InventoryModel.Models;


namespace InventoryData.IRepo
{
    public interface IProductRepo : IRepo<Product>
    {
        void Update(Product product);
    }
}
