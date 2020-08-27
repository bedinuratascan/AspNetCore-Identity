using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace membershipSystem.Helper
{
    public static class PasswordReset
    {
       public static void PasswordResetSendMail(string link)
        {
            MailMessage mail = new MailMessage();
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("bedinuratascan@gmail.com");
            mail.To.Add("bedinuratascan@gmail.com");
            mail.Subject = $"www.bedinur.com::Şifre Yenileme";
            mail.Body = "<h2>Şifrenizi yenilemek için lütfen aşağıdaki linke tıklayınız</h2><hr/>";
            mail.Body += $"<a href='{link}'>Şifre Yenileme Linki</a>";
            mail.IsBodyHtml = true;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("bedinuratascan@gmail.com", "xyz123");
            smtpClient.Send(mail);
        }
        
    }
}
