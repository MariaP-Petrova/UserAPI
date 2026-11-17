using Microsoft.EntityFrameworkCore;

namespace User.API.Database;

    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }
        public DbSet<Entity.User> Users { get; set; }
    }

