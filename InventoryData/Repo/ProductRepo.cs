using InventoryData.Data;
using InventoryData.IRepo;
using InventoryModel.Models;


namespace InventoryData.Repo
{
    public class ProductRepo : Repo<Product>, IProductRepo
    {

        private AppDbContext _context;

        public ProductRepo(AppDbContext context) : base(context) 
        {
            _context = context;
        }


        public void Update(Product product)
        {
            var productFromDb = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (productFromDb != null)
            {
                productFromDb.Name = product.Name;
                productFromDb.Category = product.Category;
                productFromDb.Price = product.Price;
                productFromDb.ProductImage = product.ProductImage;
                productFromDb.StockQty = product.StockQty;
                productFromDb.SupplierId = product.SupplierId;
            }
        }
    }
}
