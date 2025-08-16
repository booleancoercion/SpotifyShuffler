namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

public class UserStore : IUserStore
{
    private readonly ILogger _logger;
    private readonly UserStoreConfiguration _configuration;
    private readonly SemaphoreSlim _persistSemaphore = new(1, 1);
    private ConcurrentDictionary<string, AppUser> _users = [];
    private bool isDisposed = false;

    public UserStore(ILogger<UserStore> logger, UserStoreConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public IEnumerable<AppUser> GetAllUsers()
    {
        return _users.Values;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"{nameof(InitializeAsync)}: Initializing {nameof(UserStore)}.");

        if (string.IsNullOrEmpty(_configuration.StorePath))
        {
            _logger.LogWarning($"{nameof(InitializeAsync)}: Store path is unset, will not load or save store to disk.");
            return;
        }

        try
        {
            if (!File.Exists(_configuration.StorePath))
            {
                _logger.LogWarning($"{nameof(InitializeAsync)}: No file exists at store path, using empty store.");
                return;
            }

            using FileStream file = new(_configuration.StorePath, FileMode.Open);
            AppUser[]? store = await JsonSerializer.DeserializeAsync<AppUser[]>(file, cancellationToken: cancellationToken);

            if (store is null)
            {
                throw new Exception($"Could not deserialize file contents to {typeof(AppUser[]).FullName}.");
            }

            Dictionary<string, AppUser> values = [];
            List<AppUser> duplicates = [];
            foreach (AppUser user in store)
            {
                if (!values.TryAdd(user.Id, user))
                {
                    duplicates.Add(user);
                }
            }

            if (duplicates.Count > 0)
            {
                throw new Exception($"Found duplicate entries for users: {string.Join(", ", duplicates)}");
            }

            _users = new ConcurrentDictionary<string, AppUser>(values);
            _logger.LogInformation($"{nameof(InitializeAsync)}: {nameof(UserStore)} initialized successfully. Loaded {values.Count} entries.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(InitializeAsync)}: Caught exception while loading file from disk. {nameof(UserStore)} not initialized.");
        }
    }

    public async Task PersistAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"{nameof(PersistAsync)}: Persisting {nameof(UserStore)} to disk.");

        if (string.IsNullOrEmpty(_configuration.StorePath))
        {
            _logger.LogWarning($"{nameof(PersistAsync)}: Store path is unset, will not load or save store to disk.");
            return;
        }

        await _persistSemaphore.WaitAsync(cancellationToken);
        try
        {
            List<AppUser> userList = _users.Values.ToList();
            userList.Sort((u1, u2) => string.Compare(u1.Id, u2.Id));

            using FileStream file = new(_configuration.StorePath, FileMode.Create);
            await JsonSerializer.SerializeAsync(file, userList, cancellationToken: cancellationToken);

            _logger.LogInformation($"{nameof(PersistAsync)}: {nameof(UserStore)} saved successfully. Saved {userList.Count} entries.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"{nameof(PersistAsync)}: Caught exception while saving file to disk. Save state unknown.");
        }
        finally
        {
            _persistSemaphore.Release();
        }
    }

    public bool RemoveUser(string userId)
    {
        return _users.Remove(userId, out AppUser? _);
    }

    public void UpdateUser(AppUser user)
    {
        _users[user.Id] = user;
    }

    public AppUser GetUser(string userId)
    {
        if (!_users.TryGetValue(userId, out AppUser? output))
        {
            throw new Exception($"There is no user with ID {userId}");
        }

        return output;
    }
}
