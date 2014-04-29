using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using Company.Commerce.Service.Email;
using Company.Commerce.Service.Utility;
using Company.Commerce.Service.Validation;
using Company.Commerce.Service.Validation.CustomValidation;
using Company.Commerce.Service.Validation.FluentValidation;
using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service
{
    public class UserService : IUserService
    {
        #region Fields
        private String _userServiceEmailSource = "mykah89@gmail.com";

        private readonly IEmailService _emailService;

        private readonly FluentUserValidator _userValidator;

        private readonly IUnitOfWork _uow;

        private readonly PasswordValidator _passwordValidator;

        #endregion

        #region Constructors

        public UserService(IUnitOfWork uow, IEmailService emailService)
        {
            _emailService = emailService;

            _uow = uow;

            _passwordValidator = new PasswordValidator();

            _userValidator = new FluentUserValidator(this);
        }

        #endregion

        #region Public Methods

        public async Task<ServiceOperationResult<User>> CreateAsync(String username, String password, String emailAddress)
        {
            if (String.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username");

            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password");

            if (String.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentNullException("emailAddress");

            ServiceOperationResult<User> result = new ServiceOperationResult<User>();

            //Clean up the input
            emailAddress = emailAddress.Trim();
            username = username.Trim();
            password = password.Trim();

            //Hydrate the user we are going to create
            User user = new User()
            {
                AccountConfirmed = false,
                EmailAddress = emailAddress,
                Username = username
            };

            //Password must be validated and hash set via the SetPasswordMethod
            ServiceOperationResult setPasswordResult = SetPassword(user, password);

            if (setPasswordResult.Succeeded)
            {
                ValidationResult validationResult = _userValidator.Validate(user);

                if (validationResult.IsValid)
                {
                    //If we got this far we have a valid user
                    User createdUser = _uow.Repository<User>().Add(user);

                    result.Data = createdUser;

                    result.Succeeded = true;
                }
                else
                    result.Errors.AddRange(validationResult.Errors.ToValidationErrorList());
            }
            else //Password setting failed add errors
                result.Errors.AddRange(setPasswordResult.Errors);

            return result;
        }

        public String GeneratePasswordVerificationToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            String token = Guid.NewGuid().ToString("N");

            user.PasswordVerificationToken = token;

            user.PasswordVerificationTokenExpiration = DateTime.Now + TimeSpan.FromMinutes(15);

            return token;
        }

        public async Task<User> GetAsync(Int32 userID)
        {
            return _uow.Repository<User>().Find(userID);
        }

        public User GetWithOrders(Int32 userId)
        {
            return _uow.Repository<User>().Query()
                .Filter(u => u.UserId == userId)
                .Include(u => u.Orders)
                .Get()
                .FirstOrDefault();
        }

        public async Task<User> GetByEmailAsync(String emailAddress)
        {
            if (String.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentNullException("emailAddress");

            return _uow.Repository<User>().Query()
                .Filter(u => u.EmailAddress == emailAddress)
                .Get()
                .FirstOrDefault();
        }

        public async Task<User> GetByUsernameAndPasswordAsync(String username, String password)
        {
            if (String.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username");

            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password");

            User user = _uow.Repository<User>().Query()
                .Filter(u => u.Username == username)
                .Get()
                .FirstOrDefault();

            if (user != null)
            {
                if (VerifyPassword(user, password))
                {
                    return user;
                }
            }

            return null;
        }

        public async Task<User> GetWithClaimsAsync(Int32 userID)
        {
            return _uow.Repository<User>().Find(userID);

            //_uow.Repository<User>().Query()
            //    .Filter(u => u.UserId == userID)
            //    .Include(u => u.clai)
        }

        public async Task<User> GetByUsernameAsync(String username)
        {
            if (String.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username");

            return _uow.Repository<User>().Query()
                .Filter(u => u.Username == username)
                .Get()
                .FirstOrDefault();
        }

        public async Task SendEmailAsync(User user, MailMessage message)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (message == null)
                throw new ArgumentNullException("message");

            message.From = new MailAddress(_userServiceEmailSource);

            if (message.To.FirstOrDefault(m => m.Address == user.EmailAddress) == null)
            {
                MailAddress toItem = new MailAddress(user.EmailAddress);

                message.To.Add(toItem);
            }

            _emailService.SendMessage(message);
        }

        public ServiceOperationResult SetEmail(User user, String emailAddress)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (String.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentNullException("emailAddress");

            ServiceOperationResult result = new ServiceOperationResult();

            emailAddress = emailAddress.Trim();

            ValidationResult validationResult =
                _userValidator.Validate(new User() { EmailAddress = emailAddress }, ruleSet: "Email");

            if (validationResult.IsValid)
            {
                user.EmailAddress = emailAddress;

                result.Succeeded = true;
            }
            else
                result.Errors.AddRange(validationResult.Errors.ToValidationErrorList());

            return result;
        }

        public ServiceOperationResult SetPassword(User user, String password)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password");

            ServiceOperationResult result = new ServiceOperationResult();

            password = password.Trim();

            result.Errors.AddRange(_passwordValidator.Validate(password));

            if (!result.Errors.Any())
            {
                user.PasswordHash = hashPassword(password);

                result.Succeeded = true;
            }

            return result;
        }

        public ServiceOperationResult SetUsername(User user, String username)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (String.IsNullOrWhiteSpace(username))
                throw new ArgumentNullException("username");

            ServiceOperationResult result = new ServiceOperationResult();

            username = username.Trim();

            User tempToValidate = new User() { Username = username };

            ValidationResult validationResult = _userValidator.Validate(tempToValidate, ruleSet: "Username");

            if (validationResult.IsValid)
            {
                user.Username = username;

                result.Succeeded = true;
            }
            else
                result.Errors.AddRange(validationResult.Errors.ToValidationErrorList());

            return result;
        }

        public async Task<ServiceOperationResult<User>> UpdateAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (user.UserId <= 0)
                throw new ArgumentOutOfRangeException("UserID", user.UserId, "Update cannot be performed on a non existant user.");

            ServiceOperationResult<User> result = new ServiceOperationResult<User>();

            ValidationResult validationResult = _userValidator.Validate(user);

            if (validationResult.IsValid)
            {
                _uow.Repository<User>().Update(user);

                result.Data = user;

                result.Succeeded = true;
            }
            else
                result.Errors.AddRange(validationResult.Errors.ToValidationErrorList());

            return result;
        }

        public Boolean VerifyPassword(User user, String password)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password");

            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }

        #endregion

        #region Private Methods

        private String hashPassword(String password)
        {
            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException(password);

            return BCrypt.Net.BCrypt.HashPassword(password.Trim(), 12);
        }

        private Boolean verifyPassword(String passwordHash, String password)
        {
            if (String.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentNullException("passwordHash");

            if (String.IsNullOrWhiteSpace(password))
                throw new ArgumentNullException("password");

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        #endregion
    }
}
