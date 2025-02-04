using BOMLink.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BOMLink.Data {
    public class BOMLinkContext : DbContext {
        public BOMLinkContext(DbContextOptions<BOMLinkContext> options) : base(options) { }

        public DbSet<BOM> BOMs { get; set; }
        public DbSet<BOMItem> BOMItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<PO> POs { get; set; }
        public DbSet<POItem> POItems { get; set; }
        public DbSet<RFQ> RFQs { get; set; }
        public DbSet<RFQItem> RFQItems { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierManufacturer> SupplierManufacturer { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            //// ✅ Explicitly define primary key for IdentityUserRole<string>
            //modelBuilder.Entity<IdentityUserRole<string>>()
            //    .HasKey(ur => new { ur.UserId, ur.RoleId });

            //modelBuilder.Entity<IdentityUserClaim<string>>()
            //    .HasKey(uc => uc.Id);

            //modelBuilder.Entity<IdentityUserLogin<string>>()
            //    .HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

            //modelBuilder.Entity<IdentityRoleClaim<string>>()
            //    .HasKey(rc => rc.Id);

            //modelBuilder.Entity<IdentityUserToken<string>>()
            //    .HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

            //// Seed Roles First
            //var adminRole = new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" };
            //var userRole = new IdentityRole { Id = "2", Name = "User", NormalizedName = "USER" };

            //modelBuilder.Entity<IdentityRole>().HasData(adminRole, userRole);

            //var adminUser = new ApplicationUser {
            //    Id = "1001", // IdentityUser uses string ID
            //    UserName = "admin",
            //    NormalizedUserName = "ADMIN",
            //    Email = "admin@bomlink.com",
            //    NormalizedEmail = "ADMIN@BOMLINK.COM",
            //    FirstName = "Admin",
            //    LastName = "User",
            //    PasswordHash = "AQAAAAEAACcQAAAAEHnE9fMb0Et5ngudI8wg/Y9VWfcGiDT/COpFo6rNX7HGAgP3cWN5AZrw4F++0UrcDw=="
            //};

            //var normalUser = new ApplicationUser {
            //    Id = "1002",
            //    UserName = "JDS",
            //    NormalizedUserName = "JDS",
            //    Email = "user@bomlink.com",
            //    NormalizedEmail = "USER@BOMLINK.COM",
            //    FirstName = "JDS",
            //    LastName = "User",
            //    PasswordHash = "AQAAAAEAACcQAAAAEAgMAY1MlmEX4XTuJIo8de6PXc8A607hv3SPk4IVBzD6VsEX1y0MadveFphNH7FUYg=="
            //};

            //var adminUser = new ApplicationUser {
            //    Id = "1001",
            //    UserName = "admin",
            //    NormalizedUserName = "ADMIN",
            //    Email = "admin@bomlink.com",
            //    NormalizedEmail = "ADMIN@BOMLINK.COM",
            //    FirstName = "Admin",
            //    LastName = "User"
            //    // Remove PasswordHash from HasData(), set it manually at runtime
            //};

            //var normalUser = new ApplicationUser {
            //    Id = "1002",
            //    UserName = "JDS",
            //    NormalizedUserName = "JDS",
            //    Email = "user@bomlink.com",
            //    NormalizedEmail = "USER@BOMLINK.COM",
            //    FirstName = "JDS",
            //    LastName = "User"
            //    // Remove PasswordHash from HasData(), set it manually at runtime
            //};

            //modelBuilder.Entity<ApplicationUser>().HasData(adminUser, normalUser);

            //// Assign Roles
            //modelBuilder.Entity<IdentityUserRole<string>>().HasData(
            //    new IdentityUserRole<string> { UserId = "1001", RoleId = "1" }, // Admin Role
            //    new IdentityUserRole<string> { UserId = "1002", RoleId = "2" }  // User Role
            //);

            modelBuilder.Entity<User>().HasData(
                new User {
                    UserId = 1,
                    Username = "admin",
                    HashedPassword = "AQAAAAEAACcQAAAAEK9vBdtmDOq5FQfTfIHMxK835sGFRz/FevGOC092eFhYuHK0Q9BrEG8/HpLlb7dVow==",
                    FirstName = "Admin",
                    LastName = "Admin",
                    RoleId = 1
                },
                new User {
                    UserId = 2,
                    Username = "JDS",
                    HashedPassword = "AQAAAAEAACcQAAAAECUKpOK7uSJAXy6UL1uAxk4kRNFkBnw1JCdknbTQ8Gp9hhE4/1oZ/9FXemSviL6SuQ==",
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

            // Part data
            modelBuilder.Entity<Part>()
                .HasIndex(m => m.PartNumber)
                .IsUnique();  // Ensure unique part numbers
            modelBuilder.Entity<Part>()
                .Property(p => p.Labour)
                .HasPrecision(18, 2); // Set precision for Labour column    
            modelBuilder.Entity<Part>()
                .Property(p => p.Unit)
                .HasConversion<string>(); // Store Enum as String
            modelBuilder.Entity<Part>()
                .HasOne(p => p.Manufacturer)
                .WithMany()
                .HasForeignKey(p => p.ManufacturerId)
                .OnDelete(DeleteBehavior.Restrict); // Define foreign key relationship with Manufacturer
            modelBuilder.Entity<Part>().HasData(
                new Part { Id = 1, PartNumber = "P1001", Description = "Circuit Breaker", Labour = 2.5m, Unit = UnitType.each, ManufacturerId = 1 },
                new Part { Id = 2, PartNumber = "P1002", Description = "Relay", Labour = 1.0m, Unit = UnitType.each, ManufacturerId = 2 },
                new Part { Id = 3, PartNumber = "P1003", Description = "Switch", Labour = 0.5m, Unit = UnitType.each, ManufacturerId = 3 }
            );

            // Job data
            modelBuilder.Entity<Job>()
                .HasIndex(m => m.Number)
                .IsUnique();  // Ensure unique job numbers
            modelBuilder.Entity<Job>()
                .Property(j => j.StartDate)
                .HasDefaultValueSql("GETUTCDATE()"); // Uses the database default UTC date
            modelBuilder.Entity<Job>().HasData(
                new Job { Id = 1, Number = "J0001", Description = "Job 1", CustomerId = 1, ContactName = "John Doe", Status = JobStatus.Pending, UserId = 2, StartDate = new DateTime(2021, 1, 1) },
                new Job { Id = 2, Number = "J0002", Description = "Job 2", CustomerId = 2, ContactName = "Jane Doe", Status = JobStatus.Completed, UserId = 2, StartDate = new DateTime(2021, 1, 1) },
                new Job { Id = 3, Number = "J0003", Description = "Job 3", CustomerId = 3, ContactName = "Jack Doe", Status = JobStatus.Canceled, UserId = 2 , StartDate = new DateTime(2021, 1, 1) }
            );
            modelBuilder.Entity<Job>()
                .HasOne(j => j.User)
                .WithMany(u => u.Jobs)
                .HasForeignKey(j => j.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // Manufacturer data
            modelBuilder.Entity<Manufacturer>()
                .HasIndex(m => m.Name)
                .IsUnique();  // Ensure unique manufacturer names
            modelBuilder.Entity<Manufacturer>().HasData(
                new Manufacturer { ManufacturerId = 1, Name = "Schneider" },
                new Manufacturer { ManufacturerId = 2, Name = "Phoenix Contact" },
                new Manufacturer { ManufacturerId = 3, Name = "Siemens" }
            );

            // Supplier data
            modelBuilder.Entity<Supplier>()
                .HasIndex(m => m.Name)
                .IsUnique();  // Ensure unique supplier names
            modelBuilder.Entity<Supplier>()
                .HasIndex(m => m.SupplierCode)
                .IsUnique();  // Ensure unique supplier code
            modelBuilder.Entity<Supplier>().HasData(
                new Supplier { Id = 1, Name = "Graybar", ContactEmail = "test@gmail.com", SupplierCode = "GRAELE" },
                new Supplier { Id = 2, Name = "House of Electric", ContactEmail = "test@gmail.com", SupplierCode = "HOUELE" },
                new Supplier { Id = 3, Name = "Hammond", ContactEmail = "test@gmail.com", SupplierCode = "HAMMND" }
            );

            // Customer data
            modelBuilder.Entity<Customer>()
                .HasIndex(m => m.Name)
                .IsUnique(); // Ensure unique customer names
            modelBuilder.Entity<Customer>()
                .HasIndex(m => m.CustomerCode)
                .IsUnique(); // Ensure unique customer codes
            modelBuilder.Entity<Customer>().HasData(
                new Customer { Id = 1, Name = "ABC Company", CustomerCode = "ABCCO" },
                new Customer { Id = 2, Name = "XYZ Company", CustomerCode = "XYZCO" },
                new Customer { Id = 3, Name = "123 Company", CustomerCode = "123CO" }
            );

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