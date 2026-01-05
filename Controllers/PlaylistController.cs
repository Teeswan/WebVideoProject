using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Services.Interfaces;

namespace Youtube_Entertainment_Project.Controllers
{
    public class PlaylistController : Controller
    {
        private readonly IPlaylistService _playlistService;
        private readonly UserManager<AppUser> _userManager;


        public PlaylistController(IPlaylistService playlistService, UserManager<AppUser> userManager)
        {
            _playlistService = playlistService;
            _userManager = userManager;
        }

        // GET: /Playlist
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var isSuperAdmin = user != null && await _userManager.IsInRoleAsync(user, "SuperAdmin");

            var allPlaylists = await _playlistService.GetAllPlaylistsAsync();

            var visiblePlaylists = allPlaylists.Where(p =>
                p.IsPublic == true ||
                (user != null && p.OwnerUserId == user.Id) ||
                isSuperAdmin
            ).ToList();

            return View(visiblePlaylists);
        }

        // GET: /Playlist/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(id);
            if (playlist == null) return NotFound();
            return View(playlist);
        }

        // GET: /Playlist/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Playlist/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(PlaylistDto playlistDto)
        {
            if (playlistDto.OwnerUserId == Guid.Empty)
            {
                var userId = _userManager.GetUserId(User);
                if (Guid.TryParse(userId, out Guid parsedId))
                {
                    playlistDto.OwnerUserId = parsedId;
                }
            }

            ModelState.Remove("OwnerUserId");

            if (ModelState.IsValid)
            {
                await _playlistService.CreatePlaylistAsync(playlistDto);
                return RedirectToAction(nameof(Index));
            }

            return View(playlistDto);
        }

        // GET: /Playlist/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(id);
            if (playlist == null) return NotFound();
            return View(playlist);
        }

        // POST: /Playlist/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid id, PlaylistDto dto)
        {
            if (id != dto.PlaylistId) return BadRequest();

            if (ModelState.IsValid)
            {
                await _playlistService.UpdatePlaylistAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // GET: /Playlist/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var playlist = await _playlistService.GetPlaylistByIdAsync(id);
            if (playlist == null) return NotFound();
            return View(playlist);
        }

        // POST: /Playlist/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _playlistService.DeletePlaylistAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserPlaylists()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // ➡️ OPTIMIZATION: Filter at the Database level via the Service
            // Don't use "GetAll", use a specific "GetByUserId" method
            var userPlaylists = await _playlistService.GetPlaylistsByUserIdAsync(user.Id);

            var result = userPlaylists.Select(p => new {
                p.PlaylistId,
                p.Title,
                p.IsPublic // Keep this so you can show a 'Private' label in the UI
            });

            return Json(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddVideo([FromForm] Guid playlistId, [FromForm] Guid videoId)
        {
            try
            {
                await _playlistService.AddVideoToPlaylistAsync(playlistId, videoId);
                return Json(new { success = true, message = "Added to playlist!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
