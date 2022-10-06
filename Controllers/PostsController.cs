using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ImgappDemo.Data;
using ImgappDemo.Models;

namespace ImgappDemo.Controllers;

public class PostsController : Controller
{
    private readonly ILogger<PostsController> _logger;
    private readonly PostContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PostsController(ILogger<PostsController> logger, PostContext Context, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _context = Context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IActionResult> Index()
    {
        return _context.ImgPost != null ?
            View(
                await _context.ImgPost
                    .Include(u => u.User)
                    .Include(v => v.Votes)
                    .OrderByDescending(p => p.PostId)
                    .ToListAsync()) :
                    Problem("Entity set '_context.ImgPost' is null."
            );
    }

    public async Task<IActionResult> Post(int? id)
    {
        if (id == null || _context.ImgPost == null)
        {
            return NotFound();
        }

        var ImgPost = await _context.ImgPost
            .Include(c => c.Comments).ThenInclude(v => v.Votes)
            .Include(c => c.Comments).ThenInclude(c => c.User)
            .Include(u => u.User)
            .FirstOrDefaultAsync(m => m.PostId == id);

        if (ImgPost == null)
        {
            return NotFound();
        }

        return View(ImgPost);
    }

    [Authorize]
    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize]
    [HttpPost("Create")]
    public async Task<IActionResult> Create(ImgPost Post)
    {
        int? Id = HttpContext.Session.GetInt32("Id");

        if (_context == null || Id == null)
            return BadRequest();

        User? Poster = _context.User.Where(U => U.UserId == Id).FirstOrDefault();

        if (Poster == null)
            return Forbid();

        var img = new ImgPost();

        foreach (var file in Request.Form.Files)
        {
            img.Title = Post.Title;

            MemoryStream ms = new MemoryStream();
            file.CopyTo(ms);
            img.Image = ms.ToArray();

            img.User = Poster;
            img.PostDate = DateTime.Now;

            ms.Close();
            ms.Dispose();
            // Change this.
            _context.ImgPost.Add(img);
            await _context.SaveChangesAsync();
        }

        return Redirect("/");
    }
}

