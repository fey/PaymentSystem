using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentSystem.Model.Stored
{
    public class Session
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime ExpirationDateTime { get; set; }
    }
}