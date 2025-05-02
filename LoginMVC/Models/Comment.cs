using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginMVC.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; } // 어떤 게시글에 달린 댓글인지

        [ForeignKey("PostId")]
        public Post? Post { get; set; }

        [Required]
        public string? Author { get; set; }

        [Required]
        [MaxLength(500)]
        public string? Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}