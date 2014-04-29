using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Company.Commerce.Web.Helpers
{
    internal static class EmailHelpers
    {
        internal static class UserEmails
        {
            internal static MailMessage AccountPropertyChanged(String accountProperty)
            {
                if (String.IsNullOrWhiteSpace(accountProperty))
                    throw new ArgumentNullException("accountProperty");

                MailMessage message = new MailMessage();

                message.Subject = "Account " + accountProperty + " Changed";

                message.Body = "This email is to inform you that the " + accountProperty + " associated with your account has been changed.";

                return message;
            }

            internal static MailMessage ForgottenPasswordRequest(String token, String changePasswordLink)
            {
                if (String.IsNullOrWhiteSpace(token))
                    throw new ArgumentNullException("token");

                if (String.IsNullOrWhiteSpace(changePasswordLink))
                    throw new ArgumentNullException("changePasswordLink");

                MailMessage message = new MailMessage();

                message.Subject = "Forgotten Password Request";

                message.Body = @"A forgotten password request has been made, please enter this link in your browser's address bar
                    to recover your account. " + changePasswordLink;

                return message;
            }

            internal static MailMessage OrderReceived(Int32 orderId)
            {
                MailMessage message = new MailMessage();

                message.Subject = "Order Received";

                message.Body = @"Your order has been received and will begin being processed. Your order number is " + orderId +
                    " please reference it in any communication you may require with us.";

                return message;
            }
        }
    }
}