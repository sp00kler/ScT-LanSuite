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

namespace ScT_LanSuite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : ScTController
    {
        public async Task<ActionResult> Index()
        {
            var user1 = User.IsInRole("Admin");
            var users = await uow.userRepository.GetAllAsync();
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new EditUserViewModel(user);
                model.Add(u);
            }
            return View(model);
        }
        public async Task<ActionResult> ReloadUsersTable()
        {
            var users = await uow.userRepository.GetAllAsync();
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new EditUserViewModel(user);
                model.Add(u);
            }
            return PartialView("_UsersTable", model);
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = model.GetUser();
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Users");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public async Task<ActionResult> Edit(string id, ManageMessageId? Message = null)
        {
            var user = await uow.userRepository.FindAsync(x => x.UserName == id);
            var model = new EditUserViewModel(user);
            ViewBag.MessageId = Message;
            return PartialView("_Edit",model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<String> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await uow.userRepository.FindAsync(x => x.UserName == model.UserName);
                // Update the user data:
                user.FullName = model.FullName;
                user.Email = model.Email;
                await uow.userRepository.UpdateAsync(user);
                return "Success";
            }
            // If we got this far, something failed, redisplay form
            return "Fail";
        }

        public async Task<ActionResult> Delete(string id = null)
        {
            var user = await uow.userRepository.FindAsync(x => x.UserName == id);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return PartialView("_Delete", model);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<String> DeleteConfirmed(ApplicationUser uName)
        {
            var user = await uow.userRepository.FindAsync(u => u.UserName == uName.UserName);
            await uow.userRepository.RemoveAsync(user);
            //Db.Users.Remove(user);
            //await Db.SaveChangesAsync();
            return "Success";
        }

        public async Task<ActionResult> UserRoles(string id)
        {
            var user = await uow.userRepository.FindAsync(x => x.UserName == id);
            var model = new SelectUserRolesViewModel(user);
            return PartialView("_UserRoles", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserRoles(SelectUserRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var user = await uow.userRepository.FindAsync(x => x.UserName == model.UserName);
                idManager.ClearUserRoles(user.Id);
                //user.Roles.Clear();
                //await uow.userRepository.UpdateAsync(user);
                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        //await _userManager.AddToRoleAsync(user.Id, role.RoleName);
                        idManager.AddUserToRole(user.Id, role.RoleName);
                    }
                }
                return RedirectToAction("index");
            }
            return View();
        }
        protected override void Dispose(bool disposing)
        {
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