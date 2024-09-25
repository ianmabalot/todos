using Microsoft.EntityFrameworkCore;
using todos.api.Models;

namespace todos.api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<Todo> Todos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Directly hard-code the connection string here
                optionsBuilder.UseSqlServer("Server=tcp:todosequel.database.windows.net,1433;Initial Catalog=todosdb;Persist Security Info=False;User ID=ianmabalot;Password=YourPassword;");
            }
        }
    }
}
