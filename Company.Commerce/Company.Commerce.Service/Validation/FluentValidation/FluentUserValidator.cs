using Company.Commerce.Entity.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Service.Validation.FluentValidation
{
    public class FluentUserValidator : AbstractValidator<User>
    {
        private IUserService _userService;

        public FluentUserValidator(IUserService userService)
        {
            _userService = userService;

            this.CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(t => t.EmailAddress).NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("Email address must be properly formatted.")
                .Must(EmailMustNotBeAssosciatedWithAnExistingUser).WithMessage("A user is already assosciated with this email address.");

            RuleFor(t => t.Username)
            .NotEmpty().WithMessage("Username is required.")
            .Length(6, 60).WithMessage("Username must be between 6 and 60 characters long.")
            .Must(UsernameMustNotBeAssosciatedWithAnExistingUser).WithMessage("A user is already assosciated with this email address.");

            RuleFor(t => t.PasswordHash).NotEmpty().WithMessage("PasswordHash is required.");

            RuleSet("Email", () =>
            {
                RuleFor(t => t.EmailAddress).NotEmpty().WithMessage("Email address is required.")
                    .EmailAddress().WithMessage("Email address must be properly formatted.")
                    .Must(EmailMustNotBeAssosciatedWithAnExistingUser).WithMessage("A user is already associated with this email address.");
            });

            RuleSet("Username", () =>
                {
                    RuleFor(t => t.Username)
                        .NotEmpty().WithMessage("Username is required.")
                        .Length(6, 60).WithMessage("Username must be between 6 and 60 characters long.")
                        .Must(UsernameMustNotBeAssosciatedWithAnExistingUser).WithMessage("A user is already associated with this username.");
                });
        }

        private Boolean EmailMustNotBeAssosciatedWithAnExistingUser(User instance, String emailAddress)
        {
            User existingUser = _userService.GetByEmailAsync(emailAddress).Result;

            //If there is no user or the user is this user.
            if (existingUser == null || existingUser.UserId == instance.UserId)
                return true;

            return false;
        }

        public Boolean UsernameMustNotBeAssosciatedWithAnExistingUser(User instance, String username)
        {
            User existingUser = _userService.GetByUsernameAsync(username).Result;

            //If there is no user or the user is this user.
            if (existingUser == null || existingUser.UserId == instance.UserId)
                return true;

            return false;
        }
    }
}
