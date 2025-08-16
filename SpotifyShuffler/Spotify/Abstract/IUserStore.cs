namespace booleancoercion.SpotifyShuffler.Spotify.Abstract;

using booleancoercion.SpotifyShuffler.Spotify.Concrete;

public interface IUserStore
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    void UpdateUser(AppUser user);
    AppUser GetUser(string userId);
    bool RemoveUser(string userId);
    IEnumerable<AppUser> GetAllUsers();
    Task PersistAsync(CancellationToken cancellationToken = default);
}
