using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly IVideoService _videoService;
        private readonly UserManager<AppUser> _userManager;

        public CommentController(
            ICommentService commentService,
            IVideoService videoService,
            UserManager<AppUser> userManager)
        {
            _commentService = commentService;
            _videoService = videoService;
            _userManager = userManager;
        }

        // POST: Comment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            dto.UserId = user.Id;
            dto.CreatedAt = DateTime.UtcNow;

            await _commentService.CreateCommentAsync(dto);

            return RedirectToAction("Details", "Video", new { id = dto.VideoId });
        }

        // POST: Reply to another comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(Guid videoId, Guid parentId, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var reply = new CommentDto
            {
                CommentId = Guid.NewGuid(),
                VideoId = videoId,
                ParentCommentId = parentId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                UserId = user.Id
            };

            await _commentService.CreateCommentAsync(reply);

            return RedirectToAction("Details", "Video", new { id = videoId });
        }

        // POST: Comment/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var comment = await _commentService.GetCommentByIdAsync(id);

            // comment not found
            if (comment == null) return NotFound();

            // Video owner allowed
            var video = await _videoService.GetVideoByIdAsync(comment.VideoId);
            bool isVideoOwner = video.ChannelOwnerUserId == user.Id;

            // Comment owner allowed
            bool isCommentOwner = comment.UserId == user.Id;

            if (!isVideoOwner && !isCommentOwner)
                return Forbid();

            await _commentService.DeleteCommentAsync(id);

            return RedirectToAction("Details", "Video", new { id = comment.VideoId });
        }
    }
}
