using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Common.DTO;
using DIBZ.Logic;
using DIBZ.Logic.GameCatalog;

namespace DIBZ.Controllers
{
    public class PriceChartController : BaseController
    {
        public async Task<ActionResult> Index()
        {

            //var formats = ViewBag.Formats as List<DIBZ.Common.Model.Format>;
           // var Categories = ViewBag.Categories as List<DIBZ.Common.Model.Category>;

            var gameFormatLogic = LogicContext.Create<FormatLogic>();
            var gameFormat = await gameFormatLogic.GetAllFormats();

            //var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();            
            //var gameCatalog = await gameCatalogLogic.GetAllGameCatalog();

            ViewBag.format = gameFormat;
           // ViewBag.gameCatelog = gameCatalog;

            return View();
        }

        public async Task<ActionResult> GetCategoryByFormat(Int32 formatId)
        {
            //var authLogic = LogicContext.Create<AuthLogic>();
            try
            {                
                //var gameFormatLogic = LogicContext.Create<FormatLogic>();
                //var gameFormat = await gameFormatLogic.GetAllFormats();

                var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
                var gameCatalog = await gameCatalogLogic.GetGameCatalogByFormat(formatId);

                return Json(new { IsSuccess = true, data = gameCatalog }, JsonRequestBehavior.AllowGet);
            }                                   
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, fail = ex.Message, data = "" }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// This Method Use for Update And Add Both Purpuse
        /// </summary>
        ///// <param name="Request"></param>
        /// <returns></returns>
        //[HttpPost]
        //public async Task<ActionResult> AddUpdate(GameCatalogModel Request)
        //{
        //    if (TryValidateModel(Request))
        //    {
        //        var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
        //        await gameCatalogLogic.AddUpdateGameCatalog(Request);
        //        return RedirectToAction("Index", "GameCatalog");
        //    }
        //    return View("AddUpdateGameCatalog", Request);
        //}

        /// <summary>
        /// Delete GameCatalog
        /// </summary>
       // /// <param name="Id"></param>
        /// <returns></returns>
        //[HttpPost]
        //public async Task<ActionResult> Delete(int Id)
        //{
        //    if (Id > 0)
        //    {
        //        var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
        //        var result = await gameCatalogLogic.Delete(Id);
        //        if (result == true)
        //        {
        //            return RedirectToAction("Index", "GameCatalog");
        //        }
        //        else
        //        {
        //            // you can ignore if and else condition here,,it will be manage ltr
        //            return RedirectToAction("Index", "GameCatalog");
        //        }
        //    }
        //    return View("Index");
        //}


        /// <summary>
        /// Uupdate Get Method
        /// </summary>
        ///// <param name="Id"></param>
        /// <returns></returns>
        //[HttpGet]
        //public async Task<ActionResult> AddUpdate(int Id)
        //{
        //    DIBZ.Common.Model.GameCatalog gameCatalog = new DIBZ.Common.Model.GameCatalog();
        //    gameCatalog = null;
        //    if (Id > 0)
        //    {
        //        var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
        //        gameCatalog = await gameCatalogLogic.GetGameCatalogById(Id);
        //        return RedirectToAction("Edit", gameCatalog);
        //    }
        //    return View("Edit", gameCatalog);
        //}

        public async Task<ActionResult> GetImageIdByGameId(int gameCatalogId)
        {
            DIBZ.Common.Model.GameCatalog gameCatalog = new DIBZ.Common.Model.GameCatalog();

            if (gameCatalogId > 0)
            {
                var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
                gameCatalog = await gameCatalogLogic.GetGameCatalogById(gameCatalogId);
                return Json(new { IsSuccess = true , GameImageId = gameCatalog.GameImageId, GameName = gameCatalog.Name }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}