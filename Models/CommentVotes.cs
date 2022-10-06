
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace ImgappDemo.Models;

public class CommentVotes
{
    public int Id { get; set; }

    [Required]
    [ForeignKey("PostId")]
    public Comment Comment { get; set; } = default!;

    [Required]
    [ForeignKey("UserId")]
    public User User { get; set; } = default!;

    public int UserId { get; set; }

    public int PostId { get; set; }

    public int Vote { get; set; }

}