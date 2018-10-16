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
    public class GeneralController : BaseWebController
    {
        // GET: AboutUs
        [HttpGet]
        public ActionResult AboutUs()
        {
            return View();
        }
        public ActionResult FAQ()
        {
            return View();
        }
        public ActionResult TermsOfUse()
        {
            return View();
        }
    }
}