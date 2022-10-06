using Microsoft.AspNetCore.Mvc;
using ImgappDemo.Models;
using ImgappDemo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ImgappDemo.Controllers;

public class CommentController : Controller
{
    private readonly ILogger<PostsController> _logger;
    private readonly PostContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CommentController(ILogger<PostsController> logger, PostContext Context, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _context = Context;
        _httpContextAccessor = httpContextAccessor;
    }


    [Authorize]
    [HttpPost("comment")]
    public async Task<IActionResult> Comment(string postID)
    {

        // int? Id = HttpContext.Session.GetInt32("Id");
        int? Id = Convert.ToInt32(HttpContext?.User?.FindFirst("userId")?.Value);

        if (_context == null || Id == null)
            return BadRequest();

        int postId = Convert.ToInt32(Request.Form["PID"]);

        int? parentId = Convert.ToInt32(Request.Form["RID"]);

        User? Commenter = _context.User.Where(U => U.UserId == Id).FirstOrDefault();
        ImgPost? Post = _context.ImgPost.Where(P => P.PostId == postId).FirstOrDefault();

        if (Post == null)
            return BadRequest();

        if (Commenter == null)
            return Forbid();

        var comment = new Comment();
        comment.Message = Request.Form["text"];
        comment.User = Commenter;
        comment.PostDate = DateTime.Now;
        comment.Post = Post;


        if (parentId != 0)
            comment.ParentCommentId = parentId;

        if (Post.Comments == null)
            Post.Comments = new List<Comment>();

        Post.Comments.Add(comment);

        _context.ImgPost.Update(Post);
        await _context.SaveChangesAsync();

        return Redirect($"/Posts/Post/{Post.PostId}");
    }

}

