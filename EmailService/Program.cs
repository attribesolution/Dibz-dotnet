using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DIBZ.Common.Model;
using DIBZ.Common;
using DIBZ.EmailService;
using System.Threading;

namespace DIBZ.EmailService
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            try
            {
                Console.WriteLine("Email notification sender service started ....");
                LogHelper.LogInfo("Email notification sender service started ....");

                EmailSender emailSender = new EmailSender();

                Task.Factory.StartNew(() => emailSender.SavePeriodictEmailNotificationsAndUpdateScoreCardAndOfferStatus());

                Task.Factory.StartNew(() => emailSender.SaveExpiryDayRuleEmailNotificationsAndUpdateScoreCardAndOfferStatus());

                do
                {
                    emailSender.GetPendingEmails();
                    Thread.Sleep(30000);
                    emailSender.ProcessOnEmailQueue();
                } while (true);
            }
            catch (Exception ex)
            {
                LogHelper.LogError(ex.Message, ex);
            }
        }
    }
}
