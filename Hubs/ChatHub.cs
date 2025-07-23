using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using BookStore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using BookStore.Utils;
using Microsoft.Extensions.Caching.Memory; // Import để sử dụng StringValues

public class ChatHub : Hub
{
    private readonly DataContext _dataContext;
    private static HashSet<string> AdminConnections = new HashSet<string>();
    private static Dictionary<int, string> UserConnections = new Dictionary<int, string>(); // Lưu connectionId của từng user
    private static IMemoryCache? _cache; // Dùng MemoryCache thay vì Dictionary
    private string GetChatCacheKey(int user1, int user2)
    {
        return $"{Math.Min(user1, user2)}_{Math.Max(user1, user2)}"; // Luôn giữ ID nhỏ trước
    }

    public ChatHub(DataContext dataContext, IMemoryCache cache)
    {
        _dataContext = dataContext;
        _cache = cache;
    }

    //Kết nối
    public override async Task OnConnectedAsync()
    {
        var userId = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        var userType = Context.GetHttpContext()?.Request.Query["userType"].ToString(); // Ép kiểu về string

        Console.WriteLine($"🔗 Kết nối mới: userId={userId}, userType={userType}, connectionId={Context.ConnectionId}");

        if (userType == "Admin")
        {
            AdminConnections.Add(Context.ConnectionId);
        }
        else if (userType == "Customer" && int.TryParse(userId, out int parsedUserId))
        {
            // Lưu connection của user để có thể gửi tin nhắn trực tiếp
            UserConnections[parsedUserId] = Context.ConnectionId;
        }

        await base.OnConnectedAsync();
    }

    //Ngắt kết nối
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        AdminConnections.Remove(Context.ConnectionId);
        // Xóa user connection khi disconnect
        var userToRemove = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId);
        if (userToRemove.Key != 0)
        {
            UserConnections.Remove(userToRemove.Key);
        }
        await base.OnDisconnectedAsync(exception);
    }

    //Send messages
    public async Task SendMessage(int senderId, string senderType, int receiverId, string message)
    {
        try
        {
            Console.WriteLine($"📩 SendMessage: senderId={senderId}, senderType={senderType}, receiverId={receiverId}, message={message}");
            var chatMessage = new ChatModel
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Message = message,
                Timestamp = DateTime.Now
            };

            _dataContext.Chats.Add(chatMessage);
            await _dataContext.SaveChangesAsync();

            //Cập nhât cache key
            var cacheKey = GetChatCacheKey(senderId, receiverId);
            // Nếu cache đã tồn tại thì thêm tin nhắn mới vào
            if (_cache!.TryGetValue(cacheKey, out List<ChatModel>? cachedMessages))
            {
                cachedMessages?.Add(chatMessage);
            }
            else
            {
                cachedMessages = new List<ChatModel> { chatMessage };
            }

            // Lưu lại cache
            _cache!.Set(cacheKey, cachedMessages, TimeSpan.FromHours(1));

            if (senderType == "Customer")
            {
                // Gửi tin nhắn từ Customer đến tất cả Admins
                foreach (var adminConn in AdminConnections)
                {
                    await Clients.Client(adminConn).SendAsync("ReceiveMessage", senderId, receiverId, message, chatMessage.Timestamp);
                }
            }
            else if (senderType == "Admin")
            {
                // Gửi tin nhắn từ Admin đến đúng Customer
                if (UserConnections.TryGetValue(receiverId, out string? userConnectionId))
                {
                    await Clients.Client(userConnectionId).SendAsync("ReceiveMessage", senderId, receiverId, message, chatMessage.Timestamp);
                }
                // Gửi tin nhắn từ Admin đến đúng Customer
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveMessage", senderId, receiverId, message, chatMessage.Timestamp);
            }
            Console.WriteLine("✅ Tin nhắn đã gửi thành công!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Lỗi trong SendMessage: {ex.Message}");
            throw;
        }
    }

    //Load messages
    public async Task LoadMessages(int senderId, int receiverId)
    {
        var cacheKey = GetChatCacheKey(senderId, receiverId);

        //Kiểm tra xem đã có cache chưa
        if (_cache!.TryGetValue(cacheKey, out List<ChatModel>? cachedMessages))
        {
            Console.WriteLine($"📂 Lấy tin nhắn từ cache: {cacheKey}");
            await Clients.Caller.SendAsync("LoadChatHistory", cachedMessages);
            return;
        }

        //Nếu chưa có thì truy vấn
        Console.WriteLine($"🔍 Truy vấn DB cho: {cacheKey}");
        var messages = await _dataContext.Chats
            .Where(c => (c.SenderId == senderId && c.ReceiverId == receiverId) ||
                        (c.SenderId == receiverId && c.ReceiverId == senderId))
            .OrderBy(c => c.Timestamp)
            .ToListAsync();

        // Lưu vào cache để lần sau không phải truy vấn lại
        _cache!.Set(cacheKey, messages, TimeSpan.FromHours(1));

        await Clients.Caller.SendAsync("LoadChatHistory", messages);
    }

    //Send messages to admin
    public async Task SendMessageToAdmin(int senderId, string message)
    {
        await SendMessage(senderId, "Customer", 1, message);
    }
}
