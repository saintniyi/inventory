using InventoryModel.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;



namespace InventoryData.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }


        public DbSet<Supplier> Suppliers { get; set; }


        public DbSet<AppUser> AppUsers { get; set; }





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure precision for Price, not really necessay since handled in model
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 4);

        }





    }


}
