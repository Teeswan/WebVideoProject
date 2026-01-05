using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic; 
using System.Linq; 
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Services.Implementations;
using Youtube_Entertainment_Project.Services.Interfaces;

[Authorize]
public class VideoController : Controller
{
    private readonly IVideoService _videoService;
    private readonly IChannelService _channelService;
    private readonly ICategoryService _categoryService;
    private readonly ITagService _tagService;
    private readonly UserManager<AppUser> _userManager;
    private readonly ICommentService _commentService;
    private readonly ApplicationDbContext _context;
    private readonly ISubscriptionService _subscriptionService;
    private readonly INotificationService _notificationService;

    public VideoController(
        IVideoService videoService,
        IChannelService channelService,
        ICategoryService categoryService,
        ITagService tagService,
        UserManager<AppUser> userManager,
        ICommentService commentService,
        ApplicationDbContext context,
        ISubscriptionService subscriptionService,
        INotificationService notificationService)
    {
        _videoService = videoService;
        _channelService = channelService;
        _categoryService = categoryService;
        _tagService = tagService;
        _userManager = userManager;
        _commentService = commentService;
        _context = context;
        _subscriptionService = subscriptionService;
        _notificationService = notificationService;
    }

    public class IncrementViewRequest
    {
        public Guid VideoId { get; set; }
    }


    public async Task<IActionResult> Index()
    {
        var videos = await _videoService.GetAllVideosAsync();
        return View(videos);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        if (id == Guid.Empty) return NotFound();

        var video = await _videoService.GetVideoByIdAsync(id);
        if (video == null) return NotFound();

        var allVideos = await _videoService.GetAllVideosAsync();

        ViewBag.RelatedVideos = allVideos
            .Where(v => v.VideoId != id && v.ChannelId == video.ChannelId)
            .Take(5)
            .ToList();

        ViewBag.DiscoverVideos = allVideos
            .Where(v => v.VideoId != id && v.ChannelId != video.ChannelId)
            .OrderBy(v => Guid.NewGuid()) 
            .Take(8)
            .ToList();

        ViewBag.RelatedVideos = allVideos
            .Where(v => v.VideoId != id) 
            .Take(10)
            .ToList();

        var flat = (await _commentService.GetAllCommentsAsync())
            .Where(c => c.VideoId == id)
            .OrderBy(c => c.CreatedAt)
            .ToList();

        var dtoList = new List<CommentDto>();
        foreach (var c in flat)
        {
            var commentUser = await _userManager.FindByIdAsync(c.UserId.ToString());
            dtoList.Add(new CommentDto
            {
                CommentId = c.CommentId,
                VideoId = c.VideoId,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UserId = c.UserId,
                UserName = commentUser?.UserName ?? "Unknown",
                ParentCommentId = c.ParentCommentId,
                Replies = new List<CommentDto>()
            });
        }

        var currentUser = await _userManager.GetUserAsync(User);
        ViewBag.Comments = BuildCommentTree(dtoList);
        video.CommentCount = dtoList.Count;

        video.IsLikedByCurrentUser = currentUser != null &&
                                     await _videoService.HasUserLikedAsync(video.VideoId, currentUser.Id);
        video.LikeCount = await _videoService.GetLikeCountAsync(video.VideoId);
        video.SubscriberCount = await _subscriptionService.GetSubscriberCountAsync(video.ChannelOwnerUserId);

        if (currentUser != null)
        {
            var subscription = await _subscriptionService.GetSubscriptionStatusAsync(currentUser.Id, video.ChannelOwnerUserId);
            video.IsSubscribedByCurrentUser = (subscription != null);
        }
        else
        {
            video.IsSubscribedByCurrentUser = false;
        }

        return View(video);
    }

    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);
        bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        var channel = await _channelService.GetChannelByUserIdAsync(user.Id);

        if (channel == null)
        {
            return RedirectToAction("Create", "Channel");
        }

        var categories = await _categoryService.GetAllCategoriesAsync();
        var tags = await _tagService.GetAllTagsAsync();
        ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
        ViewBag.Tags = new MultiSelectList(tags, "TagId", "Text");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(268435456)]
    public async Task<IActionResult> Create(CreateVideoDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        var channel = (await _channelService.GetAllChannelsAsync())
            .FirstOrDefault(c => c.OwnerUserId == user.Id);

        if (channel == null) return Forbid();
        dto.ChannelId = channel.ChannelId;

        if (dto.VideoFile == null)
        {
            ModelState.AddModelError(nameof(dto.VideoFile), "Please select a video file to upload.");
        }

        if (!ModelState.IsValid)
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            var tags = await _tagService.GetAllTagsAsync();
            ViewBag.Categories = new SelectList(categories, "CategoryId", "Name");
            ViewBag.Tags = new MultiSelectList(tags, "TagId", "Text");
            return View(dto);
        }

        var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/videos");
        if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

        string fileName = Guid.NewGuid() + Path.GetExtension(dto.VideoFile!.FileName);
        string fullPath = Path.Combine(uploadDir, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await dto.VideoFile.CopyToAsync(stream);
        }
        dto.FilePath = "/videos/" + fileName;

        var createdVideoDto = await _videoService.CreateVideoAsync(dto, user.Id);

        string thumbUrl = "/images/default-thumbnail.jpg"; 

        if (dto.ThumbnailFile != null)
        {
            var thumbDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/thumbnails");
            if (!Directory.Exists(thumbDir)) Directory.CreateDirectory(thumbDir);

            string thumbFileName = Guid.NewGuid() + Path.GetExtension(dto.ThumbnailFile.FileName);
            string thumbFullPath = Path.Combine(thumbDir, thumbFileName);

            using (var stream = new FileStream(thumbFullPath, FileMode.Create))
            {
                await dto.ThumbnailFile.CopyToAsync(stream);
            }
            thumbUrl = "/thumbnails/" + thumbFileName;
        }

        var thumbnail = new Thumbnail
        {
            ThumbnailId = Guid.NewGuid(),
            VideoId = createdVideoDto.VideoId,
            Url = thumbUrl,
            IsDefault = true,
            Width = 1280,
            Height = 720
        };

        _context.Thumbnails.Add(thumbnail);
        await _context.SaveChangesAsync();

        if (dto.Visibility?.ToLower() == "public")
        {
            var subscribers = await _context.Subscriptions
                .Where(s => s.ChannelOwnerUserId == user.Id) 
                .Select(s => s.SubscriberUserId)           
                .ToListAsync();

            foreach (var subUserId in subscribers)
            {
                await _notificationService.CreateNotificationAsync(
                    subUserId,
                    $"{channel.Name} uploaded a new video: {dto.Title}",
                    $"/Video/Details/{createdVideoDto.VideoId}",
                    channel.OwnerProfileImagePath,
                    thumbUrl
                );
            }
        }

        return RedirectToAction(nameof(Index));
    }

    [Authorize]
    public async Task<IActionResult> Edit(Guid id)
    {
        var video = await _videoService.GetVideoByIdAsync(id);
        if (video == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);

        if (video.ChannelId != (await _channelService.GetAllChannelsAsync())
                                     .FirstOrDefault(c => c.OwnerUserId == user.Id)?.ChannelId)
        {
            return Forbid();
        }

        var updateDto = new UpdateVideoDto
        {
            Title = video.Title,
            Description = video.Description,
            CategoryId = video.CategoryId,
            TagIds = video.TagIds,
            Visibility = video.Visibility
        };

        var categories = await _categoryService.GetAllCategoriesAsync();
        var tags = await _tagService.GetAllTagsAsync();

        ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", video.CategoryId);
        ViewBag.Tags = new MultiSelectList(tags, "TagId", "Text", video.TagIds);

        return View(updateDto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Edit(Guid id, UpdateVideoDto dto)
    {
        var user = await _userManager.GetUserAsync(User);

        if (ModelState.IsValid)
        {
            await _videoService.UpdateVideoAsync(id, dto, user.Id);

            var video = await _context.Videos.FindAsync(id);
            if (video != null)
            {
                video.Visibility = dto.Visibility;
                _context.Videos.Update(video);
            }

            if (dto.ThumbnailFile != null)
            {
                var thumbDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/thumbnails");
                if (!Directory.Exists(thumbDir)) Directory.CreateDirectory(thumbDir);

                string fileName = Guid.NewGuid() + Path.GetExtension(dto.ThumbnailFile.FileName);
                string fullPath = Path.Combine(thumbDir, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await dto.ThumbnailFile.CopyToAsync(stream);
                }

                string newThumbUrl = "/thumbnails/" + fileName;

                var existingThumb = await _context.Thumbnails
                    .FirstOrDefaultAsync(t => t.VideoId == id && t.IsDefault);

                if (existingThumb != null)
                {
                    existingThumb.Url = newThumbUrl;
                    _context.Thumbnails.Update(existingThumb);
                }
                else
                {
                    _context.Thumbnails.Add(new Thumbnail
                    {
                        ThumbnailId = Guid.NewGuid(),
                        VideoId = id,
                        Url = newThumbUrl,
                        IsDefault = true
                    });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        var categories = await _categoryService.GetAllCategoriesAsync();
        var tags = await _tagService.GetAllTagsAsync();
        ViewBag.Categories = new SelectList(categories, "CategoryId", "Name", dto.CategoryId);
        ViewBag.Tags = new MultiSelectList(tags, "TagId", "Text", dto.TagIds);

        return View(dto);
    }

    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var video = await _videoService.GetVideoByIdAsync(id);
        if (video == null) return NotFound();
        return View(video);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        await _videoService.DeleteVideoAsync(id, user.Id);
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddComment(Guid videoId, string content)
    {
        var appUser = await _userManager.GetUserAsync(User);
        if (appUser == null) return Unauthorized();

        var video = await _videoService.GetVideoByIdAsync(videoId);

        var commentDto = new CommentDto
        {
            VideoId = videoId,
            Content = content,
            UserId = appUser.Id,
            CreatedAt = DateTime.UtcNow
        };

        await _commentService.CreateCommentAsync(commentDto);

        if (video != null && appUser.Id != video.ChannelOwnerUserId)
        {
            await _notificationService.CreateNotificationAsync(
                video.ChannelOwnerUserId,
                $"{appUser.DisplayName ?? appUser.UserName} commented: \"{Truncate(content)}\"",
                $"/Video/Details/{videoId}",
                appUser.ProfileImagePath,
                video.ThumbnailUrl 
            );
        }

        return RedirectToAction("Details", new { id = videoId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Reply(Guid videoId, Guid parentCommentId, string content)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var parentComment = await _commentService.GetCommentByIdAsync(parentCommentId);

        var reply = new CommentDto
        {
            VideoId = videoId,
            Content = content,
            UserId = user.Id,
            ParentCommentId = parentCommentId,
            CreatedAt = DateTime.UtcNow
        };

        await _commentService.CreateCommentAsync(reply);

        if (parentComment != null && user.Id != parentComment.UserId)
        {
            await _notificationService.CreateNotificationAsync(
                parentComment.UserId,
                $"{user.DisplayName ?? user.UserName} replied to your comment: \"{Truncate(content)}\"",
                $"/Video/Details/{videoId}",
                user.ProfileImagePath,
                null 
            );
        }

        return RedirectToAction("Details", new { id = videoId });
    }

    private List<CommentDto> BuildCommentTree(List<CommentDto> flat)
    {
        var lookup = flat.ToDictionary(c => c.CommentId);
        var root = new List<CommentDto>();

        foreach (var comment in flat)
        {
            if (comment.ParentCommentId == null)
            {
                root.Add(comment);
            }
            else if (lookup.ContainsKey(comment.ParentCommentId.Value))
            {
                lookup[comment.ParentCommentId.Value].Replies.Add(comment);
            }
        }

        return root;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IncrementView([FromBody] IncrementViewRequest request)
    {
        if (request == null || request.VideoId == Guid.Empty)
            return BadRequest("Invalid video ID");

        string sessionKey = $"Viewed_{request.VideoId}";
        if (HttpContext.Session.GetString(sessionKey) != null)
        {
            return Ok(new { message = "View already counted this session" });
        }

        var video = await _videoService.GetVideoByIdAsync(request.VideoId);
        if (video == null) return NotFound("Video not found");

        video.ViewCount++;
        await _videoService.UpdateVideoViewsAsync(request.VideoId, (int)video.ViewCount);

        HttpContext.Session.SetString(sessionKey, "true");

        return Ok(new { newViewCount = video.ViewCount });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ToggleLike([FromBody] IncrementViewRequest request)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var video = await _videoService.GetVideoByIdAsync(request.VideoId);
        if (video == null) return NotFound("Video not found");

        bool liked = await _videoService.ToggleLikeAsync(video.VideoId, user.Id);
        int likeCount = await _videoService.GetLikeCountAsync(video.VideoId);

        if (liked && user.Id != video.ChannelOwnerUserId)
        {
            await _notificationService.CreateNotificationAsync(
                video.ChannelOwnerUserId,
                $"{user.DisplayName ?? user.UserName} liked your video: {video.Title}",
                $"/Video/Details/{video.VideoId}",
                user.ProfileImagePath, 
                video.ThumbnailUrl
            );
        }

        return Ok(new { liked, likeCount });
    }
    private string Truncate(string text, int length = 25) =>
    text.Length <= length ? text : text.Substring(0, length) + "...";
}