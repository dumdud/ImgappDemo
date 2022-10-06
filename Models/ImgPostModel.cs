using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ImgappDemo.Models
{
    public class ImgPost
    {
        [Key]
        public int PostId { get; set; }

        [Required]
        public string Title { get; set; } = default!;

        [Required]
        public byte[] Image { get; set; } = default!;

        public User User { get; set; } = default!;

        [DataType(DataType.DateTime)]
        public DateTime PostDate { get; set; }

        public ICollection<Comment> Comments { get; set; } = default!;

        public ICollection<ImgVotes>? Votes { get; set; }
    }

}
