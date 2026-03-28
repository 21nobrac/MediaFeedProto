using System.Collections;
using MediaFeedProto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=mediafeed.db"));

builder.Services.AddSingleton<SessionService>();
builder.Services.AddSingleton<FeedService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Initialize database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
    
    // Seed initial data if Users table is empty
    if (!db.Users.Any())
    {
        db.Users.Add(new User { Username = "Greg", Password = "pass" });
        db.Users.Add(new User { Username = "Carbon", Password = "A form of!" });
        db.SaveChanges();
    }
    
    // Seed initial post if Posts table is empty
    if (!db.Posts.Any())
    {
        db.Posts.Add(new Post { Title = "Hey", Username = "Greg", Body = "My First Post!", ID = "atotallyuniqueID_I_SWEAR" });
        db.SaveChanges();
    }
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.MapGet("/post/{postID}/comments", (string postID) =>
{
    string comment = ExamplePosts.RandomComment();
    return Results.Content(comment, "text/html");
});

app.Run();
