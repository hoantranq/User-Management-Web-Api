using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UserManagement_Backend.Models;
using UserManagement_Backend.Services.Emails;

namespace UserManagement_Backend.Controllers
{
    [ApiController]
    [Route("api/mail")]
    public class MailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public MailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMail([FromForm] MailRequest request)
        {
            try
            {
                await _emailService.SendEmailAsync(request);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
