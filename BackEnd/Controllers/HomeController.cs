﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using System.Threading.Tasks;
using Dal;
using System.Data.Entity.Validation;
using ViewModels;

namespace ScT_LanSuite.Controllers
{
    public class HomeController : ScTController
    {

        /// <summary>
        /// Gets Cms page content
        /// </summary>
        /// <param name="id">Page ID</param>
        /// <returns>Page Content</returns>
        public async Task<ActionResult> Index(string id)
        {
            if (await uow.userRepository.CountAsync() == 0)
            {
                AddUserAndRoles();
            }
            ViewBag.isNews = false;
            if (!string.IsNullOrEmpty(id))
            {
                //show page....
                var p = await uow.pageRepository.FindAsync(x => x.ID == id);
                ViewBag.isNews = p.isNews;
                if (!p.isNews)
                {
                    ViewBag.Content = p.Content;
                }
            }
            else
            {
                try
                {
                    var p = await uow.pageRepository.FirstAsync(x => x.Place == 1 && x.isActivated);
                    ViewBag.Content = p.Content;
                    ViewBag.isNews = p.isNews;
                }
                catch
                {
                    ViewBag.Content = "Congratulations you have successfully installed ScT Lansuite.";
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                var user = await ChatHelper.GetChatUserAsync(Request, User.Identity.Name);
                if (!ChatHub.IsUserRegistered(user))
                {
                    user.Status = ChatJs.Net.ChatUser.StatusType.Online;
                    ChatHub.RegisterNewUser(user);
                    //ctx.Controller.TempData.Add("ChatUser", await ChatHelper.GetChatUserAsync(this.Request, User.Identity.Name));
                    Session["ChatUser"] = await ChatHelper.GetChatUserAsync(this.Request, User.Identity.Name);
                }
            }

            return View();
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

        public async Task<ActionResult> LoadEdition()
        {
            
            var svm = (from e in await uow.editionRepository.GetAllAsync()
                                 join r in await uow.registrationRepository.GetAllAsync() on e.ID equals r.EditionID
                                 where e.isActivated
                                 select new StatsViewModel { Edition = (e != null ? e : new Edition()), hasPaid = (r != null && r.Paid ? true : false), isParticipating = (r != null ? true : false) }).SingleOrDefault();

            if (svm == null)
            {
                var edition = await uow.editionRepository.FindAsync(x => x.isActivated);
                svm = new StatsViewModel { Edition = edition, hasPaid = false, isParticipating = false };
            }

            return PartialView("_Stats", svm);
        }
        public async Task<ActionResult> GetSeating(string id)
        {
            var seating = await uow.seatingRepository.FindAsync(x => x.ID == id);
            return PartialView("_Seating", seating);
        }
        [HttpGet]
        public ActionResult ParticipateLan()
        {
            return PartialView("_ParticipateLan");
        }
        [HttpPost]
        public async Task<string> ParticipateLan(string PaymentMethod)
        {
            var user = await uow.userRepository.FindAsync(x => x.UserName == User.Identity.Name);
            var edition = await uow.editionRepository.FindAsync(x => x.isActivated);
            var registration = new Registration();
            registration.Paid = false;
            registration.UserID = user.Id;
            registration.EditionID = edition.ID;
            await uow.registrationRepository.AddAsync(registration);
            return PaymentMethod;
        }

        [HttpGet]
        public ActionResult PayLan()
        {
            return PartialView("_PayLan");
        }
        [HttpPost]
        public string PayLan(string PaymentMethod)
        {
            return PaymentMethod;
        }
        public ActionResult SetCulture(string culture)
        {
            // Validate input
            culture = CultureHelper.GetImplementedCulture(culture);
            // Save culture in a cookie
            HttpCookie cookie = Request.Cookies["_culture"];
            if (cookie != null)
                cookie.Value = culture;   // update cookie value
            else
            {
                cookie = new HttpCookie("_culture");
                cookie.Value = culture;
                cookie.Expires = DateTime.Now.AddYears(1);
            }
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index");
        }             


        /// <summary>
        /// Gets news for specific page
        /// </summary>
        /// <param name="id">Page ID</param>
        /// <returns>all news for specified page</returns>
        public async Task<ActionResult> GetNewsFor(string id)
        {
            var page = await uow.pageRepository.FindAsync(x => x.Place == 1);
            if (!String.IsNullOrEmpty(id))
            {
                page = await uow.pageRepository.FindAsync(x => x.ID == id);
            }
            var newsList = await uow.newsRepository.FindAllAsync(x => x.PageID == page.ID);
            var newsvmList = new List<NewsViewModel>();
            foreach (var item in newsList.OrderByDescending(x => x.Place))
            {
                string DateShown = "";
                TimeSpan t = DateTime.UtcNow.Subtract(item.Date);
                if (t.Days == 0)
                {
                    if (t.Hours == 0)
                    {
                        DateShown = t.Minutes + " minutes ago.";
                    }
                    else
                    {
                        DateShown = t.Hours + " houres ago.";
                    }
                }
                else
                {
                    DateShown = item.Date.ToLocalTime().ToShortDateString() + " " + item.Date.ToLocalTime().ToShortTimeString();
                }
                var newsvm = new NewsViewModel();
                newsvm.News = item;
                newsvm.DateShown = DateShown;
                newsvm.Comments = new List<NewsCommentViewModel>();
                foreach (var comment in item.Comments)
                {
                    string DateShownComment = "";
                    TimeSpan t1 = DateTime.UtcNow.Subtract(comment.Date);
                    if (t1.Days == 0)
                    {
                        if (t1.Hours == 0)
                        {
                            DateShownComment = t1.Minutes + " minutes ago.";
                        }
                        else
                        {
                            DateShownComment = t1.Hours + " houres ago.";
                        }
                    }
                    else
                    {
                        DateShownComment = comment.Date.ToLocalTime().ToShortDateString() + " " + comment.Date.ToLocalTime().ToShortTimeString();
                    }

                    var newscommentvm = new NewsCommentViewModel();
                    newscommentvm.Comment = comment;
                    newscommentvm.DateShown = DateShownComment;
                    newscommentvm.Date = comment.Date;
                    newscommentvm.Email = (await uow.userRepository.FindAsync(x => x.UserName == comment.UserName)).Email;
                    newsvm.Comments.Add(newscommentvm);
                }
                newsvmList.Add(newsvm);
            }

            return PartialView("_News", newsvmList);
        }

        /// <summary>
        /// Reload Menu
        /// </summary>
        /// <returns>Updated Menu</returns>
        public async Task<ActionResult> ReloadMenu()
        {
            var pages = await uow.pageRepository.FindAllAsync(x => x.isActivated);
            return PartialView("_Menu", pages.OrderBy(x => x.Place));
        }


        /// <summary>
        /// Add comment to news 
        /// </summary>
        /// <param name="Comment">Comment text</param>
        /// <returns>Success or Error</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<string> AddComment(Comments Comment)
        {
            try
            {
                var news = await uow.newsRepository.FindAsync(x => x.ID == Comment.NewsID);
                Comment.UserName = User.Identity.Name;
                Comment.Date = DateTime.UtcNow;
                Comment.News = news;
                news.Comments.Add(Comment);
                await uow.pageRepository.UpdateAsync(news.Page);
                return "Success";
            }
            catch
            {
                return "Error";
            }

        }
    }
}