using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using System.Threading.Tasks;
using System.Data.Entity.Validation;

namespace ScT_LanSuite.Controllers
{
    [Authorize(Roles = "Admin, Crew")]
    public class CmsController : ScTController
    {
        //
        // GET: /Cms/

        public async Task<ActionResult> Index()
        {
            var pageList = await uow.pageRepository.GetAllAsync();
            if (User.IsInRole("Crew") && !User.IsInRole("Admin"))
            {
                pageList = await uow.pageRepository.FindAllAsync(x => x.isNews);
            }
            return View(pageList.OrderBy(x => x.Place));
        }

        /// <summary>
        /// Reload Cms Table
        /// </summary>
        /// <returns>Table with list</returns>
        public async Task<ActionResult> ReloadCmsTable()
        {
            var pageList = await uow.pageRepository.GetAllAsync();
            if (User.IsInRole("Crew") && !User.IsInRole("Admin"))
            {
                pageList = await uow.pageRepository.FindAllAsync(x => x.isNews);
            }
            return PartialView("_CmsTable", pageList.OrderBy(x => x.Place));
        }


        /// <summary>
        /// Place up
        /// </summary>
        /// <param name="id">id of selected page</param>
        /// <returns>success or error</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<String> PlaceUp(string PageID)
        {
            try
            {
                var page = await uow.pageRepository.FindAsync(x => x.ID == PageID);
                var oldSequence = page.Place;
                var page1 = await uow.pageRepository.FindAsync(x => x.Place == (oldSequence - 1));

                page1.Place = oldSequence;
                page.Place = (oldSequence - 1);
                await uow.pageRepository.UpdateAsync(page);
                await uow.pageRepository.UpdateAsync(page1);
                return "Success";
            }
            catch
            {
                return "Error";
            }

        }

        /// <summary>
        /// Place down
        /// </summary>
        /// <param name="id">id of selected page</param>
        /// <returns>success or error</returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<String> PlaceDown(string PageID)
        {
            try
            {
                var page = await uow.pageRepository.FindAsync(x => x.ID == PageID);
                var oldSequence = page.Place;
                var page1 = await uow.pageRepository.FindAsync(x => x.Place == (oldSequence + 1));

                page1.Place = oldSequence;
                page.Place = (oldSequence + 1);
                await uow.pageRepository.UpdateAsync(page);
                await uow.pageRepository.UpdateAsync(page1);
                return "Success";
            }
            catch
            {
                return "Error";
            }
        }


        /// <summary>
        /// Loads Create Partial
        /// </summary>
        /// <returns>Create partial</returns>
        [RestrictToAjax]
        public ActionResult Create()
        {
            return PartialView("_Edit", new Page { isNew = true, News = new List<News>() });
        }

        /// <summary>
        /// Loads Edit Partial
        /// </summary>
        /// <param name="id">Page ID</param>
        /// <returns>Edit Partial</returns>
        [RestrictToAjax]
        public async Task<ActionResult> Edit(string id)
        {
            var page = await uow.pageRepository.FindAsync(x => x.ID == id);
            return PartialView("_Edit", page);
        }

        /// <summary>
        /// Add News to page
        /// </summary>
        /// <param name="page">Page Object</param>
        /// <returns>Updated Page Object</returns>
        [RestrictToAjax]
        public ActionResult AddNews(Page page)
        {
            if (page.News == null)
            {
                page.News = new List<News>();
            }
            var news = new News();
            news.Place = (page.News.Count + 1);
            if (!String.IsNullOrEmpty(page.ID))
            {
                news.PageID = page.ID;
                // news.Page = page;
            }
            page.News.Add(news);

            return PartialView("_Edit", page);

        }

        /// <summary>
        /// Loads News Partial
        /// </summary>
        /// <param name="id">identifier of page</param>
        /// <returns>partial with all the news in page.</returns>
        [RestrictToAjax]
        public async Task<ActionResult> News(string id)
        {
            var page = await uow.pageRepository.FindAsync(x => x.ID == id);
            return PartialView("_News", page);
        }

        /// <summary>
        /// Details
        /// </summary>
        /// <param name="id">id of page</param>
        /// <returns>Details partial</returns>
        [RestrictToAjax]
        public async Task<ActionResult> Details(string id)
        {
            var page = await uow.pageRepository.FindAsync(x => x.ID == id);
            return PartialView("_Details", page);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id">id of page</param>
        /// <returns>Delete partial</returns>
        [RestrictToAjax]
        public async Task<ActionResult> Delete(string id)
        {
            Page page = await uow.pageRepository.FindAsync(x => x.ID == id);
            return PartialView("_Delete", page);
        }

        /// <summary>
        /// Confirm Delete
        /// </summary>
        /// <param name="id">id of page</param>
        /// <returns>Success or Error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<String> ConfirmDelete(Page page)
        {
            try
            {
                page = await uow.pageRepository.FindAsync(x => x.ID == page.ID);
                var newsList = new List<News>();
                var commentsList = new List<Comments>();
                newsList.AddRange(page.News);

                //foreach (var news in newsList)
                //{
                //    commentsList.AddRange(news.Comments);
                //    foreach (var comment in commentsList)
                //    {
                //        await uow.commentsRepository.RemoveAsync(comment);
                //    }
                //    await uow.newsRepository.RemoveAsync(news);
                //    commentsList.Clear();
                //}

                await uow.pageRepository.RemoveAsync(page);
                var pages = await uow.pageRepository.GetAllAsync();
                int i = 1;
                foreach (var item in pages.OrderBy(x => x.Place))
                {
                    item.Place = i;
                    await uow.pageRepository.UpdateAsync(item);
                    i++;
                }
                return "Success";
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return "Error";
            }
            catch (Exception e)
            {
                return "Error";
            }
        }

        /// <summary>
        /// Create page
        /// </summary>
        /// <param name="page">Page Object</param>
        /// <returns>List of pages in view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RestrictToAjax]
        public async Task<String> CreateOrUpdate(Page page)
        {
            try
            {
                DateTime t = new DateTime();
                DateTime.TryParse("1/01/0001 0:00:00", out t);
                var count = await uow.pageRepository.CountAsync();

                if (page.isNew)
                {
                    page.isNew = false;
                    page.Place = (count + 1);
                    if (page.isNews)
                    {
                        foreach (var item in page.News)
                        {
                            item.PageID = page.ID;
                            item.Page = page;
                            if (item.Date == t)
                            {
                                item.Date = DateTime.UtcNow;
                            }
                        }
                    }
                    await uow.pageRepository.AddAsync(page);
                }
                else
                {
                    var news = page.News;
                    var myPage = await uow.pageRepository.FindAsync(x => x.ID == page.ID);
                    if (page.News != null)
                    {
                        for (int i = 0; i < myPage.News.Count; i++)
                        {
                            myPage.News[i].Title = news[i].Title;
                            myPage.News[i].Content = news[i].Content;
                        }
                        for (int i = myPage.News.Count; i < news.Count; i++)
                        {
                            news[i].PageID = myPage.ID;
                            news[i].Date = DateTime.UtcNow;
                            myPage.News.Add(news[i]);
                        }
                    }
                    await uow.pageRepository.UpdateAsync(myPage);
                }
                return "Success";
            }
            catch (Exception e)
            {
                return "Error" + e.InnerException.Message;
            }
        }

        /// <summary>
        /// Change Page state
        /// </summary>
        /// <param name="pageID">Page ID</param>
        /// <returns>Success or Error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RestrictToAjax]
        public async Task<string> ChangeActiveState(string pageID)
        {
            try
            {
                var page = await uow.pageRepository.FindAsync(x => x.ID == pageID);
                page.isActivated = !page.isActivated;
                await uow.pageRepository.UpdateAsync(page);
                return "Success";
            }
            catch
            {
                return "Error";
            }
        }
    }
}