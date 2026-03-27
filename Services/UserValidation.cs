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
    public static async Task<User?> ValidateSession(string sessionID, Dictionary<string, User> activeSessions, ApplicationDbContext db)
    {
        if (sessionID == null || !activeSessions.TryGetValue(sessionID, out var user))
        {
            return null;
        }
        var dbUser = await db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        return dbUser != null && dbUser.Password == user.Password ? dbUser : null;
    }
}