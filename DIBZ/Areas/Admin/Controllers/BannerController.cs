using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Filters;
using DIBZ.Logic.Banner;
using DIBZ.Logic;
using System.IO;
using DIBZ.Common;

namespace DIBZ.Areas.Admin.Controllers
{
    public class BannerController : BaseWebController
    {
        // GET: Admin/Banner

        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Index()
        {
            var bannerLogic = LogicContext.Create<BannerLogic>();
            var bannerData = await bannerLogic.GetAllBanners();
            return View(bannerData);
        }

        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> GetImage(int fileId)
        {
            var bannerLogic = LogicContext.Create<BannerLogic>();
            var bannerData = await bannerLogic.GetFileById(fileId);
            //if (fileObj == null)
            //throw new ApiException("File not found");
            return File(Server.MapPath("~/Uploads/" + bannerData.FileNewName), "application/octet-stream", bannerData.FileNewName);
        }       

        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdate(int? id)
        {

            var newsFeedLogicLogic = LogicContext.Create<BannerLogic>();
            DIBZ.Common.Model.Banners banner = new DIBZ.Common.Model.Banners();

            if (id > 0)
            {
                banner = await newsFeedLogicLogic.GetBannerById(id.Value);
            }

            return View(banner);
        }

        [HttpPost]
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdateBanner(FormCollection formData, HttpPostedFileBase file)
        {           
            var serverPath = Server.MapPath("~/Uploads");
            string fileNewName = string.Empty;
            string fileOrignalName = string.Empty;
            int isUploaded = 0;            
            if (file != null)
            {
                Random rnd = new Random();
                int rendomNumber = rnd.Next(52);
                string extension = Path.GetExtension(file.FileName);
                var filename = Path.GetFileNameWithoutExtension(file.FileName);
                fileNewName = filename + rendomNumber + extension;
                fileOrignalName = file.FileName;

                var fileSizeInMB = file.ContentLength / 1024;
                var savePath = Path.Combine(serverPath, fileNewName.ToString());
                if (fileSizeInMB >= 1024 * 1.65)
                {
                    var originalFilePath = Path.Combine(serverPath, (fileNewName).ToString());
                    file.SaveAs(originalFilePath);
                    FileSaveHelper.ResizeTo(originalFilePath, 865, 500, savePath);
                    System.IO.File.Delete(originalFilePath);
                    isUploaded = 1;
                }
                else
                {
                    file.SaveAs(savePath);
                    isUploaded = 1;
                }

            }

            int id = Convert.ToInt32(formData["Id"]);
            var newsFeedLogicLogic = LogicContext.Create<BannerLogic>();
            DIBZ.Common.Model.Banners request = new Common.Model.Banners();
            request.Id = id;
            request.Name = formData["name"];
            request.Title = formData["title"];
            if(isUploaded == 1)
            {
                request.FileNewName = fileNewName;
                request.FileOrignalName = fileOrignalName;
            }
            else
            {
                request.FileNewName = "";
                request.FileOrignalName = "";                
            }
            var status = formData["status"];
            if(status == "1")
            {
                request.IsActive = true;
            }
            else 
            {
                request.IsActive = false;
            }          

            await newsFeedLogicLogic.AddUpdate(request);
            return RedirectToAction("Index");
        }


        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Delete(int id)
        {
            if (id > 0)
            {
                var bannerLogic = LogicContext.Create<BannerLogic>();
                await bannerLogic.Delete(id);
                return RedirectToAction("Index", "NewsFeed");
            }
            return View("Index");
        }


        [AuthOp(AdminOnly = true)]
        public ActionResult BannersDeActive(int id, int Status)
        {
            bool status = false;
            var spqueryBanner = LogicContext.Create<BannerLogic>();
            if (Status == 0)
            {
                status = true;
            }
            var banner = spqueryBanner.UpdateStatus(id, status);
            return View("~/Areas/Admin/Views/Banner/Index.cshtml", banner);

        }

        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> DeleteBanner(int id)
        {
            if (id > 0)
            {
                var Banner = LogicContext.Create<BannerLogic>();
                await Banner.Delete(id);
            }
            return RedirectToAction("index", "Banner");
        }

    }
}
