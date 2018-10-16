using DIBZ.Base;
using DIBZ.Common;
using DIBZ.Common.DTO;
using DIBZ.Logic;
using DIBZ.Logic.Auth;
using DIBZ.Logic.GameCatalog;
using DIBZ.Logic.Offer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Common.Model;
using DIBZ.Filters;

namespace DIBZ.Areas.Admin.Controllers
{
    public class GameCatalogController : BaseWebController
    {
        // GET: Admin/GameCatalog
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Index()
        {
            
            //IEnumerable<GameCatalogModel> GameCatalogList = new List<GameCatalogModel>();
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            var gameData = new List<GameCatalogModel>();
            var formatLogic = LogicContext.Create<FormatLogic>();
            ViewBag.Formats = await formatLogic.GetAllFormats();
            var categoryLogic = LogicContext.Create<CategoryLogic>();
            ViewBag.Categories = await categoryLogic.GetAllCategories();

            if (TempData["searchedGameData"] != null)
            {
                //ViewBag.GameCatalogList = TempData["searchOfferDataa"];
                gameData =  TempData["searchedGameData"] as List<GameCatalogModel>;
                TempData["searchedGameData"] = null;

            }
            else
            {
                gameData = await gameCatalogLogic.GetAllGameCatalog();
            }


            return View(gameData);
        }
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdate(int? id)
        {
            var formatLogic = LogicContext.Create<FormatLogic>();
            var categoryLogic = LogicContext.Create<CategoryLogic>();
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            DIBZ.Common.Model.GameCatalog gameCatalog = new DIBZ.Common.Model.GameCatalog();
            
            if (id > 0)
            {
                gameCatalog = await gameCatalogLogic.GetGameCatalogById(Convert.ToInt32(id));
            }
            ViewBag.Formats = await formatLogic.GetAllFormats();
            ViewBag.Categories = await categoryLogic.GetAllCategories();
            return View(gameCatalog);
        }


        public ActionResult SaveProfileImage(HttpPostedFileBase file)
        {

            var authLogic = LogicContext.Create<AuthLogic>();
            try
            {

                var serverPath = Server.MapPath("~/Uploads");

                string profileImageName = string.Empty;
                if (file != null)
                {
                    profileImageName = file.FileName;
                }
                var appUser = authLogic.EditProfileImage(CurrentLoginSession.ApplicationUserId.GetValueOrDefault(), profileImageName);

                if (appUser != null)
                {
                    var savePathBackgroundImage = Path.Combine(serverPath, appUser.ProfileImage.Id.ToString());
                    if (file != null)
                        file.SaveAs(savePathBackgroundImage);
                    return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, fail = "Some Thing Wrong!" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception lex)
            {
                return Json(new { IsSuccess = false, fail = lex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdateGame(DIBZ.Common.Model.GameCatalog request, HttpPostedFileBase file)
        {
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();

            var serverPath = Server.MapPath("~/Uploads");
            string fileName = string.Empty;
            if (file != null)
                fileName = file.FileName;
            var gameData= await gameCatalogLogic.AddUpdate(request, fileName);
            if (file != null)
            {
                var fileSizeInMB = file.ContentLength / 1024;
                var savePath = Path.Combine(serverPath, gameData.GameImage.Id.ToString());
                if (fileSizeInMB>=1024*1.65)
                {
                    var originalFilePath = Path.Combine(serverPath, (gameData.GameImage.Id + 100).ToString());
                    file.SaveAs(originalFilePath);
                    FileSaveHelper.ResizeTo(originalFilePath, 865, 500, savePath);
                    System.IO.File.Delete(originalFilePath);
                }
                else
                {
                    file.SaveAs(savePath);
                }
                
            }

            return RedirectToAction("Index");
        }
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> GetGameImage(int fileId)
        {
            var filesLogic = LogicContext.Create<FilesLogic>();
            var fileObj = await filesLogic.GetFileById(fileId);
            //if (fileObj == null)
            //throw new ApiException("File not found");
            return File(Server.MapPath("~/Uploads/" + fileObj.Id), "application/octet-stream", fileObj.Filename);
        }
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> SearchOffers(FormCollection formData)
        {
            SearchOffer search = new SearchOffer();
            search.GameName = formData["gameName"];
            search.FormatId = ConversionHelper.SafeConvertToInt32(formData["formatId"]);
            search.CategoryId = ConversionHelper.SafeConvertToInt32(formData["categoryId"]);

            var offerLogic = LogicContext.Create<OfferLogic>();

            //get all GamesCatalog
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            
            var gameCatalogList = await gameCatalogLogic.SearchGameCatalog(null, search);
            TempData["searchedGameData"] = gameCatalogList;
            return RedirectToAction("Index", "GameCatalog");
        }
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
                 await gameCatalogLogic.Delete(id);
                return RedirectToAction("Index");
            }
            catch (LogicException ex)
            {
                LogHelper.LogError("Delete of Game contoller", ex);
                return RedirectToAction("Index");
            }
        }

        [AuthOp(AdminOnly = true)]
        public JsonResult CustomServerSideSearchAction(DataTableAjaxPostModel model)
        {
            // action inside a standard controller
            int filteredResultsCount;
            int totalResultsCount;
            var res = YourCustomSearchFunc(model, out filteredResultsCount, out totalResultsCount);

            return Json(new
            {
                // this is what datatables wants sending back
                draw = model.draw,
                recordsTotal = totalResultsCount,
                recordsFiltered = filteredResultsCount,
                data = res
            });
        }

        [AuthOp(AdminOnly = true)]
        public IList<AdminGameCatalogModel> YourCustomSearchFunc(DataTableAjaxPostModel model, out int filteredResultsCount, out int totalResultsCount)
        {
            var searchBy = (model.search != null) ? model.search.value : null;
            var take = model.length;
            var skip = model.start;

            string sortBy = "";
            bool sortDir = true;
            string dic = "";
            if (model.order != null)
            {
                // in this example we just default sort on the 1st column 
                sortBy = model.columns[model.order[0].column].data;
                sortDir = model.order[0].dir.ToLower() == "asc";
                dic = model.order[0].dir.ToLower() == "asc" ? "asc" : "dsc";
            }


            // search the dbase taking into consideration table sorting and paging
            var result = GameCatalogPartialView(searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount, dic);
            if (result == null)
            {
                // empty collection...
                return new List<AdminGameCatalogModel>();
            }
            return result;
        }
        [AuthOp(AdminOnly = true)]
        public List<AdminGameCatalogModel> GameCatalogPartialView(string searchBy, int take, int skip, string sortBy, bool sortDir, out int filteredResultsCount, out int totalResultsCount, string dic)
        {
            //var appUserLogic = LogicContext.Create<AuthLogic>();
            //var appUsers = appUserLogic.GetRegisteredAppUsers(searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount, dic).ToList();
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            var gameCatalogData= gameCatalogLogic.GetAllGameCatalog(searchBy, take, skip, sortBy, sortDir, out filteredResultsCount, out totalResultsCount, dic);
            return gameCatalogData;
        }

        [AuthOp(AdminOnly = true)]
        public ActionResult UserFirstGame()
        {
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            var userFirstGame = gameCatalogLogic.GetUsersFirstGame();
            return View(userFirstGame);
        }
    }
}