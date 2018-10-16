using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.Model;
using DIBZ.Common;
using DIBZ.Logic;
using System.Threading;
using DIBZ.Logic.Scorecard;
using DIBZ.Logic.Offer;
using DIBZ.Logic.Swap;

namespace DIBZ.EmailService
{
    public class EmailSender//:BaseService 
    {
        private Queue<DIBZ.Common.Model.EmailNotification> EmailQueue;
        public LogicContext LogicContext { get; set; }
        string hostName = System.Web.HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

        public EmailSender()
        {
            EmailQueue = new Queue<DIBZ.Common.Model.EmailNotification>();
        }
        public void ProcessOnEmailQueue()
        {
            while (EmailQueue.Count() > 0)
            {
                Console.WriteLine("Processsing email queue....");
                LogHelper.LogInfo("Processsing email queue....");

                DIBZ.Common.Model.EmailNotification SendEmail = EmailQueue.Peek();

                if (SendEmail.EmailType == EmailType.Email)
                {
                    try
                    {
                        //var emailTemplateLogic = Context.Create<EmailTemplateLogic>();
                        LogicContext = new LogicContext();
                        var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();

                        Console.WriteLine("sending queued email....");
                        LogHelper.LogInfo("sending queued email....");
                        EmailHelper.Email(SendEmail.ApplicationUserEmail, SendEmail.Tiltle, SendEmail.Body);

                        Console.WriteLine("Marking email status to sent....");
                        LogHelper.LogInfo("Marking email status to sent....");
                        emailTemplateLogic.UpdateEmailNotificationStatusById(SendEmail.Id);

                        EmailQueue.Dequeue();
                    }
                    catch (System.Exception ex)
                    {
                        LogHelper.LogError(ex.Message, ex);
                    }
                }
            }
        }

        public void GetPendingEmails()
        {
            try
            {
                LogicContext = new LogicContext();
                var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();

                Console.WriteLine("Fetching all pending email notifications....");
                LogHelper.LogInfo("Fetching all pending email notifications....");

                var emailData = emailTemplateLogic.GetUnSendEmails();

                Console.WriteLine("Enqueing all pending email notifications....");
                LogHelper.LogInfo("Enqueing all pending email notifications....");
                emailData.ForEach(o => EmailQueue.Enqueue(o));
            }
            catch (System.Exception ex)
            {
                LogHelper.LogError(ex.Message, ex);
            }

        }

        public async Task SavePeriodictEmailNotificationsAndUpdateScoreCardAndOfferStatus()
        {
            Console.WriteLine("In SavePeriodictEmailNotificationsAndUpdateScoreCardAndOfferStatus method ....");
            LogHelper.LogInfo("In SavePeriodictEmailNotificationsAndUpdateScoreCardAndOfferStatus method ....");

                      
            DIBZ.Common.DTO.EmailTemplateResponse emailTemplateReminder = new Common.DTO.EmailTemplateResponse();
            DIBZ.Common.DTO.EmailTemplateResponse emailTemplatePaymentRunningOut = new Common.DTO.EmailTemplateResponse();

            try
            {
                //var emailTemplateLogic = Context.Create<EmailTemplateLogic>();

                LogicContext = new LogicContext();
                var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();

                Console.WriteLine("fetching all swaps with offer status payment needed....");
                LogHelper.LogInfo("fetching all swaps with offer status payment needed....");

                var swapsWithPaymentNeeded = await emailTemplateLogic.GetAllSwapsWithPaymentNeeded();

                
                Console.WriteLine("fetched payment needed email template....");
                LogHelper.LogInfo("fetched payment needed email template....");

                var scorecardLogic = LogicContext.Create<ScorecardLogic>();
                var offerLogic = LogicContext.Create<OfferLogic>();
                foreach (var item in swapsWithPaymentNeeded)
                {
                    var timeElapsed = DateTime.Now.Subtract(DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(item.CreatedTime)).TotalHours;
                    // saving for gamer offerer
                    /*if (!item.Offer.ApplicationUser.Transactions.Any(o => o.OfferId == item.OfferId) 
                        && (timeElapsed > SystemSettings.PeriodicEmailHour && timeElapsed < (SystemSettings.PeriodicEmailHour * 3) - 12))
                    {
                        EmailTemplateHelper templates = new EmailTemplateHelper();

                        templates.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.Offer.ApplicationUser.NickName);

                        var emailBody = templates.FillTemplate(emailTemplateReminder.Body);

                        Console.WriteLine("saving periodic reminder email notification for the offerer....");
                        LogHelper.LogInfo("saving periodic reminder email notification for the offerer....");

                        await emailTemplateLogic.SaveEmailNotification(item.Offer.ApplicationUser.Email, emailTemplateReminder.Title, emailBody, EmailType.Email, Priority.Medium);
                    }
                    else if (!item.Offer.ApplicationUser.Transactions.Any(o => o.OfferId == item.OfferId) && timeElapsed > SystemSettings.PaymentTimeInHours - 6)
                    {
                        emailTemplateReminder = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.FinalPaymentReminder);
                        EmailTemplateHelper templates = new EmailTemplateHelper();

                        templates.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.Offer.ApplicationUser.NickName);

                        var emailBody = templates.FillTemplate(emailTemplateReminder.Body);

                        Console.WriteLine("saving final reminder email notification for the offerer....");
                        LogHelper.LogInfo("saving final reminder email notification for the offerer....");

                        await emailTemplateLogic.SaveEmailNotification(item.Offer.ApplicationUser.Email, emailTemplateReminder.Title, emailBody, EmailType.Email, Priority.High);
                    }

                    // saving for gamer swapper
                    if (!item.GameSwapPserson.Transactions.Any(o => o.OfferId == item.OfferId) 
                        && (timeElapsed > SystemSettings.PeriodicEmailHour && timeElapsed < (SystemSettings.PeriodicEmailHour * 3) - 12))
                    {
                        EmailTemplateHelper templates = new EmailTemplateHelper();

                        templates.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.GameSwapPserson.NickName);

                        var emailBody = templates.FillTemplate(emailTemplateReminder.Body);

                        Console.WriteLine("saving periodic reminder email notification for the swapper....");
                        LogHelper.LogInfo("saving periodic reminder email notification for the swapper....");

                        await emailTemplateLogic.SaveEmailNotification(item.GameSwapPserson.Email, emailTemplateReminder.Title, emailBody, EmailType.Email, Priority.Medium);
                    }
                    else if (!item.GameSwapPserson.Transactions.Any(o => o.OfferId == item.OfferId) && timeElapsed > SystemSettings.PaymentTimeInHours - 6)
                    {
                        emailTemplateReminder = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.FinalPaymentReminder);

                        EmailTemplateHelper templates = new EmailTemplateHelper();

                        templates.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.GameSwapPserson.NickName);

                        var emailBody = templates.FillTemplate(emailTemplateReminder.Body);

                        Console.WriteLine("saving final reminder email notification for the swapper....");
                        LogHelper.LogInfo("saving final reminder email notification for the swapper....");

                        await emailTemplateLogic.SaveEmailNotification(item.GameSwapPserson.Email, emailTemplateReminder.Title, emailBody, EmailType.Email, Priority.High);

                    }*/


                    //Reminder email sent to the party who doesn't make the payment after 30 minutes
                    //sent to the party who has not paid
                    // saving for gamer offerer
                    if (!item.Offer.ApplicationUser.Transactions.Any(o => o.OfferId == item.OfferId) && (timeElapsed > SystemSettings.SrvcPaymentTimeInHours / 2 && timeElapsed < SystemSettings.SrvcPaymentTimeInHours))
                    {
                        emailTemplateReminder = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.FinalPaymentReminder);

                        EmailTemplateHelper templates = new EmailTemplateHelper();

                        templates.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.Offer.ApplicationUser.NickName);

                        var emailBody = templates.FillTemplate(emailTemplateReminder.Body);

                        Console.WriteLine("saving final reminder email notification for the offerer....");
                        LogHelper.LogInfo("saving final reminder email notification for the offerer....");

                        await emailTemplateLogic.SaveEmailNotification(item.Offer.ApplicationUser.Email, emailTemplateReminder.Title, emailBody, EmailType.Email, Priority.High);
                        EmailHelper.Email(item.Offer.ApplicationUser.Email, emailTemplateReminder.Title, emailBody);
                    }
                    // saving for gamer swapper
                    if (!item.GameSwapPserson.Transactions.Any(o => o.OfferId == item.OfferId) && (timeElapsed > SystemSettings.SrvcPaymentTimeInHours / 2 && timeElapsed < SystemSettings.SrvcPaymentTimeInHours))
                    {
                        emailTemplateReminder = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.FinalPaymentReminder);

                        EmailTemplateHelper templates = new EmailTemplateHelper();

                        templates.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templates.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.GameSwapPserson.NickName);

                        var emailBody = templates.FillTemplate(emailTemplateReminder.Body);

                        Console.WriteLine("saving final reminder email notification for the swapper....");
                        LogHelper.LogInfo("saving final reminder email notification for the swapper....");

                        await emailTemplateLogic.SaveEmailNotification(item.GameSwapPserson.Email, emailTemplateReminder.Title, emailBody, EmailType.Email, Priority.High);
                        EmailHelper.Email(item.GameSwapPserson.Email, emailTemplateReminder.Title, emailBody);
                    }

                    //When time has ran out : Sent to both parties in the swap
                    if (item.Offer.ApplicationUser.Transactions.Count() == 0 && timeElapsed > SystemSettings.SrvcPaymentTimeInHours)
                    {
                        await offerLogic.UpdateOfferStatusToPending(item.OfferId);
                        await scorecardLogic.UpdateScoreCardOfApplicationUserWithNoShow(item.Offer.ApplicationUserId);
                        await scorecardLogic.UpdateScoreCardOfApplicationUserWithNoShow(item.GameSwapPsersonId);

                        emailTemplatePaymentRunningOut = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.PaymentTimeRanOut);
                        // to offerer

                        EmailTemplateHelper templatesForOfferer = new EmailTemplateHelper();

                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.Offer.ApplicationUser.NickName);

                        var emailBody = templatesForOfferer.FillTemplate(emailTemplatePaymentRunningOut.Body);

                        Console.WriteLine("saving final reminder email notification for the Offerer....");
                        LogHelper.LogInfo("saving final reminder email notification for the Offerer....");

                        await emailTemplateLogic.SaveEmailNotification(item.Offer.ApplicationUser.Email, emailTemplatePaymentRunningOut.Title, emailBody, EmailType.Email, Priority.High);
                        EmailHelper.Email(item.Offer.ApplicationUser.Email, emailTemplatePaymentRunningOut.Title, emailBody);

                        // to swapper

                        EmailTemplateHelper templatesForSwapper = new EmailTemplateHelper();

                        templatesForSwapper.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templatesForSwapper.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templatesForSwapper.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.GameSwapPserson.NickName);

                        var emailBodyForSwapper = templatesForSwapper.FillTemplate(emailTemplatePaymentRunningOut.Body);

                        Console.WriteLine("saving final reminder email notification for the Offerer....");
                        LogHelper.LogInfo("saving final reminder email notification for the Offerer....");
                        await emailTemplateLogic.SaveEmailNotification(item.GameSwapPserson.Email, emailTemplatePaymentRunningOut.Title, emailBodyForSwapper, EmailType.Email, Priority.High);
                        EmailHelper.Email(item.GameSwapPserson.Email, emailTemplatePaymentRunningOut.Title, emailBodyForSwapper);
                    }
                }
                await Task.Delay((int)((SystemSettings.SrvcPaymentTimeInHours*60)/2)*60000);
                
            }
            catch (Exception ex)
            {

                LogHelper.LogError(ex.Message, ex);
            }

            
        }

        public async Task SaveExpiryDayRuleEmailNotificationsAndUpdateScoreCardAndOfferStatus()
        {
            Console.WriteLine("In SaveExpiryDayRuleEmailNotificationsAndUpdateScoreCardAndOfferStatus method ....");
            LogHelper.LogInfo("In SaveExpiryDayRuleEmailNotificationsAndUpdateScoreCardAndOfferStatus method ....");

            
            DIBZ.Common.DTO.EmailTemplateResponse emailTemplateExpiryDayRule = new Common.DTO.EmailTemplateResponse();

            try
            {
                //var emailTemplateLogic = Context.Create<EmailTemplateLogic>();

                LogicContext = new LogicContext();
                var emailTemplateLogic = LogicContext.Create<EmailTemplateLogic>();

                Console.WriteLine("fetching all swaps with offer status payment done....");
                LogHelper.LogInfo("fetching all swaps with offer status payment done....");

                var swapsWithPaymentDone = await emailTemplateLogic.GetAllSwapsWithPaymentDone();


                Console.WriteLine("fetched expiry day rule email template....");
                LogHelper.LogInfo("fetched expiry day rule email template....");

                var scorecardLogic = LogicContext.Create<ScorecardLogic>();
                var offerLogic = LogicContext.Create<OfferLogic>();
                var swapLogic = LogicContext.Create<SwapLogic>();
                foreach (var item in swapsWithPaymentDone)
                {
                    emailTemplateExpiryDayRule = await emailTemplateLogic.GetEmailTemplate(EmailType.Email, EmailContentType.ExpiryDayRule);
                    var getWorkingDaysCount = swapLogic.GetWorkingDaysCount(ConversionHelper.ConvertDateToTimeZone(item.CreatedTime));
                    
                    //Condition when both party hasn't sent game on time.
                    if (item.Offer.OfferStatus == OfferStatus.Accept
                        && item.Offer.Transactions.Count() == 2
                        && (item.SwapStatus == SwapStatus.Payment_Done_By_Offerer || item.SwapStatus == SwapStatus.Payment_Done_By_Swapper)
                        && getWorkingDaysCount > SystemSettings.SrvcDayRule)
                    {
                        await scorecardLogic.UpdateScoreCardOfApplicationUserWithNoShow(item.GameSwapPsersonId);
                        await scorecardLogic.UpdateScoreCardOfApplicationUserWithNoShow(item.Offer.ApplicationUserId);
                        await swapLogic.UpdateOfferStatusToPendingAndSetSwapToInActiveAndRemoveTransactionIfAny(item.OfferId, item.Id);

                        

                        // to offerer

                        EmailTemplateHelper templatesForOfferer = new EmailTemplateHelper();

                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.Offer.ApplicationUser.NickName);
                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>link</a>", hostName + "/Dashboard/ContactUs"));

                        var emailBody = templatesForOfferer.FillTemplate(emailTemplateExpiryDayRule.Body);

                        Console.WriteLine("saving expiry dary rule email notification for the Offerer....");
                        LogHelper.LogInfo("saving expiry dary rule email notification for the Offerer....");

                        await emailTemplateLogic.SaveEmailNotification(item.Offer.ApplicationUser.Email, emailTemplateExpiryDayRule.Title, emailBody, EmailType.Email, Priority.High);
                        EmailHelper.Email(item.Offer.ApplicationUser.Email, emailTemplateExpiryDayRule.Title, emailBody);

                        // to swapper
                        EmailTemplateHelper templatesForSwapper = new EmailTemplateHelper();

                        templatesForSwapper.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templatesForSwapper.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templatesForSwapper.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.GameSwapPserson.NickName);
                        templatesForSwapper.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>link</a>", hostName + "/Dashboard/ContactUs"));

                        var emailBodyForSwapper = templatesForSwapper.FillTemplate(emailTemplateExpiryDayRule.Body);

                        Console.WriteLine("saving expiry dary rule email notification for the swapper....");
                        LogHelper.LogInfo("saving expiry dary rule email notification for the swapper....");
                        await emailTemplateLogic.SaveEmailNotification(item.GameSwapPserson.Email, emailTemplateExpiryDayRule.Title, emailBodyForSwapper, EmailType.Email, Priority.High);
                        EmailHelper.Email(item.GameSwapPserson.Email, emailTemplateExpiryDayRule.Title, emailBodyForSwapper);
                    }

                    //Condition when offerer party hasn't sent game on time.
                    if (item.Offer.OfferStatus == OfferStatus.Accept
                        && item.Offer.Transactions.Count() == 2
                        && (item.SwapStatus != SwapStatus.Payment_Done_By_Offerer || item.SwapStatus != SwapStatus.Payment_Done_By_Swapper && item.SwapStatus==SwapStatus.Game2_Received)
                        && getWorkingDaysCount > SystemSettings.SrvcDayRule)
                    {
                        await scorecardLogic.UpdateScoreCardOfApplicationUserWithNoShow(item.Offer.ApplicationUserId);
                        await swapLogic.UpdateOfferStatusToPendingAndSetSwapToInActiveAndRemoveTransactionIfAny(item.OfferId, item.Id);

                        EmailTemplateHelper templatesForOfferer = new EmailTemplateHelper();

                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.Offer.ApplicationUser.NickName);
                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>link</a>", hostName + "/Dashboard/ContactUs"));

                        var emailBody = templatesForOfferer.FillTemplate(emailTemplateExpiryDayRule.Body);

                        Console.WriteLine("saving expiry dary rule email notification for the Offerer....");
                        LogHelper.LogInfo("saving expiry dary rule email notification for the Offerer....");

                        await emailTemplateLogic.SaveEmailNotification(item.Offer.ApplicationUser.Email, emailTemplateExpiryDayRule.Title, emailBody, EmailType.Email, Priority.High);
                        EmailHelper.Email(item.Offer.ApplicationUser.Email, emailTemplateExpiryDayRule.Title, emailBody);
                    }

                    //Condition when swapper party hasn't sent game on time.
                    if (item.Offer.OfferStatus == OfferStatus.Accept
                        && item.Offer.Transactions.Count() == 2
                        && (item.SwapStatus != SwapStatus.Payment_Done_By_Offerer || item.SwapStatus != SwapStatus.Payment_Done_By_Swapper && item.SwapStatus == SwapStatus.Game1_Received)
                        && getWorkingDaysCount > SystemSettings.SrvcDayRule)
                    {
                        await scorecardLogic.UpdateScoreCardOfApplicationUserWithNoShow(item.GameSwapPsersonId);
                        await swapLogic.UpdateOfferStatusToPendingAndSetSwapToInActiveAndRemoveTransactionIfAny(item.OfferId, item.Id);

                        EmailTemplateHelper templatesForOfferer = new EmailTemplateHelper();

                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.OfferedGame, item.Offer.GameCatalog.Name);
                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.SwappedGame, item.GameSwapWith.Name);
                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.AppUserNickName, item.GameSwapPserson.NickName);
                        templatesForOfferer.AddParam(DIBZ.Common.Model.Contants.UrlContactUs, string.Format("<a href='{0}'>link</a>", hostName + "/Dashboard/ContactUs"));

                        var emailBody = templatesForOfferer.FillTemplate(emailTemplateExpiryDayRule.Body);

                        Console.WriteLine("saving expiry dary rule email notification for the Offerer....");
                        LogHelper.LogInfo("saving expiry dary rule email notification for the Offerer....");

                        await emailTemplateLogic.SaveEmailNotification(item.GameSwapPserson.Email, emailTemplateExpiryDayRule.Title, emailBody, EmailType.Email, Priority.High);
                        EmailHelper.Email(item.GameSwapPserson.Email, emailTemplateExpiryDayRule.Title, emailBody);
                    }
                }

                await Task.Delay(SystemSettings.SrvcDayRuleRunningTimeInMS);
            }
            catch (Exception ex)
            {

                LogHelper.LogError(ex.Message, ex);
            }
        }
       
       
        public void SendPeriodictMediumPriorityEmailNotifications()
        {
            //var emailTemplateLogic = Context.Create<EmailTemplateLogic>();
            //var emailData = emailTemplateLogic.GetPeriodictMediumPriorityEmailNotifications();
        }
        public void SendPeriodicUrgentPriorityEmailNotifications()
        {
            //var emailTemplateLogic = Context.Create<EmailTemplateLogic>();
            //var emailData = emailTemplateLogic.GetPeriodictHighPriorityEmailNotifications();
        }
    }
}
