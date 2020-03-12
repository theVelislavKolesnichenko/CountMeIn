using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading;
using System.Web;
using System.Web.Configuration;

namespace Utils
{
    public class NotificationManager
    {
        private const char EmailsSeparator = ',';
        private static readonly char[] EmailsSeparators = new char[] { EmailsSeparator };

        #region Fields
        private static string notificationEmail;
        private static string smtpServer;
        private static int port;
        private static bool defaultCredentials;
        private static string password;
        private static string username;
        private static string displayName;
        
        #endregion

        #region Constructors

        static NotificationManager()
        {

            Configuration config;
            try
            {
                // For Web Application read mail settings from the web.config file
                if (HttpContext.Current != null)
                {
                    config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath); // web.config
                }
                else
                {
                    // For Email Remind Windows Service read mail settings from the app.config file
                    config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); // app.config    
                }

                MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
                notificationEmail = settings.Smtp.From;
                smtpServer = settings.Smtp.Network.Host;
                port = settings.Smtp.Network.Port;
                defaultCredentials = settings.Smtp.Network.DefaultCredentials;
                password = settings.Smtp.Network.Password;
                username = settings.Smtp.Network.UserName;
            }
            catch
            {
                // For Email Remind Windows Service read mail settings from the app.config file
                //config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None); // app.config
            }
            displayName = WebConfigurationManager.AppSettings["DefaultEmailName"];
        }

        #endregion

        #region Send Email Methods

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static string SendEmail(string displayName, string from, string to, string subject, string body)
        {
            return SendEmail(displayName, from, to, null, null, subject, body, null);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static string SendEmail(string to, string subject, string body)
        {
            return SendEmail(displayName, notificationEmail, to, null, null, subject, body, null);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="to">To.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <returns></returns>
        public static string SendEmail(string name, string from, string to, string cc, string subject, string body , string[] attachments)
        {
            return SendEmail(name, from, to, cc, null, subject, body, null, attachments);
        }

        /// <summary>
        /// Sends the email.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="cc">The cc.</param>
        /// <param name="bcc">The BCC.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="body">The body.</param>
        /// <param name="replyTo">The reply to.</param>
        /// <returns></returns>
        public static string SendEmail(string displayName, string from, string to, string cc, string bcc, string subject, string body, string replyTo , string[] attachmentFiles = null)
        {
            NotificationEmailItem emailItem = new NotificationEmailItem
            {
                From = from,
                DisplayName = displayName,
                To = to,
                CC = cc,
                BCC = bcc,
                ReplyTo = replyTo,
                AttachmentFiles = attachmentFiles,

                Subject = subject,
                Body = body
            };

            return SendEmail(emailItem);
        }

        private static void AddAddressesToCollection(string emailsText, MailAddressCollection addressCollection)
        {
            if (!string.IsNullOrEmpty(emailsText))
            {
                string[] emails = emailsText.Split(EmailsSeparators, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < emails.Length; i++)
                {
                    addressCollection.Add(emails[i]);
                }
            }
        }

        public static string SendEmail(NotificationEmailItem emailItem)
        {
            //return SendEmail(emailItem.DisplayName, emailItem.From, emailItem.To, emailItem.CC, emailItem.BCC, emailItem.Subject, emailItem.Body, emailItem.ReplyTo, emailItem.AttachmentFiles);      

            SmtpClient smtp = new SmtpClient(smtpServer);
            //smtp.EnableSsl = true;

            if (smtpServer != null)
            {
                smtp.Host = smtpServer;
                smtp.Port = port;
                smtp.UseDefaultCredentials = defaultCredentials;
            }

            if (!defaultCredentials)
            {
                System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(username, password);
                smtp.Credentials = credentials;
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["TestEmail"]))
            {
                emailItem.Body += emailItem.To;
                emailItem.To = ConfigurationManager.AppSettings["TestEmail"];
            }

            using (MailMessage mailMessage = emailItem.MailMessage)
            {
                //Note: When you make "IsBodyHtml" to true make  ValidateRequest="false" in page derective to make HTML content passed.
                mailMessage.IsBodyHtml = true;
                //mailMessage.Headers.Add("Content-Type", "text/html; charset=windows-1251");
                
                try
                {
                    smtp.Send(mailMessage);
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    throw ex;
                    //return LogManager.GetErrorText(ex);
                }
            }
        }

        public static void SendEmails(IEnumerable<NotificationEmailItem> emailItems)
        {
            foreach (NotificationEmailItem emailItem in emailItems)
            {
                SendEmail(emailItem);
            }
        }

        public static void SendAsyncEmails(IEnumerable<NotificationEmailItem> emailItems)
        {
            Thread sendEmailsJob = new Thread(delegate()
            {
                SendEmails(emailItems);
            });

            sendEmailsJob.IsBackground = true;
            sendEmailsJob.Start();
        }

        #endregion
    }
}
