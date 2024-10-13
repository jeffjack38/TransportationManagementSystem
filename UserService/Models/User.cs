namespace UserService.Models
{
    using Microsoft.AspNetCore.Identity;

    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginDate { get; set; }
        public DateTime LastProfileUpdate { get; set; }
    }

}
