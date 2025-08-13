namespace UnitTests;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Concrete;
using booleancoercion.SpotifyShuffler.Spotify.Configuration;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

[TestClass]
public class AuthenticationProviderTests
{
    [DataTestMethod]
    [DataRow("abc123", "https://example.com/callback", true, "456def", "some_scope", "https://accounts.spotify.com/authorize?client_id=abc123&response_type=code&redirect_uri=https%3A%2F%2Fexample.com%2Fcallback&show_dialog=true&state=456def&scope=some_scope")]
    [DataRow("xyz789", "http://127.0.0.1:8888/callback", false, "1122333bgda", null, "https://accounts.spotify.com/authorize?client_id=xyz789&response_type=code&redirect_uri=http%3A%2F%2F127.0.0.1%3A8888%2Fcallback&show_dialog=false&state=1122333bgda")]
    public void TestGetUserAuthorizationUri(string clientId, string redirectUri, bool showDialog, string state, string? scope, string expectedUri)
    {
        AuthenticationConfiguration config = new()
        {
            ClientId = clientId,
            RedirectUri = redirectUri,
            AuthorizeUri = "https://accounts.spotify.com/authorize",

            ClientSecret = "irrelevant",
        };
        Mock<ICsrfStore> csrfStore = new();
        csrfStore.Setup(x => x.Generate()).Returns(state);
        AuthenticationProvider provider = new(Mock.Of<ILogger<AuthenticationProvider>>(), config, Mock.Of<IApiWrapper>(), csrfStore.Object, Mock.Of<IUserStore>());

        string uri = provider.GetUserAuthorizationUri(scope, showDialog);

        uri.Should().Be(expectedUri);
    }
}
