using System;
using System.Linq;
using System.Net.Mail;

namespace Utils
{
    /// <summary>
    /// Inherits MailMessage class
    /// </summary>
    public class NotificationEmailItem
    {
        private const char EmailsSeparator = ',';
        private static readonly char[] EmailsSeparators = new char[] { EmailsSeparator };
        
        private MailMessage mailMessage = new MailMessage();
        public MailMessage MailMessage
        {
            get
            {
                return mailMessage;
            }
        }

        public string DisplayName
        {
            get 
            {
                return mailMessage.From.DisplayName;
            }
            set 
            {
                mailMessage.From = new MailAddress(mailMessage.From.Address, value);
            }
        }

        public string From
        {
            get 
            {
                return mailMessage.From.Address;
            }
            set 
            {
                mailMessage.From = new MailAddress(value, mailMessage.From.DisplayName);
            }
        }

        /// <summary>
        /// Gets or sets the address collection that contains the recipients of this e-mail message-> MailMessage.To.
        /// </summary>
        public string To
        {
            set
            {
                SetAddressesToCollection(mailMessage.To, value);
            }
            get
            {
                return GetAddressesFromCollection(mailMessage.To);
            }
        }

        /// <summary>
        /// Gets or sets the address collection that contains the carbon copy (CC) recipients of this e-mail message-> MailMessage.CC.
        /// </summary>
        public string CC
        {
            set
            {
                SetAddressesToCollection(mailMessage.CC, value);
            }
            get
            {
                return GetAddressesFromCollection(mailMessage.CC);
            }
        }

        /// <summary>
        /// Gets or sets the address collection that contains the blind carbon copy (CC) recipients of this e-mail message-> MailMessage.Bcc.
        /// </summary>
        public string BCC
        {
            set
            {
                SetAddressesToCollection(mailMessage.Bcc, value);
            }
            get
            {
                return GetAddressesFromCollection(mailMessage.Bcc);
            }
        }

        /// <summary>
        /// Gets or sets the list of addresses to reply to for the mail message.
        /// </summary>
        public string ReplyTo
        {
            set
            {
                SetAddressesToCollection(mailMessage.ReplyToList, value);
            }
            get
            {
                return GetAddressesFromCollection(mailMessage.ReplyToList);
            }
        }

        /// <summary>
        ///  Gets or sets the MailMessage.Attachments collection used to store data attached to this e-mail message.
        /// </summary>
        public string[] AttachmentFiles
        {
            set
            {
                if (value != null)
                {
                    foreach (string f in value)
                    {
                        mailMessage.Attachments.Add(new Attachment(f));
                    }
                }
            }
            get
            {
                return mailMessage.Attachments.Select(f => f.ContentStream.ToString()).ToArray();
            }
        }

        public string Subject 
        {
            get 
            {
                return mailMessage.Subject;
            }
            set 
            {
                mailMessage.Subject = value;
            }
        }

        public string Body
        {
            get
            {
                return mailMessage.Body;
            }
            set
            {
                mailMessage.Body = value;
            }
        }

        #region Private Methods

        private static void SetAddressesToCollection(MailAddressCollection addressCollection, string emailsText)
        {
            addressCollection.Clear();
            if (!string.IsNullOrEmpty(emailsText))
            {
                string[] emails = emailsText.Split(EmailsSeparators, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < emails.Length; i++)
                {
                    addressCollection.Add(emails[i]);
                }
            }
        }

        private static string GetAddressesFromCollection(MailAddressCollection addressCollection)
        {
            return string.Join(EmailsSeparator.ToString(), addressCollection.Select(e => e.Address).ToArray());
        }

        #endregion
    }
}
