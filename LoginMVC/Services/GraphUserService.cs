using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace LoginMVC.Services
{
    public class GraphUserService
    {
        private readonly GraphServiceClient _graphClient;

        // 생성자에서 GraphServiceClient 설정 (Client Credentials Flow)
        public GraphUserService(IConfiguration config)
        {
            var tenantId = config["MSGraph:TenantId"];
            var clientId = config["MSGraph:ClientId"];
            var clientSecret = config["MSGraph:ClientSecret"];

            // Azure AD 앱의 비밀 키로 인증된 토큰 생성
            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            _graphClient = new GraphServiceClient(credential);
        }

        // 사용자 목록 가져오기 + 페이징 처리
        public async Task<(List<User> Users, string? NextPageUrl)> GetUsersPageAsync(
            string? search, string? sort, string? order, string? pageUrl)
        {
            UserCollectionResponse? result;

            // Graph API에서 정렬 가능한 필드는 displayName뿐이므로 검증
            var safeSort = string.Equals(sort, "displayName", StringComparison.OrdinalIgnoreCase)
                ? "displayName"
                : null;

            var safeOrder = order == "desc" ? "desc" : "asc";

            // 초기 페이지 로딩 시
            if (string.IsNullOrEmpty(pageUrl))
            {
                result = await _graphClient.Users.GetAsync(config =>
                {
                    // 필요한 필드만 가져오기
                    config.QueryParameters.Select = new[] { "id", "displayName", "jobTitle", "mail" };
                    config.QueryParameters.Top = 10; // 한 페이지에 10명

                    // 검색어가 있을 경우 displayName 필터 적용
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        config.QueryParameters.Filter = $"startswith(displayName,'{search}')";
                    }

                    // 정렬은 검색어가 없을 때만 적용 (Graph 제약 사항 대응)
                    if (string.IsNullOrWhiteSpace(search) && !string.IsNullOrEmpty(safeSort))
                    {
                        config.QueryParameters.Orderby = new[] { $"{safeSort} {safeOrder}" };
                    }
                });
            }
            else
            {
                // 다음 페이지 URL 요청 (정렬 필드 유효성 검증)
                if (!string.IsNullOrEmpty(sort) && !pageUrl.Contains("orderby=displayName"))
                {
                    Console.WriteLine(" 잘못된 정렬 필드가 포함된 pageUrl: " + pageUrl);
                    return (new List<User>(), null);
                }

                // 다음 페이지 요청
                result = await _graphClient.Users.WithUrl(pageUrl).GetAsync();
            }

            // 사용자 목록 + 다음 페이지 URL 추출
            var users = result?.Value?.ToList() ?? new List<User>();
            var nextPageUrl = result?.OdataNextLink;

            return (users, nextPageUrl);
        }

        // 사용자 상세 정보 가져오기 (팝업에 사용됨)
        public async Task<User?> GetUserDetailAsync(string userId)
        {
            return await _graphClient.Users[userId]
                .GetAsync(config =>
                {
                    config.QueryParameters.Select = new[] {
                        "displayName", "jobTitle", "department", "mail"
                    };
                });
        }

        // 사용자 프로필 사진 가져오기
        public async Task<Stream> GetUserPhotoAsync(string userId)
        {
            try
            {
                // 바이너리 이미지 stream으로 반환
                return await _graphClient.Users[userId].Photo.Content.GetAsync();
            }
            catch
            {
                // 사진 없거나 권한 부족 시 null 반환
                return null;
            }
        }
    }
}
