using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;
using System.Threading.Tasks;

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
        public ActionResult Create()
        {
            return PartialView("_Edit", new Page { isNew = true ,News = new List<News>() });
        }

        /// <summary>
        /// Loads Edit Partial
        /// </summary>
        /// <param name="id">Page ID</param>
        /// <returns>Edit Partial</returns>
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
        [ValidateInput(false)]
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
            catch
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
        [ValidateInput(false)]
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
                    if (page.News != null)
                    {
                        foreach (var item in page.News)
                        {
                            item.PageID = page.ID;
                            if (item.Date == t)
                            {
                                item.Date = DateTime.UtcNow;
                                await uow.newsRepository.AddAsync(item);
                            }
                            else
                            {
                                await uow.newsRepository.UpdateAsync(item);
                            }
                        }
                    }
                    await uow.pageRepository.UpdateAsync(page);
                }
                return "Success";
            }
            catch
            {
                return "Error";
            }
        }

        /// <summary>
        /// Change Page state
        /// </summary>
        /// <param name="pageID">Page ID</param>
        /// <returns>Success or Error</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
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