namespace booleancoercion.SpotifyShuffler;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletonConfiguration<T>(this IServiceCollection serviceCollection, IConfiguration configuration)
        where T : class
    {
        IConfigurationSection configurationSection = configuration.GetRequiredSection(typeof(T).Name);
        T? value = configurationSection.Get<T>();
        if (value is null)
        {
            throw new Exception($"Could not find valid configuration section for {typeof(T).FullName}");
        }

        return serviceCollection.AddSingleton(value);
    }
}
