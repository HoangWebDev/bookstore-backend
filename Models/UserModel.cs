using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class UserModel
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? UserName { get; set; }

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")]
        public string? PasswordHash { get; set; }

        [Required]
        [MaxLength(50)]
        public string? FullName { get; set; }
                
        [RegularExpression(@"^((\+84|0)(2\d{9}|3\d{8}|5\d{8}|7\d{8}|8\d{8}|9\d{8}))$")]
        public int? Phone { get; set; }
        
        [MaxLength(50)]
        [RegularExpression(@"^[\w\.\-]+@([\w\-]+\.)+[a-zA-Z]{2,7}$")]
        public string? Email { get; set; }
        
        [MaxLength(100)]
        public string? Address { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Role { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
