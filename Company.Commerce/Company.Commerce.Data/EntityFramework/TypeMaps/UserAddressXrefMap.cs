using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Data.EntityFramework.TypeMaps
{
    public class UserAddressXrefMap : EntityTypeConfiguration<UserAddressXref>
    {
        public UserAddressXrefMap()
        {
            HasKey(t => new { t.UserId, t.AddressId });

            HasRequired(t => t.User).WithMany(t => t.AssociatedAddresses)
                .HasForeignKey(t => t.AddressId);

            HasRequired(t => t.Address).WithMany(t => t.AssociatedUsers)
                .HasForeignKey(t => t.UserId);

            ToTable("UserAddressXref");
        }
    }
}
