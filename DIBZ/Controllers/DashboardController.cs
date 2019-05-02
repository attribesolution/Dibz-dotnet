using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DIBZ.Base;
using DIBZ.Logic;
using DIBZ.Logic.GameCatalog;
using DIBZ.Logic.Offer;
using DIBZ.Common.DTO;
using DIBZ.Logic.Auth;
using DIBZ.Common;
using DIBZ.Logic.Notification;
using DIBZ.Services;
using DIBZ.Filters;
using System.Configuration;
using DIBZ.Logic.NewsLetter;
using DIBZ.Logic.NewsFeed;
using DIBZ.Logic.SupportsQueries;
using DIBZ.Common.Model;
using DIBZ.Data;
using MailChimp.Types;
using MailChimp;
using DIBZ.Logic.Banner;

namespace DIBZ.Controllers
{
    public class DashboardController : BaseWebController
    {
        string hostName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        // GET: Dashboard

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public async Task<ActionResult> Index(int formatId = 0)
        {
            var formatLogic = LogicContext.Create<FormatLogic>();
            ViewBag.Formats = await formatLogic.GetAllFormats();

            var bannerLogic = LogicContext.Create<BannerLogic>();
            ViewBag.BannerImage = await bannerLogic.GetAllBannerImage();            

            //string userType = System.Web.Configuration.WebConfigurationManager.AppSettings["User"];
            //if (userType == "Admin")
            //{
            //    return this.Redirect("/Admin/Login");
            //}
            ViewData["Error"] = TempData["Error"];
            LogHelper.LogInfo("Fetching dashboard data");

            var gameCatalogLogic = LogicContext.Create<GameCatalogLogic>();

            //get all GamesCatalog
            var gameCatalogs = await gameCatalogLogic.GetAllFeaturedGames();

            if (TempData["searchOfferData"] != null)
            {
                ViewBag.Offers = TempData["searchOfferData"];
                TempData["searchOfferData"] = null;
            }
            else
            {
                var offerLogic = LogicContext.Create<OfferLogic>();
                ViewBag.Offers = await offerLogic.GetAllOfferForDashboard(formatId);
            }
            return View(gameCatalogs);
        }
        public async Task<ActionResult> ViewAllOffers(int formatId)
        {
            if (TempData["Error"] != null)
                ViewData["Error"] = TempData["Error"];

            var offerLogic = LogicContext.Create<OfferLogic>();
            var allOffersData = await offerLogic.GetAllOffers(formatId);
            return View(allOffersData);
        }

        public ActionResult GetDibzCharges(string gameOwned,string gameDesired,string retailPrice, string creditValue, string cashValue)
        {
            var chargesLogic = LogicContext.Create<DibzChargesLogic>();
            try
            {                
                var chargesData = chargesLogic.GetAllDibzChargesData();
                decimal savingCNV = 0;
                decimal savingCAV = 0;
                var dibzCharges = chargesData.Result[0].Charges;
                var result = "";
                var error = "";
                if (dibzCharges != "")
                {
                    if(creditValue != "" && cashValue == "")
                    {
                        savingCNV = (Convert.ToDecimal(retailPrice) - Convert.ToDecimal(creditValue) - Convert.ToDecimal(dibzCharges));
                        if (savingCNV > 0)
                        {
                            result = " Good news! By using DIBZ you could save £" + String.Format("{0:0.00}", savingCNV) + " instead of using a credit note for your game to purchase "+ gameDesired + ".";
                        }
                        else {
                            result = "Unfortunately, it is cheaper to use a credit note value for your game to purchase "+ gameDesired + ". Thank you for your custom.";
                        }                        
                    }

                   else if (cashValue != "" && creditValue == "")
                    {
                        savingCAV = (Convert.ToDecimal(retailPrice) - Convert.ToDecimal(cashValue) - Convert.ToDecimal(dibzCharges));
                        if (savingCAV > 0)
                        {
                            result = " Good news! By using DIBZ you could save £" + String.Format("{0:0.00}", savingCAV)  + " instead of trading in your game for cash to purchase " + gameDesired + ".";
                        }
                        else
                        {
                            result = "Unfortunately, it is cheaper to use a cash for your game to purchase " + gameDesired + ". Thank you for your custom.";
                        }
                    }
                    else
                    {
                        error = "Please enter credit note value or cash value";
                        return Json(new { IsSuccess = false, fail = error, Result = result }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new { IsSuccess = true, Result = result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, fail = "Some Thing Wrong dibz charges not found!" }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception lex)
            {
                return Json(new { IsSuccess = false, fail = lex.Message }, JsonRequestBehavior.AllowGet);
            }            
        }
        public async Task<ActionResult> Register(string id, string firstName, string surname, string nickName, string email, string password, string mobileNo, string birthYear, string postalCode, string address, bool? rememberMe = true)
        {
            var authLogic = LogicContext.Create<AuthLogic>();
            try
            {
                var User = authLogic.AddUpdateUser(Convert.ToInt32(id), firstName, surname, nickName, email, password, mobileNo, birthYear, postalCode, address);
                if (User != null)
                {
                    var loginSession = authLogic.CreateLoginSession(email, password, false);
                    Response.Cookies["AuthCookie"].Value = loginSession.Token;
                    if (rememberMe == true)
                    {
                        Response.Cookies["AuthCookie"].Expires = DateTime.Now.AddYears(1);
                    }

                    EmailTemplateHelper templates = new EmailTemplateHelper();
                    EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
                    DIBZ.Common.Model.EmailNotification Email = new DIBZ.Common.Model.EmailNotification();

                    var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();
                    emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.SignUp);

                    String[] parts = email.Split(new[] { '@' });
                    String username = parts[0]; // "hello"
                    String domain = parts[1]; // "example.com"

                    templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, username);
                    templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>here</a>", hostName + "/Dashboard/ContactUs"));
                    var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

                    await emailTemplateLogic.SaveEmailNotification(email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
                    EmailHelper.Email(email, emailTemplateResponse.Title, emailBody);
                    MailChimpsSubs(email, firstName, surname, mobileNo);
                    return Json(new { IsSuccess = true, AppUserName = loginSession.ApplicationUser.NickName, AppUserId = loginSession.ApplicationUserId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { IsSuccess = false, fail = "Some Thing Wrong!" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { IsSuccess = false, fail = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        //public ActionResult Login(string email, string password, string rememberMe)
        public ActionResult Login(string email, string password, bool rememberMe)
        {
            try
            {
                var authLogic = LogicContext.Create<AuthLogic>();
                DIBZ.Common.Model.ApplicationUser ApplicationUser = new DIBZ.Common.Model.ApplicationUser();
                ApplicationUser.Email = email;
                ApplicationUser.Password = password;
                //AuthLogic.GetApplicationUserByEmail(email);

                var loginSession = authLogic.CreateLoginSession(email, password, false);
                Response.Cookies["AuthCookie"].Value = loginSession.Token;
                //if (rememberMe == "on")
                if (rememberMe)
                {
                    Response.Cookies["AuthCookie"].Expires = DateTime.Now.AddYears(1);
                }
                return Json(new { IsSuccess = true, AppUserName = loginSession.ApplicationUser.NickName, AppUserId = loginSession.ApplicationUserId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Login: ", ex);
                return Json(new { IsSuccess = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }


        }
        [AuthOp(LoggedInUserOnly = true)]
        public async Task<ActionResult> Logout()
        {
            if (Request.Cookies["AuthCookie"] != null)
            {
                var authLogic = LogicContext.Create<AuthLogic>();
                await authLogic.CloseLoginSession(Request.Cookies["AuthCookie"].Value);
                Response.Cookies["AuthCookie"].Expires = DateTime.Now.AddDays(-1);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Test()
        {

            new DIBZ.Services.ServerNotificationService().Send(CurrentLoginSession.ApplicationUserId.ToString(), "Here i am 1 !");
            return this.Json("you result", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> CounterOffer()
        {
            var notificationLogic = LogicContext.Create<NotificationLogic>();
            DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
            NotificationModel notificationModel = new NotificationModel();
            //if you want to store some addional data in notification info
            string Ingame = "Pock";
            var data = new { IntrestedUserName = "John! ", InGame = "Pock" };
            notificationModel.AdditionalData = Helpers.GetJson(data);
            notificationModel.AppUserId = Convert.ToInt32(CurrentLoginSession.ApplicationUserId);
            //Channel like Android,Ios,Web
            notificationModel.Channel = 0;
            notificationModel.Content = "is intrested in your " + Ingame + "";
            notificationModel.CreatedTime = DateTime.Now;
            notificationModel.LastError = "";
            notificationModel.OfferId = 4;
            notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.UnRead);
            notificationModel.Title = "Counter Intrest";
            notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
            notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.CounterOffer);
            //save notification in notification table
            notification = await notificationLogic.AddNotification(notificationModel);
            //sent notification to offer creater
            new DIBZ.Services.ServerNotificationService().CounterOffer(CurrentLoginSession.ApplicationUserId, notification.Id, notificationModel.Content, notificationModel.CreatedTime, notificationModel.AdditionalData);
            return this.Json("your counter offer notification has been sent to relevent person", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AcceptOffer()
        {
            //int countOfferAppUserId = 2;
            var notificationLogic = LogicContext.Create<NotificationLogic>();
            DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
            NotificationModel notificationModel = new NotificationModel();
            //if you want to store some addional data in notification info
            List<int> appUsersbyGameID = new List<int>();
            appUsersbyGameID.Add(1);
            appUsersbyGameID.Add(2);
            foreach (var appUsersId in appUsersbyGameID)
            {
                string ForGame = "Pock";
                string By = "John";
                var data = new { By = "John! ", ForGame = "Pock" };
                notificationModel.AdditionalData = Helpers.GetJson(data);
                notificationModel.AppUserId = Convert.ToInt32(appUsersId);
                //Channel like Android,Ios,Web
                notificationModel.Channel = 0;
                notificationModel.Content = "You offer has been accepted for " + ForGame + " game by " + By + ".";
                notificationModel.CreatedTime = DateTime.Now;
                notificationModel.LastError = "";
                notificationModel.OfferId = 4;
                notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.UnRead);
                notificationModel.Title = "Accept Intrest";
                notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
                notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.AcceptOffer);
                //Get All AppUser By Game ID

                //save notification in notification table
                notification = await notificationLogic.AddNotification(notificationModel);
            }
            //sent notification to offer creater

            // new DIBZ.Services.ServerNotificationService().AcceptOffer(CurrentLoginSession.ApplicationUserId, notificationModel.AppUserId, notification.Id, notificationModel.Content, notificationModel.CreatedTime, notificationModel.AdditionalData);
            return this.Json("your counter offer notification has been sent to relevent person", JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> CreateOffer()
        {
            //int countOfferAppUserId = 2;
            var notificationLogic = LogicContext.Create<NotificationLogic>();
            DIBZ.Common.Model.Notification notification = new DIBZ.Common.Model.Notification();
            NotificationModel notificationModel = new NotificationModel();

            //at this time ,we just assumed that to hardcode data
            List<int> getAllAppUsersByGameId = new List<int>();
            getAllAppUsersByGameId.Add(1);
            getAllAppUsersByGameId.Add(2);
            getAllAppUsersByGameId.Add(4);
            foreach (var appUserId in getAllAppUsersByGameId)
            {
                //if you want to store some addional data in notification info
                string ForGame = "Pock";
                string By = "John";
                var data = new { By = "John! ", ForGame = "Pock" };
                notificationModel.AdditionalData = Helpers.GetJson(data);
                notificationModel.AppUserId = Convert.ToInt32(appUserId);
                //Channel like Android,Ios,Web
                notificationModel.Channel = Convert.ToInt32(DIBZ.Common.Model.Channel.Web);
                notificationModel.Content = "You offer has been accepted for " + ForGame + " game by " + By + ".";
                notificationModel.CreatedTime = DateTime.Now;
                notificationModel.LastError = "";
                notificationModel.OfferId = 4;
                notificationModel.Status = Convert.ToInt32(DIBZ.Common.Model.NotificationStatus.UnRead);
                notificationModel.Title = "Create Intrest";
                notificationModel.NotificationType = Convert.ToInt32(DIBZ.Common.Model.NotificationType.Desktop);
                notificationModel.NotificationBusinessType = Convert.ToInt32(DIBZ.Common.Model.NotificationBusinessType.AcceptOffer);

                //save notification in notification table
                notification = await notificationLogic.AddNotification(notificationModel);

                //we dont want to notify that user who create this offer
                //if (appUserId != CurrentLoginSession.ApplicationUserId)
                //{
                // sent notification of offer creater with all user which have that game
                new DIBZ.Services.ServerNotificationService().CreateOffer(appUserId, notification.Id, notificationModel.Content, notificationModel.CreatedTime, notificationModel.AdditionalData);
                //}
            }

            return this.Json("New offer has been created", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Blogs()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }

        [AuthOp]
        public async Task<ActionResult> SendContactRequest(FormCollection form)
        {
            DIBZ.Common.Model.MyQueries myQuery = new DIBZ.Common.Model.MyQueries();
            DIBZ.Common.Model.EmailTemplate emailTemplate = new DIBZ.Common.Model.EmailTemplate();
            EmailTemplateHelper templates = new EmailTemplateHelper();
            EmailTemplateResponse EmailTemplateResponce = new EmailTemplateResponse();
            DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();
            string adminEmail = string.Empty;
            string userEmail = form["email"];
            string name = form["name"];
            string phone = form["phone"];
            string subject = "Contact Us Form - " + form["subject"];
            string message = form["message"];
            var authLogic = LogicContext.Create<AuthLogic>();
            var suportQuery = LogicContext.Create<SupportQueryLogic>();
            var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();

            if (userEmail == null)
            {
                suportQuery.SaveMessages(CurrentLoginSession.ApplicationUser.Email, name, phone, subject, message, CurrentLoginSession.ApplicationUser.Id);
                userEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmailAddress"];
                await emailTemplateLogic.SaveEmailNotification(userEmail, subject, message, EmailType.Email, Priority.Low);
                //EmailHelper.Email(userEmail, subject, message);
            }
            else
            {
                suportQuery.SaveMessages(userEmail, name, phone, subject, message, null);
                adminEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmailAddress"];
                await emailTemplateLogic.SaveEmailNotification(adminEmail, subject, message, EmailType.Email, Priority.Low);
                EmailHelper.Email(adminEmail, subject, message);
                return View("ContactUs", myQuery);
            }
            return RedirectToAction("MyQueriesIndex", "MyQueries");

        }
        public async Task<ActionResult> SaveNewsLetterEmailAddress(string emailAddress)
        {
            DIBZDbContext context = new DIBZDbContext();
            var notifierEmail = (from NotifierEmails in context.NotifierEmails
                                 orderby NotifierEmails.CreatedTime descending
                                 select new
                                 {
                                     EmailAddress = NotifierEmails.EmailAddress
                                 }).ToList();


            if (notifierEmail.Count != 0)
            {
                // Email Sending to Specific Email address which is specified by DON.
                var notifierLastEmail = notifierEmail.FirstOrDefault().EmailAddress;
                EmailTemplateHelper templates = new EmailTemplateHelper();
                EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
                DIBZ.Common.Model.EmailNotification Email = new DIBZ.Common.Model.EmailNotification();

                var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();
                emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.NewsletterSubscribe);

                templates.AddParam(DIBZ.Common.Model.Contants.EmailAddress, emailAddress);
                var emailBody = templates.FillTemplate(emailTemplateResponse.Body);
                await emailTemplateLogic.SaveEmailNotification(notifierLastEmail, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.Low);
                EmailHelper.Email(notifierLastEmail, emailTemplateResponse.Title, emailBody);
            }

            DIBZ.Common.Model.NewsLetter newsLetter = new DIBZ.Common.Model.NewsLetter();
            var newsLetterLogic = LogicContext.Create<NewsLetterLogic>();
            var result = await newsLetterLogic.AddNewsLetter(emailAddress);
            if (result == true)
            {
                return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, fail = "some thing wrong" }, JsonRequestBehavior.AllowGet);
            }

        }
        public async Task<ActionResult> SavePhoneNumberNewsLetterEmailAddress(string emailAddress, string phoneNumber)
        {
            DIBZ.Common.Model.NewsLetter newsLetter = new DIBZ.Common.Model.NewsLetter();
            var newsLetterLogic = LogicContext.Create<NewsLetterLogic>();
            var result = await newsLetterLogic.AddPhoneNumberByEmail(emailAddress, phoneNumber);
            if (result > 0)
            {
                return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, fail = "some thing wrong" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllNewsFeed(int id)
        {
            IEnumerable<DIBZ.Common.Model.NewsFeed> newsFeeds = new List<DIBZ.Common.Model.NewsFeed>();
            var newsFeedLogic = LogicContext.Create<NewsFeedLogic>();
            newsFeeds = newsFeedLogic.GetAllNewsFeed();
            if (newsFeeds != null)
            {
                return Json(new { IsSuccess = true, data = newsFeeds }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, fail = "some thing wrong" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetAllDibzLocations()
        {

            var locationLogic = LogicContext.Create<LocationLogic>();
            var locations = locationLogic.GetAllLocations();
            if (locations != null)
            {
                return Json(new { IsSuccess = true, data = locations }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, fail = "some thing wrong" }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> ViewGamerDetail(int id)
        {
            DIBZ.Common.Model.ApplicationUser appUser = new DIBZ.Common.Model.ApplicationUser();
            var authLogic = LogicContext.Create<AuthLogic>();
            appUser = await authLogic.GetAppUserWithScorecardById(id);
            return Json(new { IsSuccess = true, GamerNickName = appUser.NickName, GamerImageId = (appUser.ProfileImageId.HasValue) ? appUser.ProfileImageId.Value : 0, Proposal = appUser.Scorecard.Proposals, NoShows = appUser.Scorecard.NoShows, GamesSent = appUser.Scorecard.GamesSent, TestFail = appUser.Scorecard.TestFails, TestPass = appUser.Scorecard.TestPass, DIBz = appUser.Scorecard.DIBz }, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> SendForgotPasswordEmailNotification(string emailAddress)
        {
            DIBZ.Common.Model.ApplicationUser appUser = new DIBZ.Common.Model.ApplicationUser();
            var authLogic = LogicContext.Create<AuthLogic>();
            var result = authLogic.GetApplicationUserByEmail(emailAddress);
            if (result != null)
            {
                EmailTemplateHelper templates = new EmailTemplateHelper();
                EmailTemplateResponse emailTemplateResponse = new EmailTemplateResponse();
                DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();
                var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();

                emailTemplateResponse = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.ForgotPassword);
                templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, result.NickName);
                templates.AddParam(DIBZ.Common.Model.Contants.ForgotPassword, string.Format("<a href='{0}'>Here</a>", string.Concat(Request.Url.AbsoluteUri.Substring(0, Request.Url.AbsoluteUri.LastIndexOf("/") + 1), "ChangePassword?id=" + result.Id)));
                templates.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>link</a>", hostName + "/Dashboard/ContactUs"));
                var emailBody = templates.FillTemplate(emailTemplateResponse.Body);

                //save email data in table
                await emailTemplateLogic.SaveEmailNotification(result.Email, emailTemplateResponse.Title, emailBody, EmailType.Email, Priority.High);
                EmailHelper.Email(result.Email, emailTemplateResponse.Title, emailBody);
                return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, fail = "some thing wrong" }, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> ChangePassword(int id)
        {
            return View("ChangePassword");
        }
        public async Task<ActionResult> SetNewPassword(string id, string password)//FormCollection formData
        {
            var authLogic = LogicContext.Create<AuthLogic>();
            var result = await authLogic.ChangePassword(ConversionHelper.SafeConvertToInt32(id), password);

            if (result > 0)
            {
                return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { IsSuccess = false, fail = "some thing wrong" }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> ChangePasswordUser(int id, string newPassword, string currentPassword)
        {
            var authLogic = LogicContext.Create<AuthLogic>();
            var appUser = await authLogic.GetApplicationUserById(CurrentLoginSession.ApplicationUser.Id);
            if (appUser == null || !Helpers.ValidateHash(currentPassword, appUser.Password))
                return Json(new { IsSuccess = false, fail = "some thing wrong" }, JsonRequestBehavior.AllowGet);
            else
            {
                await authLogic.ChangePassword(ConversionHelper.SafeConvertToInt32(id), newPassword);
                return Json(new { IsSuccess = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public static void MailChimpsSubs(string email, string firstName, string surname, string phone)
        {
                string mailChimpApiKey = System.Configuration.ConfigurationManager.AppSettings["MailChimpApiKey"];
                string mailChimpListId = System.Configuration.ConfigurationManager.AppSettings["MailChimpListId"];

                var mailChimp = new MCApi(mailChimpApiKey, true);

                var lst = mailChimp.ListSubscribe(mailChimpListId, email,
                                        new List.Merges {
                                {"FNAME", firstName},
                                {"LNAME", surname},
                                { "PHONE", phone }
                                        }, new List.SubscribeOptions()
                                        {
                                            DoubleOptIn = false,
                                            SendWelcome = false,
                                            UpdateExisting = true
                                        });            
        }
    }
}