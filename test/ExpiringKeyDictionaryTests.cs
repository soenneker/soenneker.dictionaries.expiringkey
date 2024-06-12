using Soenneker.Dictionaries.ExpiringKey.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Dictionaries.ExpiringKey.Tests;

[Collection("Collection")]
public class ExpiringKeyDictionaryTests : FixturedUnitTest
{
    private readonly IExpiringKeyDictionary _util;

    public ExpiringKeyDictionaryTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IExpiringKeyDictionary>(true);
    }
}
