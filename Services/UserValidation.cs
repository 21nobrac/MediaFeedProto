using Microsoft.EntityFrameworkCore;

namespace MediaFeedProto;
public static class UserValidation
{
    public static async Task<User?> TryGetUser(string username, string password, ApplicationDbContext db)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user != null && user.Password == password)
        {
            return user;
        }
        return null;
    }
}