using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;

namespace DIBZ.Controllers
{
    public class AboutUsController : BaseWebController
    {
        // GET: AboutUs
        public ActionResult Index()
        {
            return View();
        }
    }
}