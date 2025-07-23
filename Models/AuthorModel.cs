using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class AuthorModel
    {
        [Key]
        public int AuthorId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? AuthorName { get; set; }

        [MaxLength(100)]
        public string? Slug { get; set; }

        public string? Biography { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }  // Ngày sinh

        [DataType(DataType.Date)]
        public DateTime? DateOfDeath { get; set; }  // Ngày mất (nếu có)
    }
}
