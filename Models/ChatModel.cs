using BookStore.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class ChatModel
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("Sender")]
    public int SenderId { get; set; } // Người gửi tin nhắn
    public virtual UserModel? Sender { get; set; }

    [Required]
    [ForeignKey("Receiver")]
    public int ReceiverId { get; set; } // Người nhận tin nhắn
    public virtual UserModel? Receiver { get; set; }

    [Required]
    public string Message { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
