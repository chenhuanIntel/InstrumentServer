using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.Net;


//Author: Kang He
//Function: Send Email

namespace Utility.ETL
{
    /// <summary>
    /// 
    /// </summary>
    public class EmailConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string senderName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string[] EmailAddresses { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string EmailServerType { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CEmail
    {
        EmailConfig _config = null;

        string emailConfigFile = "EmailConfig.xml";

        string _subj, _body;

        List<string> _attachments = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="attachments"></param>
        public CEmail(string subject, string body, List<string> attachments = null)
        {

            //_log = CLogger.Instance().getSysLogger();

            _config = GenericSerializer.DeserializeFromXML<EmailConfig>(emailConfigFile);

            _subj = subject;

            _body = body;

            _attachments = attachments;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool run()
        {

            try
            {
                if (_config.EmailServerType.Contains("Intel"))
                {
                    //Intel inside network email setting 
                    MailMessage mail = new MailMessage(_config.senderName + "@intel.com", String.Join(",", _config.EmailAddresses.ToArray()));
                    SmtpClient client = new SmtpClient("smtp.intel.com");
                    client.Port = 25;
                    mail.Subject = _subj;
                    mail.Body = _body;
                    //add attachments
                    if (_attachments != null && _attachments.Count != 0)
                    {
                        for (int i = 0; i < _attachments.Count; i++)
                        {
                            mail.Attachments.Add(new System.Net.Mail.Attachment(_attachments.ElementAt(i)));
                        }
                    }

                    client.EnableSsl = false;
                    Console.WriteLine("Sending emails.....");
                    client.Send(mail);
                    Console.WriteLine("Emails sent");

                    //release attachment
                    mail.Dispose();
                }
                else
                {
                    //FBN email setting
                    MailMessage mail = new MailMessage("sontaya.tmp118@hotmail.com", String.Join(",", _config.EmailAddresses.ToArray()));
                    //MailMessage mail = new MailMessage("kanghe.sppd@hotmail.com", String.Join(",", _config.EmailAddresses.ToArray()));
                    SmtpClient client = new SmtpClient("smtp.live.com");
                    client.Port = 587;
                    mail.Subject = _subj;
                    mail.Body = _body;
                    //add attachments
                    if (_attachments != null && _attachments.Count != 0)
                    {
                        for (int i = 0; i < _attachments.Count; i++)
                        {
                            mail.Attachments.Add(new System.Net.Mail.Attachment(_attachments.ElementAt(i)));
                        }
                    }
                    client.Credentials = new NetworkCredential("sontaya.tmp118@hotmail.com", "!4produse");
                    client.EnableSsl = true;
                    Console.WriteLine("Sending emails.....");
                    client.Send(mail);
                    Console.WriteLine("Emails sent");

                    //release attachment
                    mail.Dispose();
                }


            }
            catch (Exception ex)
            {
                clog.Error(ex, "INFO: CEmail::run failed");
                return false;
            }

            clog.Info(String.Format("INFO: CEmail::run succeed"));
            return true;
        }


    }
}
