using System;
using System.ComponentModel.DataAnnotations;

namespace LoginMVC.Models
{
    public class Attachment
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string FileName { get; set; } // 서버에 저장된 파일명

        [Required]
        [MaxLength(255)]
        public string OriginalFileName { get; set; } // 사용자가 업로드한 원래 파일명

        [Required]
        [MaxLength(500)]
        public string FilePath { get; set; } // 서버 저장 경로

        public long FileSize { get; set; } // 파일 크기

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // 어떤 게시글(Post)에 연결됐는지
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}