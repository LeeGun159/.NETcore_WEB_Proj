using System.Security.Claims;
using LoginMVC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoginMVC.Helpers
{
    public static class AuthenticationHelper
    {
        public static async Task HandleTokenValidationAsync(ClaimsPrincipal principal,
            SignInManager<LoginUserEx> signInManager,
            UserManager<LoginUserEx> userManager)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user == null)
            {
                // 사용자 생성 로직
                var userName = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = principal.FindFirstValue(ClaimTypes.Email);

                user = new LoginUserEx
                {
                    UserName = userName,
                    Email = email
                };

                var existingUser = await userManager.Users
                    .FirstOrDefaultAsync(u => u.Email == email); // 중복을 허용하지 않고 첫 번째로 찾기

                if (existingUser != null)
                {
                    // 이미 존재하는 사용자 로그인
                    await signInManager.SignInAsync(existingUser, isPersistent: false);
                }
                else
                {
                    // 사용자 생성
                    var result = await userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        foreach (var error in result.Errors)
                        {
                            Console.WriteLine($"Error: {error.Description}");
                        }
                        throw new InvalidOperationException("User creation failed.");
                    }
                    await signInManager.SignInAsync(user, isPersistent: false);
                }
            }
        }
    }

}
