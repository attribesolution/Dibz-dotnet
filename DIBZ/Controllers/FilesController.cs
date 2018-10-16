using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Logic;

namespace DIBZ.Controllers
{
    public class FilesController : BaseController
    {
        // GET: Files
        public async Task<ActionResult> Index(int fileId)
        {
            var filesLogic = LogicContext.Create<FilesLogic>();
            var fileObj = await filesLogic.GetFileById(fileId);
            //if (fileObj == null)
                //throw new ApiException("File not found");
            return File(Server.MapPath("~/Uploads/" + fileObj.Id), "application/octet-stream", fileObj.Filename);
        }
    }
}