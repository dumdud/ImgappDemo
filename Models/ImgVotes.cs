
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImgappDemo.Models;
public class ImgVotes
{
    public int Id { get; set; }

    [Required]
    [ForeignKey("PostId")]
    public ImgPost ImgPost { get; set; } = default!;
    public int PostId { get; set; } = default!;

    [Required]
    [ForeignKey("UserId")]
    public User User { get; set; } = default!;
    public int UserId { get; set; } = default!;

    public int Vote { get; set; }

}