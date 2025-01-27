using BOMLink.Models;
using Microsoft.EntityFrameworkCore;

namespace BOMLink.Data {
    public class BOMLinkContext : DbContext {
        public BOMLinkContext(DbContextOptions<BOMLinkContext> options) : base(options) { }

        public DbSet<BOM> BOM { get; set; }
        public DbSet<BOMItem> BOMItem { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Job> Job { get; set; }
        public DbSet<Manufacturer> Manufacturer { get; set; }
        public DbSet<Part> Part { get; set; }
        public DbSet<PO> PO { get; set; }
        public DbSet<POItem> POItem { get; set; }
        public DbSet<RFQ> RFQ { get; set; }
        public DbSet<RFQItem> RFQItem { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Supplier> Supplier { get; set; }
        public DbSet<SupplierManufacturer> SupplierManufacturer { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<User>().HasData(
                new User {
                    UserId = 1,
                    Username = "admin",
                    Password = "admin123!A",
                    FirstName = "Admin",
                    LastName = "Admin",
                    RoleId = 1
                },
                new User {
                    UserId = 2,
                    Username = "user",
                    Password = "user123!U",
                    FirstName = "User",
                    LastName = "User",
                    RoleId = 2
                }
            );

            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, Name = "HR" },
                new Role { RoleId = 2, Name = "User" }
            );

            modelBuilder.Entity<Status>().HasData(
                new Status { StatusId = 1, Name = "Open" },
                new Status { StatusId = 2, Name = "Closed" },
                new Status { StatusId = 3, Name = "Backorder" }
            );

            modelBuilder.Entity<Manufacturer>().HasData(
                new Manufacturer { ManufacturerId = 1, Name = "Acme" },
                new Manufacturer { ManufacturerId = 2, Name = "Beta" },
                new Manufacturer { ManufacturerId = 3, Name = "Gamma" }
            );

            modelBuilder.Entity<Supplier>().HasData(
                new Supplier { Id = 1, Name = "Supplier 1", ContactEmail = "test@gmail.com" },
                new Supplier { Id = 2, Name = "Supplier 2", ContactEmail = "test@gmail.com" },
                new Supplier { Id = 3, Name = "Supplier 3", ContactEmail = "test@gmail.com" }
            );

            modelBuilder.Entity<Part>()
                .Property(p => p.Labour)
                .HasPrecision(18, 2);

            modelBuilder.Entity<RFQItem>()
                .Property(r => r.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<BOM>()
                .HasOne(b => b.User)
                .WithMany(u => u.BOMs)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BOM>()
                .HasMany(bom => bom.BOMItems)
                .WithOne(bi => bi.BOM)
                .HasForeignKey(bi => bi.BOMId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Job>()
                .HasOne(b => b.User)
                .WithMany(u => u.Jobs)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Job>()
                .HasOne(j => j.Status)
                .WithMany()
                .HasForeignKey(j => j.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RFQ>()
                .HasOne(b => b.User)
                .WithMany(u => u.RFQs)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RFQ>()
                .HasOne(rfq => rfq.Status)
                .WithMany()
                .HasForeignKey(rfq => rfq.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RFQ>()
                .HasMany(r => r.RFQItems)
                .WithOne(ri => ri.RFQ)
                .HasForeignKey(ri => ri.RFQId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PO>()
                .HasOne(b => b.User)
                .WithMany(u => u.POs)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PO>()
                .HasOne(po => po.Status)
                .WithMany()
                .HasForeignKey(po => po.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PO>()
                .HasMany(p => p.POItems)
                .WithOne(pi => pi.PO)
                .HasForeignKey(pi => pi.POId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<POItem>()
                    .HasOne(poItem => poItem.RFQItem)
                    .WithMany()
                    .HasForeignKey(poItem => poItem.RFQId)
                    .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);
        }


    }
}