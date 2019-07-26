using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace DIBZ.Common
{
    public static class EmailHelper
    {
        static string BccEmail = System.Configuration.ConfigurationManager.AppSettings["BccEmailAddress"].ToString();
        static string adminEmail = System.Configuration.ConfigurationManager.AppSettings["AdminEmailAddress"].ToString();
        static string prefix = System.Configuration.ConfigurationManager.AppSettings["EmailSubjectPrefix"].ToString();
        static string NotificationEmailAdmin = System.Configuration.ConfigurationManager.AppSettings["NotificationEmailAdmin"].ToString();

        public static async Task SendEmail(string to, string subject, string message)
        {
            var email = new MailMessage();
            email.To.Add(to);
            email.BodyEncoding = Encoding.UTF8;
            email.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            email.Body = message;
            email.IsBodyHtml = true;
            email.Subject = subject;
            var client = new SmtpClient();

            using (var cts = new CancellationTokenSource(10000))
            {
                await client.SendMailExAsync(email, cts.Token);
            }

        }
        public static void SendEmails(string to, string subject, string message, AlternateView alternateView, AlternateView plainView)
        {
            var email = new MailMessage();
            email.To.Add(to);
            email.BodyEncoding = Encoding.UTF8;
            email.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            //email.Body = message;
            email.IsBodyHtml = true;
            email.Subject = subject;
            email.AlternateViews.Add(alternateView);
            email.AlternateViews.Add(plainView);

            var client = new SmtpClient();

            using (var cts = new CancellationTokenSource(10000))
            {
                 client.SendMailExAsync(email, cts.Token);
            }
        }

        public static void Email(string toEmail, string subject, string body)
        {
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(toEmail));

            string[] multipleEmailIds = BccEmail.Split(',');
            foreach (string emailId in multipleEmailIds)
            {
                if (toEmail != emailId)
                {
                    message.Bcc.Add(new MailAddress(emailId));
                }
            }
            message.Subject = prefix + subject;
            message.Body = body;
            message.IsBodyHtml = true;
            SendEmail(message);
        }

        public static void NotificationToAdmin(string NewUserEmail, string Name, string fromPage)
        {
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(NotificationEmailAdmin));

            string[] multipleEmailIds = BccEmail.Split(',');
            foreach (string emailId in multipleEmailIds)
            {
                if (adminEmail != emailId)
                {
                    message.Bcc.Add(new MailAddress(emailId));
                }
            }

            message.Subject = prefix + "New user registration from "+ fromPage+ " page";
            message.Body = "<html><div><br /> Email: " + NewUserEmail + " <br /> Timestamp: " + DateTime.Now + "</div></html>";
            
            message.IsBodyHtml = true;
            SendEmail(message);
        }

        public static async Task SendEmailBcc(List<string> to, string subject, string message)
        {
            var email = new MailMessage();
            foreach (var emailAddress in to)
            {
                email.Bcc.Add(emailAddress);
            }

            email.BodyEncoding = Encoding.UTF8;
            email.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            email.Body = message;
            email.Subject = subject;
            var client = new SmtpClient();
            //Helper.LogInfo("In Email Sending");
            using (var cts = new CancellationTokenSource(60000))
            {
                await client.SendMailExAsync(email, cts.Token);
            }
            //Helper.LogInfo("In Email Sending Complete");
        }


        public static Task SendMailExAsync(
       this System.Net.Mail.SmtpClient @this,
       System.Net.Mail.MailMessage message,
       CancellationToken token = default(CancellationToken))
        {
            // use Task.Run to negate SynchronizationContext
            return Task.Run(() => SendMailExImplAsync(@this, message, token));
        }

        private static async Task SendMailExImplAsync(
            System.Net.Mail.SmtpClient client,
            System.Net.Mail.MailMessage message,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var tcs = new TaskCompletionSource<bool>();
            System.Net.Mail.SendCompletedEventHandler handler = null;
            Action unsubscribe = () => client.SendCompleted -= handler;

            handler = async (s, e) =>
            {
                unsubscribe();

                // a hack to complete the handler asynchronously
                await Task.Yield();

                if (e.UserState != tcs)
                    tcs.TrySetException(new InvalidOperationException("Unexpected UserState"));
                else if (e.Cancelled)
                    tcs.TrySetCanceled();
                else if (e.Error != null)
                    tcs.TrySetException(e.Error);
                else
                    tcs.TrySetResult(true);
            };

            client.SendCompleted += handler;
            try
            {
                client.SendAsync(message, tcs);
                using (token.Register(() => client.SendAsyncCancel(), useSynchronizationContext: false))
                {
                    await tcs.Task;
                }
            }
            finally
            {
                unsubscribe();
            }
        }


        public static void EmailAttachement(string toEmail, string subject, string body, string attachment)
        {
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(toEmail));
            message.Bcc.Add(new MailAddress(BccEmail));
            message.Subject = prefix + subject;
            message.Body = body;
            message.IsBodyHtml = true;
            if (attachment != string.Empty)
            {
                message.Attachments.Add(new Attachment(attachment));
            }
            SendEmail(message);
        }
        private static void SendEmail(MailMessage message)
        {
            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    client.SendCompleted += new SendCompletedEventHandler(client_SendCompleted);
                    client.Send(message);
                }
            }
            catch (Exception ex)
            {

                LogHelper.LogError(ex.Message, ex);
            }




        }


        private static void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

        }
    }
}
