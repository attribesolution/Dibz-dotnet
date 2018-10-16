using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Logic;
using DIBZ.Logic.Auth;
using DIBZ.Logic.GameCatalog;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DIBZ.Services
{
    [HubName("DIBZHub")]
    public class DIBZHub : Hub
    {
        LogicContext LogicContext = new LogicContext();


        public DIBZHub()
        {
        }

      
        public async Task SubscribeToUserForCreateOffer(string appUserId)
        {
            await Groups.Add(Context.ConnectionId, "CreateOffer_" + appUserId);
        }
        public async Task SubscribeToUserForCounterOffer(string appUserId)
        {
            await Groups.Add(Context.ConnectionId, "CounterOffer_" + appUserId);
        }

         public async Task SubscribeToUserForGetAdminReply(string appUserId)
        {
            await Groups.Add(Context.ConnectionId, "SendReply_" + appUserId);
        }

        public async Task SubscribeToUserForAcceptOffer(string appUserId)
        {
            await Groups.Add(Context.ConnectionId, "AcceptOffer_" + appUserId);
        }

        public async Task SubscribeToSwapActions(string appUserId)
        {
            await Groups.Add(Context.ConnectionId, "SwapAction_" + appUserId);
        }

        //public async Task SubscribeToUserForCreateOffer(int id)
        //{
        //    await Groups.Add(Context.ConnectionId, "CreateOffer_" + id);
        //}

        public async Task UnSubscribeToUser(string userToken)
        {
            Clients.Group("UserToken_" + userToken).JoinRequestCancel();
            await Groups.Remove(Context.ConnectionId, "UserToken_" + userToken);
        }



        public async Task<BaseResponse> subscribeConnectionForUser(string token)
        {
            BaseResponse response = new BaseResponse();
            try
            {
                var appUserId = this.GetCurrentUserId(token);
       
                //general
                await SubscribeToUserForCreateOffer(Convert.ToString(appUserId));

                // we want to notify user to inform him when any user shown intrest of his offer
                await SubscribeToUserForCounterOffer(Convert.ToString(appUserId));

                //we want to notify user to inform him  when his offer intrest has been accept 
                await SubscribeToUserForAcceptOffer(Convert.ToString(appUserId));

                //we want to notify user to inform him  when Swap Status change by Admin hand
                await SubscribeToSwapActions(Convert.ToString(appUserId));

                // we want to notify user aboumt CRM Admin Message Reply
                await SubscribeToUserForGetAdminReply(Convert.ToString(appUserId));

                response.Success = true;
            }
            catch (Exception lex)
            {
                response.Success = false;
                response.FaultMessage = lex.ToString();
            }

            return response;
        }

        public async Task<BaseResponse> UnsubscribeConnection(string token)
        {
            BaseResponse response = new BaseResponse();

            try
            {
                //var userId = this.GetCurrentUserId(token);
                //LogicContext.Create<AuthLogic>().SetConnectionState(this.Context.ConnectionId, false);
                await UnSubscribeToUser(token);
                response.Success = true;
            }
            catch (Exception lex)
            {
                response.Success = false;
                response.FaultMessage = lex.ToString();
            }

            return response;
        }
        public override Task OnConnected()
        {
            //var connectionId = this.Context.ConnectionId;

            //Helper.WebConnectionId = LogicContext.Create<AuthLogic>().AddOrUpdateConnection(adminId, connectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            //string connectionId = this.Context.ConnectionId;
            //try
            //{
            //    var adminId = this.GetCurrentRestaurantId();
            //}
            //catch (LogicException lex)
            //{
            //    LogicContext.Create<AuthLogic>().SetConnectionState(connectionId, false);
            //}

            return base.OnDisconnected(stopCalled);
        }

        //public bool SendMessage()
        //{
        //    Clients.All.messageReceived("mohsin", "khan");

        //    return true;
        //}

        public int? GetCurrentUserId(string token)
        {
            bool result = false;
            int? appUserId = 0;
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    var session = LogicContext.Create<AuthLogic>().GetLoginSessionByToken(token);
                    if (session == null)
                        result = false;
                    else
                    {
                        appUserId = session.ApplicationUserId;
                        result = true;
                    }
                }
            }
            catch { result = false; }

            if (!result)
                throw new Exception("User not logged in");
            return appUserId;

        }

        public string WebConnectionId
        {
            get
            {
                var cookie = this.Context.Request.GetHttpContext().Request.Cookies["WebConnection"];
                object cookieValue = null;
                if (cookie != null)
                    cookieValue = cookie.Value;

                return Convert.ToString(cookieValue);
            }
            set
            {
                var cookie = this.Context.Request.GetHttpContext().Request.Cookies["WebConnection"];
                if (cookie == null)
                {
                    cookie = new HttpCookie("WebConnection");
                    cookie.Value = value;
                }
                else
                {
                    cookie.Value = value;
                }

                cookie.Expires = DateTime.Now.AddYears(1);
                this.Context.Request.GetHttpContext().Response.Cookies.Add(cookie);
            }
        }
    }
}
