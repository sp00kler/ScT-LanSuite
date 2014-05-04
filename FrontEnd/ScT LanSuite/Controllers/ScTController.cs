using Dal;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UnityFramework;

namespace ScT_LanSuite.Controllers
{
    public class ScTController : Controller
    {
        public UnitOfWork uow = new UnitOfWork();
        public RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(new RoleStore<ApplicationRole>(new ApplicationDbContext()));
        //public UserManager<ApplicationUser> _userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        public UserManager<ApplicationUser> _userManager = new UserManager<ApplicationUser>(new myUserStore());
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null && _roleManager != null && uow != null)
            {
                uow.Dispose();
                _userManager.Dispose();
                _roleManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}