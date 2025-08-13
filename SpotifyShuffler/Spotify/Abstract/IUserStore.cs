namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

using booleancoercion.SpotifyShuffler.Spotify.Concrete;

public interface IUserStore
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    void UpdateUser(User user);
    bool RemoveUser(string userId);
    IEnumerable<User> GetAllUsers();
    Task PersistAsync(CancellationToken cancellationToken = default);
}
