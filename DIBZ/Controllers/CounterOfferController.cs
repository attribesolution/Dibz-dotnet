using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Filters;
using DIBZ.Logic.CounterOffer;
using DIBZ.Logic.Offer;
using DIBZ.Logic.GameCatalog;
using DIBZ.Logic.Notification;
using DIBZ.Logic.Auth;
using DIBZ.Common;
using DIBZ.Common.DTO;
using DIBZ.Logic.Swap;
using DIBZ.Logic.Scorecard;
using DIBZ.Logic;
using DIBZ.Common.Model;
using System.Configuration;

namespace DIBZ.Controllers
{
    public class CounterOfferController : BaseWebController
    {
        string hostName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        // GET: CounterOffer
        [HttpGet]
        [AuthOp(LoggedInUserOnly = true)]
        // GET: MyProfile
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Index(int id)
        {
            var counterOfferLogic = LogicContext.Create<CounterOfferLogic>();
            var counterOffersData = await counterOfferLogic.GetAllCounterOffers(id);

            var scorecardLogic = LogicContext.Create<ScorecardLogic>();
            foreach (var item in counterOffersData)
            {
                item.CounterOfferPerson.Scorecard = await scorecardLogic.GetScoreCardByAppUserId(item.CounterOfferPersonId);
            }
            return View(counterOffersData);
        }


        [AuthOp(LoggedInUserOnly = true)]
        // GET: MyProfile
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> CreateDeal(int id)
        {
            var counterOfferLogic = LogicContext.Create<CounterOfferLogic>();
            var counterOffersData = await counterOfferLogic.CreateDeal(id);
            return RedirectToAction("Index", "Dashboard");
        }


        /*[HttpGet]
        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> AddCounterOffer(int id)
        {
            //this code is use when we get notificationid

            //var notificationLogic = LogicContext.Create<NotificationLogic>();
            //var OfferLogic = LogicContext.Create<OfferLogic>();
            //DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
            //DIBZ.Common.DTO.CounterOffer counterOffer = new DIBZ.Common.DTO.CounterOffer();

            //DIBZ.Common.DTO.NotificationAdditionalData notificationAdditionalData = new DIBZ.Common.DTO.NotificationAdditionalData();
            //notification =  await notificationLogic.GetNotificationById(notificationId);

            //if (notification != null)
            //{
            //    notificationAdditionalData = JsonConvert.DeserializeObject<DIBZ.Common.DTO.NotificationAdditionalData>(notification.AdditionalData);
            //    //await OfferLogic.GetOfferById(notificationAdditionalData.OfferId);
            //    counterOffer.GameCatalogId = notificationAdditionalData.GameCatalogId;
            //    counterOffer.ReturnGameCatalogId = notificationAdditionalData.ReturnGameCatalogId;
            //    counterOffer.Description = notificationAdditionalData.Description;
            //    counterOffer.OfferId = notification.OfferId;
            //    return View(counterOffer);
            //}
            //return View("Edit", notification);


            var offerLogic = LogicContext.Create<OfferLogic>();
            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
            ViewBag.MyGames = await gameCatalogLogic.GetAllGamesOfApplicationUser(CurrentLoginSession.ApplicationUserId.GetValueOrDefault());
            DIBZ.Common.Model.CounterOffer counterOffer = new DIBZ.Common.Model.CounterOffer();
            DIBZ.Common.Model.Offer offer = new DIBZ.Common.Model.Offer();
            offer = await offerLogic.GetOfferById(id);

            if (offer != null)
            {
                return View(offer);
            }
            return View(offer);
        }*/

        //[HttpGet]
        ////[AuthOp(LoggedInUserOnly = true)]
        //[OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> AddCounterOffer(int id)
        {
            try
            {
                //Variables declarations
                var notificationLogic = LogicContext.Create<NotificationLogic>();
                var offerLogic = LogicContext.Create<OfferLogic>();
                var authLogic = LogicContext.Create<AuthLogic>();
                var counterOfferLogic = LogicContext.Create<CounterOfferLogic>();
                var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();

                DIBZ.Common.Model.Offer offerDetail = new DIBZ.Common.Model.Offer();
                DIBZ.Common.Model.ApplicationUser applicationUser = new DIBZ.Common.Model.ApplicationUser();
                DIBZ.Common.DTO.NotificationModel notificationModel = new DIBZ.Common.DTO.NotificationModel();
                DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
                DIBZ.Common.Model.CounterOffer counterOffer = new DIBZ.Common.Model.CounterOffer();
                DIBZ.Common.Model.EmailTemplate emailTemplate = new DIBZ.Common.Model.EmailTemplate();
                EmailTemplateHelper templates = new EmailTemplateHelper();
                EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
                DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();

                //get offer by offerId
                offerDetail = await offerLogic.GetOfferById(id);

                //get ApplicationUser detail by appUserId
                applicationUser = await authLogic.GetApplicationUserById(Convert.ToInt16(CurrentLoginSession.ApplicationUserId));

                notificationModel.AppUserId = offerDetail.ApplicationUserId;
                notificationModel.Channel = Convert.ToInt32(DIBZ.Common.Model.Channel.Web);
                notificationModel.Content = applicationUser.NickName + " is interested in <b>" + offerDetail.GameCatalog.Name + "</b>";
                notificationModel.CreatedTime = DateTime.Now;
                notificationModel.IsActive = true;
                notificationModel.IsDeleted = false;
                notificationModel.LastError = "";
                notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.CounterOffer);
                notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
                notificationModel.OfferId = offerDetail.Id;
                notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.Unseen);
                notificationModel.Title = "Counter Offer";

                //save counteroffer data in counteroffer table
                if (offerDetail.ReturnGameCatalogId == null)
                {
                    throw new Exception("Counter Offer cannot be created without game sought in return.");
                }
                int gameCounterOfferId = offerDetail.ReturnGameCatalogId.Value;

                //getting the list of games that counter person have
                var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();
                var myGames = (await gameCatalogLogic.GetAllGamesOfApplicationUser(CurrentLoginSession.ApplicationUser.Id)).Where(o => o.IsValidForOffer).Select(o => o.GameId).ToList();
                if (!myGames.Contains(offerDetail.ReturnGameCatalogId.Value))
                {
                    throw new Exception("You do not possess the game sought in return.");
                }

                counterOffer = await counterOfferLogic.AddCounterOffer(offerDetail.Id, gameCounterOfferId, CurrentLoginSession.ApplicationUser.Id);

                //save notification in notification table
                // save additional data in the form of json string in notification table
                var additionalData = new { GameCatalogId = offerDetail.GameCatalogId, GameCatalogImageId = offerDetail.GameCatalog.GameImageId, ReturnGameCatalogId = offerDetail.ReturnGameCatalogId.Value, CounterOfferId = counterOffer.Id };
                notificationModel.AdditionalData = Helpers.GetJson(additionalData);
                notification = await notificationLogic.AddNotification(notificationModel);

                //sent notification to offer creater
                new DIBZ.Services.ServerNotificationService().CounterOffer(offerDetail.ApplicationUserId, notification.Id, notificationModel.Content, notificationModel.CreatedTime, notificationModel.AdditionalData);

                //Deleting all the offers of counter offer game.
                await offerLogic.GetAllOfferByGameAndApplicationUser(applicationUser.Id, gameCounterOfferId);

                // Deleting game of Counter person.
                var gameLogic = LogicContext.Create<GameCatalogLogic>();
                await gameLogic.RemoveGameFromCollection(CurrentLoginSession.ApplicationUser.Id, gameCounterOfferId);

                //create email template
                emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.AddInterest);
                templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, offerDetail.ApplicationUser.NickName);
                templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName_Swapper, applicationUser.NickName);

                templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, offerDetail.GameCatalog.Name);
                templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, offerDetail.ReturnGameCatalog.Name);

                templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, offerDetail.GameCatalog.Format.Name);
                templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, offerDetail.ReturnGameCatalog.Format.Name);

                templates.AddParam(DIBZ.Common.Model.Contants.UrlPossibleSwap, string.Format("<a href='{0}'>here</a>", hostName + "/Offer/PossibleSwaps"));
                templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

                //save email data in table
                await emailTemplateLogic.SaveEmailNotification(offerDetail.ApplicationUser.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
                EmailHelper.Email(offerDetail.ApplicationUser.Email, emailTemplateResponse.Title, emailBody);

                // and as well as, email sent to him
                //EmailHelper.Email(applicationUser.Email,emailTemplateResponse.Title, emailBody);
                return RedirectToAction("PossibleSwaps", "Offer");

            }
            catch (Exception ex)
            {

                TempData["Error"] = ex.Message;//"Counter Offer cannot be created without game sought in return. ";
                //return RedirectToAction("ViewAllOffers", "Dashboard");
                return Redirect(Request.UrlReferrer.ToString());
            }

        }

        [HttpPost]
        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> AddCounterOffer(DIBZ.Common.Model.Offer offer, string gameInReturn)
        {
            //Variables declarations
            var notificationLogic = LogicContext.Create<NotificationLogic>();
            var offerLogic = LogicContext.Create<OfferLogic>();
            var authLogic = LogicContext.Create<AuthLogic>();
            var counterOfferLogic = LogicContext.Create<CounterOfferLogic>();
            var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();
            string returnGameCatalogId;

            DIBZ.Common.Model.Offer offerDetail = new DIBZ.Common.Model.Offer();
            DIBZ.Common.Model.ApplicationUser applicationUser = new DIBZ.Common.Model.ApplicationUser();
            DIBZ.Common.DTO.NotificationModel notificationModel = new DIBZ.Common.DTO.NotificationModel();
            DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
            DIBZ.Common.Model.CounterOffer counterOffer = new DIBZ.Common.Model.CounterOffer();
            DIBZ.Common.Model.EmailTemplate emailTemplate = new DIBZ.Common.Model.EmailTemplate();
            EmailTemplateHelper templates = new EmailTemplateHelper();
            EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
            DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();

            //get offer by offerId
            offerDetail = await offerLogic.GetOfferById(offer.Id);

            //get ApplicationUser detail by appUserId
            applicationUser = await authLogic.GetApplicationUserById(Convert.ToInt16(CurrentLoginSession.ApplicationUserId));

            if (gameInReturn != null)
            {
                returnGameCatalogId = gameInReturn;
            }
            else
            {
                returnGameCatalogId = offer.ReturnGameCatalogId.ToString();
            }

            notificationModel.AppUserId = offerDetail.ApplicationUserId;
            notificationModel.Channel = Convert.ToInt32(DIBZ.Common.Model.Channel.Web);
            notificationModel.Content = applicationUser.NickName + " is interested in <b>" + offer.GameCatalog.Name + "</b>";
            notificationModel.CreatedTime = DateTime.Now;
            notificationModel.IsActive = true;
            notificationModel.IsDeleted = false;
            notificationModel.LastError = "";
            notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.CounterOffer);
            notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
            notificationModel.OfferId = offer.Id;
            notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.Unseen);
            notificationModel.Title = "Counter Offer";

            //save counteroffer data in counteroffer table
            int gameCounterOfferId = ConversionHelper.SafeConvertToInt32(returnGameCatalogId);
            counterOffer = await counterOfferLogic.AddCounterOffer(offer.Id, gameCounterOfferId, Convert.ToInt16(CurrentLoginSession.ApplicationUser.Id));

            //save notification in notification table
            // save additional data in the form of json string in notification table
            var additionalData = new { GameCatalogId = offer.GameCatalogId, GameCatalogImageId = offerDetail.GameCatalog.GameImageId, ReturnGameCatalogId = returnGameCatalogId, CounterOfferId = counterOffer.Id };
            notificationModel.AdditionalData = Helpers.GetJson(additionalData);
            notification = await notificationLogic.AddNotification(notificationModel);

            //sent notification to offer creater
            new DIBZ.Services.ServerNotificationService().CounterOffer(offerDetail.ApplicationUserId, notification.Id, notificationModel.Content, notificationModel.CreatedTime, notificationModel.AdditionalData);

            //create email template
            emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.AddInterest);
            templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, offerDetail.ApplicationUser.NickName);
            templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName_Swapper, applicationUser.NickName);

            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, offerDetail.GameCatalog.Name);
            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, offerDetail.ReturnGameCatalog.Name);

            templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, offerDetail.GameCatalog.Format.Name);
            templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, offerDetail.ReturnGameCatalog.Format.Name);

            templates.AddParam(DIBZ.Common.Model.Contants.UrlPossibleSwap, string.Format("<a href='{0}'>here</a>", hostName + "/Offer/PossibleSwaps"));
            templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
            var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

            //save email data in table
            await emailTemplateLogic.SaveEmailNotification(offerDetail.ApplicationUser.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
            EmailHelper.Email(offerDetail.ApplicationUser.Email, emailTemplateResponse.Title, emailBody);

            // and as well as, email sent to him
            //EmailHelper.Email(applicationUser.Email, emailTemplateResponse.Title, emailBody);
            return RedirectToAction("Index", "Dashboard");
        }


        [HttpGet]
        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> AcceptOffer(int id)
        {
            var counterOfferLogic = LogicContext.Create<CounterOfferLogic>();
            DIBZ.Common.Model.CounterOffer counterOffer = new DIBZ.Common.Model.CounterOffer();
            counterOffer = await counterOfferLogic.GetCounterOfferById(id);
            //counterOffer.CounterOfferPerson.Email

            if (counterOffer != null)
            {
                var scorecardLogic = LogicContext.Create<ScorecardLogic>();
                counterOffer.CounterOfferPerson.Scorecard = await scorecardLogic.GetScoreCardByAppUserId(counterOffer.CounterOfferPersonId);
                return View(counterOffer);
            }
            return View(counterOffer);
        }

        [HttpGet]
        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> AcceptOfferFromPossibleSwap(int id, int offererGameId)
        {
            var counterOfferLogic = LogicContext.Create<CounterOfferLogic>();
            DIBZ.Common.Model.CounterOffer counterOffer = new DIBZ.Common.Model.CounterOffer();
            counterOffer = await counterOfferLogic.GetCounterOfferById(id);
            var offerLogic = LogicContext.Create<OfferLogic>();

            //return RedirectToAction("PossibleSwaps", "Offer");
            //return await AcceptOffer(counterOffer);

            // Deleting game of offerer person
            var gameLogic = LogicContext.Create<GameCatalogLogic>();
            await gameLogic.RemoveGameFromCollection(CurrentLoginSession.ApplicationUser.Id, offererGameId);

            return await CreateDealWithOfferStatusPaymentNeeded(counterOffer);
        }
        [HttpPost]
        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> AcceptOffer(DIBZ.Common.Model.CounterOffer counterOffer)
        {
            //variable declations
            var offerLogic = LogicContext.Create<OfferLogic>();
            var counterOfferLogic = LogicContext.Create<CounterOfferLogic>();
            var notificationLogic = LogicContext.Create<NotificationLogic>();
            var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();
            DIBZ.Common.DTO.NotificationModel notificationModel = new DIBZ.Common.DTO.NotificationModel();
            DIBZ.Common.Model.Swap swap = new DIBZ.Common.Model.Swap();
            DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
            DIBZ.Common.Model.EmailTemplate emailTemplate = new DIBZ.Common.Model.EmailTemplate();
            EmailTemplateHelper templates = new EmailTemplateHelper();
            EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
            DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();

            //create deal
            swap = await counterOfferLogic.CreateDeal(counterOffer.Offer.Id, counterOffer.CounterOfferPersonId, counterOffer.GameCounterOfferWithId);

            //Deleting all the other offers of offered game
            await offerLogic.GetAllOfferByGameAndApplicationUser(counterOffer.Offer.ApplicationUserId, counterOffer.Offer.GameCatalogId);
            //Deleting all the offers of counter offer game.
            await offerLogic.GetAllOfferByGameAndApplicationUser(counterOffer.CounterOfferPersonId, counterOffer.Offer.ReturnGameCatalogId.Value);

            //sent notification to requested person to inform that his request has been accepted
            notificationModel.AppUserId = swap.GameSwapPsersonId;
            notificationModel.Channel = Convert.ToInt32(DIBZ.Common.Model.Channel.Web);
            notificationModel.Content = "Swap has been committed for " + counterOffer.Offer.GameCatalog.Name + ".";
            notificationModel.CreatedTime = DateTime.Now;
            notificationModel.IsActive = true;
            notificationModel.IsDeleted = false;
            notificationModel.LastError = "";
            notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.AcceptOffer);
            notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
            notificationModel.OfferId = counterOffer.OfferId;
            notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.Unseen);
            notificationModel.Title = "Accept Offer";
            //save notification in notification table
            var additionalData = new { OfferId = counterOffer.Offer.Id, CounterOfferPersonId = counterOffer.CounterOfferPersonId, GameCounterOfferWithId = counterOffer.GameCounterOfferWithId, GameCatalogImageId = counterOffer.Offer.GameCatalog.GameImageId };
            notificationModel.AdditionalData = Helpers.GetJson(additionalData);
            notification = await notificationLogic.AddNotification(notificationModel);

            //sent notification
            new DIBZ.Services.ServerNotificationService().AcceptOffer(counterOffer.CounterOfferPersonId, notification.Id, notificationModel.Content, notificationModel.CreatedTime, notificationModel.AdditionalData);

            //create email template
            emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(DIBZ.Common.Model.EmailType.Email, DIBZ.Common.Model.EmailContentType.AcceptOffer);
            templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, counterOffer.Offer.ApplicationUser.NickName);
            templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName_Swapper, counterOffer.CounterOfferPerson.NickName);

            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, counterOffer.Offer.GameCatalog.Name);
            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, counterOffer.Offer.ReturnGameCatalog.Name);

            templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, counterOffer.Offer.GameCatalog.Format);
            templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, counterOffer.Offer.ReturnGameCatalog.Format);

            templates.AddParam(DIBZ.Common.Model.Contants.UrlOfferDetail, string.Format("<a href='{0}{1}'>here</a>", hostName + "/Offer/OfferDetail/", counterOffer.OfferId));
            templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
            var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

            //save email data in table
            await emailTemplateLogic.SaveEmailNotification(counterOffer.CounterOfferPerson.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
            EmailHelper.Email(counterOffer.CounterOfferPerson.Email, emailTemplateResponse.Title, emailBody);

            // and as well as, email sent to him
            //EmailHelper.Email(counterOffer.CounterOfferPerson.Email, EmailTemplateResponce.Title,emailBody);
            return RedirectToAction("MySwaps", "Offer");

        }
        [HttpPost]
        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> CreateDealWithOfferStatusPaymentNeeded(DIBZ.Common.Model.CounterOffer counterOffer)
        {
            //variable declations
            var offerLogic = LogicContext.Create<OfferLogic>();
            var counterOfferLogic = LogicContext.Create<CounterOfferLogic>();
            var notificationLogic = LogicContext.Create<NotificationLogic>();
            var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();
            DIBZ.Common.DTO.NotificationModel notificationModel = new DIBZ.Common.DTO.NotificationModel();
            DIBZ.Common.Model.Swap swap = new DIBZ.Common.Model.Swap();
            DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
            DIBZ.Common.Model.EmailTemplate emailTemplate = new DIBZ.Common.Model.EmailTemplate();
            EmailTemplateHelper templates = new EmailTemplateHelper();
            EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
            DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();

            //create deal
            swap = await counterOfferLogic.CreateDealWithOfferStatusPaymentNeeded(counterOffer.Offer.Id, counterOffer.CounterOfferPersonId, counterOffer.GameCounterOfferWithId);

            //Deleting all the other offers of offered game
            await offerLogic.GetAllOfferByGameAndApplicationUser(counterOffer.Offer.ApplicationUserId, counterOffer.Offer.GameCatalogId);
            //Deleting all the offers of counter offer game.
            await offerLogic.GetAllOfferByGameAndApplicationUser(counterOffer.CounterOfferPersonId, counterOffer.Offer.ReturnGameCatalogId.Value);

            //sent notification to requested person to inform that his request has been accepted
            notificationModel.AppUserId = swap.GameSwapPsersonId;
            notificationModel.Channel = Convert.ToInt32(DIBZ.Common.Model.Channel.Web);
            notificationModel.Content = "Swap has been committed for " + counterOffer.Offer.GameCatalog.Name + ".";
            notificationModel.CreatedTime = DateTime.Now;
            notificationModel.IsActive = true;
            notificationModel.IsDeleted = false;
            notificationModel.LastError = "";
            notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.AcceptOffer);
            notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
            notificationModel.OfferId = counterOffer.OfferId;
            notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.Unseen);
            notificationModel.Title = "Accept Offer";
            //save notification in notification table
            var additionalData = new { OfferId = counterOffer.Offer.Id, CounterOfferPersonId = counterOffer.CounterOfferPersonId, GameCounterOfferWithId = counterOffer.GameCounterOfferWithId, GameCatalogImageId = counterOffer.Offer.GameCatalog.GameImageId };
            notificationModel.AdditionalData = Helpers.GetJson(additionalData);
            notification = await notificationLogic.AddNotification(notificationModel);

            //sent notification
            new DIBZ.Services.ServerNotificationService().AcceptOffer(counterOffer.CounterOfferPersonId, notification.Id, notificationModel.Content, notificationModel.CreatedTime, notificationModel.AdditionalData);

            //create email template
            emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.AcceptOffer);
            templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, counterOffer.Offer.ApplicationUser.NickName);
            templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName_Swapper, counterOffer.CounterOfferPerson.NickName);

            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, counterOffer.Offer.GameCatalog.Name);
            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, counterOffer.Offer.ReturnGameCatalog.Name);

            templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, counterOffer.Offer.GameCatalog.Format.Name);
            templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, counterOffer.Offer.ReturnGameCatalog.Format.Name);

            templates.AddParam(DIBZ.Common.Model.Contants.UrlOfferDetail, string.Format("<a href='{0}{1}'>here</a>", hostName + "/Offer/OfferDetail/", counterOffer.OfferId));
            templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
            var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

            //save email data in table
            await emailTemplateLogic.SaveEmailNotification(counterOffer.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
            EmailHelper.Email(counterOffer.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody);

            // and as well as, email sent to him
            EmailTemplateHelper templateForCounterOfferPerson = new EmailTemplateHelper();
            templateForCounterOfferPerson.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, counterOffer.CounterOfferPerson.NickName);
            templateForCounterOfferPerson.AddParam(DIBZ.Common.Model.Contants.AppUserNickName_Swapper, counterOffer.Offer.ApplicationUser.NickName);

            templateForCounterOfferPerson.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, counterOffer.Offer.ReturnGameCatalog.Name);
            templateForCounterOfferPerson.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, counterOffer.Offer.GameCatalog.Name);

            templateForCounterOfferPerson.AddParam(DIBZ.Common.Model.Contants.GameFormat, counterOffer.Offer.ReturnGameCatalog.Format.Name);
            templateForCounterOfferPerson.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, counterOffer.Offer.GameCatalog.Format.Name);

            templateForCounterOfferPerson.AddParam(DIBZ.Common.Model.Contants.UrlOfferDetail, string.Format("<a href='{0}{1}'>here</a>", hostName + "/Offer/OfferDetail/", counterOffer.OfferId));
            templateForCounterOfferPerson.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
            emailBody = templateForCounterOfferPerson.FillTemplate(emailTemplateResponse.Body);

            await emailTemplateLogic.SaveEmailNotification(counterOffer.CounterOfferPerson.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
            EmailHelper.Email(counterOffer.CounterOfferPerson.Email, emailTemplateResponse.Title, emailBody);
            return RedirectToAction("MySwaps", "Offer");

        }
        [AuthOp(LoggedInUserOnly = true)]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> AcceptOfferAfterTransactionDone(int counterOfferId)
        {
            DIBZ.Common.Model.CounterOffer counterOffer = new DIBZ.Common.Model.CounterOffer();
            var counterOfferLogic = LogicContext.Create<CounterOfferLogic>();
            counterOffer = await counterOfferLogic.GetCounterOfferById(counterOfferId);

            //variable declations
            var offerLogic = LogicContext.Create<OfferLogic>();

            var notificationLogic = LogicContext.Create<NotificationLogic>();
            var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();
            DIBZ.Common.DTO.NotificationModel notificationModel = new DIBZ.Common.DTO.NotificationModel();
            DIBZ.Common.Model.Swap swap = new DIBZ.Common.Model.Swap();
            DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
            DIBZ.Common.Model.EmailTemplate emailTemplate = new DIBZ.Common.Model.EmailTemplate();
            EmailTemplateHelper templates = new EmailTemplateHelper();
            EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
            DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();

            //create deal
            swap = await counterOfferLogic.CreateDeal(counterOffer.Offer.Id, counterOffer.CounterOfferPersonId, counterOffer.GameCounterOfferWithId);

            //Deleting all the other offers of offered game
            await offerLogic.GetAllOfferByGameAndApplicationUser(counterOffer.Offer.ApplicationUserId, counterOffer.Offer.GameCatalogId);
            //Deleting all the offers of counter offer game.
            await offerLogic.GetAllOfferByGameAndApplicationUser(counterOffer.CounterOfferPersonId, counterOffer.Offer.ReturnGameCatalogId.Value);

            //sent notification to requested person to inform that his request has been accepted
            notificationModel.AppUserId = swap.GameSwapPsersonId;
            notificationModel.Channel = Convert.ToInt32(DIBZ.Common.Model.Channel.Web);
            notificationModel.Content = "Swap has been committed for " + counterOffer.Offer.GameCatalog.Name + ".";
            notificationModel.CreatedTime = DateTime.Now;
            notificationModel.IsActive = true;
            notificationModel.IsDeleted = false;
            notificationModel.LastError = "";
            notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.AcceptOffer);
            notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
            notificationModel.OfferId = counterOffer.OfferId;
            notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.Unseen);
            notificationModel.Title = "Accept Offer";
            //save notification in notification table
            var additionalData = new { OfferId = counterOffer.Offer.Id, CounterOfferPersonId = counterOffer.CounterOfferPersonId, GameCounterOfferWithId = counterOffer.GameCounterOfferWithId, GameCatalogImageId = counterOffer.Offer.GameCatalog.GameImageId };
            notificationModel.AdditionalData = Helpers.GetJson(additionalData);
            notification = await notificationLogic.AddNotification(notificationModel);

            //sent notification
            new DIBZ.Services.ServerNotificationService().AcceptOffer(counterOffer.CounterOfferPersonId, notification.Id, notificationModel.Content, notificationModel.CreatedTime, notificationModel.AdditionalData);

            //create email template
            emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.AcceptOffer);
            templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, counterOffer.Offer.ApplicationUser.NickName);
            templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName_Swapper, counterOffer.CounterOfferPerson.NickName);

            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, counterOffer.Offer.GameCatalog.Name);
            templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name_Swapper, counterOffer.Offer.ReturnGameCatalog.Name);

            templates.AddParam(DIBZ.Common.Model.Contants.GameFormat, counterOffer.Offer.GameCatalog.Format.Name);
            templates.AddParam(DIBZ.Common.Model.Contants.GameFormatSwapper, counterOffer.Offer.ReturnGameCatalog.Format.Name);

            templates.AddParam(DIBZ.Common.Model.Contants.UrlPossibleSwap, string.Format("<a href='{0}'>here</a>", hostName + "/Offer/PossibleSwaps"));
            templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
            //templates.AddParam(DIBZ.Common.Model.Contants.GameCatalog_Name, counterOffer.Offer.GameCatalog.Name);
            var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

            //save email data in table
            await emailTemplateLogic.SaveEmailNotification(counterOffer.CounterOfferPerson.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
            EmailHelper.Email(counterOffer.CounterOfferPerson.Email, emailTemplateResponse.Title, emailBody);

            // and as well as, email sent to him
            // EmailHelper.Email(counterOffer.CounterOfferPerson.Email, EmailTemplateResponce.Title,emailBody);
            return RedirectToAction("MySwaps", "Offer");


        }
        public async Task<ActionResult> DeleteCounterOffer(int id)
        {
            var counterOfferLogic = LogicContext.Create<CounterOfferLogic>();
            var counterOfferDetail = await counterOfferLogic.GetCounterOfferById(id);


            var authLogic = LogicContext.Create<AuthLogic>();
            var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();
            //DIBZ.Common.Model.ApplicationUser applicationUser = new DIBZ.Common.Model.ApplicationUser();

            //get ApplicationUser detail by appUserId
            //applicationUser = await authLogic.GetApplicationUserById(Convert.ToInt16(CurrentLoginSession.ApplicationUserId));

            DIBZ.Common.DTO.NotificationModel notificationModel = new DIBZ.Common.DTO.NotificationModel();
            DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
            DIBZ.Common.Model.EmailTemplate emailTemplate = new DIBZ.Common.Model.EmailTemplate();
            DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();
            EmailTemplateHelper templates = new EmailTemplateHelper();
            EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
            var notificationLogic = LogicContext.Create<NotificationLogic>();

            notificationModel.AppUserId = counterOfferDetail.CounterOfferPersonId;
            notificationModel.Channel = Convert.ToInt32(DIBZ.Common.Model.Channel.Web);
            notificationModel.Content = counterOfferDetail.Offer.ApplicationUser.NickName + " has declined your offer for <b>" + counterOfferDetail.Offer.GameCatalog.Name + "</b>";//applicationUser.NickName + " has declide your offer you made for <b>" + offerDetail.GameCatalog.Name + "</b>";
            notificationModel.CreatedTime = DateTime.Now;
            notificationModel.IsActive = true;
            notificationModel.IsDeleted = false;
            notificationModel.LastError = "";
            notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.CounterOffer);
            notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
            notificationModel.OfferId = counterOfferDetail.OfferId;
            notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.Unseen);
            notificationModel.Title = "Counter Offer";

            //save notification in notification table
            // save additional data in the form of json string in notification table
            var additionalData = new { GameCatalogId = counterOfferDetail.Offer.GameCatalogId, GameCatalogImageId = counterOfferDetail.Offer.GameCatalog.GameImageId, ReturnGameCatalogId = counterOfferDetail.Offer.ReturnGameCatalogId.Value, CounterOfferId = id };
            notificationModel.AdditionalData = Helpers.GetJson(additionalData);
            notification = await notificationLogic.AddNotification(notificationModel);

            //sent notification to offer creater
            new DIBZ.Services.ServerNotificationService().CounterOffer(counterOfferDetail.CounterOfferPersonId, notification.Id, notificationModel.Content, notificationModel.CreatedTime, notificationModel.AdditionalData);

            //create email template
            emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.DeclineOffer);
            templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, counterOfferDetail.Offer.ApplicationUser.NickName);
            templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
            var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

            //save email data in table
            await emailTemplateLogic.SaveEmailNotification(counterOfferDetail.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
            EmailHelper.Email(counterOfferDetail.Offer.ApplicationUser.Email, emailTemplateResponse.Title, emailBody);

            // and as well as, email sent to him
            EmailTemplateHelper templateForCounterOfferPerson = new EmailTemplateHelper();
            templateForCounterOfferPerson.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, counterOfferDetail.CounterOfferPerson.NickName);
            templateForCounterOfferPerson.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
            emailBody = templateForCounterOfferPerson.FillTemplate(emailTemplateResponse.Body);
            await emailTemplateLogic.SaveEmailNotification(counterOfferDetail.CounterOfferPerson.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
            EmailHelper.Email(counterOfferDetail.CounterOfferPerson.Email, emailTemplateResponse.Title, emailBody);

            //  add game into the collection again to the counter person.
            var gameLogic = LogicContext.Create<GameCatalogLogic>();
            await gameLogic.AddGameIntoCollection(counterOfferDetail.CounterOfferPersonId, counterOfferDetail.GameCounterOfferWithId);

            // Deleting counter offer.
            await counterOfferLogic.Delete(id);

            return RedirectToAction("PossibleSwaps", "Offer");
        }
    }
}

