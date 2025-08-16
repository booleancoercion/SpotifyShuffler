namespace booleancoercion.SpotifyShuffler.Spotify.Concrete;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Entities;
using booleancoercion.SpotifyShuffler.Util;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public class Shuffler : IShuffler
{
    private readonly ILogger _logger;
    private readonly IAuthenticationProvider _authenticationProvider;
    private readonly IApiWrapper _apiWrapper;
    private readonly IUserStore _userStore;

    public Shuffler(
        ILogger<Shuffler> logger,
        IAuthenticationProvider authenticationProvider,
        IApiWrapper apiWrapper,
        IUserStore userStore)
    {
        _logger = logger;
        _authenticationProvider = authenticationProvider;
        _apiWrapper = apiWrapper;
        _userStore = userStore;
    }

    public async Task ShuffleForUserAsync(TraceId traceId, AppUser user, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Shuffling relevant playlists for user {user}");

        string token = await _authenticationProvider.GetTokenAsync(traceId, user.Id, cancellationToken: cancellationToken);
        IReadOnlyList<Playlist> playlists = await _apiWrapper.GetCurrentUserPlaylistsAsync(traceId, token, cancellationToken);

        // We do this sequentially to not upset Spotify's rate limit
        List<Exception> exceptions = [];
        foreach (Playlist playlist in playlists)
        {
            if (playlist.Owner.Id != user.Id)
            {
                continue;
            }

            try
            {
                await ShufflePlaylistAsync(traceId, user, playlist, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, traceId, $"Error when shuffling playlist {playlist}");
                exceptions.Add(new Exception($"Error when shuffling playlist {playlist}", ex));
            }
        }

        if (exceptions.Count > 0)
        {
            throw new AggregateException($"Error when shuffling playlists for user {user}", exceptions);
        }
    }

    private async Task ShufflePlaylistAsync(TraceId traceId, AppUser user, Playlist playlist, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Shuffling playlist {playlist} for user {user}");

        string token = await _authenticationProvider.GetTokenAsync(traceId, user.Id, cancellationToken: cancellationToken);
        IReadOnlyList<TrackOrEpisode> tracks = await _apiWrapper.GetPlaylistTracksAsync(traceId, token, playlist.Id, cancellationToken);
        TrackOrEpisode[] tracksArray = tracks.ToArray();

        Random.Shared.Shuffle(tracksArray);

        bool isFirst = true;
        foreach (TrackOrEpisode[] chunk in tracksArray.Chunk(100))
        {
            if (isFirst)
            {
                isFirst = false;
                await _apiWrapper.SetPlaylistTracksAsync(traceId, token, playlist.Id, chunk.Select(x => x.Uri), cancellationToken);
                continue;
            }

            await _apiWrapper.AddPlaylistTracksAsync(traceId, token, playlist.Id, chunk.Select(x => x.Uri), cancellationToken);
        }
    }
}
