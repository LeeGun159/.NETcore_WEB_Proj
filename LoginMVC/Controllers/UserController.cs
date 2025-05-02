using LoginMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoginMVC.Controllers
{
    public class UserController : Controller
    {
        // 사용자 데이터를 처리하는 Graph 서비스 클래스 주입
        private readonly GraphUserService _userService;

        public UserController(GraphUserService userService)
        {
            _userService = userService;
        }

        // 사용자 목록을 기본 페이지에서 보여줌 (검색, 정렬, 페이징 포함)
        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? sort = "displayName", string? order = "asc", string? pageUrl = null)
        {
            // 사용자 목록과 다음 페이지 URL을 가져옴
            var (users, nextPageUrl) = await _userService.GetUsersPageAsync(search, sort, order, pageUrl);

            // 뷰에 검색/정렬 조건 및 다음 페이지 정보 전달
            ViewData["Search"] = search;
            ViewData["Sort"] = sort;
            ViewData["Order"] = order;
            ViewData["NextPageUrl"] = nextPageUrl;

            return View(users); // Index.cshtml로 전달
        }

        // 더보기 버튼 클릭 시 추가 사용자 데이터만 부분 뷰로 로드
        [HttpGet]
        public async Task<IActionResult> More(string url, string? search, string? sort, string? order)
        {
            // 다음 페이지 URL 기반으로 사용자 데이터 불러오기
            var (users, nextPageUrl) = await _userService.GetUsersPageAsync(search, sort, order, url);

            // 응답 헤더에 다음 페이지 URL 추가 (프론트 JS에서 사용)
            Response.Headers.Add("X-Next-Link", nextPageUrl ?? "");

            return PartialView("_UserRows", users); // 테이블 행만 반환
        }

        // 사용자 이름 클릭 시 상세 정보 팝업 데이터 반환
        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("사용자 ID가 필요합니다.");
            }

            // ID로 사용자 상세 정보 조회
            var user = await _userService.GetUserDetailAsync(id);

            if (user == null)
            {
                return NotFound("사용자를 찾을 수 없습니다.");
            }

            return PartialView("_UserDetailPopup", user); // 상세정보만 렌더링
        }

        // 사용자 프로필 사진을 가져옴 (없으면 기본 이미지 반환)
        [HttpGet]
        public async Task<IActionResult> Photo(string id)
        {
            // 사용자 ID로 Graph에서 사진 stream 가져오기
            var stream = await _userService.GetUserPhotoAsync(id);

            if (stream != null)
            {
                return File(stream, "image/jpeg");
            }

            // 사진이 없을 경우 기본 프로필 이미지 반환
            var defaultImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "default-profile.jpg");
            var bytes = System.IO.File.ReadAllBytes(defaultImagePath);
            return File(bytes, "image/jpeg");
        }
    }
}
