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
        [HttpPost]
        public async Task<String> PlaceUp(string id)
        {
            try
            {
                var page = await uow.pageRepository.FindAsync(x => x.ID == id);
                var oldSequence = page.Place;
                var page1 = await uow.pageRepository.FindAsync(x => x.Place == (oldSequence - 1));

                page1.Place = oldSequence;
                page.Place = (oldSequence - 1);
                await uow.pageRepository.UpdateAsync(page);
                await uow.pageRepository.UpdateAsync(page1);
                await uow.SaveAsync();
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
        [HttpPost]
        public async Task<String> PlaceDown(string id)
        {
            try
            {
                var page = await uow.pageRepository.FindAsync(x => x.ID == id);
                var oldSequence = page.Place;
                var page1 = await uow.pageRepository.FindAsync(x => x.Place == (oldSequence + 1));

                page1.Place = oldSequence;
                page.Place = (oldSequence + 1);
                await uow.pageRepository.UpdateAsync(page);
                await uow.pageRepository.UpdateAsync(page1);
                await uow.SaveAsync();
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
            return PartialView("_Edit", new Page { News = new List<News>() });
        }

        public async Task<ActionResult> Edit(string id)
        {
            var page = await uow.pageRepository.FindAsync(x => x.ID == id);
            return PartialView("_Edit", page);
        }

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
            var page = await uow.pageRepository.FindAsync(x => x.ID == id);
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
                await uow.pageRepository.RemoveAsync(page);
                var pages = await uow.pageRepository.GetAllAsync();
                int i = 1;
                foreach (var item in pages.OrderBy(x => x.Place))
                {
                    item.Place = i;
                    await uow.pageRepository.UpdateAsync(item);
                    i++;
                }
                //uow.pageRepository.Delete(page.ID);
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
            if (ModelState.IsValid)
            {
                DateTime t = new DateTime();
                DateTime.TryParse("1/01/0001 0:00:00", out t);
                var count = await uow.pageRepository.CountAsync();

                if (page.isNew)
                {
                    page.isNew = false;
                    page.Guid = Guid.NewGuid();
                    page.Place = (count + 1);
                    if (page.isNews)
                    {
                        foreach (var item in page.News)
                        {
                            //DateTime t = new DateTime();
                            //DateTime.TryParse("1/01/0001 0:00:00", out t);
                            if (item.Date == t)
                            {
                                item.Date = DateTime.UtcNow;
                            }
                        }
                    }
                    await uow.pageRepository.AddAsync(page);
                    //uow.pageRepository.Insert(page);
                }
                else
                {
                    if (page.isNews)
                    {
                        foreach (var item in page.News)
                        {
                            //DateTime t = new DateTime();
                            //DateTime.TryParse("1/01/0001 0:00:00", out t);
                            if (item.Date == t)
                            {
                                item.Date = DateTime.UtcNow;
                            }
                            item.PageID = page.ID;
                            if (String.IsNullOrEmpty(item.ID))
                            {
                                await uow.newsRepository.AddAsync(item);
                            }
                            else
                            {
                                await uow.newsRepository.UpdateAsync(item);
                            }
                        }
                    }
                    await uow.pageRepository.UpdateAsync(page);
                    //uow.pageRepository.Update(page);
                }
                await uow.SaveAsync();
                return "Success";
            }
            else
            {
                return "Error";
            }
        }
    }
}