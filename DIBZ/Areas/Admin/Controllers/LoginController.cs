using DIBZ.Base;
using DIBZ.Common;
using DIBZ.Filters;
using DIBZ.Logic.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DIBZ.Areas.Admin.Controllers
{
    public class LoginController : BaseWebController
    {
        // GET: Admin/Login
        public ActionResult Index()
        {
            string userType = System.Web.Configuration.WebConfigurationManager.AppSettings["User"];
            if (userType != "Admin")
            {
                return this.Redirect("/Dashboard/Index");
            }
            return View();
        }
        public ActionResult LogIn(string email, string password)
        {
            try
            {
                var authLogic = LogicContext.Create<AuthLogic>();
                DIBZ.Common.Model.Admin admin = new DIBZ.Common.Model.Admin();
                admin.Email = email;
                admin.Password = password;
                //AuthLogic.GetApplicationUserByEmail(email);

                var loginSession = authLogic.CreateLoginSessionForAdmin(email, password, true);
                Response.Cookies["AuthCookie"].Value = loginSession.Token;
                return RedirectToAction("Index","SwapListing");
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Login: ", ex);
                return RedirectToAction("Index");
            }
        }

        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Logout()
        {
            if (Request.Cookies["AuthCookie"] != null)
            {
                var authLogic = LogicContext.Create<AuthLogic>();
                await authLogic.CloseLoginSession(Request.Cookies["AuthCookie"].Value);
                Response.Cookies["AuthCookie"].Expires = DateTime.Now.AddDays(-1);
            }
            return RedirectToAction("Index","Login");
        }
    }
}