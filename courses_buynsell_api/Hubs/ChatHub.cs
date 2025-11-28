using courses_buynsell_api.DTOs.Chat;
using courses_buynsell_api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace courses_buynsell_api.Hubs;

[Authorize]
public class ChatHub : Hub
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    // SỬA HÀM NÀY - Lấy userId từ JWT Claims
    private int GetUserId()
    {
        try
        {
            // CÁCH 1: Lấy từ JWT Claims (chuẩn)
            var userIdClaim = Context.User?.FindFirst("id")?.Value
                ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? Context.User?.FindFirst("sub")?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                _logger.LogInformation($"Got userId from JWT Claims: {userId}");
                return userId;
            }

            _logger.LogError("Cannot determine userId - Claims: {Claims}",
                string.Join(", ", Context.User?.Claims.Select(c => $"{c.Type}={c.Value}") ?? Array.Empty<string>()));

            throw new HubException("Không xác định được người dùng hiện tại.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting userId");
            throw new HubException($"Error getting userId: {ex.Message}");
        }
    }

    // Join vào một conversation room
    public async Task JoinConversation(int conversationId)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation($"User {userId} joining conversation {conversationId}");

            // Kiểm tra quyền truy cập
            var hasAccess = await _chatService.HasAccessToConversationAsync(userId, conversationId);
            if (!hasAccess)
            {
                throw new HubException("You don't have access to this conversation");
            }

            // Join room
            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");

            // Đánh dấu tin nhắn đã đọc
            await _chatService.MarkMessagesAsReadAsync(userId, conversationId);

            // Thông báo cho người khác trong room
            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("UserJoined", userId, Context.ConnectionId);

            _logger.LogInformation($"User {userId} successfully joined conversation {conversationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error joining conversation {conversationId}");
            throw new HubException($"Error joining conversation: {ex.Message}");
        }
    }

    // Leave khỏi conversation room
    public async Task LeaveConversation(int conversationId)
    {
        try
        {
            var userId = GetUserId();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");

            await Clients.Group($"conversation_{conversationId}")
                .SendAsync("UserLeft", userId, Context.ConnectionId);

            _logger.LogInformation($"User {userId} left conversation {conversationId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error leaving conversation {conversationId}");
        }
    }

    // Gửi tin nhắn
    public async Task SendMessage(SendMessageDto dto)
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation($"User {userId} sending message to conversation {dto.ConversationId}");

            // Gửi tin nhắn qua service
            var message = await _chatService.SendMessageAsync(userId, dto);

            // Broadcast tin nhắn tới tất cả members trong conversation room
            await Clients.Group($"conversation_{dto.ConversationId}")
                .SendAsync("ReceiveMessage", message);

            // Gửi notification tới người nhận (nếu họ không ở trong room)
            await NotifyNewMessage(dto.ConversationId, userId, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            throw new HubException($"Error sending message: {ex.Message}");
        }
    }

    // Typing indicator
    public async Task UserTyping(int conversationId, bool isTyping)
    {
        try
        {
            var userId = GetUserId();

            // Kiểm tra quyền truy cập
            var hasAccess = await _chatService.HasAccessToConversationAsync(userId, conversationId);
            if (!hasAccess)
            {
                throw new HubException("You don't have access to this conversation");
            }

            // Gửi typing status tới người khác trong room (trừ người gửi)
            await Clients.OthersInGroup($"conversation_{conversationId}")
                .SendAsync("UserTypingStatus", userId, isTyping);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending typing status");
            throw new HubException($"Error sending typing status: {ex.Message}");
        }
    }

    // Đánh dấu đã đọc
    public async Task MarkAsRead(int conversationId)
    {
        try
        {
            var userId = GetUserId();
            await _chatService.MarkMessagesAsReadAsync(userId, conversationId);

            // Thông báo cho người khác biết tin nhắn đã được đọc
            await Clients.OthersInGroup($"conversation_{conversationId}")
                .SendAsync("MessagesMarkedAsRead", userId, conversationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking messages as read");
            throw new HubException($"Error marking messages as read: {ex.Message}");
        }
    }

    // Override OnConnectedAsync
    public override async Task OnConnectedAsync()
    {
        try
        {
            var userId = GetUserId();
            _logger.LogInformation($"User {userId} connected to ChatHub, ConnectionId: {Context.ConnectionId}");

            // Join vào personal room để nhận notifications
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in OnConnectedAsync");
            throw new HubException($"Error connecting: {ex.Message}");
        }
    }

    // Override OnDisconnectedAsync
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            var userId = GetUserId();
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            _logger.LogInformation($"User {userId} disconnected from ChatHub");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error in OnDisconnectedAsync (ignored)");
        }

        await base.OnDisconnectedAsync(exception);
    }

    // Helper method để gửi notification
    private async Task NotifyNewMessage(int conversationId, int senderId, MessageDto message)
    {
        try
        {
            var conversation = await _chatService.GetConversationDetailAsync(senderId, conversationId);
            if (conversation == null) return;

            // Xác định người nhận (người không phải là sender)
            var receiverId = conversation.BuyerId == senderId
                ? conversation.SellerId
                : conversation.BuyerId;

            // Gửi notification tới personal room của người nhận
            await Clients.Group($"user_{receiverId}")
                .SendAsync("NewMessageNotification", new
                {
                    Message = message,
                    Conversation = conversation
                });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error sending notification (ignored)");
        }
    }
}