using LoginMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Neoplus.NetCore.WorkLib.Models;

namespace LoginMVC.Data
{
    public class UserDbContext : IdentityDbContext<LoginUserEx>
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }


}
