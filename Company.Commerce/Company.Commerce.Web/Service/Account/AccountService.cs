using Company.Commerce.Entity.Models;
using Company.Commerce.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.Service.Account
{
    public class AccountService
    {
        private readonly IUserService _userService;

        public AccountService(IUserService userService)
        {
            _userService = userService;
        }
    }
}