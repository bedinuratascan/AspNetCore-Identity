using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace membershipSystem.Helper
{
    public class EmailConfirmation
    {
        public static void SendMail(string link, string email)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("bedinuratascan@gmail.com");
            mail.To.Add(email);
            mail.Subject = $"www.bedinur.com::Email Doğrulama";
            mail.Body = "<h2>Email adresinizi doğrulamak için lütfen aşağıdaki linke tıklayınız</h2><hr/>";
            mail.Body += $"<a href='{link}'>Email Doğrulama Linki</a>";
            mail.IsBodyHtml = true;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("bedinuratascan@gmail.com", "xyz123");
            smtpClient.Send(mail);
        }
    }
}
