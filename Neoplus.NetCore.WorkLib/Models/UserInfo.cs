
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;



namespace Neoplus.NetCore.WorkLib.Models
{
    public class UserInfo : IdentityUser
    {
        public string? DisplayName { get; set; }

        public string? JobTitle { get; set; }
        public string? OfficeLocation { get; set; }

        [NotMapped]
        public string? SDisplayName
        {
            get
            {
                if (DisplayName != null && !DisplayName.Contains("/"))
                {
                    return DisplayName;
                }
                return DisplayName?.Split('/').FirstOrDefault();
            }
        }
    }
}
