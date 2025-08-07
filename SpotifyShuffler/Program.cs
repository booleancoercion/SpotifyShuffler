namespace booleancoercion.SpotifyShuffler;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddHostedService<SpotifyShufflerBackgroundService>();

        IHost host = builder.Build();
        host.Run();
    }
}
