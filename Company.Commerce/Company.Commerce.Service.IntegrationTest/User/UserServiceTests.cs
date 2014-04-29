using Company.Commerce.Data;
using Company.Commerce.Data.EntityFramework.Context;
using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using Company.Commerce.Service.Utility;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Company.Commerce.Service.IntegrationTest.TestHelper;
using Company.Commerce.Service.Validation.FluentValidation;
using FluentValidation.Results;
using FluentValidation.TestHelper;
using FluentValidation;
using Company.Commerce.Service.Validation.CustomValidation;
using Company.Commerce.Service.Validation;

namespace Company.Commerce.Service.IntegrationTest
{
    [TestFixture]
    public class UserServiceTests
    {
        private User _existingUser;

        private const string SixtyOneCharacterString = "1111111111111111111111111111111111111111111111111111111111111";

        private IDbContext _context;

        private UserService _userService;

        private IUnitOfWork _uow;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            MainContext context = new MainContext(TestVariables.ConnectionString);

            User existingUser = new User()
            {
                AccountConfirmed = true,
                EmailAddress = "ExistingEmail@domain.com",
                PasswordHash = "ExistingPassword",
                Username = "ExistingUsername"
            };

            context.Set<User>().Add(existingUser);

            context.SaveChanges();

            _existingUser = existingUser;
        }

        [SetUp]
        public void Setup()
        {
            _context = new MainContext(TestVariables.ConnectionString);

            _uow = new EfUnitOfWork(_context);

            _userService = new UserService(_uow, null);
        }

        [TearDown]
        public void FixtureTearDown()
        {
            _context.Dispose();
        }

        [Test]
        public void Fluent_User_Validator()
        {
            FluentUserValidator userValidator = new FluentUserValidator(_userService);

            ValidationResult validationResult;

            #region Valid
            User valid = validUser();

            validationResult = userValidator.Validate(valid);

            Assert.IsTrue(validationResult.IsValid);
            #endregion

            #region Email
            //Email
            //  Empty
            User emptyEmailAddress = validUser();

            emptyEmailAddress.EmailAddress = "";

            validationResult = userValidator.Validate(emptyEmailAddress);

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<User>(u => u.EmailAddress));

            //  Invalid Format
            User invalidFormateEmailAddress = validUser();

            invalidFormateEmailAddress.EmailAddress = "emailInvalidFormat";

            validationResult = userValidator.Validate(invalidFormateEmailAddress);

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<User>(u => u.EmailAddress));

            //  A user already exists with this email
            User alreadyExistingEmailAddress = validUser();

            alreadyExistingEmailAddress.EmailAddress = _existingUser.EmailAddress;

            validationResult = userValidator.Validate(alreadyExistingEmailAddress);

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<User>(u => u.EmailAddress));
            #endregion

            #region PasswordHash
            //PasswordHash
            //  Empty
            User emptyPasswordHash = validUser();

            emptyPasswordHash.PasswordHash = "";

            validationResult = userValidator.Validate(emptyPasswordHash);

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<User>(u => u.PasswordHash));
            #endregion

            #region Username
            //Username
            //  Empty
            User emptyUsername = validUser();

            emptyUsername.Username = "";

            validationResult = userValidator.Validate(emptyUsername);

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<User>(u => u.Username));
            //  Too Short MinLength 6
            User usernameTooShort = validUser();

            usernameTooShort.Username = "short";

            validationResult = userValidator.Validate(usernameTooShort);

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<User>(u => u.Username));
            //  Too Long MaxLength 60
            User usernameTooLong = validUser();

            usernameTooLong.Username = SixtyOneCharacterString;

            validationResult = userValidator.Validate(usernameTooLong);

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<User>(u => u.Username));

            //  A user already exists with this username
            User usernameAlreadyExists = validUser();

            usernameAlreadyExists.Username = _existingUser.Username;

            validationResult = userValidator.Validate(usernameAlreadyExists);

            Assert.IsTrue(validationResult.Errors.ContainsErrorForProperty<User>(u => u.Username));
            #endregion
        }

        [Test]
        public void Password_Validator()
        {
            PasswordValidator validator = new PasswordValidator();

            //  Too short (8 character minimum)
            List<ValidationError> errors = validator.Validate("1234567");

            Assert.AreEqual(1, errors.Count());
        }

        [Test]
        public async void Create()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                #region Valid Creates

                ServiceOperationResult<User> result =
                 await _userService.CreateAsync("SomeValidUsername", "SomeValidPassword", "Somevalid@domain.com");

                Assert.IsTrue(result.Succeeded);
                Assert.IsTrue(!result.Errors.Any());
                Assert.IsNotNull(result.Data);

                _uow.Commit();

                User found = await _userService.GetAsync(((User)result.Data).UserId);

                Assert.IsNotNull(found);

                #endregion

                #region Invalid Fails

                result = await _userService.CreateAsync("SomeValidUsername", "SomeValidPassword", "InvalidEmail");

                Assert.IsTrue(!result.Succeeded);
                Assert.AreNotEqual(0, result.Errors.Count());
                Assert.IsNull(result.Data);

                #endregion

                //Any transaction will roll back
            }
        }

        [Test]
        public async void Update()
        {
            using (TransactionScope scope = new TransactionScope())
            {
                #region Setup
                ServiceOperationResult<User> create =
                    await _userService.CreateAsync("SomeValidUsername", "SomeValidPassword", "valid@domain.com");

                _uow.Commit();

                #endregion

                User existingUser = create.Data;

                #region Valid Updates

                existingUser.Username = "NewValidUsername";

                ServiceOperationResult<User> result =
                    await _userService.UpdateAsync(existingUser);

                Assert.IsTrue(result.Succeeded);
                Assert.AreEqual(0, result.Errors.Count());
                Assert.IsNotNull(result.Data);

                User found = await _userService.GetAsync(existingUser.UserId);

                Assert.AreEqual("NewValidUsername", found.Username);

                #endregion

                #region Invalid Fails

                existingUser.Username = "short";

                result = await _userService.UpdateAsync(existingUser);

                Assert.IsTrue(!result.Succeeded);
                Assert.AreNotEqual(0, result.Errors.Count());
                Assert.IsNull(result.Data);

                #endregion

                //Any transaction will roll back
            }
        }

        [Test]
        public async void SetEmail()
        {
            //Sets when valid
            User user = new User();

            ServiceOperationResult result = _userService.SetEmail(user, "valid@domain.com");

            Assert.AreEqual("valid@domain.com", user.EmailAddress);
            Assert.IsTrue(result.Succeeded);
            Assert.IsTrue(!result.Errors.Any());

            //Does not set when invalid
            user = new User();

            result = _userService.SetEmail(user, "invalid");

            Assert.AreNotEqual("invalid", user.Username);
            Assert.IsTrue(!result.Succeeded);
            Assert.IsTrue(result.Errors.Any());
        }

        [Test]
        public async void SetPassword()
        {
            //Sets when valid
            User user = new User();

            ServiceOperationResult result =
                 _userService.SetPassword(user, "ValidPassword");

            Assert.IsNotNullOrEmpty(user.PasswordHash);
            Assert.IsTrue(result.Succeeded);
            Assert.IsTrue(!result.Errors.Any());


            //Does not set when invalid
            user = new User();

            result = _userService.SetPassword(user, "short");

            Assert.IsNullOrEmpty(user.PasswordHash);
            Assert.IsTrue(!result.Succeeded);
            Assert.IsTrue(result.Errors.Any());
        }

        [Test]
        public async void SetUsername()
        {
            //Sets when valid
            User user = new User();

            ServiceOperationResult result = _userService.SetUsername(user, "ValidUsername");

            Assert.AreEqual("ValidUsername", user.Username);
            Assert.IsTrue(result.Succeeded);
            Assert.IsTrue(!result.Errors.Any());

            //Does not set when invalid
            user = new User();

            result = _userService.SetUsername(user, "short");

            Assert.AreNotEqual("short", user.Username);
            Assert.IsTrue(!result.Succeeded);
            Assert.IsTrue(result.Errors.Any());
        }

        [Test]
        public void VerifyPassword()
        {
            User user = new User();

            _userService.SetPassword(user, "SomeValidPassword");

            Assert.IsTrue(_userService.VerifyPassword(user, "SomeValidPassword"));
        }

        #region Helpers
        private User validUser()
        {
            return new User()
            {
                AccountConfirmed = true,
                EmailAddress = "Valid@domain.com",
                PasswordHash = "12345678910",
                Username = "ValidUsername"
            };
        }

        private User invalidUser()
        {
            return new User();
        }
        #endregion
    }
}
