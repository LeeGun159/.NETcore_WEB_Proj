using System.Security.Claims;
using LoginMVC.Data;          // UserDbContext, AppDbContext 정의
using LoginMVC.Models;        // LoginUserEx 정의
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LoginMVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using LoginMVC.Helpers;

var builder = WebApplication.CreateBuilder(args);

// 1) 주문 전용 DbContext
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("OrderDB")));

// 2) 사용자 전용 DbContext
builder.Services.AddDbContext<UserDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("UserInfoDB")));

builder.Services.AddDbContext<BulletinBoardDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserInfoDB")));


// 3) Identity 등록: UserDbContext 사용
builder.Services.AddIdentity<LoginUserEx, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

// Azure AD OpenID Connect 추가

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(1); // 쿠키의 유효 시간 설정
        options.LoginPath = "/Identity/Account/Login";  // 로그인 경로
        options.LogoutPath = "/Identity/Account/Logout"; // 로그아웃 경로
        options.AccessDeniedPath = "/Identity/Account/AccessDenied"; // 접근 거부 경로
    })
    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = builder.Configuration["AzureAd:ClientId"];
        options.ClientSecret = builder.Configuration["AzureAd:ClientSecret"];
        options.Authority = $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/v2.0";
        options.CallbackPath = "/signin-oidc";  // Azure에서 리디렉션을 받기 위한 경로
        options.ResponseType = "code";
        options.SaveTokens = true; // 토큰 저장
        options.Scope.Add("email");
        options.Scope.Add("openid");
        options.Scope.Add("profile");

        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = async context =>
            {
                var signInManager =
                    context.HttpContext.RequestServices.GetRequiredService<SignInManager<LoginUserEx>>();
                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<LoginUserEx>>();

                // 사용자 검증 및 로그인 처리 메서드 호출
                await AuthenticationHelper.HandleTokenValidationAsync(context.Principal, signInManager, userManager);
            }
            };
    });


builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Identity/Account/Login"; // 로그인 경로
    opts.LogoutPath = "/Identity/Account/Logout"; // 로그아웃 경로
    opts.AccessDeniedPath = "/Identity/Account/AccessDenied"; // 접근 거부 경로
});

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<GraphUserService>();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 미들웨어 파이프라인
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 인증 및 권한 미들웨어
app.UseAuthentication();
app.UseAuthorization();

// 기본 라우팅 설정
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
