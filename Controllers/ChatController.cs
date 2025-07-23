using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly DataContext _context;

    public ChatController(DataContext context)
    {
        _context = context;
    }

    // Lấy danh sách Customers đã gửi tin nhắn đến Admin
    [Authorize(Roles = "Admin")]
    [HttpGet("customers")]
    public async Task<IActionResult> GetChatCustomers()
    {
        var customers = await _context.Chats
            .Where(c => c.Sender != null && c.Sender.Role != "Admin") // Lọc bỏ Admin (Admin có ID = 0)
            .GroupBy(c => c.SenderId)
            .Select(g => new
            {
                User = _context.Users.Where(u => u.UserId == g.Key).Select(u => new
                {
                    u.UserId,
                    u.FullName
                }).FirstOrDefault(),
                LastMessage = g.OrderByDescending(c => c.Timestamp)
                               .Select(c => new { c.Message, c.Timestamp })
                               .FirstOrDefault()
            })
            .ToListAsync();

        return Ok(customers);
    }   
}
