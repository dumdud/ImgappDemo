using Microsoft.EntityFrameworkCore;
using ImgappDemo.Models;

namespace ImgappDemo.Data;
public class PostContext : DbContext
{
    public PostContext(DbContextOptions<PostContext> options) : base(options)
    {
    }

    public DbSet<Comment> Comment => Set<Comment>();
    public DbSet<User> User => Set<User>();
    public DbSet<ImgPost> ImgPost => Set<ImgPost>();
    public DbSet<ImgVotes> ImgVotes => Set<ImgVotes>();
    public DbSet<CommentVotes> CommentVotes => Set<CommentVotes>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        foreach (var fkey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            fkey.DeleteBehavior = DeleteBehavior.NoAction;
        }
    }
}