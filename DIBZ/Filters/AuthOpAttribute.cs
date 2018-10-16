using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using DIBZ.Base;

namespace DIBZ.Filters
{
    public class AuthOpAttribute:ActionFilterAttribute
    {
        public bool LoggedInUserOnly { get; set; }
        public bool AdminOnly { get; set; }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.Request;
            var baseController = (BaseController)filterContext.Controller;
            var redirectUrl = baseController.Url.Action("Index", "Dashboard");
            var adminRedirectUrl = baseController.Url.Action("Index", "Login");

            if (LoggedInUserOnly && baseController.CurrentLoginSession== null)
            {
                filterContext.Result = new RedirectResult(redirectUrl);
            }
            else if (LoggedInUserOnly && baseController.CurrentLoginSession != null && baseController.CurrentLoginSession.ApplicationUser==null)
            {
                filterContext.Result = new RedirectResult(redirectUrl);
            }

            //for admin
            if (AdminOnly && baseController.CurrentLoginSession == null)
            {
                filterContext.Result = new RedirectResult(adminRedirectUrl);
            }
            else if (AdminOnly && baseController.CurrentLoginSession != null && baseController.CurrentLoginSession.Admin == null)
            {
                filterContext.Result = new RedirectResult(adminRedirectUrl);
            }
                base.OnActionExecuting(filterContext);
        }

    }
}