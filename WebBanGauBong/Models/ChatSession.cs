namespace WebBanGauBong.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Model đại diện cho một phiên chat giữa khách hàng và chatbot.
    // Mỗi phiên có một SessionToken duy nhất (GUID) để theo dõi cuộc hội thoại.

    [Table("ChatSession")]
    public class ChatSession
    {
        public ChatSession()
        {
            ChatMessages = new HashSet<ChatMessage>();
        }

        [Key]
        public int ChatSessionId { get; set; }

        // Token duy nhất để nhận diện phiên chat (GUID).
        // Lưu ở cookie/localStorage phía client để duy trì hội thoại.
        [Required]
        [StringLength(100)]
        public string SessionToken { get; set; }

        // ID người dùng (NULL nếu khách chưa đăng nhập)
        public int? UserID { get; set; }

        public DateTime CreatedAt { get; set; }

        // Thời điểm tin nhắn cuối - dùng để kiểm tra session hết hạn
        public DateTime LastActivityAt { get; set; }

        // Trạng thái: true = đang hoạt động, false = đã kết thúc
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual Users User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
    }
}
