using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class ContactModel
    {
        [Key]
        public int ContactId { get; set; }

        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
