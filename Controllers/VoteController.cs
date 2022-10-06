using Microsoft.AspNetCore.Mvc;
using ImgappDemo.Models;
using ImgappDemo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;

namespace ImgappDemo.Controllers;

public class VoteController : Controller
{
    private readonly ILogger<VoteController> _logger;
    private readonly PostContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public VoteController(ILogger<VoteController> Logger, PostContext Context, IHttpContextAccessor HttpContextAccessor)
    {
        _logger = Logger;
        _context = Context;
        _httpContextAccessor = HttpContextAccessor;
    }

    [Authorize]
    [HttpPost("~/api/Vote")]
    public async Task<IActionResult> Vote([FromBody] JsonElement jsonReq)
    {

        // int? Id = HttpContext.Session.GetInt32("Id");
        int? Id = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.FindFirst("UserId")?.Value);

        if (_context == null || Id == null)
            return BadRequest();

        var PostIdArr = jsonReq.GetProperty("Postid")
                            .ToString()
                            .Split("-");

        var postId = Convert.ToInt32(PostIdArr[1]);

        var voteDir = jsonReq.GetProperty("Vote")
                            .GetInt32();

        if (PostIdArr[0] == "ip")
        {
            var post = await _context.ImgVotes.Where(p => p.UserId == Id && p.PostId == postId).FirstOrDefaultAsync();

            if (post == null)
            {
                var vote = new ImgVotes
                {
                    UserId = (int)Id,
                    PostId = postId,
                    Vote = voteDir
                };

                _context.ImgVotes.Add(vote);
            }
            else
            {
                post.Vote = voteDir;
                _context.ImgVotes.Update(post);
            }


        }
        else if (PostIdArr[0] == "cm")
        {
            var post = await _context.CommentVotes.Where(p => p.UserId == Id && p.PostId == postId).FirstOrDefaultAsync();

            if (post == null)
            {
                var vote = new CommentVotes
                {
                    UserId = (int)Id,
                    PostId = postId,
                    Vote = voteDir
                };

                _context.CommentVotes.Add(vote);
            }
            else
            {
                post.Vote = voteDir;
                _context.CommentVotes.Update(post);
            }
        }

        await _context.SaveChangesAsync();
        return Json(jsonReq);
    }
}