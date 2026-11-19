
using Microsoft.EntityFrameworkCore;
using User.Data.Entity;

namespace User.Data.Database
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }
        public DbSet<User.Data.Entity.User> Users { get; set; }
    }
}
