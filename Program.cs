using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Youtube_Entertainment_Project.Data;
using Youtube_Entertainment_Project.Data.Entity;
using Youtube_Entertainment_Project.Data.Seeders;
using Youtube_Entertainment_Project.Hubs;
using Youtube_Entertainment_Project.Mappings;
using Youtube_Entertainment_Project.Repositories.Implementations;
using Youtube_Entertainment_Project.Repositories.Interfaces;
using Youtube_Entertainment_Project.Services.Implementations;
using Youtube_Entertainment_Project.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------
// Database
// ------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ------------------------------------------------------
// IDENTITY 
// ------------------------------------------------------
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 1;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 524288000; 
});

builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 524288000;
});

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 268435456;
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// ------------------------------------------------------
// Email Sender 
// ------------------------------------------------------
builder.Services.AddTransient<IEmailSender, FakeEmailSender>();  

// ------------------------------------------------------
// MVC & Razor Pages
// ------------------------------------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// ------------------------------------------------------
// Repositories
// ------------------------------------------------------
builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IChannelRepository, ChannelRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IPlaylistRepository, PlaylistRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// ------------------------------------------------------
// Services
// ------------------------------------------------------
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<IChannelService, ChannelService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IPlaylistService, PlaylistService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// ------------------------------------------------------
// AutoMapper
// ------------------------------------------------------
builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<YouTubeProfile>();
});

//-------------------------------------------------------
// SignalR
//-------------------------------------------------------
builder.Services.AddSignalR();

// ------------------------------------------------------
// Build App
// ------------------------------------------------------
var app = builder.Build();

// ------------------------------------------------------
// Seed Roles 
// ------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    await RoleSeeder.SeedRolesAsync(roleManager);
    await RoleSeeder.SeedSuperAdminAsync(userManager);
}

// ------------------------------------------------------
// Middleware Pipeline
// ------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub");

// ------------------------------------------------------
// Routes
// ------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
