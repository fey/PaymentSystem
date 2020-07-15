using Microsoft.EntityFrameworkCore;
using PaymentSystem.Model.Stored;

namespace PaymentSystem.Database
{
    public class UserContext: DbContext
    {
        public UserContext(DbContextOptions<UserContext> options): base(options) =>
            Database.EnsureCreated();

        public DbSet<User> Users { get; set; }
    }
}