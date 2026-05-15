using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using WebBanGauBong.Models;

namespace WebBanGauBong.Services
{
    public class ChatbotService
    {
        private static readonly string GEMINI_API_KEY = ConfigurationManager.AppSettings["GeminiApiKey"];

        private static readonly string GEMINI_API_URL =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key=" + GEMINI_API_KEY;

        private static readonly int SESSION_TIMEOUT_MINUTES = 30;

        // HttpClient dùng chung (tránh tạo mới liên tục gây Socket Exhaustion)
        private static readonly HttpClient _httpClient = new HttpClient();
        // QUẢN LÝ SESSION
        public ChatSession GetOrCreateSession(string sessionToken, int? userId)
        {
            using (var db = new QL_THU_BONG())
            {
                ChatSession session = null;

                if (!string.IsNullOrEmpty(sessionToken))
                {
                    session = db.ChatSession
                        .FirstOrDefault(s => s.SessionToken == sessionToken && s.IsActive);

                    if (session != null)
                    {
                        var timeSinceLastActivity = DateTime.Now - session.LastActivityAt;
                        if (timeSinceLastActivity.TotalMinutes > SESSION_TIMEOUT_MINUTES)
                        {
                            session.IsActive = false;
                            db.SaveChanges();
                            session = null;
                        }
                    }
                }

                if (session == null)
                {
                    session = new ChatSession
                    {
                        SessionToken = Guid.NewGuid().ToString("N"),
                        UserID = userId,
                        CreatedAt = DateTime.Now,
                        LastActivityAt = DateTime.Now,
                        IsActive = true
                    };
                    db.ChatSession.Add(session);
                    db.SaveChanges();
                }

                return session;
            }
        }
        // LƯU TIN NHẮN VÀO DATABASE

        public ChatMessage SaveMessage(int chatSessionId, string content, string senderType)
        {
            using (var db = new QL_THU_BONG())
            {
                var message = new ChatMessage
                {
                    ChatSessionId = chatSessionId,
                    MessageContent = content,
                    SenderType = senderType,
                    Timestamp = DateTime.Now
                };

                db.ChatMessage.Add(message);

                var session = db.ChatSession.Find(chatSessionId);
                if (session != null)
                {
                    session.LastActivityAt = DateTime.Now;
                }

                db.SaveChanges();
                return message;
            }
        }

        // LẤY LỊCH SỬ CHAT

        public List<ChatMessage> GetChatHistory(int chatSessionId, int maxMessages = 10)
        {
            using (var db = new QL_THU_BONG())
            {
                return db.ChatMessage
                    .Where(m => m.ChatSessionId == chatSessionId)
                    .OrderByDescending(m => m.Timestamp)
                    .Take(maxMessages)
                    .OrderBy(m => m.Timestamp)
                    .ToList();
            }
        }

        // TRA CỨU SẢN PHẨM

        public string QueryProductByName(string productName)
        {
            using (var db = new QL_THU_BONG())
            {
                var products = db.Product
                    .Include(p => p.ProductSize)
                    .Include(p => p.Discount)
                    .Where(p => p.ProductName.Contains(productName) && p.Isenabled == 1)
                    .Take(5)
                    .ToList();

                if (!products.Any())
                {
                    return $"Không tìm thấy sản phẩm nào có tên \"{productName}\".";
                }

                var sb = new StringBuilder();
                sb.AppendLine($"Tìm thấy {products.Count} sản phẩm:");

                foreach (var product in products)
                {
                    sb.AppendLine($"\n- {product.ProductName}");

                    var sizes = product.ProductSize.OrderBy(s => s.SizeName).ToList();
                    if (sizes.Any())
                    {
                        foreach (var size in sizes)
                        {
                            string stockStatus = (size.StockQuantity > 0) ? "Còn hàng" : "Hết hàng";
                            sb.AppendLine($"   Size {size.SizeName}cm: {size.Price:N0}đ ({stockStatus})");
                        }
                    }

                    var activeDiscount = product.Discount
                        .FirstOrDefault(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now);
                    if (activeDiscount != null)
                    {
                        sb.AppendLine($"   Đang giảm giá {activeDiscount.DiscountRate}% ({activeDiscount.DiscountName})");
                    }
                }

                return sb.ToString();
            }
        }

        // TRÍCH XUẤT TỪ KHÓA SẢN PHẨM

        private string ExtractProductKeyword(string message)
        {
            string[] knownProducts = { "teddy", "stitch", "lotso", "capybara",
                                       "melody", "kuromi", "kitty", "shin",
                                       "labubu", "loopy", "lena", "doraemon",
                                       "panda", "baby three", "cinnamoroll",
                                       "hươu cao cổ", "hải cẩu", "rái cá",
                                       "dưa hấu", "sầu riêng", "husky" };

            string lowerMsg = message.ToLower();
            foreach (var product in knownProducts)
            {
                if (lowerMsg.Contains(product))
                    return product;
            }
            return message;
        }

        // XÂY DỰNG PROMPT CHO GEMINI 
        private object BuildGeminiContents(int chatSessionId, string currentMessage)
        {
            var contents = new List<object>();

            // --- System Instruction (nằm trong tin nhắn user đầu tiên) ---
            string systemPrompt = BuildSystemPrompt(chatSessionId, currentMessage);

            // --- Lịch sử chat cũ → chuyển thành multi-turn format ---
            var history = GetChatHistory(chatSessionId, 8);
            if (history.Any())
            {
                // Tin nhắn đầu tiên của user = system prompt + tin nhắn đầu tiên trong lịch sử
                bool firstUserSent = false;

                foreach (var msg in history)
                {
                    string role = msg.SenderType == "Customer" ? "user" : "model";

                    if (role == "user" && !firstUserSent)
                    {
                        // Gắn system prompt vào tin nhắn user đầu tiên
                        contents.Add(new
                        {
                            role = "user",
                            parts = new[] { new { text = systemPrompt + "\n\nKhách hàng: " + msg.MessageContent } }
                        });
                        firstUserSent = true;
                    }
                    else
                    {
                        contents.Add(new
                        {
                            role = role,
                            parts = new[] { new { text = msg.MessageContent } }
                        });
                    }
                }

                // Tin nhắn hiện tại của khách
                contents.Add(new
                {
                    role = "user",
                    parts = new[] { new { text = currentMessage } }
                });
            }
            else
            {
                // Chưa có lịch sử → gửi system prompt + tin nhắn đầu tiên
                contents.Add(new
                {
                    role = "user",
                    parts = new[] { new { text = systemPrompt + "\n\nKhách hàng: " + currentMessage } }
                });
            }

            return contents;
        }

        private string BuildSystemPrompt(int chatSessionId, string currentMessage)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Bạn là trợ lý AI của shop \"Gấu Bông Cao Cấp\".");
            sb.AppendLine("Thông tin shop:");
            sb.AppendLine("- Địa chỉ: 486 Lê Văn Sỹ, P.14, Quận 3, TP.HCM");
            sb.AppendLine("- Hotline/Zalo: 0967110738 (9:00 - 21:30)");
            sb.AppendLine("- Bảo hành đường chỉ trọn đời");
            sb.AppendLine("- Đơn >300k giảm 30k ship, gói quà & hút chân không miễn phí");
            sb.AppendLine("- Tích điểm 3% giá trị đơn hàng");
            sb.AppendLine();
            sb.AppendLine("Quy tắc trả lời:");
            sb.AppendLine("- Trả lời bằng tiếng Việt, thân thiện, ngắn gọn (tối đa 3-4 câu)");
            sb.AppendLine("- Dùng emoji phù hợp");
            sb.AppendLine("- Nếu khách hỏi sản phẩm, dùng thông tin bên dưới để trả lời chính xác");
            sb.AppendLine("- Nếu không biết, hướng dẫn khách liên hệ Hotline");

            // Tra cứu sản phẩm nếu khách hỏi
            string[] productKeywords = { "giá", "bao nhiêu", "còn hàng", "size", "mua",
                                          "gấu", "teddy", "stitch", "lotso", "capybara",
                                          "melody", "kuromi", "kitty", "shin", "labubu",
                                          "loopy", "lena", "doraemon", "panda", "heo",
                                          "mèo", "chó", "vịt", "thỏ", "voi" };

            string lowerMessage = currentMessage.ToLower();
            bool isProductQuery = productKeywords.Any(kw => lowerMessage.Contains(kw));

            if (isProductQuery)
            {
                string productInfo = QueryProductByName(ExtractProductKeyword(currentMessage));
                sb.AppendLine();
                sb.AppendLine("=== DỮ LIỆU SẢN PHẨM TỪ DATABASE ===");
                sb.AppendLine(productInfo);
            }

            return sb.ToString();
        }
        // GỌI API
        public async Task<string> CallGeminiAPI(string userMessage, string context, int chatSessionId)
        {
            int maxRetries = 2;

            for (int attempt = 0; attempt <= maxRetries; attempt++)
            {
                try
                {
                    var contents = BuildGeminiContents(chatSessionId, userMessage);

                    var requestBody = new
                    {
                        contents = contents,
                        generationConfig = new
                        {
                            temperature = 0.7,
                            maxOutputTokens = 300,
                            topP = 0.9
                        }
                    };

                    string jsonBody = JsonConvert.SerializeObject(requestBody);

                    var httpRequest = new HttpRequestMessage(HttpMethod.Post, GEMINI_API_URL);
                    httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    System.Diagnostics.Debug.WriteLine($"[ChatbotService] Calling Gemini API (attempt {attempt + 1})...");

                    var response = await _httpClient.SendAsync(httpRequest);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    System.Diagnostics.Debug.WriteLine($"[ChatbotService] Status: {response.StatusCode}");

                    if (response.IsSuccessStatusCode)
                    {
                        var json = JObject.Parse(responseBody);
                        string botReply = json["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                        if (string.IsNullOrEmpty(botReply))
                        {
                            botReply = "Xin lỗi, mình chưa hiểu ý bạn. Bạn có thể hỏi lại được không? 😊";
                        }

                        return botReply.Trim();
                    }

                    // Nếu bị rate limit 
                    if ((int)response.StatusCode == 429 && attempt < maxRetries)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"[ChatbotService] Rate limited (429). Đợi 5 giây rồi retry...");
                        await Task.Delay(5000);
                        continue;
                    }

                    // Lỗi khác hoặc đã hết retry
                    System.Diagnostics.Debug.WriteLine($"[ChatbotService] Gemini Error: {responseBody}");

                    if ((int)response.StatusCode == 429)
                    {
                        return "Hệ thống đang bận do quá nhiều yêu cầu. " +
                               "Bạn vui lòng đợi 1 phút rồi thử lại nhé! ⏳";
                    }

                    return "Xin lỗi, hệ thống đang bận. Bạn vui lòng thử lại sau hoặc " +
                           "liên hệ Hotline 0967110738 để được hỗ trợ nhé! 🙏";
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[ChatbotService] Exception: {ex.Message}");

                    if (attempt < maxRetries)
                    {
                        await Task.Delay(3000);
                        continue;
                    }

                    return "Xin lỗi, đã có lỗi xảy ra. Bạn vui lòng thử lại sau nhé! 🙏";
                }
            }

            return "Xin lỗi, hệ thống đang bận. Vui lòng thử lại sau! 🙏";
        }

        //XỬ LÝ TIN NHẮN 
        public async Task<ChatResponse> ProcessMessage(string sessionToken, string userMessage, int? userId)
        {
            var session = GetOrCreateSession(sessionToken, userId);
            SaveMessage(session.ChatSessionId, userMessage, "Customer");
            string context = BuildSystemPrompt(session.ChatSessionId, userMessage);
            string botReply = await CallGeminiAPI(userMessage, context, session.ChatSessionId);
            SaveMessage(session.ChatSessionId, botReply, "Bot");
            return new ChatResponse
            {
                SessionToken = session.SessionToken,
                BotMessage = botReply,
                Timestamp = DateTime.Now
            };
        }
    }

    public class ChatResponse
    {
        public string SessionToken { get; set; }
        public string BotMessage { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ChatRequest
    {
        public string SessionToken { get; set; }
        public string Message { get; set; }
    }
}
