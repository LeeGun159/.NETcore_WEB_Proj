using LoginMVC.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LoginMVC.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Order>().ToTable("Orders");

            // 컬럼 타입 맞추기
            modelBuilder.Entity<Order>()
                .Property(o => o.OrderDate).HasColumnType("datetime");
            modelBuilder.Entity<Order>()
                .Property(o => o.Freight).HasColumnType("money");

            base.OnModelCreating(modelBuilder);
        }
    }
}
