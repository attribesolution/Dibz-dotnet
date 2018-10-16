using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Filters;

namespace DIBZ.Areas.Admin.Controllers
{
    public class SwapController : BaseWebController
    {
        // GET: Admin/Swap
        [AuthOp(AdminOnly = true)]
        public ActionResult Index()
        {
            return View();
        }
    }
}