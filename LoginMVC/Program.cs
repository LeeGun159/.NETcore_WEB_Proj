using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Microsoft.Kiota.Abstractions.Authentication;
using LoginMVC.Data;
using LoginMVC.Models;
using LoginMVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using System.Text.Json;
using Azure.Core;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

// -------------------- DB Context 등록 --------------------
builder.Services.AddDbContext<AppDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("OrderDB")));

builder.Services.AddDbContext<UserDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("UserInfoDB")));

builder.Services.AddDbContext<BulletinBoardDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("UserInfoDB")));

// -------------------- Identity --------------------
builder.Services.AddIdentity<LoginUserEx, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

// -------------------- Azure AD 인증 + 토큰 획득 --------------------
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"))
    .EnableTokenAcquisitionToCallDownstreamApi(new[] { "User.Read", "Calendars.Read", "Mail.Read" })
    .AddInMemoryTokenCaches();



// -------------------- MS Graph --------------------
builder.Services.AddScoped<IAuthenticationProvider>(provider =>
{
    var tokenAcquisition = provider.GetRequiredService<ITokenAcquisition>();
    return new GraphAuthProvider(tokenAcquisition, new[] { "User.Read", "Calendars.Read", "Mail.Read" });
});

builder.Services.AddScoped<GraphServiceClient>(provider =>
{
    var authProvider = provider.GetRequiredService<IAuthenticationProvider>();
    return new GraphServiceClient(authProvider);
});

// -------------------- 로그인/접근 거부 경로 설정 --------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login"; //  ASP.NET Identity 로그인 페이지
    options.AccessDeniedPath = "/MicrosoftIdentity/Account/AccessDenied";
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
});

// -------------------- 기타 서비스 --------------------
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<GraphUserService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI(); // Azure Identity UI 사용 가능

var app = builder.Build();

// -------------------- 미들웨어 파이프라인 --------------------
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();


