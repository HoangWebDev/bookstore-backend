using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class PublisherModel
    {
        [Key]
        public int PublisherId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? PublisherName { get; set; }

        [MaxLength(100)]
        public string? Slug { get; set; }

        public string? Address { get; set; }        
    }
}
