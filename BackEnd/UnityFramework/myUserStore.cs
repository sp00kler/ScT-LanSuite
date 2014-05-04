using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace UnityFramework
{
    public class myUserStore : IUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserRoleStore<ApplicationUser>, IUserLoginStore<ApplicationUser>
    {
        private UnityFramework.UnitOfWork uow;
        public myUserStore()
        {
            this.uow = new UnityFramework.UnitOfWork();
        }
        public async Task CreateAsync(ApplicationUser user)
        {
            await uow.userRepository.AddAsync(user);
        }
        public async Task DeleteAsync(ApplicationUser user)
        {
            await uow.userRepository.RemoveAsync(user);
        }
        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            return (await uow.userRepository.FindAsync(x => x.Id == userId));
        }
        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            return (await uow.userRepository.FindAsync(x => x.UserName == userName));
        }
        public async Task UpdateAsync(ApplicationUser user)
        {
            await uow.userRepository.UpdateAsync(user);
        }
        public async Task<string> GetPasswordHashAsync(ApplicationUser user)
        {
            return (await uow.userRepository.FindAsync(x => x.UserName == user.UserName)).PasswordHash;
        }
        public async Task<bool> HasPasswordAsync(ApplicationUser user)
        {
            return !String.IsNullOrEmpty((await uow.userRepository.FindAsync(x => x.UserName == user.UserName)).PasswordHash);
        }
        public async Task SetPasswordHashAsync(ApplicationUser user, string passwordHash)
        {
            //user = await uow.userRepository.FindAsync(x => x.UserName == user.UserName);
            user.PasswordHash = passwordHash;
            //await uow.userRepository.UpdateAsync(user);
        }
        public async Task AddToRoleAsync(ApplicationUser user, string roleName)
        {
            var role = await uow.roleRepository.FindAsync(x => x.Name == roleName);
            user.Roles.Add(new IdentityUserRole(){ UserId = user.Id, RoleId = role.Id });
            await uow.userRepository.UpdateAsync(user);
        }
        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            List<string> roleNames = new List<string>();
            foreach (var item in await uow.roleRepository.GetAllAsync())
            {
                if (await IsInRoleAsync(user, item.Name))
                {
                    roleNames.Add(item.Name);
                }
            }
            return roleNames;
        }
        public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName)
        {
            var role = await uow.roleRepository.FindAsync(x => x.Name == roleName);
            foreach (var item in user.Roles)
            {
                if (item.RoleId == role.Id)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName)
        {
            var role = await uow.roleRepository.FindAsync(x => x.Name == roleName);
            var roleToRemove = user.Roles.Single(x => x.RoleId == role.Id);
            user.Roles.Remove(roleToRemove);
            await uow.userRepository.UpdateAsync(user);
        }
        public async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            user.Logins.Add(new IdentityUserLogin() { UserId = user.Id, LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey });
            await uow.userRepository.UpdateAsync(user);
        }
        public async Task<ApplicationUser> FindAsync(UserLoginInfo login)
        {
            var user = await uow.userRepository.FindAsync(x => x.Logins.Any(y => y.ProviderKey == login.ProviderKey));
            return user;
        }
        public async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user)
        {
            var uli = new List<UserLoginInfo>();
            var logins = await uow.loginRepository.FindAllAsync(x => x.UserId == user.Id);
            foreach (var item in logins)
            {
                uli.Add(new UserLoginInfo(item.LoginProvider, item.ProviderKey));
            }
            return uli;
        }
        public async Task RemoveLoginAsync(ApplicationUser user, UserLoginInfo login)
        {
            var realLogin = await uow.loginRepository.FindAsync(x => x.LoginProvider == login.LoginProvider && x.ProviderKey == login.ProviderKey);
            user.Logins.Remove(realLogin);
            await uow.userRepository.UpdateAsync(user);
        }
        public void Dispose()
        {
            uow.Dispose();
        }
    }
}