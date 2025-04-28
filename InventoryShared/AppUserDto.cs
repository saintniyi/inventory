using System.ComponentModel.DataAnnotations;

namespace InventoryShared
{
    public class AppUserDto
    {
        public string Email { get; set; }


        public string? PhoneNumber { get; set; }


        [MaxLength(100)]
        public string? FirstName { get; set; }



        [MaxLength(100)]
        public string? LastName { get; set; }


    }



}
