using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace LoginMVC.Models
{
    public class LoginUserEx : IdentityUser
    {
        // [PersonalData]
        // public string? FirstName { get; set; }
        //
        // [PersonalData]
        // public string? LastName { get; set; }

        [PersonalData]
        public string? DisplayName { get; set; }

        [PersonalData]
        public string? JobTitle { get; set; }
        [NotMapped]
        public string? SDisplayName
        {
            get
            {
                if (!string.IsNullOrEmpty(DisplayName) && !DisplayName.Contains("/"))
                    return DisplayName;
                return DisplayName?.Split('/').FirstOrDefault();
            }
        }
    }
}
