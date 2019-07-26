using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Filters;
using DIBZ.Logic;
using System.IO;
using DIBZ.Common;
using DIBZ.Common.Model;

namespace DIBZ.Areas.Admin.Controllers
{
    public class CompetitionSetupController : BaseWebController
    {
        // GET: Admin/CompetitionSetup

        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Index()
        {
            var bannerLogic = LogicContext.Create<CompetitionLogic>();
            var bannerData = await bannerLogic.GetAllCompetition();
            return View(bannerData);
        }

        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> GetImage(int fileId)
        {
            var bannerLogic = LogicContext.Create<CompetitionLogic>();
            var bannerData = await bannerLogic.GetFileById(fileId);
            //if (fileObj == null)
            //throw new ApiException("File not found");
            return File(Server.MapPath("~/Uploads/" + bannerData.FileNewName), "application/octet-stream", bannerData.FileNewName);
        }

        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdate(int? id)
        {

            var newsFeedLogicLogic = LogicContext.Create<CompetitionLogic>();
            DIBZ.Common.Model.Competition banner = new DIBZ.Common.Model.Competition();

            if (id > 0)
            {
                banner = await newsFeedLogicLogic.GetCompetitionById(id.Value);
            }

            return View(banner);
        }

        [HttpPost, ValidateInput(false)]
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> AddUpdateContent(FormCollection formData, HttpPostedFileBase file)
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
            var newsFeedLogicLogic = LogicContext.Create<CompetitionLogic>();
            DIBZ.Common.Model.Competition request = new Common.Model.Competition();
            request.Id = id;
            request.Name = formData["txtname"];
            request.Title = formData["txttitle"];
            request.Content = formData["txtcontent"];
            if (isUploaded == 1)
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
            if (status == "1")
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
                var bannerLogic = LogicContext.Create<CompetitionLogic>();
                await bannerLogic.Delete(id);
                return RedirectToAction("Index", "CompetitionSetup");
            }
            return View("Index");
        }


        [AuthOp(AdminOnly = true)]
        public ActionResult CompetitionDeActive(int id, int Status)
        {
            bool status = false;
            var spqueryBanner = LogicContext.Create<CompetitionLogic>();
            if (Status == 0)
            {
                status = true;
            }
            var banner = spqueryBanner.UpdateStatus(id, status);
            return View("~/Areas/Admin/Views/CompetitionSetup/Index.cshtml", banner);

        }        

    }
}