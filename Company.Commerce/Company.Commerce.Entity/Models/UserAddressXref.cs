using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class UserAddressXref
    {
        public Int32 AddressId { get; set; }

        public Boolean IsDefault { get; set; }

        public Int32 UserId { get; set; }

        public virtual Address Address { get; set; }

        public virtual User User { get; set; }
    }
}
