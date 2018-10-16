using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Common;
using DIBZ.Logic.Offer;
using DIBZ.Base;
using DIBZ.Logic;

namespace DIBZ.Controllers
{
    public class SearchController : BaseWebController
    {
        // GET: Search
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Search()
        {
            ViewData["Error"] = TempData["Error"];
            LogHelper.LogInfo("Fetching dashboard data");

            var offerlogic = LogicContext.Create<OfferLogic>();

            if (TempData["searchOfferData"] != null)
            {
                ViewBag.Offers = TempData["searchOfferData"];
                var count = TempData["searchOfferCount"];
                var currentPage = TempData["currentPage"];
                var Pages = offerlogic.PagingLogic(Convert.ToInt32(currentPage), 4, Convert.ToInt32(count));
                ViewBag.Pages = Pages;
                ViewBag.GameName = TempData["gameName"];
                ViewBag.FormatId = TempData["formatId"];
                ViewBag.CategoryId = TempData["categoryId"];
                ViewBag.CurrentPage = (int)currentPage == -1 ? 1 : currentPage;
                TempData["searchOfferData"] = null;
                TempData["currentPage"] = null;
            }
            else
            {
                var offerLogic = LogicContext.Create<OfferLogic>();
                ViewBag.Offers = await offerLogic.GetAllOfferForDashboard(0);
            }

            var selectedFormat = TempData["formatId"];
            var selectedCategory = TempData["categoryId"];

            var formatLogic = LogicContext.Create<FormatLogic>();
            var format = await formatLogic.GetAllFormats();
            ViewBag.Formats = new SelectList(format, "Id", "Name", selectedFormat);

            var categoryLogic = LogicContext.Create<CategoryLogic>();
            var categories = await categoryLogic.GetAllCategories();
            ViewBag.Categories = new SelectList(categories, "Id", "Name", selectedCategory);

            return View();
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async  Task<ActionResult> SearchOffer(int page, int? formatId, string gameName,int? categoryId, FormCollection formData)
        {
            SearchOffer search = new SearchOffer();
            
            if (formData.Count == 0)
            {
                if(gameName == "" || gameName == null)
                {
                    search.GameName = "";
                }
                else
                {
                    search.GameName = gameName;
                }
                search.FormatId = Convert.ToInt32(formatId);
                search.CategoryId = Convert.ToInt32(categoryId);
            }
            else
            {
                search.GameName = formData["gameName"];
                search.FormatId = ConversionHelper.SafeConvertToInt32(formData["formatId"]);
                search.CategoryId = ConversionHelper.SafeConvertToInt32(formData["categoryId"]);
            }

            var offerLogic = LogicContext.Create<OfferLogic>();

            //get all GamesCatalog
            var searchOfferData =await offerLogic.SearchOffers(search, page);
            var searchOfferCount =await offerLogic.GetSearchOffersTotalCount(search);
            TempData["searchOfferCount"] = searchOfferCount;
            TempData["searchOfferData"] = searchOfferData;
            TempData["currentPage"] = page - 1;
            TempData["gameName"] = search.GameName;
            TempData["formatId"] = search.FormatId;
            TempData["categoryId"] = search.CategoryId;
            return RedirectToAction("Search", "Search");
        }
    }
}