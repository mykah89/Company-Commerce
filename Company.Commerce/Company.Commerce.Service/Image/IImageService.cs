using Company.Commerce.Entity.Models;
using Company.Commerce.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public interface IImageService
    {
        ServiceOperationResult<Image> Create(Image image);
    }
}
