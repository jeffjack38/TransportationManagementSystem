namespace SharedModels.Models
{
    using Microsoft.AspNetCore.Identity;
    using System.ComponentModel.DataAnnotations;

    public class User : IdentityUser
    {
        [Required] 
        [StringLength(50)]  
        public string FirstName { get; set; }

        [Required]  
        [StringLength(50)] 
        public string LastName { get; set; }

        [StringLength(100)]  
        public string? Address { get; set; }

        [StringLength(50)]  
        public string? City { get; set; }

        [StringLength(50)]  
        public string? State { get; set; }

        [StringLength(10)] 
        public string? ZipCode { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginDate { get; set; }

        public DateTime LastProfileUpdate { get; set; }

    }

}
