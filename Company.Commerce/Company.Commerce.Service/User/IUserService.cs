using Company.Commerce.Entity.Models;
using Company.Commerce.Service.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public interface IUserService
    {
        Task<ServiceOperationResult<User>> CreateAsync(String username, String password, String emailAddress);

        String GeneratePasswordVerificationToken(User user);

        Task<User> GetAsync(Int32 userId);

        User GetWithOrders(Int32 userId);

        Task<User> GetByEmailAsync(String emailAddress);

        Task<User> GetByUsernameAsync(String username);

        Task<User> GetByUsernameAndPasswordAsync(String username, String password);

        Task<User> GetWithClaimsAsync(Int32 userID);

        Task SendEmailAsync(User user, MailMessage message);

        ServiceOperationResult SetPassword(User user, String password);

        Task<ServiceOperationResult<User>> UpdateAsync(User user);

        Boolean VerifyPassword(User user, String password);

    }
}
