$(document).ready(function () {

    // BIẾN TOÀN CỤC
    // Lấy Session Token từ localStorage (nếu đã chat trước đó)
    let chatSessionToken = localStorage.getItem('chatSessionToken') || null;

    // Trạng thái: true khi đang chờ bot trả lời (chống spam click)
    let isWaitingResponse = false;

    // Tham chiếu đến các phần tử DOM
    const $widget = $('#chatbot-widget');
    const $toggleBtn = $('#chatbot-toggle-btn');
    const $closeBtn = $('#chatbot-close-btn');
    const $messagesArea = $('#chatbot-messages');
    const $input = $('#chatbot-input');
    const $sendBtn = $('#chatbot-send-btn');

    //  TOGGLE MỞ/ĐÓNG KHUNG CHAT

    // Click nút tròn ở góc phải → mở/đóng chat widget
    $toggleBtn.on('click', function () {
        $widget.toggleClass('active');

        if ($widget.hasClass('active')) {
            // Khi mở chat → focus vào ô input, tải lịch sử
            $input.focus();
            loadChatHistory();
            // Ẩn animation pulse khi đã mở
            $toggleBtn.css('animation', 'none');
            $toggleBtn.find('i').removeClass('fa-comments').addClass('fa-times');
        } else {
            $toggleBtn.find('i').removeClass('fa-times').addClass('fa-comments');
        }
    });

    // Nút X đóng chat
    $closeBtn.on('click', function () {
        $widget.removeClass('active');
        $toggleBtn.find('i').removeClass('fa-times').addClass('fa-comments');
    });

    // GỬI TIN NHẮN
    // Click nút Gửi
    $sendBtn.on('click', function () {
        sendMessage();
    });

    // Nhấn Enter trong ô input
    $input.on('keypress', function (e) {
        if (e.which === 13) { // Enter key
            e.preventDefault();
            sendMessage();
        }
    });

    function sendMessage() {
        var message = $input.val().trim();

        // Validate: không gửi tin rỗng, không gửi khi đang chờ
        if (!message || isWaitingResponse) return;

        // Xóa nội dung input và ẩn gợi ý nhanh
        $input.val('');
        $('.quick-replies').fadeOut(200);

        // Hiển thị tin nhắn của khách hàng lên khung chat
        appendMessage(message, 'customer');

        // Hiển thị hiệu ứng "Bot đang gõ..."
        showTypingIndicator();

        // Khóa gửi tin (chống spam)
        isWaitingResponse = true;
        $sendBtn.prop('disabled', true);

        // Gọi API Backend - KHÔNG gọi thẳng sang Dialogflow
        $.ajax({
            url: '/Chat/SendMessage',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                SessionToken: chatSessionToken,
                Message: message
            }),
            success: function (response) {
                // Xóa hiệu ứng "đang gõ"
                hideTypingIndicator();

                if (response.Success) {
                    // Lưu session token vào localStorage để duy trì hội thoại
                    chatSessionToken = response.Data.SessionToken;
                    localStorage.setItem('chatSessionToken', chatSessionToken);

                    // Hiển thị câu trả lời của bot
                    appendMessage(response.Data.BotMessage, 'bot');
                } else {
                    // Hiển thị lỗi từ server
                    appendMessage(
                        response.Error || 'Xin lỗi, đã có lỗi xảy ra. Vui lòng thử lại! 🙏',
                        'bot'
                    );
                }
            },
            error: function () {
                hideTypingIndicator();
                appendMessage(
                    'Không thể kết nối đến server. Vui lòng kiểm tra mạng và thử lại! 🔌',
                    'bot'
                );
            },
            complete: function () {
                // Mở khóa gửi tin
                isWaitingResponse = false;
                $sendBtn.prop('disabled', false);
                $input.focus();
            }
        });
    }
    // HIỂN THỊ TIN NHẮN LÊN MÀN HÌNH
    function appendMessage(text, sender, time) {
        var now = time || new Date().toLocaleTimeString('vi-VN', {
            hour: '2-digit',
            minute: '2-digit'
        });

        var msgHtml =
            '<div class="chat-msg ' + sender + '">' +
                '<span>' + escapeHtml(text) + '</span>' +
                '<span class="msg-time">' + now + '</span>' +
            '</div>';

        $messagesArea.append(msgHtml);

        // Tự động cuộn xuống tin nhắn mới nhất
        scrollToBottom();
    }

    // Hiển thị 3 chấm nhảy 
    function showTypingIndicator() {
        var typingHtml =
            '<div class="typing-indicator" id="typing-dots">' +
                '<div class="dot"></div>' +
                '<div class="dot"></div>' +
                '<div class="dot"></div>' +
            '</div>';

        $messagesArea.append(typingHtml);
        scrollToBottom();
    }
     // Ẩn hiệu ứng "đang gõ" khi bot đã trả lời

    function hideTypingIndicator() {
        $('#typing-dots').remove();
    }
    // TẢI LỊCH SỬ CHAT (khi mở lại widget)
    function loadChatHistory() {
        if (!chatSessionToken) return;

        $.ajax({
            url: '/Chat/GetHistory',
            type: 'GET',
            data: { sessionToken: chatSessionToken },
            success: function (response) {
                if (response.Success && response.Data && response.Data.length > 0) {
                    // Xóa nội dung welcome mặc định
                    $messagesArea.find('.chat-welcome').remove();

                    // Hiển thị lại từng tin nhắn
                    response.Data.forEach(function (msg) {
                        var senderClass = msg.SenderType === 'Customer' ? 'customer' : 'bot';
                        appendMessage(msg.MessageContent, senderClass, msg.Timestamp);
                    });
                }
            }
        });
    }

    // GỢI Ý CÂU HỎI NHANH (Quick Replies)

    // Khi click vào nút gợi ý → tự động gửi câu hỏi đó
    $(document).on('click', '.quick-reply-btn', function () {
        var quickMessage = $(this).text();
        $input.val(quickMessage);
        sendMessage();
    });

    //HÀM TIỆN ÍCH
    function scrollToBottom() {
        var messagesDiv = $messagesArea[0];
        setTimeout(function () {
            messagesDiv.scrollTop = messagesDiv.scrollHeight;
        }, 100);
    }

    // Người dùng không thể chèn mã HTML/JS độc hại qua tin nhắn
    function escapeHtml(text) {
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(text));
        return div.innerHTML;
    }

});
