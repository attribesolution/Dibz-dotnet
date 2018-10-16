using DIBZ.Common.Model;
using DIBZ.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Data;

namespace DIBZ.Logic
{
 public  class EmailTemplateLogic : BaseLogic
    {

        public EmailTemplateLogic(LogicContext context) : base(context)
        {
        }
        
        public async Task<EmailTemplateResponse> GetEmailTemplate(EmailType emailType, EmailContentType emailContentType)
        {
            var template = (await Db.Query<EmailTemplate>(e => !e.IsDeleted && e.EmailType == emailType && e.EmailContentType == emailContentType).QueryAsync()).FirstOrDefault();
            if (template != null)
            {
                return new EmailTemplateResponse()
                {
                    Title = template.Title,  
                     Body = template.Body,
                };
            }
            else
                return null;
        }
        public async Task<int> SaveEmailNotification(string ApplicationUserEmail, string title, string body, EmailType emailType, Priority priority)
        {
            DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();
            email.Tiltle = title;
            email.Body = body;
            email.ApplicationUserEmail = ApplicationUserEmail;
            email.EmailType = emailType;
            email.Priority = priority;
            email.IsActive = true;
            email.IsDeleted = false;
            email.IsSend = false;
            Db.Add(email);
           return await Db.SaveAsync(); 
        }

        public async Task<bool> SaveEmailNotificationList(List<string> ApplicationUserEmail, string title, string body, EmailType emailType, Priority priority)
        {
            foreach (var emailAddress in ApplicationUserEmail)
            {

                DIBZ.Common.Model.EmailNotification email = new DIBZ.Common.Model.EmailNotification();
                email.Tiltle = title;
                email.Body = body;
                email.ApplicationUserEmail = emailAddress;
                email.EmailType = emailType;
                email.Priority = priority;
                email.IsActive = true;
                email.IsDeleted = false;
                email.IsSend = false;
                Db.Add(email);
                await Db.SaveAsync();
            }
            return true;
        }

        public List<DIBZ.Common.Model.EmailNotification> GetUnSendEmails()
        { 
            var response = Db.Query<DIBZ.Common.Model.EmailNotification>(o => !o.IsDeleted && !o.IsSend).OrderBy(e=> e.CreatedTime).OrderBy(o=>o.Priority).Take(20).ToList();
            return response;
        }

        public async Task< List<DIBZ.Common.Model.Swap>> GetAllSwapsWithPaymentNeeded()
        {
            var swapData = (await Db.Query<DIBZ.Common.Model.Swap>(o => !o.IsDeleted && o.Offer.OfferStatus == OfferStatus.PaymentNeeded && o.Offer.Transactions.Count < 2).QueryAsync()).ToList();
            return swapData;
            //&& DateTime.Now.Subtract(DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(o.CreatedTime)).TotalHours>12
            //var swapFilteredData= swapData.Select(o => DateTime.Now.Subtract(DIBZ.Common.ConversionHelper.ConvertDateToTimeZone(o.CreatedTime)).TotalHours > 12).ToList();
            //return swapFilteredData;
           
            //List<DIBZ.Common.Model.ApplicationUser> appUserList= swapData.Where(o => o.Offer.ApplicationUser.Transactions.Any()).Select(o => o.Offer.ApplicationUser).ToList();
            //appUserList.AddRange(swapData.Where(o => o.GameSwapPserson.Transactions.Any()).Select(o => o.GameSwapPserson).ToList());


        }

        public async Task<List<DIBZ.Common.Model.Swap>> GetAllSwapsWithPaymentDone()
        {
            var swapData = (await Db.Query<DIBZ.Common.Model.Swap>(o => !o.IsDeleted && o.Offer.OfferStatus == OfferStatus.Accept && o.Offer.Transactions.Count == 2).QueryAsync()).ToList();
            return swapData;
           

        }

        public int UpdateEmailNotificationStatusById(int id)
        {
            EmailNotification emailNotification = null;
            emailNotification = GetEmailNotificationById(id);
            emailNotification.IsSend = true;
            return Db.Save();


        }
        public EmailNotification GetEmailNotificationById(int id)
        {
           return  Db.Query<EmailNotification>(c => c.Id == id).FirstOrDefault();
        }

    }
}
