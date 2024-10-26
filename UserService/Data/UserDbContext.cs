using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedModels.Models;

namespace UserService.Data
{
    //UserDbContext inherit from IdentityDbContext<User>, get predefined tables for AspNetUsers, AspNetRoles, AspNetUserRoles
    // Use User model to get custom properties, FirstName, LastName....
    public class UserDbContext : IdentityDbContext<User>
    {
        //Constructor - Accepts DbContextOptions to configure DbContext settings (database provider SQL Server)
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        //method to configure how entities should be mapped to the database using ModelBuilder
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //calls base method from IdentityDbContext to configure ASP.NET Identity's built-in tables
            base.OnModelCreating(builder);

            // Seed roles for Dispatcher, Driver, Customer... NormalizedName - case-insensitive, uppercase version of the role name, ASP.NET uses for faster lookups.
            // These will populate in the AspNetRoles table
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "Dispatcher", NormalizedName = "DISPATCHER" },
                new IdentityRole { Name = "Driver", NormalizedName = "DRIVER" },
                new IdentityRole { Name = "Customer", NormalizedName = "CUSTOMER" }
            );
        }

    }
}
