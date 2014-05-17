namespace Dal.Migrations
{
    using Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Dal.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            this.AddUserAndRoles();
            this.AddSettings();
        }

        bool AddSettings() 
        {
            bool success = false;

            var sManager = new SettingsManager();

            success = sManager.insertSetting("PayPal_Business", "tech-facilitator@sp00kler.be");
            if (!success == true) return success;

            success = sManager.insertSetting("PayPal_CancelReturn", "http://sct-lansuite.apphb.com/PayPal/CancelFromPaypal");
            if (!success == true) return success;

            success = sManager.insertSetting("PayPal_Return", "http://sct-lansuite.apphb.com/PayPal/RedirectFromPaypal");
            if (!success == true) return success;

            success = sManager.insertSetting("PayPal_ActionUrl", "https://www.sandbox.paypal.com/cgi-bin/webscr");
            if (!success == true) return success;

            success = sManager.insertSetting("PayPal_NotifyUrl", "http://sct-lansuite.apphb.com/PayPal/NotifyFromPaypal");
            if (!success == true) return success;

            success = sManager.insertSetting("PayPal_CurrencyCode", "EUR");
            if (!success == true) return success;

            success = sManager.insertSetting("Registration_Price", "1");
            if (!success == true) return success;
          

            return success;
        }
        bool AddUserAndRoles()
        {
            bool success = false;

            var idManager = new IdentityManager();
            // Add the Description as an argument:
            success = idManager.CreateRole("Admin", "Global Access");
            if (!success == true) return success;

            // Add the Description as an argument:
            success = idManager.CreateRole("Crew", "Can post news and moderate their compo's");
            if (!success == true) return success;

            // Add the Description as an argument:
            success = idManager.CreateRole("ForumMod", "Forum Moderator");
            if (!success) return success;


            var newUser = new ApplicationUser()
            {
                UserName = "administrator",
                FullName = "The Big Bad Administrator",
                Email = "admin@sp00kler.be"
            };

            // Be careful here - you  will need to use a password which will 
            // be valid under the password rules for the application, 
            // or the process will abort:
            success = idManager.CreateUser(newUser, "administrator");
            if (!success) return success;

            success = idManager.AddUserToRole(newUser.Id, "Admin");
            if (!success) return success;

            success = idManager.AddUserToRole(newUser.Id, "Crew");
            if (!success) return success;

            success = idManager.AddUserToRole(newUser.Id, "ForumMod");
            if (!success) return success;

            return success;
        }
    }
}
