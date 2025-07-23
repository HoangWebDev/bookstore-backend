using System.ComponentModel.DataAnnotations;

namespace BookStore.Request
{
    public class RegisterRequest
    {
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string FullName { get; set; } = string.Empty;
    }
}
