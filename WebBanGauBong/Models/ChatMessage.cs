namespace WebBanGauBong.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Model đại diện cho một tin nhắn trong phiên chat.
    // Mỗi tin nhắn thuộc về một ChatSession và có SenderType cho biết
    // tin nhắn từ khách hàng ('Customer') hay từ bot ('Bot').
    [Table("ChatMessage")]
    public class ChatMessage
    {
        [Key]
        public int ChatMessageId { get; set; }

        [Required]
        public int ChatSessionId { get; set; }

        [Required]
        public string MessageContent { get; set; }

        [Required]
        [StringLength(20)]
        public string SenderType { get; set; }

        public DateTime Timestamp { get; set; }

        [ForeignKey("ChatSessionId")]
        public virtual ChatSession ChatSession { get; set; }
    }
}
