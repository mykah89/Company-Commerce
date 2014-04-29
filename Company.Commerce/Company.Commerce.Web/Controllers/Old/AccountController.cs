using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Project.MVC.Data;
using Project.MVC.Entity.Models;
using Framework.Repository;
using Project.MVC.Service;
using Project.MVC.Web.Security;
using Project.MVC.Web.ViewModels.Account;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using Project.MVC.Web.ViewModels.General;
using System.Reflection;
using Project.MVC.Web.Helpers;
using System.Transactions;
using PagedList;
using Project.MVC.Web.ViewModels;
using System.Net;
using Project.MVC.Services.Email;
using System.Net.Mail;
using Project.MVC.Service.Configuration;
using Project.MVC.Web.Core.Config.Account;
using Project.MVC.Web.Configuration;
using System.Configuration;
using System.Web.Configuration;

namespace Project.MVC.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ICartService _shoppingCartService;
        private IEmailService _emailService;
        private ISettingsService _settingsService;
        private IUnitOfWork _uow;
        private IUserService _userService;

        public AccountController(IUnitOfWork uow, ICartService shoppingCartService, IEmailService emailService, ISettingsService settingsService, IUserService userService)
        {
            _emailService = emailService;
            _shoppingCartService = shoppingCartService;
            _settingsService = settingsService;
            _uow = uow;
            _userService = userService;
        }

        //
        // GET: /Account/
        public ActionResult Index()
        {
            var user = _userService.GetUserWithOrdersAndProfile(IdentityHelpers.GetUserId(this.User.Identity));

            object importantAccountUpdates;
            TempData.TryGetValue("ImportantAccountUpdates", out importantAccountUpdates);

            var model = new AccountIndexViewModel()
            {
                ImportantAccountUpdates = importantAccountUpdates as List<String> ?? new List<String>(),
                RecentOrders = user.Orders.OrderByDescending(o => o.OrderDate).Take(5).ToList(),
                User = user,
                UserProfile = user.UserProfile
            };

            return View(model);
        }

        //
        // GET: /Account/AddAddress/
        public ActionResult AddAddress()
        {
            var model = new AddNewAddressViewModel();

            return View(model);
        }

        //
        // POST: /Account/AddAddress/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAddress(AddNewAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userID = IdentityHelpers.GetUserId(User.Identity);

                var address = Mapper.Map<Address>(model);

                address.UserID = userID;

                _userService.AddNewAddress(userID, address);

                _uow.Save();

                return RedirectToAction("EditAddresses");
            }

            return View(model);
        }

        //
        // GET: /Account/AccountConfirmed
        [AllowAnonymous]
        public ActionResult AccountConfirmed()
        {
            return View();
        }

        //
        // GET: /Account/ConfirmAccount/?confirmationToken=&?email=
        [AllowAnonymous]
        public ActionResult ConfirmAccount(ConfirmAccountViewModel model)
        {
            //Returns true if successful, false if confirmation token was invalid
            if (_userService.ConfirmAccount(model.Email, model.ConfirmationToken))
                _uow.Save();
            else
                return RedirectToAction("ResendConfirmationEmail", new { email = model.Email });

            return RedirectToAction("AccountConfirmed");
        }

        //
        // GET: /Account/DisassociateAddress/?addressId=
        public ActionResult DisassociateAddress(Int32 addressId)
        {
            var user = _userService.GetUserWithAddresses(IdentityHelpers.GetUserId(User.Identity));

            var add = user.Addresses.FirstOrDefault(a => a.AddressID == addressId);

            if (add != null)
            {
                return View(add);
            }

            throw new HttpException(Convert.ToInt32(HttpStatusCode.NotFound), "Attempt to disassosciate non existing address.");
        }

        //
        // POST: /Account/DisassociateAddress/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DisassociateAddress(Int32 addressId, String diff = "")
        {
            _userService.DisassociateAddress(IdentityHelpers.GetUserId(User.Identity), addressId);

            _uow.Save();

            return RedirectToAction("EditAddresses", "Account");
        }

        //
        // GET: /Account/EditAddresses
        public ActionResult EditAddresses()
        {
            var user = _userService.GetUserWithAddresses(IdentityHelpers.GetUserId(User.Identity));

            return View(user.Addresses);
        }

        //
        // GET: /Account/EditProfile
        public ActionResult EditProfile()
        {
            var user = _userService.GetUserWithProfile(IdentityHelpers.GetUserId(User.Identity));

            var model = Mapper.Map<UserProfileViewModel>(user.UserProfile);

            return View(model);
        }

        //
        // POST: /Account/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile([Bind(Exclude = "UserID")]UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var updatedProfile = Mapper.Map<UserProfile>(model);

                var userID = IdentityHelpers.GetUserId(this.User.Identity);

                updatedProfile.UserID = userID;

                _userService.UpdateUserProfile(userID, updatedProfile);

                _uow.Save();

                return RedirectToAction("Index", "Account");
            }
            else
                return View(model);
        }

        //
        // GET: /Account/EditEmail
        public ActionResult EditEmail()
        {
            var user = _userService.Get(IdentityHelpers.GetUserId(User.Identity));

            var model = new EditEmailViewModel() { CurrentEmailAddress = user.Email };

            return View(model);
        }

        //
        // POST: /Account/EditEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditEmail(EditEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.Get(IdentityHelpers.GetUserId(User.Identity));

                if (_userService.IsPasswordMatchForUser(user.UserID, model.Password))
                {
                    String originalEmail = user.Email;

                    //Check if the new email address is already assosciated with another account
                    if (_userService.GetByEmail(model.NewEmailAddress) != null)
                    {
                        _userService.SetEmailAddress(user.UserID, model.NewEmailAddress);

                        _uow.Save();

                        //Send notification email
                        String subject = "Account Email Changed";
                        String body = "This email is to inform you that the email address associated with your account has been changed.";

                        var message = new MailMessage("mykah89@gmail.com", originalEmail, subject, body);

                        await _emailService.SendMessageAsync(message);

                        ViewBag.SuccessMessage = "Your email address has been changed.";

                        ModelState.Clear();

                        return View();
                    }

                    ModelState.AddPropertyError<EditEmailViewModel>(m => m.NewEmailAddress, "An account with this email already exists.");
                }
                else
                {

                    ModelState.AddPropertyError<EditEmailViewModel>(m => m.Password, "Invalid password.");

                    return View(model);
                }
            }

            //We got this far, some.panel .panel-default failed
            return View(model);
        }

        //
        // GET: /Account/EditPassword
        public ActionResult EditPassword()
        {
            return View();
        }


        //
        // POST: /Account/EditPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPassword(EditPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.Get(IdentityHelpers.GetUserId(User.Identity));

                if (_userService.IsPasswordMatchForUser(user.UserID, model.CurrentPassword))
                {
                    _userService.SetPassword(user.UserID, model.NewPassword);

                    _uow.Save();

                    String subject = "Account Password Change";

                    String body = "This email is to inform you that the password associated with your account has been changed.";

                    await _userService.SendEmailAsync(user.UserID, subject, body);

                    ViewBag.SuccessMessage = "Password has been changed successfully.";

                    ModelState.Clear();

                    return View();
                }
                else
                {
                    ModelState.AddModelError("CurrentPassword", "Password is invalid.");

                    return View(model);
                }
            }
            else
                return View(model);
        }

        //
        // GET: /Account/EditPasswordForgot
        [AllowAnonymous]
        public ActionResult EditPasswordForgot(String confirmationToken)
        {
            var model = new EditPasswordForgotViewModel();

            model.ConfirmationToken = confirmationToken;

            return View(model);
        }

        //
        // POST: /Account/EditPasswordForgot
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPasswordForgot(EditPasswordForgotViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetByEmailWithMembership(model.Email);

                if (user != null)
                {
                    //If the password verification token has expired
                    if (user.Membership.PasswordVerificationTokenExpiration > DateTime.Now)
                    {
                        if (model.ConfirmationToken == user.Membership.PasswordVerificationToken)//user.ConfirmationToken)
                        {
                            _userService.SetPassword(user.UserID, model.NewPassword);

                            _uow.Save();

                            String subject = "Account Password Changed";

                            String body = "This email is to inform you that the password associated with your account has been changed.";

                            await _userService.SendEmailAsync(user.UserID, subject, body);

                            ModelState.Clear();

                            ViewBag.SuccessMessage = "Your password has been changed.";

                            return View();
                        }
                    }

                    return RedirectToAction("ForgotPassword", new { passwordVerificationTokenExpired = true });
                }

                ModelState.AddModelError("Email", "Invalid email address.");
            }

            return View(model);
        }


        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword(Boolean? passwordVerificationTokenExpired)
        {
            var model = new ForgotPasswordViewModel();

            ViewBag.PasswordVerificationTokenExpired = passwordVerificationTokenExpired.HasValue ? passwordVerificationTokenExpired.Value : false;

            return View(model);
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetByEmailWithMembership(model.Email);

                if (user != null)
                {
                    using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
                    {
                        try
                        {
                            var verificationToken = _userService.GeneratePasswordVerificationToken(user.UserID);

                            _uow.Save();

                            String changePasswordLink = Url.Action("EditPasswordForgot", "Account", new { confirmationToken = verificationToken }, Request.Url.Scheme);

                            String subject = "Forgotten Password Request";
                            String body = "A forgotten password request has been made, please enter this link in your browser's address bar "
                                + "to recover your account."
                                + changePasswordLink;

                            await _userService.SendEmailAsync(user.UserID, subject, body);

                            scope.Complete();

                            ViewBag.SuccessMessage = "An email has been sent detailing how to recover your account.";

                            return View();
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                }

                ModelState.AddPropertyError<ForgotPasswordViewModel>(m => m.Email, "Invalid Email.");
            }

            return View(model);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetByEmailWithMembership(model.Email);

                if (user != null)
                {
                    if (user.IsAccountConfirmed)
                    {
                        if (!user.IsLockedOut)
                        {
                            if (_userService.IsPasswordMatchForUser(user.UserID, model.Password))
                            {
                                SignIn(user, model.RememberMe);

                                _userService.SuccessfulLogin(user.UserID);

                                _shoppingCartService.MigrateCart(ShoppingCartHelpers.GetShoppingCartID(this.ControllerContext), user.Email);

                                ShoppingCartHelpers.SetShoppingCartID(this.ControllerContext, user.Email);

                                _uow.Save();

                                return RedirectToLocal(returnUrl);
                            }

                            _userService.InvalidPasswordLoginAttempt(user.UserID);

                            if (user.Membership.PasswordFailsSinceLastSuccess >= 5)
                            {
                                _userService.LockAccount(user.UserID, AccountLockReason.InvalidPasswordAttempts);

                                ModelState.AddModelError("", "Due to invalid password attempts your account has been locked for " + (user.Membership.LastPasswordFailureDate.AddMinutes(15) - DateTime.Now).Minutes + " minute(s).");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid password attempt. You have " + (5 - user.Membership.PasswordFailsSinceLastSuccess) + " attempt(s) left before your account is locked for 15 minutes.");
                            }

                            _uow.Save();

                            return View(model);
                        }
                        else
                        {
                            if (user.AccountLockReason == AccountLockReason.AdministratorRequested)
                                return View("AdministratorLockout");

                            if (user.AccountLockReason == AccountLockReason.InvalidPasswordAttempts)
                            {
                                var unlockTime = user.Membership.LastPasswordFailureDate.AddMinutes(15);

                                if (unlockTime < DateTime.Now)
                                {
                                    _userService.UnlockAccount(user.UserID);

                                    SignIn(user, model.RememberMe);

                                    _userService.SuccessfulLogin(user.UserID);

                                    _shoppingCartService.MigrateCart(ShoppingCartHelpers.GetShoppingCartID(this.ControllerContext), user.UserID.ToString());

                                    ShoppingCartHelpers.SetShoppingCartID(this.ControllerContext, user.UserID.ToString());

                                    _uow.Save();

                                    return RedirectToLocal(returnUrl);
                                }

                                ModelState.AddModelError("", "Due to invalid password attempts your account has been locked for " + (unlockTime - DateTime.Now).Minutes + " minute(s).");
                            }

                            return View(model);
                        }

                    }

                    return View("AccountNotConfirmed");//, model.Email);
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View(model);

            //try
            //{
            //    _shoppingCartService.MigrateCart(ShoppingCartHelpers.GetShoppingCartID(this.ControllerContext), user.UserID.ToString());

            //    _uow.Save();
            //}
            //catch (Exception)
            //{
            //    //Log error, its not really a big deal if the cart items get destroyed in this process
            //    //it shouldnt happen, but such is life.
            //}

            //ShoppingCartHelpers.SetShoppingCartID(this.ControllerContext, user.UserID.ToString());

            //return RedirectToLocal(returnUrl);
        }

        //
        // GET: /Account/OrderDetails
        public ActionResult OrderDetails(Int32 orderId)
        {
            var order = _userService.GetCompleteOrder(IdentityHelpers.GetUserId(this.User.Identity), orderId);

            return View(order);
        }

        //
        // GET: /Account/OrderHistory
        public ActionResult OrderHistory(OrderHistoryViewModel model)
        {
            Int32 itemsPerPage = 5;

            var user = _userService.GetUserWithOrders(IdentityHelpers.GetUserId(User.Identity));

            var ordersQuery = user.Orders.AsQueryable();

            if (model.From != null)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate > model.From);

            }

            if (model.To != null)
            {
                ordersQuery = ordersQuery.Where(o => o.OrderDate < model.To);
            }

            model.Orders = ordersQuery.OrderByDescending(o => o.OrderDate).ToPagedList<Order>(model.PageNumber, itemsPerPage);

            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var creationResult = _userService.Create(model.Email, model.Username, model.Password);

                if (creationResult.Success)
                {
                    await _uow.SaveAsync();

                    var user = creationResult.CreatedUser;

                    String registrationConfirmationLink = Url.Action("ConfirmAccount", "Account", new { email = user.Email, confirmationToken = user.Membership.AccountConfirmationToken }, Request.Url.Scheme);

                    String subject = "Account Registration";

                    String body = "Thank you for signing up.  In order to complete the registration process, please enter this address in "
                        + "your browser's address bar."
                        + registrationConfirmationLink;

                    await _userService.SendEmailAsync(user.UserID, subject, body);

                    ViewBag.SuccessMessage = "Thank you for registering, an email has been sent with instructions on how to complete the registration process.";

                    ModelState.Clear();

                    return View();
                }
                else
                {
                    creationResult.Errors.ForEach(e => ModelState.AddModelError(e.Key, e.Value));
                }
            }

            // If we got this far, some.panel .panel-default failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResendConfirmationEmail
        [AllowAnonymous]
        public ActionResult ResendConfirmationEmail(String email)
        {
            var model = new ResendConfirmationEmailViewModel();

            model.Email = email;

            return View(model);
        }

        //
        // POST: /Account/ResendConfirmationEmail
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResendConfirmationEmail(ResendConfirmationEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetByEmailWithMembership(model.Email);

                if (user != null)
                {
                    if (_userService.IsPasswordMatchForUser(user.UserID, model.Password))
                    {
                        String confirmationLink = Url.Action("ConfirmAccount", "Account", new { email = user.Email, confirmationToken = user.Membership.AccountConfirmationToken }, Request.Url.Scheme);

                        String subject = "Account Registration";

                        String body = "Thank you for signing up.  In order to complete the registration process, please enter this address in "
                            + "your browser's address bar."
                            + confirmationLink;

                        await _userService.SendEmailAsync(user.UserID, subject, body);

                        ModelState.Clear();

                        ViewBag.SuccessMessage = "An email has been sent to you.";

                        return View();
                    }
                }

                ModelState.AddModelError("", "Invalid email or password.");
            }

            return View(model);
        }

        #region Template Generated
        ////
        //// POST: /Account/Disassociate
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        //{
        //    ManageMessageId? message = null;
        //    IdentityResult result = await UserManager.RemoveLoginAsync(IdentityHelpers.GetUserId(User.Identity), new UserLoginInfo(loginProvider, providerKey));
        //    if (result.Succeeded)
        //    {
        //        message = ManageMessageId.RemoveLoginSuccess;
        //    }
        //    else
        //    {
        //        message = ManageMessageId.Error;
        //    }
        //    return RedirectToAction("Manage", new { Message = message });
        //}

        ////
        //// GET: /Account/Manage
        //public ActionResult Manage(ManageMessageId? message)
        //{
        //    ViewBag.StatusMessage =
        //        message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
        //        : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
        //        : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
        //        : message == ManageMessageId.Error ? "An error has occurred."
        //        : "";
        //    ViewBag.HasLocalPassword = HasPassword();
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    return View();
        //}

        ////
        //// POST: /Account/Manage
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Manage(ManageUserViewModel model)
        //{
        //    bool hasPassword = HasPassword();
        //    ViewBag.HasLocalPassword = hasPassword;
        //    ViewBag.ReturnUrl = Url.Action("Manage");
        //    if (hasPassword)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            IdentityResult result = await UserManager.ChangePasswordAsync(IdentityHelpers.GetUserId(User.Identity), model.OldPassword, model.NewPassword);

        //            if (result.Succeeded)
        //            {
        //                _uow.Save();

        //                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
        //            }
        //            else
        //            {
        //                AddErrors(result);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // User does not have a password so remove any validation errors caused by a missing OldPassword field
        //        ModelState state = ModelState["OldPassword"];
        //        if (state != null)
        //        {
        //            state.Errors.Clear();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            IdentityResult result = await UserManager.AddPasswordAsync(IdentityHelpers.GetUserId(User.Identity), model.NewPassword);
        //            if (result.Succeeded)
        //            {
        //                return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
        //            }
        //            else
        //            {
        //                AddErrors(result);
        //            }
        //        }
        //    }

        //    // If we got this far, some.panel .panel-default failed, redisplay form
        //    return View(model);
        //}

        ////
        //// POST: /Account/ExternalLogin
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public ActionResult ExternalLogin(string provider, string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        ////
        //// GET: /Account/ExternalLoginCallback
        //[AllowAnonymous]
        //public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Login");
        //    }

        //    // Sign in the user with this external login provider if the user already has a login
        //    var user = await UserManager.FindAsync(loginInfo.Login);
        //    if (user != null)
        //    {
        //        await SignInAsync(user, isPersistent: false);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    else
        //    {
        //        // If the user does not have an account, then prompt the user to create an account
        //        ViewBag.ReturnUrl = returnUrl;
        //        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        //        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
        //    }
        //}

        ////
        //// POST: /Account/LinkLogin
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult LinkLogin(string provider)
        //{
        //    // Request a redirect to the external login provider to link a login for the current user
        //    return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.IdentityHelpers.GetUserId(context.HttpContext.User.Identity)());
        //}

        ////
        //// GET: /Account/LinkLoginCallback
        //public async Task<ActionResult> LinkLoginCallback()
        //{
        //    var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.IdentityHelpers.GetUserId(context.HttpContext.User.Identity)());
        //    if (loginInfo == null)
        //    {
        //        return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //    }
        //    var result = await UserManager.AddLoginAsync(IdentityHelpers.GetUserId(User.Identity), loginInfo.Login);
        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction("Manage");
        //    }
        //    return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        //}

        ////
        //// POST: /Account/ExternalLoginConfirmation
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        return RedirectToAction("Manage");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        // Get the information about the user from the external login provider
        //        var info = await AuthenticationManager.GetExternalLoginInfoAsync();
        //        if (info == null)
        //        {
        //            return View("ExternalLoginFailure");
        //        }
        //        var user = new User() { Username = model.UserName };
        //        var result = await UserManager.CreateAsync(user);
        //        if (result.Succeeded)
        //        {
        //            result = await UserManager.AddLoginAsync(IdentityHelpers.GetUserId(User.Identity), info.Login);
        //            if (result.Succeeded)
        //            {
        //                await SignInAsync(user, isPersistent: false);
        //                return RedirectToLocal(returnUrl);
        //            }
        //        }
        //        AddErrors(result);
        //    }

        //    ViewBag.ReturnUrl = returnUrl;
        //    return View(model);
        //}

        ////
        //// GET: /Account/ExternalLoginFailure
        //[AllowAnonymous]
        //public ActionResult ExternalLoginFailure()
        //{
        //    return View();
        //}

        //[ChildActionOnly]
        //public ActionResult RemoveAccountList()
        //{
        //    var linkedAccounts = UserManager.GetLogins(IdentityHelpers.GetUserId(User.Identity));
        //    ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
        //    return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        //}

        #endregion

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();

            //Clear the shopping cart id
            ShoppingCartHelpers.SetShoppingCartID(this.ControllerContext, null);

            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)// && UserManager != null)
            {
                //UserManager.Dispose();
                //UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void SignIn(User user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            var identity = IdentityHelpers.CreateIdentity(user);

            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        //private async Task SignInAsync(User user, bool isPersistent)
        //{
        //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

        //    var identity = IdentityHelpers.CreateIdentity(user);
        //    //var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

        //    AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        //}

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        //private bool HasPassword()
        //{
        //    //var user = UserManager.FindById(IdentityHelpers.GetUserId(User.Identity));

        //    //if (user != null)
        //    //{
        //    //    return user.PasswordHash != null;
        //    //}

        //    return false;
        //}

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}