using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.DTOs;
using Youtube_Entertainment_Project.Services.Implementations;
using Youtube_Entertainment_Project.Services.Interfaces;

[Authorize]
public class ChannelController : Controller
{
    private readonly IChannelService _channelService;
    private readonly UserManager<AppUser> _userManager;
    private readonly IMapper _mapper;
    private readonly ISubscriptionService _subscriptionService;

    public ChannelController(IChannelService channelService, UserManager<AppUser> userManager, IMapper mapper, ISubscriptionService subscriptionService)
    {
        _channelService = channelService;
        _userManager = userManager;
        _mapper = mapper;
        _subscriptionService = subscriptionService;
    }

    // GET: Channels
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        // Get all channels from service
        var channels = await _channelService.GetAllChannelsAsync();

        // Map to DTO (optional if service already returns DTO)
        var channelDtos = _mapper.Map<List<ChannelDto>>(channels);

        return View(channelDtos);
    }

    // GET: /Channel/Details/5
    [AllowAnonymous]
    public async Task<IActionResult> Details(Guid id)
    {
        var channelDto = await _channelService.GetChannelByIdAsync(id);
        if (channelDto == null) return NotFound();

        ViewBag.SubscriberCount = await _subscriptionService.GetSubscriberCountAsync(channelDto.OwnerUserId);

        if (User.Identity.IsAuthenticated)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null)
            {
                System.Diagnostics.Debug.WriteLine($"Checking: SubID {currentUser.Id} to OwnerID {channelDto.OwnerUserId}");

                ViewBag.IsSubscribed = await _subscriptionService.IsSubscribedAsync(currentUser.Id, channelDto.OwnerUserId);
            }
        }

        return View(channelDto);
    }

    // GET: /Channel/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var user = await _userManager.GetUserAsync(User);
        var hasChannel = await _channelService.ChannelExistsByOwnerIdAsync(user.Id);

        if (hasChannel)
        {
            return RedirectToAction("Index", "Home"); 
        }

        return View();
    }

    // POST: /Channel/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize] 
    public async Task<IActionResult> Create(ChannelDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        dto.OwnerUserId = user.Id;

        if (ModelState.IsValid)
        {
            await _channelService.CreateChannelAsync(dto, user.Id);

            return RedirectToAction(nameof(Index));
        }

        return View(dto);
    }

    // GET: /Channel/Edit/5
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var channel = await _channelService.GetChannelByIdAsync(id);
        if (channel == null) return NotFound();

        var user = await _userManager.GetUserAsync(User);
        if (channel.OwnerUserId != user.Id) return Forbid();

        return View(channel);
    }

    // POST: /Channel/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Edit(Guid id, ChannelDto dto)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (ModelState.IsValid)
        {
            await _channelService.UpdateChannelAsync(id, dto, user.Id);
            return RedirectToAction(nameof(Index));
        }

        return View(dto);
    }

    // GET: Channels/Delete/5
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var channel = await _channelService.GetChannelByIdAsync(id);
        if (channel == null) return NotFound();
        return View(channel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        await _channelService.DeleteChannelAsync(id, user.Id);
        return RedirectToAction(nameof(Index));
    }


}
