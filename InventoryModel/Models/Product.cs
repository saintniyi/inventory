using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace InventoryModel.Models
{
    public class Product
    {
        public int Id { get; set; }


        [Required]
        [StringLength(100)]
        [Display(Name = "Product Name")]
        public string? Name { get; set; }



        [Required]
        [Display(Name = "Category")]
        public ProductCategory Category { get; set; }



        [Required]
        [Display(Name = "Unit Price")]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }



        [Display(Name = "Product Image")]
        public byte[]? ProductImage { get; set; }



        [Required]
        [Display(Name = "Available Stock")]
        public int StockQty { get; set; }



        [Required]
        public int SupplierId { get; set; }



        [ForeignKey("SupplierId")]
        [ValidateNever]
        public Supplier? Supplier { get; set; }

    }



    public enum ProductCategory
    {
        Electronics = 1,
        Apparel = 2,
        Groceries = 3,
        Furniture = 4,
        Books = 5,
        Others = 6
    }





}
