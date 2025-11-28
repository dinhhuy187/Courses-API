using courses_buynsell_api.DTOs.Chat;
using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using courses_buynsell_api.DTOs;
using System.Security.Claims;

namespace courses_buynsell_api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    private int GetUserId()
    {
        int id = HttpContext.Items["UserId"] as int? ?? -1;
        if (id == -1)
        {
            throw new UnauthorizedAccessException("Không xác định được người dùng hiện tại.");
        }
        return id;
    }

    /// <summary>
    /// Tạo hoặc lấy conversation giữa buyer và seller về một course
    /// </summary>
    [HttpPost("conversations")]
    public async Task<ActionResult<ConversationDto>> GetOrCreateConversation([FromBody] CreateConversationDto dto)
    {
        try
        {
            var userId = GetUserId();
            var conversation = await _chatService.GetOrCreateConversationAsync(userId, dto);
            return Ok(conversation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy tất cả conversations của user hiện tại (có phân trang)
    /// </summary>
    [HttpGet("conversations")]
    public async Task<ActionResult<PagedResult<ConversationDto>>> GetMyConversations(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetUserId();
            var conversations = await _chatService.GetUserConversationsAsync(userId, page, pageSize);
            return Ok(conversations);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    /// <summary>
    /// Lấy tất cả conversations về một course cụ thể (cho seller) - có phân trang
    /// </summary>
    [HttpGet("conversations/course/{courseId}")]
    public async Task<ActionResult<PagedResult<ConversationDto>>> GetCourseConversations(
        int courseId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var userId = GetUserId(); // sellerId
            var conversations = await _chatService.GetCourseConversationsAsync(userId, courseId, page, pageSize);
            return Ok(conversations);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    /// <summary>
    /// Lấy chi tiết một conversation
    /// </summary>
    [HttpGet("conversations/{conversationId}")]
    public async Task<ActionResult<ConversationDto>> GetConversationDetail(int conversationId)
    {
        try
        {
            var userId = GetUserId();
            var conversation = await _chatService.GetConversationDetailAsync(userId, conversationId);

            if (conversation == null)
                return NotFound(new { message = "Conversation not found or you don't have access" });

            return Ok(conversation);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Gửi message (REST API - fallback nếu SignalR không khả dụng)
    /// </summary>
    [HttpPost("messages")]
    public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageDto dto)
    {
        try
        {
            var userId = GetUserId();
            var message = await _chatService.SendMessageAsync(userId, dto);
            return Ok(message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy messages của một conversation (có phân trang)
    /// </summary>
    [HttpGet("conversations/{conversationId}/messages")]
    public async Task<ActionResult<List<MessageDto>>> GetMessages(
        int conversationId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var userId = GetUserId();
            var dto = new GetMessagesDto
            {
                ConversationId = conversationId,
                Page = page,
                PageSize = pageSize
            };

            var messages = await _chatService.GetConversationMessagesAsync(userId, dto);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Đánh dấu tất cả messages trong conversation đã đọc
    /// </summary>
    [HttpPut("conversations/{conversationId}/mark-read")]
    public async Task<ActionResult> MarkAsRead(int conversationId)
    {
        try
        {
            var userId = GetUserId();
            await _chatService.MarkMessagesAsReadAsync(userId, conversationId);
            return Ok(new { message = "Messages marked as read" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Lấy tổng số tin nhắn chưa đọc
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount()
    {
        try
        {
            var userId = GetUserId();
            var count = await _chatService.GetUnreadCountAsync(userId);
            return Ok(new { unreadCount = count });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Kiểm tra quyền truy cập vào conversation
    /// </summary>
    [HttpGet("conversations/{conversationId}/check-access")]
    public async Task<ActionResult<bool>> CheckAccess(int conversationId)
    {
        try
        {
            var userId = GetUserId();
            var hasAccess = await _chatService.HasAccessToConversationAsync(userId, conversationId);
            return Ok(new { hasAccess });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}