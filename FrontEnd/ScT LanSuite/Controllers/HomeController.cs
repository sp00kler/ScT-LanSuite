using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using System.Threading.Tasks;

namespace ScT_LanSuite.Controllers
{
    public class HomeController : ScTController
    {
        public async Task<ActionResult> Index(string id)
        {
            ViewBag.isNews = false;
            if (!string.IsNullOrEmpty(id))
            {
                Guid page = new Guid(id);
                //show page....
                var p = await uow.pageRepository.FindAsync(x => x.Guid == page);
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
                    var p = await uow.pageRepository.FirstAsync(x => x.Place == 1);
                    ViewBag.Content = p.Content;
                    ViewBag.isNews = p.isNews;
                }
                catch
                {
                }
            }
            var activeEdition = await uow.editionRepository.FindAsync(x => x.Active);
            if (activeEdition == null)
            {
                var e = new Edition();
                e.Active = true;
                e.Seats = 300;
                e.Title = "2015";
                await uow.editionRepository.AddAsync(e);
               // await uow.SaveAsync();
            }

            return View();
        }

        public async Task<ActionResult> GetNewsFor(string id)
        {
            Guid pageId = new Guid();
            var page = await uow.pageRepository.FindAsync(x => x.Place == 1);
            //int pageID = uow.pageRepository.Get(x => x.Place == 1).Single().ID;
            if (!String.IsNullOrEmpty(id))
            {
                pageId = new Guid(id);
                page = await uow.pageRepository.FindAsync(x => x.Guid == pageId);
                //pageID = uow.pageRepository.Get(x => x.Guid == pageId).Single().ID;
            }
            var newsList = await uow.newsRepository.FindAllAsync(x => x.PageID == page.ID);// uow.newsRepository.Get(x => x.PageID == pageID).OrderByDescending(x => x.Place);
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
                    newscommentvm.Email = (await uow.userRepository.FindAsync(x => x.UserName == comment.UserName)).Email;
                    newsvm.Comments.Add(newscommentvm);
                }
                newsvmList.Add(newsvm);
            }

            return PartialView("_News", newsvmList);
        }

        public async Task<ActionResult> ReloadMenu()
        {
            var pages = await uow.pageRepository.GetAllAsync();
            return PartialView("_Menu", pages.OrderBy(x => x.Place));
        }

        [Authorize]
        [HttpPost]
        public async Task<string> AddComment(Comments Comment)
        {
            Comment.UserName = User.Identity.Name;
            Comment.Date = DateTime.UtcNow;
            await uow.commentsRepository.AddAsync(Comment);
            //await uow.SaveAsync();
            //uow.commentsRepository.Insert(Comment);
            //uow.Save();
            return "SUCCESS";
        }
    }
}