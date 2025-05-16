using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryModel.Models
{
    public class Supplier
    {
        public int Id { get; set; }



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
