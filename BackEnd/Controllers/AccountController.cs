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
using Models;
using Dal;
using Postal;

namespace ScT_LanSuite.Controllers
{
    [Authorize]
    public class AccountController : ScTController
    {
        private string CreateConfirmationToken()
        {
            return Guid.NewGuid().ToString();
        }

        private void SendEmailConfirmation(string to, string username, string confirmationToken)
        {
            dynamic email = new Email("RegEmail");
            email.To = to;
            email.UserName = username;
            email.ConfirmationToken = confirmationToken;
            email.Send();
        }
        public async Task<string> ResendEmailConfirmation()
        {
            try
            {
                string confirmationToken = CreateConfirmationToken();
                string uid = User.Identity.GetUserId();
                var user = await uow.userRepository.FindAsync(x => x.Id == uid);
                user.ConfirmationToken = confirmationToken;

                if (!user.EmailConfirmed)
                    SendEmailConfirmation(user.Email, user.UserName, confirmationToken);
                else
                    return "Error";

                await uow.userRepository.UpdateAsync(user);
                return "Success";
            }
            catch
            {
                return "Error";
            }            
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
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindAsync(model.UserName, model.Password);
                var user = await uow.userRepository.FindAsync(x => x.Email == model.UserName || x.UserName == model.UserName);
                var passHash = _userManager.PasswordHasher.HashPassword(model.Password);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [AllowAnonymous]
        public async Task<ActionResult> RegisterConfirmation(string Id)
        {
            var user = await uow.userRepository.FindAsync(x => x.ConfirmationToken == Id);
            user.EmailConfirmed = true;
            await uow.userRepository.UpdateAsync(user);
            return RedirectToAction("index", "home");
        }



        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
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
                string confirmationToken = CreateConfirmationToken();
                var user = new ApplicationUser() { UserName = model.UserName, Email = model.Email, FullName = model.FullName, ConfirmationToken = confirmationToken };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    SendEmailConfirmation(model.Email, model.UserName, confirmationToken);
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    AddErrors(result);
                }
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await _userManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.UpdateProfileSuccess ? "Your profile has been updated."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.AddNewClanSuccess ? "Your clan has been created."
                : message == ManageMessageId.ClanInviteSuccess ? "A clan invite has been sent."
                : message == ManageMessageId.ClanInviteAccepted ? "You accepted a clan invite."
                : message == ManageMessageId.ClanInviteDeclined ? "You declined a clan invite."
                : message == ManageMessageId.ClanMemberKicked ? "You successfully kicked a member."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await _userManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await _userManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName, Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await _userManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName, FullName = model.FullName, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {

            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = _userManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public async Task<ActionResult> Edit(string id, ManageMessageId? Message = null)
        {
            var Db = new ApplicationDbContext();
            var user = await uow.userRepository.FindAsync(u => u.UserName == User.Identity.Name);

            var model = new EditUserViewModel(user);
            ViewBag.MessageId = Message;
            return PartialView("_Edit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Db = new ApplicationDbContext();
                var user = Db.Users.First(u => u.UserName == User.Identity.Name);
                // Update the user data:
                user.FullName = model.FullName;
                user.Email = model.Email;
                Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                await Db.SaveChangesAsync();
                return RedirectToAction("Manage", new { Message = ManageMessageId.UpdateProfileSuccess });
                //return "Success";
            }
            // If we got this far, something failed, redisplay form
            //return "Fail";
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        public ActionResult Clans()
        {
            var user = Task.Run(async () => await uow.userRepository.FindAsync(x => x.UserName == User.Identity.Name)).Result;
            var clanInviteVm = new ClanInvitationsViewModel();
            if (user.ClanInvitations.Count != 0)
            {
                clanInviteVm.ClanInvitations = user.ClanInvitations;
            }
            if (user.Clan != null)
            {
                clanInviteVm.Clan = user.Clan;
                if (user.Clan != null)
                {
                    clanInviteVm.isInClan = true;
                }
                if (user.Id == user.Clan.UserID)
                {
                    clanInviteVm.isLeader = true;
                }
            }
            return PartialView("_Clans", clanInviteVm);
        }

        [HttpPost]
        public async Task<ActionResult> Clans(ClanInvitationsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var clan = model.Clan;
                var user = await uow.userRepository.FindAsync(x => x.UserName == User.Identity.Name);
                clan.Leader = user;
                clan.Users = new List<ApplicationUser>();
                clan.Users.Add(user);
                user.Clan = clan;
                await uow.userRepository.UpdateAsync(user);
                //await uow.clanRepository.AddAsync(clan);


                return RedirectToAction("Manage", new { Message = ManageMessageId.AddNewClanSuccess });
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        public async Task<JsonResult> GetUsers(string term)
        {
            var users = await uow.userRepository.FindAllAsync(x => x.UserName.Contains(term));
            var userStrings = new List<string>();
            foreach (var item in users)
            {
                userStrings.Add(item.UserName);
            }
            return Json(userStrings.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SendClanInvite(string User, string Clan)
        {
            var user = await uow.userRepository.FindAsync(x => x.UserName == User);
            var clan = await uow.clanRepository.FindAsync(x => x.ID == Clan);
            if (user != null)
            {
                var ci = new ClanInvitation();
                ci.ClanID = Clan;
                ci.Clan = clan;
                ci.UserID = user.Id;
                ci.User = user;
                await uow.clanInvitationRepository.AddAsync(ci);
                return RedirectToAction("Manage", new { Message = ManageMessageId.ClanInviteSuccess });
            }

            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AcceptClanInvite(string ClanInvitationID)
        {
            try
            {
                var clanInvitation = await uow.clanInvitationRepository.FindAsync(x => x.ID == ClanInvitationID);
                var user = await uow.userRepository.FindAsync(x => x.Id == clanInvitation.UserID);
                var clan = await uow.clanRepository.FindAsync(x => x.ID == clanInvitation.ClanID);
                user.Clan = clan;
                clan.Users.Add(user);
                await uow.userRepository.UpdateAsync(user);
                await uow.clanRepository.UpdateAsync(clan);
                await uow.clanInvitationRepository.RemoveAsync(clanInvitation);
                return RedirectToAction("Manage", new { Message = ManageMessageId.ClanInviteAccepted });
            }
            catch
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
        }
        public async Task<ActionResult> DeclineClanInvite(string ClanInvitationID)
        {
            try
            {
                var clanInvitation = await uow.clanInvitationRepository.FindAsync(x => x.ID == ClanInvitationID);
                await uow.clanInvitationRepository.RemoveAsync(clanInvitation);
                return RedirectToAction("Manage", new { Message = ManageMessageId.ClanInviteDeclined });
            }
            catch
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
        }

        public async Task<ActionResult> KickClanMember(string clanId, string userId)
        {
            try
            {
                var user = await uow.userRepository.FindAsync(x => x.Id == userId);
                var clan = await uow.clanRepository.FindAsync(x => x.ID == clanId);
                clan.Users.Remove(user);
                user.ClanID = null;
                user.Clan = null;
                await uow.clanRepository.UpdateAsync(clan);
                await uow.userRepository.UpdateAsync(user);
                return RedirectToAction("Manage", new { Message = ManageMessageId.ClanMemberKicked });
            }
            catch
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }

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

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = _userManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            UpdateProfileSuccess,
            AddNewClanSuccess,
            ClanInviteSuccess,
            ClanInviteAccepted,
            ClanInviteDeclined,
            ClanMemberKicked,
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