using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Company.Commerce.Web.Settings
{
    public static class AccountSettings
    {
        public static Boolean IsRegistrationEnabled { get; set; }

        public static Boolean RequireEmailConfirmationForRegistration { get; set; }

        static AccountSettings()
        {
            IsRegistrationEnabled = true;
            RequireEmailConfirmationForRegistration = false;
        }
    }
}