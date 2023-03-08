using System.Collections.Generic;
using System.Net.Mail;

namespace Utility
{
    /// <summary>
    /// Class for sending emails using a specified SMTP server
    /// </summary>
    public class SmtpMailer
    {
        //Sender email address
        string senderEmail;

        //SmtpClient object for sending an email
        SmtpClient smtpClient;

        //Email message object to compose
        MailMessage mail;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="smtpServer">Smtp Server IP</param>
        /// <param name="senderEmail">Sender Email</param>
        /// <param name="senderPassword">Sender Password</param>
        /// <param name="port">Server Port</param>
        /// <param name="sslEnabled">SSL enabled/disabled</param>
        public SmtpMailer(string smtpServer, string senderEmail, string senderPassword, int port = 587, bool sslEnabled = true)
        {
            this.senderEmail = senderEmail;
            this.smtpClient = new SmtpClient(smtpServer);
            this.smtpClient.Port = port;
            this.smtpClient.UseDefaultCredentials = false;
            this.smtpClient.Credentials = new System.Net.NetworkCredential(senderEmail, senderPassword);
            this.smtpClient.EnableSsl = sslEnabled;

            this.mail = new MailMessage();
        }

        /// <summary>
        /// Set Display Name, Subject and Content
        /// </summary>
        /// <param name="displayName">Display Name of the Sender</param>
        /// <param name="subject">Email Subject</param>
        /// <param name="body">Email Content</param>
        /// <param name="attachFiles">Filename Array for Attachments</param>
        public void SetContent(string displayName, string subject, string body, string[] attachFiles = null)
        {
            this.mail.From = new MailAddress(this.senderEmail, displayName);
            this.mail.Subject = subject;
            this.mail.Body = body;

            this.mail.Attachments.Clear();
            if (attachFiles != null)
                for (int i = 0; i < attachFiles.Length; i++)
                    this.mail.Attachments.Add(new Attachment(attachFiles[i]));
        }

        /// <summary>
        /// Set Recipient Email Addresses
        /// </summary>
        /// <param name="recipients">A List of Recipient Emails</param>
        public void SetRecipients(List<string> recipients)
        {
            this.mail.To.Clear();
            foreach (string recipient in recipients)
                this.mail.To.Add(recipient);
        }


        /// <summary>
        /// Sending Email Method
        /// </summary>
        /// <param name="isHTML">Specify the email content as HTML or not</param>
        public void Send(bool isHTML = false)
        {
            this.mail.IsBodyHtml = isHTML;
            smtpClient.Send(mail);
        }

    }
}
