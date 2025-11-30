using courses_buynsell_api.Data;
using courses_buynsell_api.DTOs.Chat;
using courses_buynsell_api.DTOs;
using courses_buynsell_api.Entities;
using courses_buynsell_api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace courses_buynsell_api.Services;

public class ChatService : IChatService
{
    private readonly AppDbContext _context;

    public ChatService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ConversationDto> GetOrCreateConversationAsync(int buyerId, CreateConversationDto dto)
    {
        // Kiểm tra course tồn tại
        var course = await _context.Courses
            .Include(c => c.Seller)
            .FirstOrDefaultAsync(c => c.Id == dto.CourseId);

        if (course == null)
            throw new Exception("Course not found");

        // Kiểm tra seller có đúng là owner của course không
        if (course.SellerId != dto.SellerId)
            throw new Exception("Invalid seller for this course");

        // Tìm conversation đã tồn tại
        var existingConversation = await _context.Conversations
            .Include(c => c.Course)
            .Include(c => c.Buyer)
            .Include(c => c.Seller)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .FirstOrDefaultAsync(c =>
                c.CourseId == dto.CourseId &&
                c.BuyerId == buyerId &&
                c.SellerId == dto.SellerId);

        if (existingConversation != null)
        {
            return MapToConversationDto(existingConversation, buyerId);
        }

        // Tạo conversation mới
        var conversation = new Conversation
        {
            CourseId = dto.CourseId,
            BuyerId = buyerId,
            SellerId = dto.SellerId,
            CreatedAt = DateTime.UtcNow,
            LastMessageAt = DateTime.UtcNow
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        // Load lại với includes
        conversation = await _context.Conversations
            .Include(c => c.Course)
            .Include(c => c.Buyer)
            .Include(c => c.Seller)
            .FirstAsync(c => c.Id == conversation.Id);

        return MapToConversationDto(conversation, buyerId);
    }

    public async Task<PagedResult<ConversationDto>> GetUserConversationsAsync(int userId, int page, int pageSize)
    {
        var query = _context.Conversations
            .Include(c => c.Course)
            .Include(c => c.Buyer)
            .Include(c => c.Seller)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .Where(c => c.BuyerId == userId || c.SellerId == userId);

        var total = await query.CountAsync();

        var conversations = await query
            .OrderByDescending(c => c.LastMessageAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<ConversationDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = conversations.Select(c => MapToConversationDto(c, userId)).ToList()
        };
    }

    public async Task<PagedResult<ConversationDto>> GetCourseConversationsAsync(
    int sellerId, int courseId, int page, int pageSize)
    {
        // Kiểm tra quyền
        var courseExists = await _context.Courses
            .AnyAsync(c => c.Id == courseId && c.SellerId == sellerId);

        if (!courseExists)
            throw new Exception("Course not found or you don't have permission");

        var query = _context.Conversations
            .Include(c => c.Course)
            .Include(c => c.Buyer)
            .Include(c => c.Seller)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .Where(c => c.CourseId == courseId && c.SellerId == sellerId);

        var total = await query.CountAsync();

        var conversations = await query
            .OrderByDescending(c => c.LastMessageAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<ConversationDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = total,
            Items = conversations.Select(c => MapToConversationDto(c, sellerId)).ToList()
        };
    }


    public async Task<MessageDto> SendMessageAsync(int senderId, SendMessageDto dto)
    {
        // Kiểm tra quyền truy cập conversation
        if (!await HasAccessToConversationAsync(senderId, dto.ConversationId))
            throw new Exception("You don't have access to this conversation");

        var message = new Message
        {
            ConversationId = dto.ConversationId,
            SenderId = senderId,
            Content = dto.Content.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsRead = false
        };

        _context.Messages.Add(message);

        // Cập nhật LastMessageAt của conversation
        var conversation = await _context.Conversations
            .FirstAsync(c => c.Id == dto.ConversationId);

        conversation.LastMessageAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Load lại message với sender info
        message = await _context.Messages
            .Include(m => m.Sender)
            .FirstAsync(m => m.Id == message.Id);

        return MapToMessageDto(message, senderId);
    }

    public async Task<List<MessageDto>> GetConversationMessagesAsync(int userId, GetMessagesDto dto)
    {
        // Kiểm tra quyền truy cập
        if (!await HasAccessToConversationAsync(userId, dto.ConversationId))
            throw new Exception("You don't have access to this conversation");

        var messages = await _context.Messages
            .Include(m => m.Sender)
            .Where(m => m.ConversationId == dto.ConversationId)
            .OrderByDescending(m => m.CreatedAt)
            .Skip((dto.Page - 1) * dto.PageSize)
            .Take(dto.PageSize)
            .ToListAsync();

        // Reverse để hiển thị tin nhắn cũ nhất trước
        messages.Reverse();

        return messages.Select(m => MapToMessageDto(m, userId)).ToList();
    }

    public async Task MarkMessagesAsReadAsync(int userId, int conversationId)
    {
        // Kiểm tra quyền truy cập
        if (!await HasAccessToConversationAsync(userId, conversationId))
            throw new Exception("You don't have access to this conversation");

        // Đánh dấu tất cả tin nhắn chưa đọc mà user không phải là người gửi
        var unreadMessages = await _context.Messages
            .Where(m => m.ConversationId == conversationId &&
                       m.SenderId != userId &&
                       !m.IsRead)
            .ToListAsync();

        foreach (var message in unreadMessages)
        {
            message.IsRead = true;
        }

        if (unreadMessages.Any())
        {
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> HasAccessToConversationAsync(int userId, int conversationId)
    {
        return await _context.Conversations
            .AnyAsync(c => c.Id == conversationId &&
                          (c.BuyerId == userId || c.SellerId == userId));
    }

    public async Task<ConversationDto?> GetConversationDetailAsync(int userId, int conversationId)
    {
        var conversation = await _context.Conversations
            .Include(c => c.Course)
            .Include(c => c.Buyer)
            .Include(c => c.Seller)
            .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
            .FirstOrDefaultAsync(c => c.Id == conversationId &&
                                     (c.BuyerId == userId || c.SellerId == userId));

        return conversation != null ? MapToConversationDto(conversation, userId) : null;
    }

    public async Task<int> GetUnreadCountAsync(int userId)
    {
        return await _context.Messages
            .Where(m => m.Conversation.BuyerId == userId || m.Conversation.SellerId == userId)
            .Where(m => m.SenderId != userId && !m.IsRead)
            .CountAsync();
    }

    // Helper methods
    private ConversationDto MapToConversationDto(Conversation conversation, int currentUserId)
    {
        var lastMessage = conversation.Messages?.FirstOrDefault();
        var unreadCount = conversation.Messages?
            .Count(m => m.SenderId != currentUserId && !m.IsRead) ?? 0;

        return new ConversationDto
        {
            Id = conversation.Id,
            CourseId = conversation.CourseId,
            CourseTitle = conversation.Course?.Title ?? "",
            CourseImageUrl = conversation.Course?.ImageUrl ?? "",
            BuyerId = conversation.BuyerId,
            BuyerName = conversation.Buyer?.FullName ?? "",
            BuyerAvatar = conversation.Buyer?.AvatarUrl ?? "",
            SellerId = conversation.SellerId,
            SellerName = conversation.Seller?.FullName ?? "",
            SellerAvatar = conversation.Seller?.AvatarUrl ?? "",
            CreatedAt = conversation.CreatedAt,
            LastMessageAt = conversation.LastMessageAt,
            LastMessage = lastMessage != null ? MapToMessageDto(lastMessage, currentUserId) : null,
            UnreadCount = unreadCount
        };
    }

    private MessageDto MapToMessageDto(Message message, int currentUserId)
    {
        return new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            SenderName = message.Sender?.FullName ?? "",
            SenderAvatar = message.Sender?.AvatarUrl ?? "",
            Content = message.Content,
            CreatedAt = message.CreatedAt,
            IsRead = message.IsRead,
            IsSentByCurrentUser = message.SenderId == currentUserId
        };
    }

    public async Task<int> CountUnreadConversationsAsync(int userId)
    {
        return await _context.Messages
            .Where(m =>
                m.SenderId != userId &&   // người gửi là đối phương
                !m.IsRead                 // chưa đọc
            )
            .Select(m => m.ConversationId)
            .Distinct()
            .CountAsync();
    }

    public async Task<List<UnreadConversationCountDto>> GetUnreadConversationsByCourseAsync(int sellerId)
    {
        var result = await _context.Conversations
            .Where(c => c.SellerId == sellerId)
            .Select(c => new
            {
                c.CourseId,
                Unread = c.Messages!
                    .Any(m => !m.IsRead && m.SenderId != sellerId)
            })
            .Where(x => x.Unread)
            .GroupBy(x => x.CourseId)
            .Select(g => new UnreadConversationCountDto
            {
                CourseId = g.Key,
                UnreadCount = g.Count()
            })
            .ToListAsync();

        return result;
    }
}