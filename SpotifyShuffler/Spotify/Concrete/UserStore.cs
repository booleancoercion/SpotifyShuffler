namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UserStore : IUserStore
{
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<string, User> _users = [];

    public UserStore(ILogger<UserStore> logger)
    {
        _logger = logger;
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _users.Values;
    }

    public Task InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public Task PersistAsync()
    {
        throw new NotImplementedException();
    }

    public bool RemoveUser(string userId)
    {
        return _users.Remove(userId, out User? _);
    }

    public void UpdateUser(User user)
    {
        _users[user.Id] = user;
    }
}
