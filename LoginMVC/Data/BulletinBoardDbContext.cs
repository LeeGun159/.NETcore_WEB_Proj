using LoginMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace LoginMVC.Data
{
    public class BulletinBoardDbContext : DbContext
    {
        public BulletinBoardDbContext(DbContextOptions<BulletinBoardDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Attachment> Attachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Post>().ToTable("Posts");
            modelBuilder.Entity<Comment>().ToTable("Comments");
            modelBuilder.Entity<Attachment>().ToTable("Attachments");

            modelBuilder.Entity<Attachment>()
                .HasOne(a => a.Post)
                .WithMany()
                .HasForeignKey(a => a.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }

}