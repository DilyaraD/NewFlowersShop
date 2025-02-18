using Microsoft.EntityFrameworkCore;
using NewFlowersShop.Models;

namespace NewFlowersShop.Models
{
    public class NewFlowersShopContext : DbContext
    {
        public NewFlowersShopContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<CashBook> CashBook { get; set; }
        public virtual DbSet<MainPage> MainPage { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<Deliveries> Deliveries { get; set; }
        //public virtual DbSet<Discounts> Discounts { get; set; }
        public virtual DbSet<Documents> Documents { get; set; }
        public virtual DbSet<Employees> Employees { get; set; }
        public virtual DbSet<FlowerCategories> FlowerCategories { get; set; }
        public virtual DbSet<FlowerType> FlowerType { get; set; }
        public virtual DbSet<Invoices> Invoices { get; set; }
        public virtual DbSet<OrderContents> OrderContents { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<ProductContents> ProductContents { get; set; }
        public virtual DbSet<ProductDiscounts> ProductDiscounts { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<RestockRequests> RestockRequests { get; set; }
        public virtual DbSet<Reviews> Reviews { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Statuses> Statuses { get; set; }
        public virtual DbSet<StoreFlowerStocks> StoreFlowerStocks { get; set; }
        public virtual DbSet<Stores> Stores { get; set; }
        //public virtual DbSet<WorkSchedules> WorkSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Customers>().HasKey(c => c.CustomerID);
            builder.Entity<MainPage>().HasKey(b => b.BackgroundId);
            builder.Entity<Deliveries>().HasKey(d => d.DeliveryID);
            //builder.Entity<Discounts>().HasKey(d => d.DiscountID);
            builder.Entity<Documents>().HasKey(d => d.DocumentID);
            builder.Entity<Employees>().HasKey(e => e.EmployeeID);
            builder.Entity<FlowerCategories>().HasKey(e => e.CategoryID);
            builder.Entity<Invoices>().HasKey(e => e.InvoiceID);
            builder.Entity<OrderContents>().HasKey(e => e.OrderContentID);
            builder.Entity<Orders>().HasKey(e => e.OrderID);
            builder.Entity<ProductContents>().HasKey(e => e.ProductContentID);
            builder.Entity<ProductDiscounts>().HasKey(e => e.ProductDiscountID);
            builder.Entity<Products>().HasKey(e => e.ProductID);
            builder.Entity<RestockRequests>().HasKey(e => e.RestockRequestID);
            builder.Entity<Reviews>().HasKey(e => e.ReviewID);
            builder.Entity<Roles>().HasKey(e => e.RoleID);
            builder.Entity<Statuses>().HasKey(e => e.StatusID);
            builder.Entity<StoreFlowerStocks>().HasKey(e => e.StoreFlowerStockID);
            builder.Entity<Stores>().HasKey(e => e.StoreID);
            //builder.Entity<WorkSchedules>().HasKey(e => e.ScheduleID);

        }


    }
}
