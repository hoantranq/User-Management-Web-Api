using System.Threading.Tasks;
using UserManagement_Backend.Models;

namespace UserManagement_Backend.Services.Emails
{
    public interface IEmailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
