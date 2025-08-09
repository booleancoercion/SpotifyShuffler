namespace UnitTests;

using booleancoercion.SpotifyShuffler.Spotify.Abstract;
using booleancoercion.SpotifyShuffler.Spotify.Concrete;
using FluentAssertions;
using Moq;

[TestClass]
public class CsrfStoreTests
{
    private readonly Mock<ITimeProvider> _timeProvider;
    private readonly CsrfStore _instance;

    public CsrfStoreTests()
    {
        _timeProvider = new Mock<ITimeProvider>();
        _instance = new CsrfStore(_timeProvider.Object);
    }

    [TestMethod]
    public void BasicFlow()
    {
        string token = _instance.Generate();
        bool exists = _instance.Consume(token);

        token.Should().NotBeEmpty();
        exists.Should().BeTrue();
    }

    [TestMethod]
    public void TokensShouldBeDifferent()
    {
        string token1 = _instance.Generate();
        string token2 = _instance.Generate();

        token1.Should().NotBe(token2);
    }

    [TestMethod]
    public void InvalidTokenShouldNotBeConsumed()
    {
        bool exists = _instance.Consume("invalid");

        exists.Should().BeFalse();
    }

    [TestMethod]
    public void CleanupBasicFlow()
    {
        DateTime baseTime = new(2025, 8, 10, 1, 18, 20);

        _timeProvider.SetupSequence(x => x.Now)
            .Returns(baseTime)
            .Returns(baseTime + TimeSpan.FromMinutes(6))
            .Returns(baseTime + TimeSpan.FromMinutes(8));

        string token1 = _instance.Generate();
        string token2 = _instance.Generate();

        _instance.Cleanup(maxAge: TimeSpan.FromMinutes(5));

        bool exists1 = _instance.Consume(token1);
        bool exists2 = _instance.Consume(token2);

        exists1.Should().BeFalse();
        exists2.Should().BeTrue();
    }
}
