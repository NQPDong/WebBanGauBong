using System;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebBanGauBong.Models;
using WebBanGauBong.Services;

namespace WebBanGauBong.Controllers
{
    public class ChatController : Controller
    {
        private readonly ChatbotService _chatbotService;

        public ChatController()
        {
            _chatbotService = new ChatbotService();
        }

        [HttpPost]
        public async Task<ActionResult> SendMessage()
        {
            try
            {
                Request.InputStream.Position = 0;
                string requestBody;
                using (var reader = new StreamReader(Request.InputStream))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                // Log request body để debug
                System.Diagnostics.Debug.WriteLine($"[ChatController] Request Body: {requestBody}");

                var request = JsonConvert.DeserializeObject<ChatRequest>(requestBody);

                // Validate input - kiểm tra request không null và tin nhắn không rỗng
                if (request == null || string.IsNullOrWhiteSpace(request.Message))
                {
                    return Json(new
                    {
                        Success = false,
                        Error = "Vui lòng nhập tin nhắn."
                    });
                }

                // Giới hạn độ dài tin nhắn chống spam
                if (request.Message.Length > 500)
                {
                    return Json(new
                    {
                        Success = false,
                        Error = "Tin nhắn quá dài. Vui lòng nhập dưới 500 ký tự."
                    });
                }

                // Lấy UserID từ Session (nếu đã đăng nhập)
                int? userId = null;
                if (Session["User"] != null)
                {
                    var user = Session["User"] as Users;
                    userId = user?.UserID;
                }

                // Gọi ChatbotService xử lý toàn bộ flow
                var response = await _chatbotService.ProcessMessage(
                    request.SessionToken,
                    request.Message.Trim(),
                    userId
                );

                // Trả JSON response về Frontend
                return Json(new
                {
                    Success = true,
                    Data = new
                    {
                        response.SessionToken,
                        response.BotMessage,
                        response.Timestamp
                    }
                });
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết
                System.Diagnostics.Debug.WriteLine($"[ChatController] Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[ChatController] StackTrace: {ex.StackTrace}");

                return Json(new
                {
                    Success = false,
                    Error = "Đã có lỗi xảy ra. Vui lòng thử lại sau."
                });
            }
        }

        [HttpGet]
        public ActionResult GetHistory(string sessionToken)
        {
            try
            {
                if (string.IsNullOrEmpty(sessionToken))
                {
                    return Json(new { Success = true, Data = new object[] { } },
                                JsonRequestBehavior.AllowGet);
                }

                var session = _chatbotService.GetOrCreateSession(sessionToken, null);
                var history = _chatbotService.GetChatHistory(session.ChatSessionId, 50);

                var messages = history.ConvertAll(m => new
                {
                    m.MessageContent,
                    m.SenderType,
                    Timestamp = m.Timestamp.ToString("HH:mm")
                });

                return Json(new { Success = true, Data = messages },
                            JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ChatController] GetHistory Error: {ex.Message}");
                return Json(new { Success = false, Error = "Không thể tải lịch sử chat." },
                            JsonRequestBehavior.AllowGet);
            }
        }
    }
}
