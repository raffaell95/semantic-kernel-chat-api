using Microsoft.EntityFrameworkCore;
using ApiAi.Models;

namespace ApiAi.Data;

public class ChatDbContext : DbContext
{
    public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options) { }

    public DbSet<Chat> Chats {get; set; }
    public DbSet<ChatMessage> Messages { get; set; }
    
}
