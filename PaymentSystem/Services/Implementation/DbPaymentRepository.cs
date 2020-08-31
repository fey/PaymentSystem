using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PaymentSystem.Database;
using PaymentSystem.Model.Dto.Payments;
using PaymentSystem.Model.Stored;
using PaymentSystem.Services.Exceptions;
using PaymentSystem.Services.Interfaces;

namespace PaymentSystem.Services.Implementations
{
    public class DbPaymentRepository : IPaymentRepository
    {
        private readonly PaymentContext _context;
        public TimeSpan SessionExpirationTimeSpan { get; set; }

        public DbPaymentRepository(PaymentContext context)
        {
            _context = context;
            SessionExpirationTimeSpan = TimeSpan.FromMinutes(10);
        }

        public IAsyncEnumerable<PaymentRecord> GetPaymentHistoryAsync(
            DateTime start, DateTime end
        ) =>
            _context.Payments
                .AsNoTracking()
                .Include(payment => payment.AssociatedSession)
                .Where(payment =>
                    payment.AssociatedSession.CreationDateTime.Date >= start.Date &&
                    payment.AssociatedSession.CreationDateTime.Date <= end.Date
                ).Select(payment => new PaymentRecord(){
                    PaymentSum = payment.Sum,
                    Purpose = payment.Purpose,
                    CardNumber = payment.CardNumber,
                    PaymentRequestDateTime = payment.AssociatedSession.CreationDateTime
                }).AsAsyncEnumerable();

        public async Task MakePaymentAsync(Guid sessionId, Card paymentCard)
        {
            Payment payment = await _context.Payments
                .Include(payment => payment.AssociatedSession)
                .FirstOrDefaultAsync(payment => payment.SessionId.Equals(sessionId) && 
                    payment.CardNumber == null &&
                    payment.AssociatedSession.ExpirationDateTime > DateTime.Now);
            if (payment == null)
                throw new PaymentException();
            
            payment.CardNumber = paymentCard.Number;
            _context.Payments.Update(payment);
            if (await _context.SaveChangesAsync() == 0)
                throw new PaymentException();
        }

        public async Task<Guid> RecordPaymentAsync(PaymentRequest payment)
        {
            DateTime timeMark = DateTime.Now;
            Guid newSessionId = (await _context.Sessions.AddAsync(new Session(){
                CreationDateTime = timeMark,
                ExpirationDateTime = timeMark.Add(SessionExpirationTimeSpan)
            })).Entity.Id;
            await _context.Payments.AddAsync(new Payment()
            {
                SessionId = newSessionId,
                Sum = payment.Sum,
                Purpose = payment.Purpose
            });
            _context.SaveChanges();
            return newSessionId;
        }

        public async Task SessionIsActiveAsync(Guid sessionId) {
            var sessionIsActive = await _context.Payments.Include(payment => payment.AssociatedSession)
                .AnyAsync(payment => 
                    payment.SessionId.Equals(sessionId) &&
                    payment.CardNumber == null &&
                    DateTime.Now < payment.AssociatedSession.ExpirationDateTime
                );
            if (!sessionIsActive)
                throw new SessionException();
        }
    }
}