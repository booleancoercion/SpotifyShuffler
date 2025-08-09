namespace booleancoercion.SpotifyShuffler;

using booleancoercion.SpotifyShuffler.Spotify;
using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Concrete;
using booleancoercion.SpotifyShuffler.Spotify.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public static class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        RegisterServices(builder.Services, builder.Configuration);

        IHost host = builder.Build();
        host.Run();
    }

    private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingletonConfiguration<ApiConfiguration>(configuration);
        services.AddSingleton<IApiWrapper, ApiWrapper>();

        services.AddSingletonConfiguration<AuthenticationConfiguration>(configuration);

        services.AddSingleton<IShuffler, Shuffler>();

        services.AddHostedService<SpotifyShufflerBackgroundService>();
    }
}
