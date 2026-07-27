using Quartz.Impl;
using Quartz;
using System.Net.Mail;
using System.Text;
using AskalePortal.BLL;

namespace AskalePortal.API.Jobs
{
  

    public class EmailJob : IJob
    {
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public EmailJob(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
  
        public string? SendSMS(string toNumbers, string smsText)
        {
            string value =  SendNotification.SendSMS(toNumbers, smsText);
            return value;

        }
        private void SendMail(string subject, string toAddress, string emailText, string file = "", int mailTuru = 1)
        {
            SendFromServer(subject, toAddress, emailText, file, mailTuru);
        }
        private void SendFromServer(string subject, string toEmail, string mailText, string file = "", int mailTuru = 1)
        {
            try
            {
                var fromMailAddress = string.Empty;
                var mailPassword = string.Empty;
                var smtpRequireSSL = true;
                var smtpAddress = "mail.askalecimento.com.tr";
                var smtpPort = 587;
                if (mailTuru == 1)
                {
                    fromMailAddress = "sapwep@askalecimento.com.tr";
                    mailPassword = "06Tfn1453*";
                }
                else if (mailTuru == 2)
                {
                    fromMailAddress = "ITSatisOnaylari@askalecimento.com.tr";
                    mailPassword = "AskCem2506/";
                }
                else if (mailTuru == 3)
                {
                    fromMailAddress = "ITHarcamaOnaylari@askalecimento.com.tr";
                    mailPassword = "AskCem2506*";
                }
                else if (mailTuru == 4)
                {
                    fromMailAddress = "ITDahiliYazisma@askalecimento.com.tr";
                    mailPassword = "AskCem2506-";
                }
                else if (mailTuru == 5)
                {
                    fromMailAddress = "ITMusteriSikayetleri@askalecimento.com.tr";
                    mailPassword = "AskCem2506+";
                }
                else if (mailTuru == 6)
                {
                    fromMailAddress = "ITBankaOdemeleri@askalecimento.com.tr";
                    mailPassword = "AskCem2506.";
                }

                MailMessage mail = new MailMessage();
                if (toEmail.Contains(";"))
                {
                    string[] emailList = toEmail.Split(';');
                    foreach (var item in emailList)
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            mail.To.Add(item);
                        }
                       
                    }
                }
                else
                    mail.To.Add(toEmail);
                mail.From = new MailAddress(fromMailAddress);
                mail.Subject = subject;
                mail.Body = mailText;
                mail.Priority = MailPriority.Normal;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;
                if (file != "" && file != null)
                {
                    Attachment data = new Attachment(file);
                    mail.Attachments.Add(data);
                }


                SmtpClient smtp = new SmtpClient();
                smtp.Host = smtpAddress;
                smtp.Port = smtpPort;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(fromMailAddress, mailPassword);
                smtp.EnableSsl = smtpRequireSSL;

                smtp.Send(mail);
            }
            catch
            {

            }

            // return output;
        }

        public Task Execute(IJobExecutionContext context)
        {
            BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
            var list = bllEmailMessages.GetUnsend();
            foreach (var item in list)
            {
                int mailTuru = item.mailTuru.HasValue ? item.mailTuru.Value : 1;
                SendMail(item.subject, item.toAddress, item.emailText, item.dosya, mailTuru);
                item.isSent = true;
                bllEmailMessages.Update(item);
                
            }

            BLLActions.SMSMessages bllSMS = new BLLActions.SMSMessages(_configuration, _env);
            var listSMS = bllSMS.GetUnsend();
            foreach (var item in listSMS)
            {
                SendSMS(item.toNumbers, item.smsText);
                item.isSent = true;
                bllSMS.Update(item);
              
            }
            return Task.CompletedTask;
        }
    }
}
