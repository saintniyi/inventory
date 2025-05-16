using InventoryModel.Models;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace InventoryDto
{
    public class ProductWriteDto
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Product Name")]
        public string Name { get; set; }



        [Required]
        [Display(Name = "Category")]
        public ProductCategory Category { get; set; }



        [Required]
        [Display(Name = "Unit Price")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }



        [Display(Name = "Product Image")]
        public IFormFile? ProductImageFile { get; set; }



        [Required]
        [Display(Name = "Available Stock")]
        public int StockQty { get; set; }



        [Required]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }
    }



}
