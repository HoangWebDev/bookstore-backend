using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Models
{
    public class BookModel
    {
        [Key]
        public int BookId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? BookName { get; set; }

        [MaxLength(100)]
        public string? Slug { get; set; }

        public string? Description { get; set; }

        [Required]       
        public long Price { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal DiscountPercent { get; set; } = 0; // Mặc định không giảm giá

        // Tính giá sau khi giảm, nhưng KHÔNG lưu vào database
        [NotMapped]
        public decimal DiscountedPrice => Price * (1 - DiscountPercent / 100);

        // Khóa ngoại liên kết đến tác giả
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        public virtual AuthorModel? Author { get; set; }

        // Khóa ngoại liên kết đến danh mục
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual CategoryModel? Category { get; set; }

        // Khóa ngoại liên kết đến nhà xuất bản
        [ForeignKey("Publisher")]
        public int PublisherId { get; set; }
        public virtual PublisherModel? Publisher { get; set; }

        // Ảnh bìa sách
        public string? Image { get; set; }

        // Ngày tạo sách trong hệ thống
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Thêm các trường mới
        public int PublishYear { get; set; }

        public int PageCount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Weight { get; set; } // Để lưu gram, nên dùng decimal thay vì int

        [MaxLength(50)]
        public string? Size { get; set; } // Kích thước sách (ví dụ: "13.5 x 20.5 cm")

        [MaxLength(50)]
        public string? CoverType { get; set; } // Loại bìa (ví dụ: "Bìa mềm")
    }
}
