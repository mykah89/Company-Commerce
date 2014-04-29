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
    internal class StateMap : EntityTypeConfiguration<State>
    {
        public StateMap()
        {
            Property(t => t.StateId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Name).IsRequired().HasMaxLength(20);

            Property(t => t.TaxRate).IsRequired().HasPrecision(4, 2);
        }
    }
}
