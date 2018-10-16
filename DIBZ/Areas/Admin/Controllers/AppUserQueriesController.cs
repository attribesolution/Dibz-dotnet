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
using System.Threading.Tasks;
using DIBZ.Filters;
using DIBZ.Common.DTO;
using DIBZ.Logic.Notification;

namespace DIBZ.Areas.Admin.Controllers
{
    public class AppUserQueriesController : BaseWebController
    {
        //get users message 
        [AuthOp]
        public ActionResult UserQueries()
      {
            var spqueryLogic = LogicContext.Create<SupportQueryLogic>();
            var response = spqueryLogic.GetMyQueries();
            if (response != null)
            { 
                return View(response);
            }
            else
            {
                return RedirectToAction("UserQueries", "AppUserQueries");
            }
        }

        // save message conversation for admin
        [AuthOp]
        public async Task <ActionResult> AddMessageForAdmin(int queryId, string message)
        {
            var spqueryLogic = LogicContext.Create<SupportQueryLogic>();
            var authLogic = LogicContext.Create<AuthLogic>();
            var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();
            var query = spqueryLogic.GetMyQueryInfo(queryId);
            {
                spqueryLogic.SaveConversationAdmin(CurrentLoginSession.Admin.Id, queryId, message);

                var notificationLogic = LogicContext.Create<NotificationLogic>();
                DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
                NotificationModel notificationModel = new NotificationModel();
                //string Ingame = "Pock";
                //var data = new { IntrestedUserName = "John! ", InGame = "Pock" };
                //notificationModel.AdditionalData = Helpers.GetJson(data);
                if (query.AppUserId.HasValue)
                {
                    int appUserId = ConversionHelper.SafeConvertToInt32Nullable(query.AppUserId.Value, 0).Value;
                    notificationModel.AppUserId = appUserId;//Convert.ToInt32(CurrentLoginSession.Admin.Id);
                                                            //Channel like Android,Ios,Web
                    notificationModel.Channel = 0;
                    notificationModel.Content = "Admin replied to your query";
                    notificationModel.CreatedTime = DateTime.Now;
                    notificationModel.LastError = "";
                    notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.Unseen);
                    notificationModel.Title = "Query Reply";
                    notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
                    notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.CRMAdminReply);
                    //save notification in notification table
                    notification = await notificationLogic.AddNotificationForCRM_AdminReply(notificationModel);
                    //sent notification to offer creater
                    new DIBZ.Services.ServerNotificationService().SendReplyByAdmin(appUserId, notification.Id, notificationModel.Content, notification.CreatedTime);
                }
                // save email to user from admin against the query
                await emailTemplateLogic.SaveEmailNotification(query.Email, notificationModel.Content, message, EmailType.Email, Priority.Low);
            }
            return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
        }

        //update user status
        public ActionResult changeUserStatus(int queryId, int statusCode)
        {
            var sp_Query = LogicContext.Create<SupportQueryLogic>();
            var Query = sp_Query.GetMyQueryInfo(queryId);
            var isStatusUpdated = sp_Query.UpdateQueryStatus(queryId, statusCode);
            if (isStatusUpdated != null)
            {
                return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, fail = "Some Thing Wrong!" }, JsonRequestBehavior.AllowGet);
            }
        }
        
        public ActionResult GetMyQueryDetailByMyQueryIdForAdmin(int id)
        {
            var spqueryLogic = LogicContext.Create<SupportQueryLogic>();
            var UserQueries = spqueryLogic.GetQueryDetailById(id);
            return  PartialView("~/Areas/Admin/Views/AppUserQueries/_UserQueriesPartial.cshtml", UserQueries);

        }
        
        //delete message Conversation
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Delete(int id)
        {
            if (id > 0)
            {
                var spqueryLogic = LogicContext.Create<SupportQueryLogic>();
                await spqueryLogic.DeleteForAdmin(id);
            }
            return RedirectToAction("UserQueries", "AppUserQueries");
        }
    }   
    
}



