/* This code was taken from 2018_Semester1_Week7\Lecture6\Data */
using Microsoft.EntityFrameworkCore;
using MagicInventoryWebsite.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MagicInventoryWebsite.Data
{
    public class MagicInventoryContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<OwnerInventory> OwnerInventory { get; set; }
        public DbSet<StockRequest> StockRequests { get; set; }
        //Store Inventory is implicitly created

        //related to customer cart
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails{ get; set; }

        public MagicInventoryContext(DbContextOptions<MagicInventoryContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StoreInventory>().HasKey(x => new { x.StoreID, x.ProductID });
            //tells entity framework what the composite PK is for storeinventory 
        }

        public DbSet<MagicInventoryWebsite.Models.StoreInventory> StoreInventory { get; set; }
    }
}
/* This code was taken from 2018_Semester1_Week7\Lecture6\Data 
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MagicInventoryWebsite.Models;

namespace MagicInventoryWebsite.Data
{
    public class MagicInventoryContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<OwnerInventory> OwnerInventory { get; set; }
        public DbSet<StockRequest> StockRequests { get; set; }
        //Store Inventory is implicitly created

        public MagicInventoryContext(DbContextOptions<MagicInventoryContext> options) : base(options)
        { }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<StoreInventory>().HasKey(x => new { x.StoreID, x.ProductID });
            //tells entity framework what the composite PK is for storeinventory 
        }

        public DbSet<MagicInventoryWebsite.Models.StoreInventory> StoreInventory { get; set; }
    }
}*/
