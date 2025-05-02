using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginMVC.Models
{
    [Table("Posts")]
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("Title")]
        public string Title { get; set; } = null!;

        [Required]
        [Column("Contents")]
        public string Content { get; set; } = null!;

        [Column("Date")]
        public DateTime? Date { get; set; }

        [Column("Created")]
        public DateTime? Created { get; set; }

        [Column("CreatorId")]
        public string? CreatorId { get; set; }   // <- 수정: ? 추가

        [Column("CategoryKey")]
        public string? CategoryKey { get; set; } // <- 수정: ? 추가

        [Column("Status")]
        public string? Status { get; set; }       // <- 수정: ? 추가
    }
}