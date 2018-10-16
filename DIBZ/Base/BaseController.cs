using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Common;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Logic;
using DIBZ.Logic.Auth;

namespace DIBZ.Base
{
    public class BaseController : Controller
    {
        internal LogicContext LogicContext { get; set; }

        private LoginSession _currentLoginSession = null;
        public LoginSession CurrentLoginSession
        {
            get
            {
                return _currentLoginSession;
            }
        }

        public BaseController()
        {
            LogicContext = new LogicContext();
        }

        protected override void Dispose(bool disposing)
        {
            LogicContext.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.Request;

            string token = null;

            if (filterContext.ActionParameters.ContainsKey("token"))
                token = filterContext.ActionParameters["token"] as string;
            else if (Request.Cookies["AuthCookie"] != null)
            {
                token = Request.Cookies["AuthCookie"].Value as string;
            }
            else if (filterContext.ActionParameters.Count > 0 && filterContext.ActionParameters.Values.First() is RequestWithToken)
            {
                var requestWithToken = filterContext.ActionParameters.Values.First() as RequestWithToken;
                token = requestWithToken.Token;
            }

            if (!string.IsNullOrEmpty(token))
            {
                var authLogic = LogicContext.Create<AuthLogic>();
                _currentLoginSession = authLogic.GetLoginSessionByToken(token);
            }
            base.OnActionExecuting(filterContext);
        }


        protected override void OnException(ExceptionContext filtercontext)
        {
            //log error

            LogHelper.LogError("an error occured while accessing an api", filtercontext.Exception);

            //if the request is ajax return json else redirect user to error view.
            //var apiexception = filtercontext.Exception as ApiException;
            var logicexception = filtercontext.Exception as LogicException;

            string message = "sorry, an error occurred while processing your request.";
            int statuscode = (int)HttpStatusCode.InternalServerError;

            //if (apiexception != null)
            //{
            //    message = apiexception.Message;
            //    statuscode = (int)apiexception.StatusCode;
            //}

            if (logicexception != null)
            {
                message = logicexception.Message;
            }

            //return json
            filtercontext.HttpContext.Response.StatusCode = statuscode;
            filtercontext.Result = new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = new { error = message }
            };
            filtercontext.ExceptionHandled = true;
            base.OnException(filtercontext);
        }
    }
}