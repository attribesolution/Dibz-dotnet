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
    public class CompetitionController : BaseWebController
    {
        string hostName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        // public ActionResult Index()
        public async Task<ActionResult> Index()
        {
            //var competitionLogic = LogicContext.Create<CompetitionLogic>();
            var competitionLogic = LogicContext.Create<CompetitionLogic>();
            ViewBag.CompetitionData = await competitionLogic.GetAllCompetitionData();

            return View();
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

                    templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, nickName);
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