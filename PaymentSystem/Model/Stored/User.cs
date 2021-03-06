using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentSystem.Model.Stored
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public String Login { get;set; }
        public String HashedPassword { get; set; }
    }
}