using Company.Commerce.Entity.Models;
using Company.Commerce.Repository;
using Company.Commerce.Service;
using Company.Commerce.Service.Utility;
using Company.Commerce.Web.ViewModels.Account;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Company.Commerce.Web.Utility;
using Company.Commerce.Web.Settings;
using Company.Commerce.Web.Helpers;
using System.Transactions;
using PagedList;

namespace Company.Commerce.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private const String KeyTempDataAccountUpdates = "AccountUpdates";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private readonly IUnitOfWork _uow;
        private readonly IUserService _userService;

        public AccountController(IUnitOfWork uow, IUserService userService)
        {
            _uow = uow;
            _userService = userService;
        }

        //
        // GET: /Account/AnonymousAccountUpdates
        [AllowAnonymous]
        public async Task<ActionResult> AnonymousAccountUpdates()
        {
            Object updates;

            if (TempData.TryGetValue(KeyTempDataAccountUpdates, out updates))
                return View(updates as List<String>);

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ConfirmAccount
        [AllowAnonymous]
        public async Task<ActionResult> AccountRequiresConfirmation()
        {
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/EditEmail
        public async Task<ActionResult> EditEmail()
        {
            User user = await _userService.GetAsync(Utility.Helpers.IdentityHelpers.GetUserId(this.HttpContext.User.Identity));

            EditEmailViewModel model = new EditEmailViewModel()
            {
                CurrentEmailAddress = user.EmailAddress
            };

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
                User user = await _userService.GetAsync(Utility.Helpers.IdentityHelpers.GetUserId(this.HttpContext.User.Identity));

                if (_userService.VerifyPassword(user, model.Password))
                {
                    user.EmailAddress = model.NewEmailAddress;

                    ServiceOperationResult result = await _userService.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        _uow.Commit();

                        await _userService.SendEmailAsync(user, EmailHelpers.UserEmails.AccountPropertyChanged("Email Address"));

                        TempData.Add(KeyTempDataAccountUpdates, new List<String>() { "Your email address has been changed." });

                        return RedirectToAction("Index");
                    }
                    else
                        ModelState.MergeErrors(result.Errors);

                }
                else
                    ModelState.AddErrorForProperty<EditEmailViewModel>(m => m.Password, "Invalid password.");

            }

            //We got this far, something failed
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
                User user = await _userService.GetAsync(Utility.Helpers.IdentityHelpers.GetUserId(this.HttpContext.User.Identity));

                if (_userService.VerifyPassword(user, model.CurrentPassword))
                {
                    ServiceOperationResult result = _userService.SetPassword(user, model.NewPassword);

                    if (result.Succeeded)
                    {
                        _uow.Commit();

                        await _userService.SendEmailAsync(user, EmailHelpers.UserEmails.AccountPropertyChanged("Password"));

                        TempData.Add(KeyTempDataAccountUpdates, new List<String>() { "Password has been changed successfully." });

                        return RedirectToAction("Index");
                    }
                }
                else
                    ModelState.AddErrorForProperty<EditPasswordViewModel>(m => m.CurrentPassword, "Password is invalid.");
            }

            //If we got this far something failed
            return View(model);
        }

        //
        // GET: /Account/EditPasswordForgot
        [AllowAnonymous]
        public ActionResult EditPasswordForgot(String confirmationToken)
        {
            EditPasswordForgotViewModel model = new EditPasswordForgotViewModel()
            {
                ConfirmationToken = confirmationToken
            };

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
                User forgottenUser = await _userService.GetByUsernameAsync(model.Username);

                if (forgottenUser != null)
                {
                    //If the password verification token has not expired
                    if (forgottenUser.PasswordVerificationTokenExpiration > DateTime.Now)
                    {
                        if (model.ConfirmationToken == forgottenUser.PasswordVerificationToken)
                        {
                            ServiceOperationResult setPasswordResult =
                                _userService.SetPassword(forgottenUser, model.NewPassword);

                            if (setPasswordResult.Succeeded)
                            {
                                _uow.Commit();

                                await _userService.SendEmailAsync(forgottenUser, EmailHelpers.UserEmails.AccountPropertyChanged("Password"));

                                TempData.Add(KeyTempDataAccountUpdates, new List<String>() { "Account password has been changed." });

                                return RedirectToAction("AnonymousAccountUpdates");
                            }
                            else
                                ModelState.MergeErrors(setPasswordResult.Errors);

                            return View(model);
                        }
                    }

                    return RedirectToAction("ForgotPassword", new { passwordVerificationTokenExpired = true });
                }

                ModelState.AddErrorForProperty<EditPasswordForgotViewModel>(m => m.Username, "Invalid username.");
            }

            return View(model);
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword(Boolean? passwordVerificationTokenExpired)
        {
            ForgotPasswordViewModel model = new ForgotPasswordViewModel();

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
                User forgottenPasswordUser = await _userService.GetByEmailAsync(model.Email);

                if (forgottenPasswordUser != null)
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        try
                        {
                            String token = _userService.GeneratePasswordVerificationToken(forgottenPasswordUser);

                            _uow.Commit();

                            String changePasswordLink = Url.Action("EditPasswordForgot", "Account", new { confirmationToken = token }, Request.Url.Scheme);

                            await _userService.SendEmailAsync(forgottenPasswordUser, EmailHelpers.UserEmails.ForgottenPasswordRequest(token, changePasswordLink));

                            scope.Complete();

                            TempData.Add(KeyTempDataAccountUpdates, new List<String>() { "An email has been sent detailing how to recover your account." });

                            return RedirectToAction("AnonymousAccountUpdates");
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }

                ModelState.AddErrorForProperty<ForgotPasswordViewModel>(m => m.Email, "Invalid Email.");
            }

            return View(model);
        }

        //
        // GET: /Account/
        public async Task<ActionResult> Index()
        {
            AccountIndexViewModel model = new AccountIndexViewModel();

            Object accountUpdates;

            if (TempData.TryGetValue(KeyTempDataAccountUpdates, out accountUpdates))
                model.ImportantAccountUpdates.AddRange(accountUpdates as IEnumerable<String>);

            Int32 currentUserId = Convert.ToInt32(((ClaimsIdentity)this.User.Identity).Claims.FirstOrDefault(ci => ci.Type == ClaimTypes.Sid).Value);

            model.User = await _userService.GetAsync(currentUserId);

            return View(model);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public async Task<ActionResult> Login()
        {
            LoginViewModel model = new LoginViewModel();

            return View(model);
        }

        //
        // POST: /Account/Login
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User userToLogin = await _userService.GetByUsernameAsync(model.Username);

                if (userToLogin != null)
                {
                    if (!AccountSettings.RequireEmailConfirmationForRegistration || userToLogin.AccountConfirmed)
                    {
                        if (_userService.VerifyPassword(userToLogin, model.Password))
                        {
                            await SignInAsync(userToLogin, model.RememberMe);

                            return RedirectToAction("Index", "Home");
                        }
                        else
                            ModelState.AddModelError("", "Invalid username or password.");
                    }
                    else
                        return RedirectToAction("AccountRequiresConfirmation");
                }
                else
                    ModelState.AddModelError("", "Invalid username or password.");
            }

            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/OrderDetails
        public ActionResult OrderDetails(Int32 orderId)
        {
            User user = _userService.GetWithOrders(Utility.Helpers.IdentityHelpers.GetUserId(this.HttpContext.User.Identity));

            Order order = user.Orders.FirstOrDefault(o => o.OrderId == orderId);

            if (order != null)
                return View(order);

            return RedirectToAction("Index");
        }

        //
        // GET: /Account/OrderHistory
        public ActionResult OrderHistory(OrderHistoryViewModel model)
        {
            Int32 itemsPerPage = 5;

            User user = _userService.GetWithOrders(Utility.Helpers.IdentityHelpers.GetUserId(this.HttpContext.User.Identity));

            IQueryable<Order> ordersQuery = user.Orders.AsQueryable();

            if (model.From != null)
                ordersQuery = ordersQuery.Where(o => o.OrderDate > model.From);

            if (model.To != null)
                ordersQuery = ordersQuery.Where(o => o.OrderDate < model.To);

            model.Orders = ordersQuery.OrderByDescending(o => o.OrderDate).ToPagedList<Order>(model.PageNumber, itemsPerPage);

            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public async Task<ActionResult> Register()
        {
            if (!AccountSettings.IsRegistrationEnabled)
                return RedirectToAction("Index", "Account");
            else
            {
                RegisterViewModel model = new RegisterViewModel();

                return View(model);
            }
        }

        //
        // POST: /Account/Register
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //User toCreate = new User()
                //{
                //    EmailAddress = model.Email,
                //    Username = model.Username
                //};

                //ServiceOperationResult<User> createUserResult =
                //     await _userService.CreateAsync(toCreate, model.Password);

                ServiceOperationResult<User> createUserResult =
                     await _userService.CreateAsync(model.Username, model.Password, model.Email);

                if (createUserResult.Succeeded)
                {
                    _uow.Commit();

                    if (AccountSettings.RequireEmailConfirmationForRegistration)
                        return RedirectToAction("AccountRequiresConfirmation");
                    else
                    {
                        await SignInAsync(createUserResult.Data, false);

                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                    ModelState.MergeErrors(createUserResult.Errors);
            }

            return View(model);
        }

        private async Task SignInAsync(User user, bool isPersistent)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            User userWithClaims = await _userService.GetWithClaimsAsync(user.UserId);

            ClaimsIdentity identity = convertUserToIdentity(userWithClaims);

            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private ClaimsIdentity convertUserToIdentity(User user)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            List<Claim> claims = new List<Claim>();

            Claim emailClaim = new Claim(ClaimTypes.Email, user.EmailAddress);

            claims.Add(emailClaim);

            Claim sidClaim = new Claim(ClaimTypes.Sid, user.UserId.ToString());

            claims.Add(sidClaim);

            ClaimsIdentity identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            return identity;
        }
    }
}