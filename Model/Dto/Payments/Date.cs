using System;

namespace PaymentSystem.Model.Dto.Payments
{
    public class Date
    {
        short Day { get; set; }
        int Year { get; set; }

        public DateTime ToDateTime() =>
            new DateTime(DateTime.Today.Year / 1000 * 1000 + Year, Day, 1);
    }
}