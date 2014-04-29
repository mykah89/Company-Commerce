using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class User
    {
        public User()
        {
            AssociatedAddresses = new List<UserAddressXref>();
            Orders = new List<Order>();
        }

        public Boolean AccountConfirmed { get; set; }

        [EmailAddress]
        public String EmailAddress { get; set; }

        public String PasswordVerificationToken { get; set; }

        public DateTime PasswordVerificationTokenExpiration { get; set; }

        public String PasswordHash { get; set; }

        public Int32 UserId { get; set; }

        [MinLength(6)]
        public String Username { get; set; }

        public virtual IList<UserAddressXref> AssociatedAddresses { get; set; }

        public virtual IList<Order> Orders { get; set; }
    }
}
