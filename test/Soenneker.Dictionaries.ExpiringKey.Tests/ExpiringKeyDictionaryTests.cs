using AwesomeAssertions;
using Soenneker.Tests.FixturedUnit;
using Xunit;


namespace Soenneker.Dictionaries.ExpiringKey.Tests;

[Collection("Collection")]
public class ExpiringKeyDictionaryTests : FixturedUnitTest
{
    public ExpiringKeyDictionaryTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void TryAdd_should_function()
    {
        var dictionary = new ExpiringKeyDictionary();

        dictionary.TryAdd("test", 2000);

        dictionary.ContainsKey("test").Should().BeTrue();

    }
}
