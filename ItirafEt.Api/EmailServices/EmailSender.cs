using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
using ItirafEt.Api.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace ItirafEt.Api.EmailServices
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpUser = _config["EmailSettings:SmtpUser"];
            var smtpPass = _config["EmailSettings:SmtpPass"];
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_config["EmailSettings:SmtpPort"]);

            using var mail = new MailMessage();
            mail.From = new MailAddress(smtpUser);
            mail.Subject = subject;
            mail.Body = htmlMessage;
            mail.IsBodyHtml = true;
            mail.To.Add(email);

            using var client = new SmtpClient(smtpServer, smtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 10000
            };

            await client.SendMailAsync(mail);
        }
    }

    public static class EmailTypes
    {
        public const string Welcome = "welcome";
        public const string Reset = "reset";
        public const string Ban = "ban";
    }

    public class EmailMessageDto
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
    public static class EmailCreateFactory
    {
        public static EmailMessageDto CreateEmail(string emailTypes, User user, string? resetUrl = null)
        {
            return new EmailMessageDto()
            {
                To = user.Email,
                Subject = GetSubjectTemplate(emailTypes, user),
                Body = GetBodyTemplate(emailTypes, user, resetUrl),
            };

        }
        private static string GetBodyTemplate(string emailTypes, User user, string? resetUrl = null)
        {
            return emailTypes switch
            {
                EmailTypes.Welcome => WelcomeTemplate(user),
                EmailTypes.Reset => ResetTemplate(user, resetUrl),
                EmailTypes.Ban => BanTemplate(user),
                _ => throw new ArgumentException("Invalid email type"),
            };
        }

        private static string GetSubjectTemplate(string emailTypes, User? user = null)
        {
            return emailTypes switch
            {
                EmailTypes.Welcome => "Hesabınıza Hoş Geldiniz – ItirafEt",
                EmailTypes.Reset => "🔑 Şifre Sıfırlama Talebi – ItirafEt",
                EmailTypes.Ban => user.IsBanned ? "Hesabınız Banlanmıştır – ItirafEt" : "Hesabınızın Banı Kaldırıldı – ItirafEt",
                _ => throw new ArgumentException("Invalid email type"),
            };
        }

        private static string WelcomeTemplate(User user) =>
                $@"
<div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
    <h2 style='color: #27ae60;'>Merhaba {user.UserName},</h2>
    <p style='font-size: 14px; color: #555;'>
        Hesabınız başarıyla oluşturuldu! Platformumuza hoş geldiniz.
    </p>

    <p style='font-size: 14px; color: #555;'>
        Artık diğer kullanıcılarla etkileşimde bulunabilir, içerik paylaşabilir ve tüm özelliklerden faydalanabilirsiniz.
    </p>

    <p style='text-align: center; margin: 30px 0;'>
        <a href='https://itirafetweb.runasp.net' 
           style='
               display:inline-block;
               background: linear-gradient(135deg, #4b6cb7 0%, #182848 100%);
               color: #fff;
               border-radius: 8px;
               padding: 12px 24px;
               text-decoration: none;
               font-size: 16px;
               font-weight: bold;
               transition: all 0.2s ease;
               box-shadow: 0 4px 10px rgba(24, 119, 242, 0.25);
           '>
            Platforma Başla
        </a>
    </p>

    <p style='font-size: 13px; color: #999;'>
        Eğer bu hesabı siz oluşturmadıysanız, lütfen destek ekibimizle iletişime geçin.
    </p>

    <hr style='margin: 20px 0;'/>
    <p style='font-size: 12px; color: #aaa; text-align: center;'>
        © {DateTime.UtcNow.Year} ItirafEt Ekibi
    </p>
</div>";
        private static string ResetTemplate(User user, string resetUrl) =>
            $@"
    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
        <h2 style='color: #333;'>Merhaba {user.UserName},</h2>
        <p style='font-size: 14px; color: #555;'>
            Şifrenizi sıfırlamak için aşağıdaki butona tıklayın. 
            Bu bağlantı yalnızca <strong>30 dakika</strong> boyunca geçerlidir.
        </p>

        <p style='text-align: center; margin: 30px 0;'>
            <a href='{resetUrl}' 
               style='
                   display:inline-block;
                   background: linear-gradient(135deg, #4b6cb7 0%, #182848 100%);
                   color: #fff;
                   border-radius: 8px;
                   padding: 12px 24px;
                   text-decoration: none;
                   font-size: 16px;
                   font-weight: bold;
                   transition: all 0.2s ease;
                   box-shadow: 0 4px 10px rgba(24, 119, 242, 0.25);
               '>
                Şifremi Sıfırla
            </a>
        </p>

        <p style='font-size: 13px; color: #999;'>
            Eğer bu talebi siz oluşturmadıysanız, lütfen bu e-postayı dikkate almayın.
        </p>

        <hr style='margin: 20px 0;'/>
        <p style='font-size: 12px; color: #aaa; text-align: center;'>
            © {DateTime.UtcNow.Year} ItirafEt Ekibi
        </p>
    </div>";
        private static string BanTemplate(User user) =>
            $@"<div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                        <h2 style='color: #c0392b;'>Merhaba {user.UserName},</h2>
                        <p style='font-size: 14px; color: #555;'>
                            {(user.IsBanned
                                                ? $"Hesabınız, <strong>{DateTime.UtcNow:dd.MM.yyyy HH:mm} (UTC)</strong> tarihinde yöneticilerimiz tarafından <strong>banlanmıştır</strong>."
                                                : "Hesabınıza uygulanan ban kaldırılmıştır. Artık platformu tekrar kullanabilirsiniz.")}
                        </p>
                    
                        {(user.IsBanned
                                            ? $@"<p style='font-size: 14px; color: #555;'>
                                    Ban <strong>{(user.BannedDateUntil.HasValue
                                                                    ? user.BannedDateUntil.Value.ToString("dd.MM.yyyy HH:mm") + " (UTC)"
                                                                    : "süresiz")}</strong> tarihine kadar geçerlidir. 
                                </p>
                                <p style='background: #f9f2f4; color: #a94442; border: 1px solid #ebccd1; padding: 12px; border-radius: 6px; font-size: 14px;'>
                                    Bu süre boyunca hesabınızda oturum açamaz ve platformu kullanamazsınız.
                                </p>"
                                            : "")}
                    
                        <p style='font-size: 13px; color: #555; margin-top: 20px;'>
                            Eğer bu işlemde bir hata olduğunu düşünüyorsanız, lütfen destek ekibimizle iletişime geçin.
                        </p>
                    
                        <hr style='margin: 20px 0;'/>
                        <p style='font-size: 12px; color: #aaa; text-align: center;'>
                            © {DateTime.UtcNow.Year} ItirafEt Ekibi
                        </p>
                    </div>";
    }

}
