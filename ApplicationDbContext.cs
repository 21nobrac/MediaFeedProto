using Microsoft.EntityFrameworkCore;

namespace MediaFeedProto;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User
        modelBuilder.Entity<User>()
            .HasKey(u => u.Username);

        // Configure Post
        modelBuilder.Entity<Post>()
            .HasKey(p => p.ID);

        // Configure Comment
        modelBuilder.Entity<Comment>()
            .HasKey(c => c.ID);
    }
}
