using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.Email
{
    public interface IEmailService
    {
        void SendMessage(MailMessage message);
    }
}
