using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DIBZ.Common;
using DIBZ.Base;
using DIBZ.Logic;
using System.Web.Mvc;
using DIBZ.Logic.Auth;
using DIBZ.Logic.SupportsQueries;
using DIBZ.Common.Model;
using DIBZ.Filters;
using System.Threading.Tasks;

namespace DIBZ.Controllers
{
    public class MyQueriesController : BaseWebController
    {
        // GET: MyQueries
        public ActionResult MyQueriesIndex()
        {
            var spqueryLogic = LogicContext.Create<SupportQueryLogic>();
            var response = spqueryLogic.GetMessagesByApplicationUserEmail(CurrentLoginSession.ApplicationUser.Email);
            if (response != null)
            {
                return View(response);
            }
            else
            {
                return RedirectToAction("Index","Dashboard");
            }
        }

        // save message conversation for Application User
        [AuthOp]
        public ActionResult AddMessageForAppUser( int queryId, string message)
        {
            var spqueryLogic = LogicContext.Create<SupportQueryLogic>();
            spqueryLogic.SaveConversationForAppUser(queryId, message);

            //var query = spqueryLogic.GetMessagesByMyQueryId(queryId);

            //if (query != null)
            //{
            //    spqueryLogic.SaveConversationForAppUser(queryId, message);
            //}
            return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("MyQueriesIndex", "MyQueries");
        }

        //delete message Conversation
        [AuthOp(LoggedInUserOnly=true)]
        public async Task<ActionResult> Delete(int id)
        {
            if (id > 0)
            {
                var spqueryLogic = LogicContext.Create<SupportQueryLogic>();
                await spqueryLogic.DeleteForAppUser(id);
            }
            return RedirectToAction("MyQueriesIndex", "MyQueries");
        }
        
        public ActionResult GetMyQueryDetailByMyQueryIdForAppUser(int id)
        {
            var spqueryLogic = LogicContext.Create<SupportQueryLogic>();
            var UserQueries = spqueryLogic.GetQueryDetailById(id);
            return PartialView("~/Views/MyQueries/_MyQueriesIndexPartial.cshtml", UserQueries);
            
        }

    }
}



