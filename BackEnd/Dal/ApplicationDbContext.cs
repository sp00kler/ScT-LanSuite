using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Data.Entity;
using System;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Models;

namespace Dal
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        new public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }
        new public void SaveChanges()
        {
            base.SaveChanges();
        }
        new public void Dispose()
        {
            base.Dispose();
        }
        public DbSet<Page> Page { get; set; }
        public DbSet<Clan> Clan { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Compo> Compo { get; set; }
        public DbSet<Edition> Edition { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Registration> Registration { get; set; }
        public DbSet<Team> Team { get; set; }
        public DbSet<ClanInvitation> ClanInvitation { get; set; }
        public DbSet<Seating> Seating { get; set; }
        public DbSet<ClanSeating> ClanSeating { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
           // base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Comments>().HasRequired(x => x.News).WithMany(x => x.Comments).WillCascadeOnDelete();
            modelBuilder.Entity<News>().HasRequired(x => x.Page).WithMany(x => x.News).WillCascadeOnDelete();
         }
    }

    public class IdentityManager
    {
        // Swap ApplicationRole for IdentityRole:
        RoleManager<ApplicationRole> _roleManager = new RoleManager<ApplicationRole>(
            new RoleStore<ApplicationRole>(new ApplicationDbContext()));

        UserManager<ApplicationUser> _userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(new ApplicationDbContext()));

        ApplicationDbContext _db = new ApplicationDbContext();


        public bool RoleExists(string name)
        {
            return _roleManager.RoleExists(name);
        }


        public bool CreateRole(string name, string description = "")
        {
            // Swap ApplicationRole for IdentityRole:
            var idResult = _roleManager.Create(new ApplicationRole(name, description));
            return idResult.Succeeded;
        }


        public bool CreateUser(ApplicationUser user, string password)
        {
            var idResult = _userManager.Create(user, password);
            return idResult.Succeeded;
        }


        public bool AddUserToRole(string userId, string roleName)
        {
            var idResult = _userManager.AddToRole(userId, roleName);
            return idResult.Succeeded;
        }


        public void ClearUserRoles(string userId)
        {
            var user = _userManager.FindById(userId);
            var currentRoles = new List<IdentityUserRole>();

            currentRoles.AddRange(user.Roles);
            foreach (var role in currentRoles)
            {
                string roleName = _roleManager.FindById(role.RoleId).Name;
                _userManager.RemoveFromRole(userId, roleName);
            }
        }
    }
}
