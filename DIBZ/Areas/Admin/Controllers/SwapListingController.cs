using DIBZ.Base;
using DIBZ.Common;
using DIBZ.Common.DTO;
using DIBZ.Common.Model;
using DIBZ.Filters;
using DIBZ.Logic.Auth;
using DIBZ.Logic.GameCatalog;
using DIBZ.Logic.Notification;
using DIBZ.Logic.Offer;
using DIBZ.Logic.Scorecard;
using DIBZ.Logic.Swap;
using DIBZ.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace DIBZ.Areas.Admin.Controllers
{
    public class SwapListingController : BaseWebController
    {
        string hostName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        // GET: Admin/SwapListing
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> Index()
        {
            /*
            //declarations
            IEnumerable<DIBZ.Common.Model.Swap> swapList = new List<DIBZ.Common.Model.Swap>();
            List<DIBZ.Common.Model.Swap> tempSwapList = new List<DIBZ.Common.Model.Swap>();
            List<DIBZ.Common.DTO.Swap> swapListToShow = new List<DIBZ.Common.DTO.Swap>();
            DIBZ.Common.Model.Swap swap = new DIBZ.Common.Model.Swap();
            var swapLogic = LogicContext.Create<SwapLogic>();

            //get all swaps
            swapList = await swapLogic.GetAllSwaps();

            //AllSwapsRecord Group by with OfferID and get list decrease order
            var groupbyOfferIDs = swapList.OrderByDescending(d => d.Id).GroupBy(g => g.OfferId);

            //now for loop to get latest swap status
            foreach (var item in groupbyOfferIDs)
            {
                //to get latest swap status
                var tempdata =  item.First();
                swap = (DIBZ.Common.Model.Swap)tempdata;
                tempSwapList.Add(swap);
            }
            foreach(var tempSwap in tempSwapList)
            {
                DIBZ.Common.DTO.Swap swapModel = new DIBZ.Common.DTO.Swap();
                if (tempSwap.SwapStatus == DIBZ.Common.Model.SwapStatus.Game2_Received)
                {
                    var result = swapList.Where(d => d.OfferId == tempSwap.OfferId).Any(r => r.SwapStatus == DIBZ.Common.Model.SwapStatus.Game1_Received);
                    if (result)
                    {
                        //this assignment is for ,which button to show
                        swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Testing;
                    }
                    else
                    {
                        swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Game1_Received;
                    }

                }
                else if (tempSwap.SwapStatus == DIBZ.Common.Model.SwapStatus.Game1_Received)
                {
                    var result = swapList.Where(d => d.OfferId == tempSwap.OfferId).Any(r => r.SwapStatus == DIBZ.Common.Model.SwapStatus.Game2_Received);
                    if (result)
                    {
                        swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Testing;
                    }
                    else
                    {
                        swapModel.SwapButtonToShow = DIBZ.Common.Model.SwapStatus.Game2_Received;
                    }
                }
                swapModel.Id = tempSwap.Id;
                swapModel.CreatedTime = tempSwap.CreatedTime;
                swapModel.OfferedGameImageId = tempSwap.Offer.GameCatalog.GameImageId;
                swapModel.SwapGameImageId = tempSwap.GameSwapWith.GameImageId;
                swapModel.SwapGameImageId = tempSwap.GameSwapWith.GameImageId;
                swapModel.GameSwapPersonNickName = tempSwap.GameSwapPserson.NickName;
                swapModel.OfferPersonNickName = tempSwap.Offer.ApplicationUser.NickName;
                swapModel.GameSwapWithId = tempSwap.GameSwapWithId;
                swapModel.GameSwapPersonId = tempSwap.GameSwapPsersonId;
                swapModel.OfferPersonId = tempSwap.Offer.ApplicationUserId;
                swapModel.SwapStatus = tempSwap.SwapStatus;
                swapModel.OfferId = tempSwap.OfferId;
                swapModel.GameOffererDFOM = tempSwap.Offer.GameOffererDFOM;
                swapModel.GameSwapperDFOM = tempSwap.Offer.GameSwapperDFOM;
                swapListToShow.Add(swapModel);
            }
            return View(swapListToShow);
            */

            var swapLogic = LogicContext.Create<SwapLogic>();
            var swapListToShow = await swapLogic.GetAllSwaps();
            return View(swapListToShow);
        }
        [AuthOp(AdminOnly = true)]
        public async Task<ActionResult> ChangeSwipStatus(int swapStatus, int offerId, int gameSwipWithId, int gameSwapPersonId, int offerPersonId, string failReasonVal, string failGameVal)
        {
            //declarations
            int failReasonIntValue = 0;
            int failGameIntVal = 0;
            string notificationMessage = string.Empty;
            DIBZ.Common.Model.Swap swap = new DIBZ.Common.Model.Swap();
            DIBZ.Common.Model.Offer offer = new DIBZ.Common.Model.Offer();
            DIBZ.Common.Model.ApplicationUser swapPerson = new DIBZ.Common.Model.ApplicationUser();
            DIBZ.Common.Model.GameCatalog gameCatalog = new DIBZ.Common.Model.GameCatalog();
            DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
            DIBZ.Common.Model.EmailTemplate emailTemplate = new DIBZ.Common.Model.EmailTemplate();
            EmailTemplateHelper templates = new EmailTemplateHelper();
            EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
            DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();

            var scorecardLogic = LogicContext.Create<ScorecardLogic>();
            var swapLogic = LogicContext.Create<SwapLogic>();
            var offerLogic = LogicContext.Create<OfferLogic>();
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            var AuthLogic = LogicContext.Create<AuthLogic>();
            var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();

            swap.SwapStatus = (DIBZ.Common.Model.SwapStatus)swapStatus;
            swap.OfferId = offerId;
            swap.GameSwapWithId = gameSwipWithId;
            swap.GameSwapPsersonId = gameSwapPersonId;
            swap.IsActive = true;
            swap.UpdatedTime = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss"));

           //get offer by Id to get Id of Offer Creater person
            offer = await offerLogic.GetOfferById(offerId);

            //add swap
            swap = await swapLogic.AddSwap(swap);

            //get applicationUserBy applicationUserID
            swapPerson = await AuthLogic.GetApplicationUserById(swap.GameSwapPsersonId);
            
            //get gamecatalog by gameCatalogId
            gameCatalog = await gameCatalogLogic.GetGameCatalogById(swap.GameSwapWithId);

            if (swap != null)
            {
                var status = (DIBZ.Common.Model.SwapStatus)swapStatus;
                if (status == DIBZ.Common.Model.SwapStatus.Game1_NoShow)
                {
                    //if it is SentGame case so update status of only one party at one one
                    await scorecardLogic.UpdateScoreCardByAppUserId(offer.ApplicationUserId, swapStatus, failReasonIntValue, false);

                    //Save notification
                    notificationMessage = "Your " + swap.Offer.GameCatalog.Name + " game hasn't been received with in 5 day, Thank you.";
                    int gameImageId = swap.Offer.GameCatalog.GameImageId;
                    notification = await SaveNotificationForSwapAction(swap, swap.Offer.ApplicationUserId, gameImageId, notificationMessage);

                    //sent notification to user
                    new DIBZ.Services.ServerNotificationService().SwapAction(notification.AppUserId, notification.AdditionalData);

                    //create email template
                    emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.Game_1_NoShow);
                    templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swap.Offer.ApplicationUser.NickName);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.Offer.GameCatalog.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, swap.Offer.GameCatalog.Format.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, gameCatalog.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, gameCatalog.Format.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                    //templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.Offer.GameCatalog.Name);
                    var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

                    //save email data in table
                    await emailTemplateLogic.SaveEmailNotification(swap.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
                    EmailHelper.Email(swap.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody);
                }
                else if (status == DIBZ.Common.Model.SwapStatus.Game2_NoShow)
                {
                    //if it is SentGame case so update status of only one party at one one
                    await scorecardLogic.UpdateScoreCardByAppUserId(swap.GameSwapPsersonId, swapStatus, failReasonIntValue, false);

                    //Save notification
                    notificationMessage = "Your " + swap.Offer.ReturnGameCatalog.Name + " game hasn't been received with in 5 day, Thank you.";
                    int gameImageId = swap.Offer.ReturnGameCatalog.GameImageId;
                    notification = await SaveNotificationForSwapAction(swap, swap.GameSwapPsersonId, gameImageId, notificationMessage);

                    //sent notification to user
                    new DIBZ.Services.ServerNotificationService().SwapAction(notification.AppUserId, notification.AdditionalData);

                    //create email template
                    emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.Game_2_NoShow);
                    templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swap.GameSwapPserson.NickName);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.Offer.GameCatalog.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, swap.Offer.GameCatalog.Format.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, gameCatalog.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, gameCatalog.Format.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                    //templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.Offer.ReturnGameCatalog.Name);
                    var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

                    //save email data in table
                    await emailTemplateLogic.SaveEmailNotification(swap.GameSwapPserson.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
                    EmailHelper.Email(swap.GameSwapPserson.Email, emailTemplateResponse.Title, emailBody);
                }

                else if (status == DIBZ.Common.Model.SwapStatus.All_NoShow)
                {
                    //get email template
                    emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.All_NoShow);
                    //Save notification
                    notificationMessage = "Your " + swap.Offer.GameCatalog.Name + " game hasn't been received with in 5 day, Thank you.";
                    int offerGameImageId = swap.Offer.GameCatalog.GameImageId;
                    notification = await SaveNotificationForSwapAction(swap, swap.Offer.ApplicationUserId, offerGameImageId, notificationMessage);

                    //sent notification to user
                    new DIBZ.Services.ServerNotificationService().SwapAction(notification.AppUserId, notification.AdditionalData);

                    templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swap.Offer.ApplicationUser.NickName);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.Offer.GameCatalog.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, swap.Offer.GameCatalog.Format.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, swap.Offer.ReturnGameCatalog.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, swap.Offer.ReturnGameCatalog.Format.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                    //templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.Offer.GameCatalog.Name);
                    var emailBodyOfferCreator = templates.FillTemplate(emailTemplateResponse.Body);

                    //save email data in table
                    await emailTemplateLogic.SaveEmailNotification(swap.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBodyOfferCreator, EmailType.Email, Priority.Low);
                    EmailHelper.Email(swap.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBodyOfferCreator);

                    //Save notification
                    notificationMessage = "Your " + swap.Offer.ReturnGameCatalog.Name + " game hasn't been received with in 5 day, Thank you.";
                    int gameImageId = swap.Offer.ReturnGameCatalog.GameImageId;
                    notification = await SaveNotificationForSwapAction(swap, swap.GameSwapPsersonId, gameImageId, notificationMessage);

                    //sent notification to user
                    new DIBZ.Services.ServerNotificationService().SwapAction(notification.AppUserId, notification.AdditionalData);

                    //create email template
                    EmailTemplateHelper template2 = new EmailTemplateHelper();
                    EmailTemplateResponse emailTemplateResponse2 = new EmailTemplateResponse();
                    emailTemplateResponse2 = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.All_NoShow);
                    template2.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swap.GameSwapPserson.NickName);
                    template2.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.Offer.ReturnGameCatalog.Name);
                    template2.AddParam(DIBZ.Common.Model.Contants.GameFormat, swap.Offer.ReturnGameCatalog.Format.Name);
                    template2.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, swap.Offer.GameCatalog.Name);
                    template2.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, swap.Offer.GameCatalog.Format.Name);
                    template2.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                    var emailBodyGameSwapPerson = template2.FillTemplate(emailTemplateResponse2.Body);

                    //save email data in table
                    await emailTemplateLogic.SaveEmailNotification(swap.GameSwapPserson.Email, emailTemplateResponse2.Title, emailBodyGameSwapPerson, EmailType.Email, Priority.Low);
                    EmailHelper.Email(swap.GameSwapPserson.Email, emailTemplateResponse2.Title, emailBodyGameSwapPerson);
                }
                else if (status == DIBZ.Common.Model.SwapStatus.Game1_Received)
                {
                    //if it is SentGame case so update status of only one party at one one
                    await scorecardLogic.UpdateScoreCardByAppUserId(offer.ApplicationUserId, swapStatus, failReasonIntValue, true);
                    //Save notification

                    notificationMessage = "Your" + swap.Offer.GameCatalog.Name + "game has been received! Thank you.";
                    int gameImageId = swap.Offer.GameCatalog.GameImageId;
                    notification = await SaveNotificationForSwapAction(swap, swap.Offer.ApplicationUserId, gameImageId, notificationMessage);

                    //sent notification to user
                    new DIBZ.Services.ServerNotificationService().SwapAction(notification.AppUserId, notification.AdditionalData);

                    //create email template
                    emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email,EmailContentType.Game_1_Recieved);
                    templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swap.Offer.ApplicationUser.NickName);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.Offer.GameCatalog.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, swap.Offer.GameCatalog.Format.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, swap.Offer.ReturnGameCatalog.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, swap.Offer.ReturnGameCatalog.Format.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                    var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

                    LogHelper.LogInfo("sending email...");
                    LogHelper.LogInfo("UrlContactUsPage: " + ConfigurationManager.AppSettings["UrlContactUsPage"]);
                    //save email data in table
                    await emailTemplateLogic.SaveEmailNotification(swap.Offer.ApplicationUser.Email, emailTemplateResponse.Title,emailBody,EmailType.Email,Priority.Low);
                    EmailHelper.Email(swap.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody);
                    //sent email
                    //  await EmailHelper.SendEmail(swap.Offer.ApplicationUser.Email,EmailTemplateResponce.Title, emailBody);
                }
                else if (status == DIBZ.Common.Model.SwapStatus.Game2_Received)
                {
                    //if it is SentGame case so update status of only one party at one one
                    await scorecardLogic.UpdateScoreCardByAppUserId(gameSwapPersonId, swapStatus, failReasonIntValue, true);

                    //Save notification
                    notificationMessage = "Your " + gameCatalog.Name + " game has been received! Thank you.";
                    int gameImageId = gameCatalog.GameImageId;
                    notification = await SaveNotificationForSwapAction(swap, swap.GameSwapPsersonId, gameImageId, notificationMessage);

                    //sent notification to user
                    new DIBZ.Services.ServerNotificationService().SwapAction(notification.AppUserId, notification.AdditionalData);

                    //create email template
                    emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(DIBZ.Common.Model.EmailType.Email,DIBZ.Common.Model.EmailContentType.Game_2_Recieved);
                    templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swap.GameSwapPserson.NickName);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, gameCatalog.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, gameCatalog.Format.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, swap.Offer.GameCatalog.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, swap.Offer.GameCatalog.Format.Name);
                    templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                    //templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, gameCatalog.Name);
                    var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

                    LogHelper.LogInfo("sending email...");
                    LogHelper.LogInfo("UrlContactUsPage: "+ConfigurationManager.AppSettings["UrlContactUsPage"]);
                    //save email data in table
                    await emailTemplateLogic.SaveEmailNotification(swap.GameSwapPserson.Email, emailTemplateResponse.Title, emailBody, EmailType.Email,Priority.Low);
                    EmailHelper.Email(swap.GameSwapPserson.Email, emailTemplateResponse.Title, emailBody);
                }
                else
                {

                    if (failReasonVal != "" || failReasonVal == string.Empty)
                    {
                        failReasonIntValue = ConversionHelper.SafeConvertToInt32(failReasonVal);
                    }
                    if (failGameVal != "" || failGameVal == string.Empty)
                    {
                        failGameIntVal = ConversionHelper.SafeConvertToInt32(failGameVal);
                    }

                    //if is not a SentGame case so update status of both parties
                    if (status != DIBZ.Common.Model.SwapStatus.Test_Fail)
                    {
                        await scorecardLogic.UpdateScoreCardByAppUserId(offer.ApplicationUserId, swapStatus, failReasonIntValue, false);
                        await scorecardLogic.UpdateScoreCardByAppUserId(gameSwapPersonId, swapStatus, failReasonIntValue, false);
                    }

                    if (status == DIBZ.Common.Model.SwapStatus.Test_Pass)
                    {
                        notificationMessage = "Congratulation! your swap test has been passed!";
                    }
                    if (status == DIBZ.Common.Model.SwapStatus.Test_Fail)
                    {
                        string failGameName = string.Empty;
                        if (failGameIntVal == (int)DIBZ.Common.Model.SwapStatus.Game1_Received)
                        {
                            //get OfferCreater GameName
                            failGameName = swap.Offer.GameCatalog.Name;

                            //update score card only offer creater
                            await scorecardLogic.UpdateScoreCardByAppUserId(offer.ApplicationUserId, swapStatus, failReasonIntValue, false);
                        }
                        if (failGameIntVal == (int)DIBZ.Common.Model.SwapStatus.Game2_Received)
                        {
                            //get SwapPerson GameName
                            failGameName = gameCatalog.Name;

                            //update score card only SwapPerson
                            await scorecardLogic.UpdateScoreCardByAppUserId(gameSwapPersonId, swapStatus, failReasonIntValue, false);

                        }
                        if (failReasonIntValue == (int)DIBZ.Common.Model.FailCasses.DiscScratched)
                        {
                            notificationMessage = "Sorry! " + failGameName + " has been failed due to discScratched!";
                        }
                        else if (failReasonIntValue == (int)DIBZ.Common.Model.FailCasses.CaseOrInstructionsInPoorCondition)
                        {
                            notificationMessage = "Sorry! " + failGameName + " has been failed due to case/instruction in poor condition!";
                        }
                        else if (failReasonIntValue == (int)DIBZ.Common.Model.FailCasses.GameFailedTesting)
                        {
                            notificationMessage = "Sorry! " + failGameName + " has been failed due to game failed testing!";
                        }
                    }
                    if (status == DIBZ.Common.Model.SwapStatus.Dispatched)
                    {
                        LogHelper.LogInfo("swaps status set to dispatach.");
                        // removing game from offer creator's collection.
                        await gameCatalogLogic.RemoveGameFromCollectionOnDispatch(offer.ApplicationUserId, offer.GameCatalogId);
                        // removing game from swapper's collection.
                        await gameCatalogLogic.RemoveGameFromCollectionOnDispatch(swap.GameSwapPsersonId, swap.GameSwapWithId);
                        notificationMessage = "Congratulation! swap has been Successfuly dibz!";
                    }

                    if (status == DIBZ.Common.Model.SwapStatus.Test_Pass || status == DIBZ.Common.Model.SwapStatus.Test_Fail || status == DIBZ.Common.Model.SwapStatus.Dispatched)
                    {
                        int gameImageId = swap.Offer.GameCatalog.GameImageId;
                        int swapperGameImageId = swap.GameSwapWithId;
                        //Save notification to one User
                        notification = await SaveNotificationForSwapAction(swap, swap.Offer.ApplicationUserId, gameImageId, notificationMessage);
                        //sent notification to One user
                        new DIBZ.Services.ServerNotificationService().SwapAction(notification.AppUserId, notification.AdditionalData);

                        //Save notification to anotherUSerr
                        notification = await SaveNotificationForSwapAction(swap, swap.GameSwapPsersonId, swapperGameImageId, notificationMessage);
                        // sent notification to  anotherUSer
                        new DIBZ.Services.ServerNotificationService().SwapAction(notification.AppUserId, notification.AdditionalData);

                        //sent Bcc email
                        List<string> emailList = new List<string>();
                        emailList.Add(swapPerson.Email);
                        emailList.Add(swap.Offer.ApplicationUser.Email);

                        if (status == SwapStatus.Test_Pass)
                        {
                            //create email template for offerrer
                            emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.Test_Pass);
                            templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swap.Offer.ApplicationUser.NickName);
                            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.Offer.GameCatalog.Name);
                            templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, swap.Offer.GameCatalog.Format.Name);
                            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, swap.Offer.ReturnGameCatalog.Name);
                            templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, swap.Offer.ReturnGameCatalog.Format.Name);
                            templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                            var emailBody = templates.FillTemplate(emailTemplateResponse.Body);
                            //save email data in table
                            await emailTemplateLogic.SaveEmailNotification(swap.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
                            EmailHelper.Email(swap.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody);

                            //create template for swapper
                            EmailTemplateResponse emailTemplateResponse2 = new EmailTemplateResponse();
                            EmailTemplateHelper template2 = new EmailTemplateHelper();
                            emailTemplateResponse2 = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.Test_Pass);
                            template2.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swap.GameSwapPserson.NickName);
                            template2.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.GameSwapWith.Name);
                            template2.AddParam(DIBZ.Common.Model.Contants.GameFormat, swap.GameSwapWith.Format.Name);
                            template2.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, swap.Offer.GameCatalog.Name);
                            template2.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, swap.Offer.GameCatalog.Format.Name);
                            template2.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                            var emailBodySwapper = template2.FillTemplate(emailTemplateResponse2.Body);
                            //save email data in table
                            await emailTemplateLogic.SaveEmailNotification(swapPerson.Email, emailTemplateResponse2.Title, emailBodySwapper, EmailType.Email, Priority.Low);
                            EmailHelper.Email(swapPerson.Email, emailTemplateResponse2.Title, emailBodySwapper);
                        }
                        else if (status == SwapStatus.Test_Fail)
                        {
                            if (failGameIntVal == (int)DIBZ.Common.Model.SwapStatus.Game1_Received)
                            {
                                //create email template game1 test failed
                                emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.Test_Fail);
                                templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName,swap.Offer.ApplicationUser.NickName);
                                templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name,swap.Offer.GameCatalog.Name);
                                templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, swap.Offer.GameCatalog.Format.Name);
                                if (failReasonIntValue == (int)DIBZ.Common.Model.FailCasses.DiscScratched)
                                {
                                    notificationMessage = "Sorry! " + swap.Offer.GameCatalog.Name + " has been failed due to discScratched!";
                                    templates.AddParam(DIBZ.Common.Model.Contants.FailReason, notificationMessage);
                                }
                                else if (failReasonIntValue == (int)DIBZ.Common.Model.FailCasses.CaseOrInstructionsInPoorCondition)
                                {
                                    notificationMessage = "Sorry! " + swap.Offer.GameCatalog.Name + " has been failed due to case/instruction in poor condition!";
                                    templates.AddParam(DIBZ.Common.Model.Contants.FailReason, notificationMessage);
                                }
                                else if (failReasonIntValue == (int)DIBZ.Common.Model.FailCasses.GameFailedTesting)
                                {
                                    notificationMessage = "Sorry! " + swap.Offer.GameCatalog.Name + " has been failed due to game failed testing!";
                                    templates.AddParam(DIBZ.Common.Model.Contants.FailReason, notificationMessage);
                                }
                                templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, gameCatalog.Name);
                                templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, gameCatalog.Format.Name);
                                templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                                var emailBody = templates.FillTemplate(emailTemplateResponse.Body);
                                //save email data in table
                                await emailTemplateLogic.SaveEmailNotification(offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
                                EmailHelper.Email(offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody);
                            }
                            else
                            {
                                //create email template game2 test failed
                                EmailTemplateResponse emailTemplateResponse2 = new EmailTemplateResponse();
                                EmailTemplateHelper template2 = new EmailTemplateHelper();
                                emailTemplateResponse2 = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.Test_Fail);
                                template2.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swap.GameSwapPserson.NickName);
                                template2.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, gameCatalog.Name);
                                template2.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, gameCatalog.Format.Name);
                                template2.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.Offer.GameCatalog.Name);
                                template2.AddParam(DIBZ.Common.Model.Contants.GameFormat, swap.Offer.GameCatalog.Format.Name);
                                template2.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                                var emailBody = template2.FillTemplate(emailTemplateResponse2.Body);
                                //save email data in table
                                await emailTemplateLogic.SaveEmailNotification(swap.GameSwapPserson.Email, emailTemplateResponse2.Title, emailBody, EmailType.Email, Priority.Low);
                                EmailHelper.Email(swap.GameSwapPserson.Email, emailTemplateResponse2.Title, emailBody);
                            }
                        }
                        else if(status == SwapStatus.Dispatched)
                        {
                            //create email template for Offerrer
                            emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.Dispatch);
                            templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swap.Offer.ApplicationUser.NickName);
                            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.Offer.GameCatalog.Name);
                            templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, swap.Offer.GameCatalog.Format.Name);
                            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, swap.Offer.ReturnGameCatalog.Name);
                            templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, swap.Offer.ReturnGameCatalog.Format.Name);
                            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Dispatch, swap.Offer.ReturnGameCatalog.Name);
                            templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper_Dispatch, swap.Offer.ReturnGameCatalog.Format.Name);
                            templates.AddParam(DIBZ.Common.Model.Contants.AppUserAddress, swap.Offer.ApplicationUser.Address);
                            templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                            var emailBody = templates.FillTemplate(emailTemplateResponse.Body);
                            //save email data in table
                            await emailTemplateLogic.SaveEmailNotification(swap.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
                            EmailHelper.Email(swap.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody);

                            //create email template for Swapper
                            EmailTemplateResponse emailTemplateResponse2 = new EmailTemplateResponse();
                            EmailTemplateHelper template2 = new EmailTemplateHelper();
                            emailTemplateResponse2 = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.Dispatch);
                            template2.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, swap.GameSwapPserson.NickName);
                            template2.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, swap.GameSwapWith.Name);
                            template2.AddParam(DIBZ.Common.Model.Contants.GameFormat, swap.GameSwapWith.Format.Name);
                            template2.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, swap.Offer.GameCatalog.Name);
                            template2.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, swap.Offer.GameCatalog.Format.Name);
                            template2.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Dispatch, swap.Offer.GameCatalog.Name);
                            template2.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper_Dispatch, swap.Offer.GameCatalog.Format.Name);
                            template2.AddParam(DIBZ.Common.Model.Contants.AppUserAddress, swap.GameSwapPserson.Address);
                            template2.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                            var emailBodySwapper = template2.FillTemplate(emailTemplateResponse2.Body);
                            //save email data in table
                            await emailTemplateLogic.SaveEmailNotification(swapPerson.Email, emailTemplateResponse2.Title, emailBodySwapper, EmailType.Email, Priority.Low);
                            EmailHelper.Email(swapPerson.Email, emailTemplateResponse2.Title, emailBodySwapper);
                        }
                        // await EmailHelper.SendEmailBcc(emailList, "Dibz swap status update", notificationMessage);
                    }
                }

                return Json(new { IsSuccess = true, data = string.Empty }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, fail = "Some thing wrong!" }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<DIBZ.Common.Model.Notification> SaveNotificationForSwapAction(DIBZ.Common.Model.Swap swap,int tosent, int gameCatalogImageId, string message)
        {
            NotificationModel notificationModel = new NotificationModel();
            DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
            var notificationLogic = LogicContext.Create<NotificationLogic>();

            var additionalData = new { OfferId = swap.OfferId , GameCatalogImageId = gameCatalogImageId };
            notificationModel.AdditionalData = Helpers.GetJson(additionalData);
            notificationModel.AppUserId = Convert.ToInt32(tosent);
            //Channel like Android,Ios,Web
            notificationModel.Channel = Convert.ToInt32(DIBZ.Common.Model.Channel.Web);
            notificationModel.Content = message;
            notificationModel.CreatedTime = DateTime.Now;
            notificationModel.LastError = "";
            notificationModel.OfferId = swap.OfferId;
            //notificationModel.OfferIds = allOffers.Select(o => o.Id).ToList();
            notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.Unseen);
            notificationModel.Title = "Swap Action";
            notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
            notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.SwapAction);
            return await notificationLogic.AddNotification(notificationModel);
        }

        public async Task<ActionResult> ReadQR(int id)
        {
            var swapLogic = LogicContext.Create<SwapLogic>();
            var swapDetail = await swapLogic.GetSwapDetailById(id);

            return View(swapDetail);
        }

        public ActionResult SendQrAgain(int appUserId,int swapId)
        {
            try {
                var authLogic = LogicContext.Create<AuthLogic>();
                var appUser = authLogic.GetUserById(appUserId);
                QRHelper.GenerateAndSaveQrCode(appUser.Email, swapId, this.Request.Url.Scheme);
                return Json(new { IsSuccess = true, data = string.Empty }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception lex)
            {
                return Json(new { IsSuccess = false, fail = lex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        // Shoaib Code
        public async Task<ActionResult> GetLastWeekRegisterUserCount()
        {
            var authLogic = LogicContext.Create<AuthLogic>();
            var result1 = await authLogic.GetLastWeekLoginUserCount();
            var result = await authLogic.GetLastWeekRegisterUserCount();
            var perAvg = (result1.Count() / 7);
            if (result != null && result1 != null)
            {
                return Json(new
                {
                    IsSuccess = true,
                    result = "Last Week Register Users (" + result.Count().ToString() + ")",
                    result1 = "Last Week Logins per Day Average (" + perAvg.ToString() + ")"
                }, JsonRequestBehavior.AllowGet);
            }
            else if (result != null && result1 == null)
            {
                return Json(new
                {
                    IsSuccess = true,
                    result = "Last Week Register Users (" + result.Count().ToString() + ")",
                    result1 = "Last Week Logins per Day Average (" + "0" + ")"
                }, JsonRequestBehavior.AllowGet);
            }
            else if (result == null && result1 != null)
            {
                return Json(new
                {
                    IsSuccess = true,
                    result = "Last Week Register Users (" + "0" + ")",
                    result1 = "Last Week Logins per Day Average (" + perAvg.ToString() + ")"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, fail = "some thing wrong" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}