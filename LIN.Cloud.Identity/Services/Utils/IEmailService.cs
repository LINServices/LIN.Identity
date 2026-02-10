
namespace LIN.Cloud.Identity.Services.Utils
{
    public interface IEmailService
    {
        Task<bool> SendMail(string[] to, string person, string subject, string body);
    }
}