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
    public class GameCatalogController : BaseController
    {

        //Variable Declarations
        IEnumerable<DIBZ.Common.Model.GameCatalog> gameCatalogs = new List<DIBZ.Common.Model.GameCatalog>();
        List<DIBZ.Common.DTO.GameCatalogModel> gameCatalogList = new List<DIBZ.Common.DTO.GameCatalogModel>();
        public async Task<ActionResult> Index()
        {
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();

            /*//get all GamesCatalog
            gameCatalogs = await gameCatalogLogic.GetAllGameCatalog();

            //Get values for Presentation Layer Model To show 
            gameCatalogList = gameCatalogs.Select(x => new GameCatalogModel
            {
                Id = x.Id,
                Description = x.Description,
                //CreatedBy = x.CreatedBy,
                CreatedTime = x.CreatedTime,
                FormatId = x.FormatId,
                Name = x.Name,
                UpdatedTime = x.UpdatedTime,

            }).ToList();*/
            var data=await gameCatalogLogic.GetAllGameCatalog();
            return View(data);
        }


        /// <summary>
        /// This Method Use for Update And Add Both Purpuse
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddUpdate(GameCatalogModel Request)
        {
            if (TryValidateModel(Request))
            {
                var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
                await gameCatalogLogic.AddUpdateGameCatalog(Request);
                return RedirectToAction("Index", "GameCatalog");
            }
            return View("AddUpdateGameCatalog", Request);
        }

        /// <summary>
        /// Delete GameCatalog
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Delete(int Id)
        {
            if (Id > 0)
            {
                var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
                var result = await gameCatalogLogic.Delete(Id);
                if (result == true)
                {
                    return RedirectToAction("Index", "GameCatalog");
                }
                else
                {
                    // you can ignore if and else condition here,,it will be manage ltr
                    return RedirectToAction("Index", "GameCatalog");
                }
            }
            return View("Index");
        }


        /// <summary>
        /// Uupdate Get Method
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> AddUpdate(int Id)
        {
            DIBZ.Common.Model.GameCatalog gameCatalog = new DIBZ.Common.Model.GameCatalog();
            gameCatalog = null;
            if (Id > 0)
            {
                var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
                gameCatalog = await gameCatalogLogic.GetGameCatalogById(Id);
                return RedirectToAction("Edit", gameCatalog);
            }
            return View("Edit", gameCatalog);
        }

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