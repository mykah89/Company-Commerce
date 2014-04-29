using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class Address
    {
        public Address()
        {
            AssociatedUsers = new List<UserAddressXref>();
        }

        public Int32 AddressId { get; set; }

        public String AddressLine1 { get; set; }

        public String AddressLine2 { get; set; }

        public String City { get; set; }

        public String PostalCode { get; set; }

        public String State { get; set; }

        public virtual IList<UserAddressXref> AssociatedUsers { get; set; }
    }
}
