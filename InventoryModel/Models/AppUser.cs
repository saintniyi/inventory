using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;



namespace InventoryModel.Models
{
    public class AppUser : IdentityUser
    {
        public string Email { get; set; }



        public string PhoneNumber { get; set; }



        [MaxLength(100)]
        public string FirstName { get; set; }



        [MaxLength(100)]
        public string LastName { get; set; }


        [Range(1, 140)]
        public int Age { get; set; }


    }
}
