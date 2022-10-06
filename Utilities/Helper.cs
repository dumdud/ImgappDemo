using ImgappDemo.Models;
using System.Security.Claims;
using NodaTime;



namespace ImgappDemo.Utilities;
public class Helper
{
    public static string DateToString(DateTime date)
    {
        Period period = Period.Between(LocalDateTime.FromDateTime(date),
                LocalDateTime.FromDateTime(DateTime.Now));

        if (!period.HasDateComponent)
        {
            if (period.Hours >= 1)
            {
                return period.Hours > 1 ? $"{period.Hours} Hours ago" : $"{period.Hours} Hour ago";
            }
            else if (period.Minutes >= 1)
            {
                return period.Minutes > 1 ? $"{period.Minutes} Minutes ago" : $"{period.Minutes} Minute ago";
            }
            else if (period.Seconds >= 1)
            {
                return period.Seconds > 1 ? $"{period.Seconds} Seconds ago" : $"{period.Seconds} Seconds ago";
            }
        }
        else
        {
            if (period.Years >= 1)
            {
                return period.Years > 1 ? $"{period.Years} Years ago" : $"{period.Years} Year ago";
            }
            else if (period.Months >= 1)
            {
                return period.Months > 1 ? $"{period.Months} Months ago" : $"{period.Months} Month ago";
            }
            else if (period.Weeks >= 1)
            {
                return period.Weeks > 1 ? $"{period.Weeks} Weeks ago" : $"{period.Weeks} Week ago";
            }
            else if (period.Days >= 1)
            {
                return period.Days > 1 ? $"{period.Days} Days ago" : $"{period.Days} Day ago";
            }
        }

        return "Just now"; ;
    }

    public static bool GetPostVoteDir(ImgPost post, ClaimsPrincipal? userClaim, int dir)
    {
        if (userClaim == null)
        {
            return false;
        }

        int userId = Convert.ToInt32(userClaim.FindFirst("UserId")?.Value);
        var vote = post.Votes?.Where(x => x.UserId == userId).FirstOrDefault()?.Vote;

        return vote == dir;
    }

    public static bool GetPostVoteDir(Comment comment, ClaimsPrincipal? userClaim, int dir)
    {
        if (userClaim == null)
        {
            return false;
        }

        int userId = Convert.ToInt32(userClaim.FindFirst("UserId")?.Value);
        var vote = comment.Votes?.Where(x => x.UserId == userId).FirstOrDefault()?.Vote;

        return vote == dir;
    }
}