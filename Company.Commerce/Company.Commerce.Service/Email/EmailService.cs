using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.Email
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;

        public EmailService()
        {
            _smtpClient = new SmtpClient();
        }

        public void SendMessage(MailMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            _smtpClient.Send(message);
        }
    }
}
