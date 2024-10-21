using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SharedModels.Models;
namespace UserService.Data
{
    public class UserDbContext : IdentityDbContext<User>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed roles 
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Name = "Dispatcher", NormalizedName = "DISPATCHER" },
                new IdentityRole { Name = "Driver", NormalizedName = "DRIVER" },
                new IdentityRole { Name = "Customer", NormalizedName = "CUSTOMER" }
            );
        }

    }
}
