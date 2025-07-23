using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class CategoryModel
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? CategoryName { get; set; }

        [MaxLength(100)]
        public string? Slug { get; set; }

        public string? Description { get; set; }
    }
}
