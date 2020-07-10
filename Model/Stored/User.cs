using System;

namespace PaymentSystem.Model.Stored
{
    public class User
    {
        public Guid Id { get; set; }
        public String Login { get;set; }
        public String HashedPassword { get; set; }
    }
}