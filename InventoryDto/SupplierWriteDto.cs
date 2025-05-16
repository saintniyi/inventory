using System.ComponentModel.DataAnnotations;


namespace InventoryDto
{
    public class SupplierWriteDto
    {
        [Required]
        [StringLength(150)]
        [Display(Name = "Supplier Name")]
        public string? Name { get; set; }


        [StringLength(100)]
        [Display(Name = "Contact Email")]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }


        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }
    }



}
