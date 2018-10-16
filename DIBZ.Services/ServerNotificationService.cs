using DIBZ.Common.DTO;
using DIBZ.Logic;
using DIBZ.Logic.Auth;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIBZ.Services
{
    public class ServerNotificationService
    {
        LogicContext LogicContext = new LogicContext();
        public void SendMessage(string message, int notificationID)
        {
            GlobalHost.ConnectionManager.GetHubContext<DIBZHub>().Clients.All.offerRecieved(message, notificationID, 1);
        }

        public BaseResponse Send(string appUserId, string messageText)
        {
            var response = new BaseResponse();
            try
            {
                GlobalHost.ConnectionManager.GetHubContext<DIBZHub>().Clients.Group("UserId_" + appUserId).offerRecieved(messageText);
            }
            catch (Exception lex)
            {
                response.Success = false;
                response.FaultMessage = lex.ToString();
            }
            //catch (Exception ex)
            //{
            //    response.Success = false;
            //    response.FaultMessage = ex.ToString();
            //}

            return response;
        }

        public BaseResponse CounterOffer(int? appUserId, int notificationId, string content, DateTime notificationCreatedTime, string additionalData)
        {
            var response = new BaseResponse();
            try
            {
                string createdTime = notificationCreatedTime.ToShortDateString();
                GlobalHost.ConnectionManager.GetHubContext<DIBZHub>().Clients.Group("CounterOffer_" + appUserId).counterOfferRecieved(notificationId, content, createdTime, additionalData);
            }
            catch (Exception lex)
            {
                response.Success = false;
                response.FaultMessage = lex.ToString();
            }
            return response;
        }

        public BaseResponse SendReplyByAdmin(int appUserId, int notificationId, string content, DateTime notificationCreatedTime)
        {
            var response = new BaseResponse();
            try
            {
                string createdTime = notificationCreatedTime.ToShortDateString();
                GlobalHost.ConnectionManager.GetHubContext<DIBZHub>().Clients.Group("SendReply_" + appUserId).adminReplyRecieved(appUserId,notificationId,content, createdTime);
                //GlobalHost.ConnectionManager.GetHubContext<DIBZHub>().Clients.Group("CounterOffer_" + appUserId).counterOfferRecieved(appUserId, notificationId, content, createdTime);
            }
            catch (Exception lex)
            {
                response.Success = false;
                response.FaultMessage = lex.ToString();
            }
            return response;
        }







        public BaseResponse AcceptOffer(int? appUserId, int notificationId, string content, DateTime notificationCreatedTime, string additionalData)
        {
            var response = new BaseResponse();
            try
            {
                string createdTime = notificationCreatedTime.ToShortDateString();
                GlobalHost.ConnectionManager.GetHubContext<DIBZHub>().Clients.Group("AcceptOffer_" + appUserId).counterOfferRecieved(notificationId, content, createdTime, additionalData);
            }
            catch (Exception lex)
            {
                response.Success = false;
                response.FaultMessage = lex.ToString();
            }
            return response;
        }

        public BaseResponse CreateOffer(int? appUserId, int notificationId, string content, DateTime notificationCreatedTime, string additionalData)
        {
            var response = new BaseResponse();
            try
            {
                string createdTime = notificationCreatedTime.ToShortDateString();
                GlobalHost.ConnectionManager.GetHubContext<DIBZHub>().Clients.Group("CreateOffer_" + appUserId).CreateOffer(notificationId, content, createdTime, additionalData);
            }
            catch (Exception lex)
            {
                response.Success = false;
                response.FaultMessage = lex.ToString();
            }
            return response;
        }

        public BaseResponse SwapAction(int? appUserId, string additionalData)
        {
            var response = new BaseResponse();
            try
            {
                GlobalHost.ConnectionManager.GetHubContext<DIBZHub>().Clients.Group("SwapAction_" + appUserId).SwapAction(additionalData);
            }
            catch (Exception lex)
            {
                response.Success = false;
                response.FaultMessage = lex.ToString();
            }
            return response;
        }



        //public int GetCurrentUserId()
        //{
        //    bool result = false;
        //    int userId = 0;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(LogicContext.LoginSession.Token))
        //        {
        //            var authCookieToken = LogicContext.LoginSession.Token;
        //            var session = LogicContext.Create<AuthLogic>().GetLoginSessionByToken(authCookieToken);
        //            if (session == null)
        //                result = false;
        //            else
        //                userId = session.AdminId.GetValueOrDefault();
        //        }
        //    }
        //    catch { result = false; }

        //    if (!result)
        //        throw new Exception("User not logged in");
        //    return userId;

        //}
    }
}
