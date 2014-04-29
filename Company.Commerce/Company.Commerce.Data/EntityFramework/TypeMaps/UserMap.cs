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
    internal class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            HasKey(t => t.UserId)
                .Property(t => t.UserId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.EmailAddress).IsRequired().HasMaxLength(200);

            Property(t => t.PasswordHash).IsRequired().HasMaxLength(400);

            Property(t => t.Username).IsRequired().HasMaxLength(60);

            HasMany(t => t.Orders).WithRequired(t => t.User).HasForeignKey(t => t.UserId);
        }
    }
}
