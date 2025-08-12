namespace booleancoercion.SpotifyShuffler;

using booleancoercion.SpotifyShuffler.Spotify;
using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Concrete;
using booleancoercion.SpotifyShuffler.Spotify.Configuration;
using booleancoercion.SpotifyShuffler.Util;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        RegisterServices(builder.Services, builder.Configuration);

        WebApplication host = builder.Build();
        host.MapControllers();
        host.Run("http://127.0.0.1:8888");
    }

    private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ICsrfStore, CsrfStore>();

        services.AddSingletonConfiguration<UserStoreConfiguration>(configuration);
        services.AddSingleton<IUserStore, UserStore>();

        services.AddSingletonConfiguration<ApiConfiguration>(configuration);
        services.AddSingleton<IApiWrapper, ApiWrapper>();

        services.AddSingletonConfiguration<AuthenticationConfiguration>(configuration);
        services.AddSingleton<IAuthenticationProvider, AuthenticationProvider>();

        services.AddSingleton<IShuffler, Shuffler>();

        services.AddHostedService<SpotifyShufflerBackgroundService>();

        services.AddControllers();
    }
}
