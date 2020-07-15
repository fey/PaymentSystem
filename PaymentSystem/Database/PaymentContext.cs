using Microsoft.EntityFrameworkCore;
using PaymentSystem.Model.Stored;

namespace PaymentSystem.Database
{
    public class PaymentContext: DbContext
    {
        public PaymentContext(DbContextOptions<PaymentContext> options): base(options) =>
            Database.EnsureCreated();

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}