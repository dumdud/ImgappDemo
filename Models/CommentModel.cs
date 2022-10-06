using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImgappDemo.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [InverseProperty("Comments")]
        public User User { get; set; } = default!;
        public ImgPost Post { get; set; } = default!;
        public string? Message { get; set; }

        [DataType(DataType.Date)]
        public DateTime PostDate { get; set; }

        public ICollection<CommentVotes>? Votes { get; set; }

        [ForeignKey("ParentCommentId")]
        public ICollection<Comment>? Replies { get; set; }
        public int? ParentCommentId { get; set; }
    }

}
