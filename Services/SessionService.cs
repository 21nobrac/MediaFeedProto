using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace MediaFeedProto;
public class SessionService
{
    /// <summary>
    /// The public-facing dictionary of active sessions. Maps session IDs to users. Readonly to prevent issues. Interact through ValidateSession and CreateSession.
    /// </summary>
    public IReadOnlyDictionary<string, User> ActiveSessions => activeSessions;
    // The backing dictionary.
    private readonly ConcurrentDictionary<string, User> activeSessions = new();
    
    /// <summary>
    /// Creates a new session for the user and returns the unique session ID, which is used for validation.
    /// </summary>
    /// <param name="user">The user for whom the session is being created.</param>
    /// <returns>The unique session ID.</returns>
    public string CreateSession(User user)
    {
        var sessionID = Guid.NewGuid().ToString();
        activeSessions[sessionID] = user;
        return sessionID;
    }

    /// <summary>
    /// Validates the session by checking for the id and validating its associated user against the database.
    /// </summary>
    /// <param name="sessionID">The unique session ID to validate.</param>
    /// <param name="db">The database context to use for validation.</param>
    /// <returns>User, if the session is valid; otherwise, null.</returns>
    public async Task<User?> ValidateSession(string sessionID, ApplicationDbContext db)
    {
        if (sessionID == null || !activeSessions.TryGetValue(sessionID, out var user))
        {
            return null;
        }
        var dbUser = await db.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
        return dbUser != null && dbUser.Password == user.Password ? dbUser : null;
    }
}
