using Company.Commerce.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Data.EntityFramework.TypeMaps
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            HasKey(t => t.AddressId);

            Property(t => t.AddressId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.AddressLine1).IsRequired();

            Property(t => t.City).IsRequired();

            Property(t => t.PostalCode).IsRequired();

            Property(t => t.State).IsRequired();
        }
    }
}
