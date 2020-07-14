using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentSystem.Model.Stored
{
    public class Payment
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid SessionId { get; set; }
        [ForeignKey("SessionId")]
        public virtual Session AssociatedSession { get; set; }
        public Decimal Sum { get; set; }
        public String Purpose { get; set; }
        public String CardNumber { get; set; }
    }
}