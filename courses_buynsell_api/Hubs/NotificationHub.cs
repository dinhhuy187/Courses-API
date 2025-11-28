using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace courses_buynsell_api.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    // ✅ Helper method để lấy User ID một cách an toàn
    private int GetUserIdFromClaims()
    {
        // Thử lấy từ các claim types khác nhau
        var idClaim = Context.User?.FindFirst("id")?.Value;
        var nameidClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var subClaim = Context.User?.FindFirst("sub")?.Value;

        _logger.LogInformation(
            "📋 Claims check - id: {Id}, nameid: {Nameid}, sub: {Sub}",
            idClaim ?? "null", nameidClaim ?? "null", subClaim ?? "null");

        // Thử parse từng claim theo thứ tự ưu tiên
        if (!string.IsNullOrEmpty(idClaim) && int.TryParse(idClaim, out int userId))
        {
            _logger.LogInformation("✅ Using 'id' claim: {UserId}", userId);
            return userId;
        }

        if (!string.IsNullOrEmpty(nameidClaim) && int.TryParse(nameidClaim, out userId))
        {
            _logger.LogInformation("✅ Using 'NameIdentifier' claim: {UserId}", userId);
            return userId;
        }

        if (!string.IsNullOrEmpty(subClaim) && int.TryParse(subClaim, out userId))
        {
            _logger.LogInformation("✅ Using 'sub' claim: {UserId}", userId);
            return userId;
        }

        // Nếu không tìm thấy, log tất cả claims để debug
        var allClaims = Context.User?.Claims.Select(c => $"{c.Type}={c.Value}") ?? Array.Empty<string>();
        _logger.LogError("❌ Cannot find valid integer user ID. Available claims: {Claims}",
            string.Join(", ", allClaims));

        throw new HubException($"Cannot find valid integer user ID in token. Please ensure your JWT contains a numeric 'id' or 'nameid' claim.");
    }

    public async Task JoinSellerGroup(int sellerId)
    {
        try
        {
            // ✅ Lấy User ID từ claims
            int userId = GetUserIdFromClaims();

            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value
                        ?? Context.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value
                        ?? Context.User?.FindFirst("role")?.Value;

            _logger.LogInformation(
                "🔐 Authorization check - UserId: {UserId}, SellerId: {SellerId}, Role: {Role}",
                userId, sellerId, userRole ?? "None");

            // Kiểm tra quyền
            if (userId != sellerId && userRole != "Admin")
            {
                _logger.LogWarning(
                    "⚠️ Unauthorized: User {UserId} (Role: {Role}) tried to join group of Seller {SellerId}",
                    userId, userRole ?? "None", sellerId);
                throw new HubException("Unauthorized: You can only join your own notification group");
            }

            var groupName = $"seller_{sellerId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation(
                "✅ User {UserId} (ConnectionId: {ConnectionId}) joined group {GroupName}",
                userId, Context.ConnectionId, groupName);

            await Clients.Caller.SendAsync("JoinedGroup", new
            {
                sellerId = sellerId,
                groupName = groupName,
                userId = userId,
                message = "Successfully joined notification group"
            });
        }
        catch (HubException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error while joining seller group {SellerId}", sellerId);
            throw new HubException("An error occurred while joining the group");
        }
    }

    public async Task LeaveSellerGroup(int sellerId)
    {
        try
        {
            int userId = GetUserIdFromClaims();

            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value
                        ?? Context.User?.FindFirst("role")?.Value;

            if (userId != sellerId && userRole != "Admin")
            {
                _logger.LogWarning(
                    "⚠️ Unauthorized leave attempt: User {UserId} tried to leave group of Seller {SellerId}",
                    userId, sellerId);
                throw new HubException("Unauthorized");
            }

            var groupName = $"seller_{sellerId}";
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            _logger.LogInformation(
                "✅ User {UserId} left group {GroupName}",
                userId, groupName);

            await Clients.Caller.SendAsync("LeftGroup", new
            {
                sellerId = sellerId,
                groupName = groupName,
                message = "Successfully left notification group"
            });
        }
        catch (HubException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error while leaving seller group {SellerId}", sellerId);
            throw new HubException("An error occurred while leaving the group");
        }
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            int userId = GetUserIdFromClaims();
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value
                        ?? Context.User?.FindFirst("unique_name")?.Value
                        ?? Context.User?.FindFirst("name")?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value
                        ?? Context.User?.FindFirst("role")?.Value;

            _logger.LogInformation(
                "✅ User connected - Username: {Username}, ID: {UserId}, Role: {Role}, ConnectionId: {ConnectionId}",
                username ?? "Unknown", userId, userRole ?? "None", Context.ConnectionId);

            // Tự động join vào group của chính user
            var groupName = $"seller_{userId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation("✅ Auto-joined user to their own group: {GroupName}", groupName);

            await base.OnConnectedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in OnConnectedAsync");
            throw;
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try
        {
            // Cố gắng lấy userId, nhưng không throw nếu thất bại (connection đang đóng)
            try
            {
                int userId = GetUserIdFromClaims();
                var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value
                            ?? Context.User?.FindFirst("unique_name")?.Value;

                if (exception != null)
                {
                    _logger.LogWarning(
                        "⚠️ User {Username} (ID: {UserId}) disconnected with error: {Error}",
                        username ?? "Unknown", userId, exception.Message);
                }
                else
                {
                    _logger.LogInformation(
                        "✅ User {Username} (ID: {UserId}) disconnected normally",
                        username ?? "Unknown", userId);
                }
            }
            catch
            {
                _logger.LogInformation("✅ User disconnected (unable to retrieve user info)");
            }

            await base.OnDisconnectedAsync(exception);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error in OnDisconnectedAsync");
        }
    }

    public async Task GetConnectionInfo()
    {
        try
        {
            int userId = GetUserIdFromClaims();
            var username = Context.User?.FindFirst(ClaimTypes.Name)?.Value
                        ?? Context.User?.FindFirst("unique_name")?.Value;
            var userRole = Context.User?.FindFirst(ClaimTypes.Role)?.Value
                        ?? Context.User?.FindFirst("role")?.Value;

            var info = new
            {
                connectionId = Context.ConnectionId,
                userId = userId,
                username = username,
                role = userRole,
                connectedAt = DateTime.UtcNow
            };

            await Clients.Caller.SendAsync("ConnectionInfo", info);
            _logger.LogInformation("📊 Connection info requested by user {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error getting connection info");
            throw new HubException("Could not retrieve connection info");
        }
    }
}