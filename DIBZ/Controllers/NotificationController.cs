using DIBZ.Base;
using DIBZ.Logic.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DIBZ.Controllers
{
    public class NotificationController : BaseWebController
    {
        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> GetTopNotifications(int appUserId)
        {
            try
            {
               IEnumerable<DIBZ.Common.Model.Notification> topNotifications = new List<DIBZ.Common.Model.Notification>();
                if (appUserId > 0)
                {
                    var notificationLogic = LogicContext.Create<NotificationLogic>();
                    topNotifications = await notificationLogic.GetTopNotifications(appUserId);
                    foreach (var item in topNotifications)
                    {
                        item.CreatedTime = DIBZ.Common.ConversionHelper.ParseDateUsingDefaultFormat(item.CreatedTime.ToString(), DateTime.Now).Value;
                    }
                    return Json(new { IsSuccess = true, data = topNotifications }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception lex)
            {
                return Json(new { IsSuccess = false, fail = lex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsSuccess = false, fail = "Something wrong!" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetUnSeenNotification(int appUserId)
        {
            try
            {
                IEnumerable<DIBZ.Common.Model.Notification> allUnSeenNotifications = new List<DIBZ.Common.Model.Notification>();
                if (appUserId > 0)
                {
                    var notificationLogic = LogicContext.Create<NotificationLogic>();
                    allUnSeenNotifications = await notificationLogic.GetAllUnSeenNotificationsByAppUserId(appUserId);
                    int totalUnSeenNotifications = allUnSeenNotifications.Count();
                    return Json(new { IsSuccess = true, totalUnSeenNotificationCount = totalUnSeenNotifications }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception lex)
            {
                return Json(new { IsSuccess = false, fail = lex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsSuccess = false, fail = "Something wrong!" }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> UpdateNotificationStatus(int id, int status, bool isSeen)
        {
            try
            {
                if (id > 0)
                {
                    var notificationLogic = LogicContext.Create<NotificationLogic>();
                    var result = await notificationLogic.UpdateNoticationStatus(id, status, isSeen);
                    if (result == true)
                    {
                        return Json(new { IsTrue = true , data = string.Empty}, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception lex)
            {
                return Json(new { IsTrue = false, fail = lex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { IsTrue = false, fail = "Something wrong!" }, JsonRequestBehavior.AllowGet);
        }
    }
}