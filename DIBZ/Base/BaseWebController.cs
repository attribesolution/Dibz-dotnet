using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using DIBZ.Common;
using System.Web;

namespace DIBZ.Base
{
    public class BaseWebController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (CurrentLoginSession != null)
            {
                ViewData["AppUser"] = CurrentLoginSession.ApplicationUser;
                ViewData["AppToken"] = CurrentLoginSession.Token;
            }
            ViewData["Message"] = TempData["Message"];
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            var baseController = (BaseController)filterContext.Controller;
            LogHelper.LogError(filterContext.Exception.Message, filterContext.Exception.InnerException);

            filterContext.ExceptionHandled = true;
            filterContext.Result = new RedirectToRouteResult(
                new RouteValueDictionary
                {
                    { "controller", "Dashboard" },
                    { "action", "Index" }
                });


            //var redirectUrl = baseController.Url.Action("Index", "Dashboard");
            //filterContext.Result = new RedirectResult(redirectUrl);
            //base.OnException(filterContext);
            //filterContext.Result = this.RedirectToAction("Index", "Error");
        }
    }
}