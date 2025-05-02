using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoginMVC.Models;
using LoginMVC.Data;
using Microsoft.AspNetCore.Identity;

namespace LoginMVC.Controllers
{
    public class PostsController : Controller
    {
        private readonly UserDbContext _userDbContext;
        private readonly BulletinBoardDbContext _boardDbContext;
        private readonly UserManager<LoginUserEx> _userManager;
        private readonly IWebHostEnvironment _env;

        public PostsController(
            UserDbContext userDbContext,
            BulletinBoardDbContext boardDbContext,
            UserManager<LoginUserEx> userManager,
            IWebHostEnvironment env)
        {
            _userDbContext = userDbContext;
            _boardDbContext = boardDbContext;
            _userManager = userManager;
            _env = env;
        }

        // 글 목록
        public async Task<IActionResult> Index(string? keyword, int page = 1)
        {
            int pageSize = 10;
            var query = _boardDbContext.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(p => p.Title.Contains(keyword));
            }

            var posts = await query
                .OrderByDescending(p => p.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.Keyword = keyword;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)await query.CountAsync() / pageSize);

            return View(posts);
        }

        // 글 상세보기
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _boardDbContext.Posts.FirstOrDefaultAsync(p => p.Id == id);
            if (post == null) return NotFound();

            var comments = await _boardDbContext.Comments
                .Where(c => c.PostId == id)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            var attachments = await _boardDbContext.Attachments
                .Where(a => a.PostId == id)
                .ToListAsync();

            ViewBag.Comments = comments;
            ViewBag.Attachments = attachments;

            return View(post);
        }


        // 글 작성 폼
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserId = user?.Id;
            return View();
        }

        // 글 작성 처리
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Post post, List<IFormFile> files)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                post.CreatorId = user?.Id; // 로그인된 사용자 ID 저장
                post.Created = DateTime.Now;

                if (string.IsNullOrEmpty(post.Status))  // Status가 비어있으면 기본값 세팅
                {
                    post.Status = "Create"; 
                }

                _boardDbContext.Posts.Add(post);
                await _boardDbContext.SaveChangesAsync();

                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {
                        var filePath = Path.Combine(uploadsFolder, file.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var attachment = new Attachment
                        {
                            PostId = post.Id,
                            FileName = file.FileName,
                            OriginalFileName = file.FileName,
                            FilePath = "/uploads/" + file.FileName,
                            UploadedAt = DateTime.Now
                        };
                        _boardDbContext.Attachments.Add(attachment);
                    }
                }
                await _boardDbContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // 글 수정 폼
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var post = await _boardDbContext.Posts.FindAsync(id);
            if (post == null) return NotFound();

            return View(post);
        }

        // 글 수정 처리
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Post post)
        {
            if (id != post.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingPost = await _boardDbContext.Posts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                if (existingPost == null) return NotFound();

                post.Created = existingPost.Created;

                _boardDbContext.Update(post);
                await _boardDbContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(post);
        }

        // 글 삭제 폼
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var post = await _boardDbContext.Posts.FindAsync(id);
            if (post == null) return NotFound();

            return View(post);
        }

        // 글 삭제 처리
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _boardDbContext.Posts.FindAsync(id);
            if (post != null)
            {
                _boardDbContext.Posts.Remove(post);
                await _boardDbContext.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 댓글 작성
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.CreatedAt = DateTime.Now;
                _boardDbContext.Comments.Add(comment);
                await _boardDbContext.SaveChangesAsync();
                return RedirectToAction("Details", new { id = comment.PostId });
            }

            return RedirectToAction("Details", new { id = comment.PostId });
        }

        // 댓글 삭제
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteComment(int id, int postId)
        {
            var comment = await _boardDbContext.Comments.FindAsync(id);
            if (comment != null)
            {
                _boardDbContext.Comments.Remove(comment);
                await _boardDbContext.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = postId });
        }

    }
}
