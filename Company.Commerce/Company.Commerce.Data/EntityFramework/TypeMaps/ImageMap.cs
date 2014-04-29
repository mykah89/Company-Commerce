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
    public class ImageMap : EntityTypeConfiguration<Image>
    {
        public ImageMap()
        {
            Property(t => t.ImageId).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.ImagePath).IsRequired().HasMaxLength(300);
        }
    }
}
