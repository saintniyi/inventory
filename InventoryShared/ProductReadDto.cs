using InventoryModel.Models;

namespace InventoryShared
{
    public class ProductReadDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProductCategory Category { get; set; }
        public decimal Price { get; set; }
        public byte[]? ProductImage { get; set; }
        public string? ProductImageBase64 =>
            ProductImage != null ? $"data:image/png;base64,{Convert.ToBase64String(ProductImage)}" : null;
        public int StockQty { get; set; }
        public SupplierReadDto? Supplier { get; set; }
    }

    

}
