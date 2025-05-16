using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace LoginMVC.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public class MailController : Controller
    {
        private readonly GraphServiceClient _graph;

        public MailController(GraphServiceClient graph)
        {
            _graph = graph;
        }

        // 목록 조회
        public async Task<IActionResult> TodayMails()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var mailResponse = await _graph.Me.MailFolders["Inbox"].Messages.GetAsync(config =>
            {
                config.QueryParameters.Filter =
                    $"receivedDateTime ge {today:O} and receivedDateTime lt {tomorrow:O}";
                config.QueryParameters.Top = 50;
                config.QueryParameters.Orderby = new[] { "receivedDateTime desc" };
                config.QueryParameters.Select = new[] { "subject", "from", "receivedDateTime", "id" };
                config.Headers.Add("Prefer", "outlook.markAsRead=false"); // ✅ 읽음 상태 유지
            });

            var mails = mailResponse?.Value?.Select(m => new
            {
                Id = m.Id,
                Subject = m.Subject,
                From = m.From?.EmailAddress?.Name,
                Received = m.ReceivedDateTime?.ToLocalTime().ToString("g")
            });

            return View(mails);
        }



        // 상세 본문 조회 (AJAX)
        [HttpGet]
        public async Task<IActionResult> GetMailBody(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var mail = await _graph.Me.Messages[id].GetAsync(config =>
            {
                config.QueryParameters.Select = new[] { "body" };
            });

            return Content(mail?.Body?.Content ?? "⚠ 본문 없음", "text/html");
        }
    }


}