namespace UserService.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    public class User : IdentityUser
    {
        [Required]  // Ensures this field is not null in the database
        [StringLength(50)]  // Limits the length of First Name to 50 characters
        public string FirstName { get; set; }

        [Required]  // Ensures this field is not null in the database
        [StringLength(50)]  // Limits the length of Last Name to 50 characters
        public string LastName { get; set; }

        [StringLength(100)]  // Limits the length of Address to 100 characters
        public string? Address { get; set; }

        [StringLength(50)]  // Limits the length of City to 50 characters
        public string? City { get; set; }

        [StringLength(50)]  // Limits the length of State to 50 characters
        public string? State { get; set; }

        [StringLength(10)]  // Limits the length of ZipCode to 10 characters
        public string? ZipCode { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginDate { get; set; }

        public DateTime LastProfileUpdate { get; set; }
    }

}
