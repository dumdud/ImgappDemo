using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ImgappDemo.Models;

public class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    public string Name { get; set; } = default!;

    [DataType(DataType.DateTime)]
    public DateTime JoinDate { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = default!;
    public byte[] Avatar { get; set; } = File.ReadAllBytes("./wwwroot/defaultAvatars/testSloth.jpg");


    public ICollection<ImgPost> Posts { get; set; } = default!;
    public ICollection<ImgVotes> ImgVotes { get; set; } = default!;

    public ICollection<Comment> Comments { get; set; } = default!;
    public ICollection<CommentVotes> CommentVotes { get; set; } = default!;

}