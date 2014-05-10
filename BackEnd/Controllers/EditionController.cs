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
using ScT_LanSuite;
using System.Threading.Tasks;

namespace ScT_LanSuite.Controllers
{
    [Authorize(Roles = "Admin, Crew")]
    public class EditionController : ScTController
    {
        public async Task<ActionResult> Index()
        {
            var editionList = await uow.editionRepository.GetAllAsync();
            return View(editionList.OrderByDescending(x => x.Place));
        }

        [RestrictToAjax]
        public ActionResult Create()
        {
            var e = new Edition();
            e.isNew = true;
            e.Seating = new Seating() { ID = e.ID };
            return PartialView("_Edit", e);
        }

        [RestrictToAjax]
        public async Task<ActionResult> Edit(string id)
        {
            var edition = await uow.editionRepository.FindAsync(x => x.ID == id);
            return PartialView("_Edit", edition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RestrictToAjax]
        public async Task<String> CreateOrUpdate(Edition edition)
        {
            //edition.Seating.Edition = edition;
            var count = await uow.editionRepository.CountAsync();
            try
            {
                if (edition.isNew)
                {
                    edition.Place = count + 1;
                    await uow.editionRepository.AddAsync(edition);
                }
                else
                {
                    edition.Seating.Edition = edition;
                    edition.Seating.ID = edition.ID;
                    await uow.seatingRepository.UpdateAsync(edition.Seating);
                    await uow.editionRepository.UpdateAsync(edition);

                }
                return "Success";
            }
            catch
            {
                return "Error";
            }
        }
        [RestrictToAjax]
        public async Task<ActionResult> Delete(string id)
        {
            Edition edition = await uow.editionRepository.FindAsync(x => x.ID == id);
            return PartialView("_Delete", edition);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<String> ConfirmDelete(Edition edition)
        {
            try
            {
                edition = await uow.editionRepository.FindAsync(x => x.ID == edition.ID);
                await uow.seatingRepository.RemoveAsync(edition.Seating);
                await uow.editionRepository.RemoveAsync(edition);
                var editions = await uow.editionRepository.GetAllAsync();
                FixPlaces(editions);
                return "Success";
            }
            catch
            {
                return "Error";
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RestrictToAjax]
        public async Task<string> ChangeActiveState(string editionID)
        {
            try
            {
                var activeEdition = await uow.editionRepository.FindAsync(x => x.isActivated);
                var edition = await uow.editionRepository.FindAsync(x => x.ID == editionID);
                edition.isActivated = !edition.isActivated;
                if (await uow.editionRepository.CountAsync() != 1)
                {
                    activeEdition.isActivated = !activeEdition.isActivated;
                }

                await uow.editionRepository.UpdateAsync(edition);
                await uow.editionRepository.UpdateAsync(activeEdition);
                return "Success";
            }
            catch
            {
                return "Error";
            }
        }

        public async void FixPlaces(List<Edition> editions)
        {
            int i = 1;
            foreach (var item in editions.OrderBy(x => x.Place))
            {
                item.Place = i;
                await uow.editionRepository.UpdateAsync(item);
                i++;
            }
        }

        [RestrictToAjax]
        public async Task<ActionResult> ReloadEditionTable()
        {
            var editionList = await uow.editionRepository.GetAllAsync();
            return PartialView("_EditionTable", editionList.OrderByDescending(x => x.Place));
        }
    }
}