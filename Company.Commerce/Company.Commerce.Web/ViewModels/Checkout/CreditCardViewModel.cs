using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.ViewModels.Checkout
{
    public enum Month
    {
        January = 1,
        February,
        March,
        April,
        May,
        June,
        July,
        August,
        September,
        October,
        November,
        December
    }

    public class CreditCardViewModel
    {
        public CreditCardViewModel()
        {
            ExpirationYears = new List<DateTime>();

            for (int i = 0; i < 10; i++)
            {
                ExpirationYears.Add(DateTime.Now.AddYears(i));
            }

            ExpirationMonths = Enum.GetValues(typeof(Month)).Cast<Month>();
        }

        [Display(Name = "Card #")]
        public String CardNumber { get; set; }

        [Display(Name = "Month")]
        public Month? ExpirationMonth { get; set; }

        [Display(Name = "Year")]
        [DataType(DataType.Date)]
        public DateTime? ExpirationYear { get; set; }

        public IList<DateTime> ExpirationYears { get; set; }

        public IEnumerable<Month> ExpirationMonths { get; set; }

        [Display(Name = "CVV")]
        public String VerificationNumber { get; set; }
    }
}