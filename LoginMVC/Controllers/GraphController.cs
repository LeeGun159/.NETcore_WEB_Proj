using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System.Text.Json;

namespace LoginMVC.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public class GraphController : Controller
    {
        private readonly GraphServiceClient _graph;

        public GraphController(GraphServiceClient graph)
        {
            _graph = graph;
        }


        //  View 렌더링용 - HTML 페이지 반환
        [Authorize]
        public IActionResult Events()
        {
            return View(); // => Views/Graph/Events.cshtml 렌더링
        }
        
        //   JSON 데이터 API용 - FullCalendar가 호출
        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var groupId = "fab736f0-6b77-4de8-9d89-fefe7ab144fa";
            var events = await _graph.Groups[groupId].Calendar.Events
                .GetAsync(config =>
                {
                    config.QueryParameters.Top = 50;
                    config.QueryParameters.Select = new[] { "subject", "start", "end" };
                });
        
            var result = events?.Value?.Select(e => new
            {
                title = e.Subject ?? "(제목 없음)",
                start = e.Start?.DateTime ?? "",
                end = e.End?.DateTime ?? "",
                allDay = true
            });
        
            return Json(result);
        }



    }
}