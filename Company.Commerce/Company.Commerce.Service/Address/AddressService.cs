using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using Company.Commerce.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public class AddressService
    {
        private readonly IUnitOfWork _uow;

        public AddressService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ServiceOperationResult<Address>> CreateAsync(Address address)
        {
            if (address == null)
                throw new ArgumentNullException("address");


            ServiceOperationResult<Address> result = new ServiceOperationResult<Address>();

            //FluentAddressValidator

            throw new NotImplementedException();
        }
    }
}
