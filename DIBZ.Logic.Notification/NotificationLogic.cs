using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Data;

namespace DIBZ.Logic.Notification
{
    public class NotificationLogic : BaseLogic
    {
        public NotificationLogic(LogicContext context) : base(context)
        {
        }

        public async Task<DIBZ.Common.Model.Notification> AddNotification(NotificationModel request)
        {
            DIBZ.Common.Model.Notification Notification = new DIBZ.Common.Model.Notification();
            Notification.Content = request.Content;
            Notification.Title = request.Title;
            
            Notification.Channel = request.Channel;
            Notification.AppUserId = request.AppUserId;
            Notification.AdditionalData = request.AdditionalData;
            Notification.IsActive = true;
            Notification.IsDeleted = false;
            Notification.LastError = request.LastError;
            Notification.OfferId = request.OfferId;
            Notification.Status = request.Status;
            Notification.NotificationType = request.NotificationType;
            Notification.NotificationBusinessType = request.NotificationBusinessType;
            Notification.CreatedTime = request.CreatedTime;
            Db.Add(Notification);
            await Db.SaveAsync();
            return Notification;
        }

        public async Task<DIBZ.Common.Model.Notification> AddNotificationForCRM_AdminReply(NotificationModel request)
        {
            DIBZ.Common.Model.Notification Notification = new DIBZ.Common.Model.Notification();
            Notification.AppUserId = request.AppUserId;
            Notification.Content = request.Content;
            Notification.Title = request.Title;
            Notification.Channel = request.Channel;
            Notification.IsActive = true;
            Notification.IsDeleted = false;
            Notification.LastError = request.LastError;
            Notification.Status = request.Status;
            Notification.NotificationType = request.NotificationType;
            Notification.NotificationBusinessType = request.NotificationBusinessType;
            Notification.CreatedTime = request.CreatedTime;
            Db.Add(Notification);
            await Db.SaveAsync();
            return Notification;
        }

        public async Task<DIBZ.Common.Model.Notification> AddNotificationForSwapAction(NotificationModel request)
        {
            DIBZ.Common.Model.Notification Notification = new DIBZ.Common.Model.Notification();
            Notification.Content = request.Content;
            Notification.Title = request.Title;

            Notification.Channel = request.Channel;
            Notification.AppUserId = request.AppUserId;
            Notification.AdditionalData = request.AdditionalData;
            Notification.IsActive = true;
            Notification.IsDeleted = false;
            Notification.LastError = request.LastError;
            Notification.OfferId = request.OfferId;
            Notification.Status = request.Status;
            Notification.NotificationType = request.NotificationType;
            Notification.NotificationBusinessType = request.NotificationBusinessType;
            Notification.CreatedTime = request.CreatedTime;
            Db.Add(Notification);
            await Db.SaveAsync();
            return Notification;
        }
        public async Task<List<DIBZ.Common.Model.Notification>> AddNotifications(NotificationModel request)
        {
            List<DIBZ.Common.Model.Notification> notifications = new List<Common.Model.Notification>();
            foreach (var offerId in request.OfferIds)
            {
                DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
                notification.Content = request.Content;
                notification.Title = request.Title;

                notification.Channel = request.Channel;
                notification.AppUserId = request.AppUserId;
                notification.AdditionalData = request.AdditionalData;
                notification.IsActive = true;
                notification.IsDeleted = false;
                notification.LastError = request.LastError;
                notification.OfferId = offerId;
                notification.Status = request.Status;
                notification.NotificationType = request.NotificationType;
                notification.NotificationBusinessType = request.NotificationBusinessType;
                notification.CreatedTime = request.CreatedTime;
                notifications.Add(notification);
            }
            Db.AddAll(notifications);
            await Db.SaveAsync();
            return notifications;
        }
        public async Task<IEnumerable<DIBZ.Common.Model.Notification>> GetAllNotification()
        {
            return await Db.Query<DIBZ.Common.Model.Notification>().QueryAsync();
        }

        public async Task<IEnumerable<DIBZ.Common.Model.Notification>> GetTopNotifications(int appUserId)
        {
            var topNotifications= await Db.Query<DIBZ.Common.Model.Notification>().Where(x => x.AppUserId == appUserId).OrderByDescending(g => g.CreatedTime).Take(10).QueryAsync();
            foreach (var item in topNotifications)
            {
                item.DisplayDateTime = ConversionHelper.FormatDate(ConversionHelper.ConvertDateToTimeZone( item.CreatedTime));
            }
            return topNotifications;
        }

        public async Task<IEnumerable<DIBZ.Common.Model.Notification>> GetAllUnSeenNotificationsByAppUserId(int appUserId)
        {
            return await Db.Query<DIBZ.Common.Model.Notification>().Where(x => x.AppUserId == appUserId && x.Status == 1).QueryAsync();
        }
        public async Task<DIBZ.Common.Model.Notification> GetNotificationById(int notificationId)
        {
            return (await Db.Query<DIBZ.Common.Model.Notification>().Where(x =>x.Id == notificationId).QueryAsync()).SingleOrDefault();
        }
        public async Task<DIBZ.Common.Model.Notification> GetUnseenNotificationById(int notificationId)
        {
            return (await Db.Query<DIBZ.Common.Model.Notification>().Where(x => x.Id == notificationId && x.Status == 1).QueryAsync()).SingleOrDefault();
        }
        public async Task<IEnumerable<DIBZ.Common.Model.Notification>> GetAllNotificationsByAppUserId(int appUserId)
        {
            return (await Db.Query<DIBZ.Common.Model.Notification>().Where(x => x.AppUserId == appUserId).QueryAsync());
        }
        public async Task<IEnumerable<DIBZ.Common.Model.Notification>> GetAllUnseenNotificationsByAppUserId(int appUserId)
        {
            return (await Db.Query<DIBZ.Common.Model.Notification>().Where(x => x.AppUserId == appUserId && x.Status == 1).QueryAsync());
        }
        public async Task<bool> UpdateNoticationStatus(int id, int status, bool isSeen)
        {
         
            IEnumerable<DIBZ.Common.Model.Notification> notifications = null;
            DIBZ.Common.Model.Notification getNotification = null;
            //if isSeen is true thats meanz all notification have been seen and change all notification of that user with status seen (status = 2)
            //if isSeen is true  thats meanz here id is AppUserId
            if (isSeen == true)
            {
                notifications = await GetAllUnseenNotificationsByAppUserId(id);
                if (notifications != null)
                {
                    foreach(var notification in notifications)
                    {
                        notification.Status = status;
                    }
                    
                }
            }
            else
            {
                //if isSeen is false thats meanz that notification status has been change with status read (read = 3)
                //if isSeen is false  thats meanz here id is NotificationIs
                getNotification = await GetNotificationById(id);
                getNotification.Status = status;
            }

            await Db.SaveAsync();
            return true;
        }
    }
}
