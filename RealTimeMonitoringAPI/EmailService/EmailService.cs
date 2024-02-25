using System.Net.Mail;
using System.Net;
using RealTimeMonitoringAPI.DTOs;

namespace RealTimeMonitoringAPI.EmailService
{
    public class EmailService
    {
        public static void SendEmailNotification(string body, IConfiguration config, int type, string emailTo)
        {
            string emailMessage = "";
            string subject = "";
            string cetNumber = "012715002"; 
            string cetEmail = "hr@herconomy.com";            
            if (type == 0)
            {
                subject = "New User Registration";
            }
            if (type == 1)
            {
                subject = "Your reset was successful";
            }
            if (type == 2)
            {
                subject = "Transaction Alert";
            }
            string filePath = "C:\\EmailTemplates\\Herconomy-EmailNotificationTemplate.html"; 
            string year = DateTime.Now.Year.ToString();
            if (System.IO.File.Exists(filePath))
            {
                FileStream f1 = new FileStream(filePath, FileMode.Open);
                StreamReader sr = new StreamReader(f1);
                emailMessage = emailMessage + sr.ReadToEnd();
                emailMessage = emailMessage.Replace("[Context]", body);
                emailMessage = emailMessage.Replace("[CETNumber]", cetNumber);
                emailMessage = emailMessage.Replace("[CETEmail]", cetEmail);
                emailMessage = emailMessage.Replace("[CurrentYear]", year);
                f1.Close();
            }
            EmailRequest sendEmail = new EmailRequest();
            sendEmail.isBodyHtml = true;
            sendEmail.Message = emailMessage;
            sendEmail.ToEmail = emailTo; // _configuration["EmailConfiguration:emailTo"];
            sendEmail.ToCc = "sodiqolayemi9@gmail.com";
            sendEmail.Subject = subject;
            EmailService.SendEmailNotificationAsync(sendEmail, config);
        }

        public static void SendEmailNotificationAsync(EmailRequest request, IConfiguration config)
        {
            EmailResponse response = new EmailResponse();
            string smtpServer = "smtp.office365.com"; 
            string port = "587"; 
            string mailusername = "Appnotify@arm.com.ng"; 
            string password = "Atlas@123"; 
            bool enableSsl = true;
            string credentialRequired = "yes"; 
            string fromEmailId = "Appnotify@arm.com.ng";  
            string accountName = "Herconomy Fintech Company"; 
            string clientDomain = ""; 

            MailMessage mailMessage = new MailMessage();
            SmtpClient smtpClient = new SmtpClient(smtpServer, Convert.ToInt16(port));

            try
            {
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                NetworkCredential mNetworkCredential = new NetworkCredential(mailusername, password, clientDomain);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.EnableSsl = enableSsl;
                smtpClient.Credentials = mNetworkCredential;
                request.Message = request.Message.Replace("%%emailaddress%%", request.ToEmail);
                mailMessage.IsBodyHtml = request.isBodyHtml;
                mailMessage.To.Add(request.ToEmail);
                if (!string.IsNullOrEmpty(request.ToCc))
                {
                    mailMessage.CC.Add(request.ToCc);
                }
                mailMessage.From = new MailAddress(fromEmailId, accountName);
                mailMessage.Subject = request.Subject;
                mailMessage.Body = request.Message;
                smtpClient.Send(mailMessage);
                response.Status = true;
                response.ResponseString = "Sent";
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.ResponseString = "Not Sent";
            }
            finally
            {
                mailMessage.Dispose();
                smtpClient = null;
            }

            response.Recipient = request.ToEmail;
            return;
        }
    }
}
