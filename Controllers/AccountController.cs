using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImgappDemo.Data;
using ImgappDemo.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ImgappDemo.Controllers;

public class AccountController : Controller
{
    private readonly ILogger<PostsController> _logger;
    private readonly PostContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AccountController(ILogger<PostsController> logger, PostContext Context, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _context = Context;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("Login")]
    public IActionResult Login(string ReturnUrl)
    {
        ViewData["ReturnUrl"] = ReturnUrl;
        return View();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(User userForm, string? ReturnUrl)
    {

        if (_context is null)
            return Forbid();

        var UserDB = _context.User.Where(u => u.Name == userForm.Name).FirstOrDefault();

        if (UserDB != null && HashPassword(userForm.Password) == UserDB.Password)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, UserDB.Name as string),
                    new Claim("UserId", UserDB.UserId.ToString())
                };

            var claimIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);

            HttpContext.Session.SetInt32("Id", UserDB.UserId);

            if (ReturnUrl != null)
                return Redirect(ReturnUrl);

            return Redirect("/");

        }

        TempData["Error"] = "something went wrong";
        return View();
    }


    [HttpGet("Register")]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register(User newUser)
    {

        if (await _context.User.Where(x => x.Name == newUser.Name).FirstOrDefaultAsync() != null)
        {
            TempData["Error"] = "Username already in use.";
            return View();
        }

        if (await _context.User.Where(x => x.Email == newUser.Email).FirstOrDefaultAsync() != null)
        {
            TempData["Error"] = "Email already in use.";
            return View();
        }

        newUser.JoinDate = DateTime.Now;
        newUser.Password = HashPassword(newUser.Password);
        await _context.User.AddAsync(newUser);
        await _context.SaveChangesAsync();
        return Redirect("/login");
    }

    [Authorize]
    [HttpGet("Settings")]
    public IActionResult Settings()
    {
        int? userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value);

        var user = _context.User.Where(u => u.UserId == userId).FirstOrDefault();


        return View(user);
    }

    [Authorize]
    [HttpPost("Settings")]
    public async Task<IActionResult> EditSettings(User editedUser)
    {
        _context.User.Update(editedUser);
        await _context.SaveChangesAsync();

        return View();
    }

    [Authorize]
    [HttpGet("Logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return Redirect("/");
    }

    private string HashPassword(string? password)
    {
        if (password == null)
            return "";

        SHA256 sha = SHA256.Create();

        var passwordBytes = System.Text.Encoding.Default.GetBytes(password);

        var sb = new StringBuilder();

        foreach (byte b in sha.ComputeHash(passwordBytes))
        {
            sb.Append(b);
        }

        return sb.ToString();
    }

    [Authorize]
    [HttpGet("Profile")]
    public async Task<IActionResult> Profile()
    {
        int? userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value);

        if (userId == null)
            Forbid();

        var help = await _context.User
            .Where(x => x.UserId == userId)
            .Include(x => x.Posts)
            .ThenInclude(x => x.Votes)
            .FirstOrDefaultAsync();

        ViewData["Current"] = "posts";
        return View(help);
    }

    [Authorize]
    [HttpGet("Profile/Upvotes")]
    public async Task<IActionResult> Upvotes()
    {
        int? userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value);

        if (userId == null)
            Forbid();

        var help = await _context.ImgVotes
            .Where(x => x.UserId == userId)
            .Include(x => x.ImgPost)
            .ThenInclude(x => x.User)
            .Include(x => x.ImgPost.Votes)
            .Where(x => x.Vote == 1)
            .ToListAsync();
        ViewData["Current"] = "upvotes";
        return View(help);
    }

    [Authorize]
    [HttpGet("Profile/Downvotes")]
    public async Task<IActionResult> Downvotes()
    {
        int? userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value);

        if (userId == null)
            Forbid();

        var help = await _context.ImgVotes
            .Where(x => x.UserId == userId)
            .Include(x => x.ImgPost)
            .ThenInclude(x => x.User)
            .Include(x => x.ImgPost.Votes)
            .Where(x => x.Vote == -1)
            .ToListAsync();

        ViewData["Current"] = "downvotes";

        return View(help);
    }

    [Authorize]
    [HttpGet("Profile/Comments")]
    public async Task<IActionResult> Comments()
    {
        int? userId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User?.FindFirst("UserId")?.Value);

        if (userId == null)
            Forbid();

        var help = await _context.User
            .Where(x => x.UserId == userId)
            .Include(x => x.Comments)
            .ThenInclude(x => x.Post)
            .FirstOrDefaultAsync();
        ViewData["Current"] = "comments";

        return View(help);
    }
}