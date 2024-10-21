using System.ComponentModel.DataAnnotations;

namespace UserService.DTOs
{
    public class UserDTO
    {
        [Required]  
        public string Id { get; set; }

        [Required]  
        [EmailAddress]  
        public string Email { get; set; }

        [Required]  
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required] 
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }

        [Required]  
        public bool IsActive { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public string Role { get; set; }
    }


}
