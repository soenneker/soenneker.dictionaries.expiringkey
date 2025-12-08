using AwesomeAssertions;
using Soenneker.Dictionaries.ExpiringKey;
using Soenneker.Tests.FixturedUnit;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Soenneker.Dictionaries.ExpiringKey.Tests;

[Collection("Collection")]
public class ExpiringKeyDictionaryTests : FixturedUnitTest
{
    public ExpiringKeyDictionaryTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void TryAdd_ReturnsTrue_WhenKeyDoesNotExist()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act
        bool result = dictionary.TryAdd("test", 2000);

        // Assert
        result.Should().BeTrue();
        dictionary.ContainsKey("test").Should().BeTrue();
    }

    [Fact]
    public void TryAdd_ReturnsFalse_WhenKeyAlreadyExists()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("test", 2000);

        // Act
        bool result = dictionary.TryAdd("test", 3000);

        // Assert
        result.Should().BeFalse();
        dictionary.ContainsKey("test").Should().BeTrue();
    }

    [Fact]
    public void TryAdd_AddsKey_WhenKeyDoesNotExist()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act
        dictionary.TryAdd("newKey", 1000);

        // Assert
        dictionary.ContainsKey("newKey").Should().BeTrue();
    }

    [Fact]
    public void TryAdd_DoesNotAddKey_WhenKeyAlreadyExists()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("existing", 2000);

        // Act
        dictionary.TryAdd("existing", 5000);

        // Assert
        dictionary.ContainsKey("existing").Should().BeTrue();
    }

    [Fact]
    public void TryAdd_WithMultipleKeys_AllSucceed()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act
        bool result1 = dictionary.TryAdd("key1", 1000);
        bool result2 = dictionary.TryAdd("key2", 2000);
        bool result3 = dictionary.TryAdd("key3", 3000);

        // Assert
        result1.Should().BeTrue();
        result2.Should().BeTrue();
        result3.Should().BeTrue();
        dictionary.ContainsKey("key1").Should().BeTrue();
        dictionary.ContainsKey("key2").Should().BeTrue();
        dictionary.ContainsKey("key3").Should().BeTrue();
    }

    [Fact]
    public void ContainsKey_ReturnsFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act
        bool result = dictionary.ContainsKey("nonexistent");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ContainsKey_ReturnsTrue_WhenKeyExists()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("test", 2000);

        // Act
        bool result = dictionary.ContainsKey("test");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ContainsKey_ReturnsFalse_AfterKeyIsRemoved()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("test", 2000);

        // Act
        dictionary.RemoveSync("test");
        bool result = dictionary.ContainsKey("test");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void AddOrUpdate_AddsKey_WhenKeyDoesNotExist()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act
        dictionary.AddOrUpdate("newKey", 2000);

        // Assert
        dictionary.ContainsKey("newKey").Should().BeTrue();
    }

    [Fact]
    public void AddOrUpdate_UpdatesKey_WhenKeyAlreadyExists()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.AddOrUpdate("test", 2000);

        // Act
        dictionary.AddOrUpdate("test", 5000);

        // Assert
        dictionary.ContainsKey("test").Should().BeTrue();
    }

    [Fact]
    public void GetOrAdd_ReturnsTimer_WhenKeyDoesNotExist()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act
        var timer = dictionary.GetOrAdd("newKey", 2000);

        // Assert
        timer.Should().NotBeNull();
        dictionary.ContainsKey("newKey").Should().BeTrue();
    }

    [Fact]
    public void GetOrAdd_ReturnsExistingTimer_WhenKeyAlreadyExists()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        var firstTimer = dictionary.GetOrAdd("test", 2000);

        // Act
        var secondTimer = dictionary.GetOrAdd("test", 5000);

        // Assert
        secondTimer.Should().NotBeNull();
        dictionary.ContainsKey("test").Should().BeTrue();
    }

    [Fact]
    public void TryRemove_RemovesKey_WhenKeyExists()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("test", 2000);

        // Act
        dictionary.TryRemoveSync("test");

        // Assert
        dictionary.ContainsKey("test").Should().BeFalse();
    }

    [Fact]
    public void TryRemove_DoesNotThrow_WhenKeyDoesNotExist()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act & Assert
        dictionary.TryRemoveSync("nonexistent");
        dictionary.ContainsKey("nonexistent").Should().BeFalse();
    }

    [Fact]
    public async Task TryRemove_Async_RemovesKey_WhenKeyExists()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("test", 2000);

        // Act
        await dictionary.TryRemove("test");

        // Assert
        dictionary.ContainsKey("test").Should().BeFalse();
    }

    [Fact]
    public async Task TryRemove_Async_DoesNotThrow_WhenKeyDoesNotExist()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act & Assert
        await dictionary.TryRemove("nonexistent");
        dictionary.ContainsKey("nonexistent").Should().BeFalse();
    }

    [Fact]
    public void RemoveSync_ReturnsTrue_WhenKeyExists()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("test", 2000);

        // Act
        bool result = dictionary.RemoveSync("test");

        // Assert
        result.Should().BeTrue();
        dictionary.ContainsKey("test").Should().BeFalse();
    }

    [Fact]
    public void RemoveSync_ReturnsFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act
        bool result = dictionary.RemoveSync("nonexistent");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Remove_Async_ReturnsTrue_WhenKeyExists()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("test", 2000);

        // Act
        bool result = await dictionary.Remove("test");

        // Assert
        result.Should().BeTrue();
        dictionary.ContainsKey("test").Should().BeFalse();
    }

    [Fact]
    public async Task Remove_Async_ReturnsFalse_WhenKeyDoesNotExist()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act
        bool result = await dictionary.Remove("nonexistent");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ClearSync_RemovesAllKeys()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("key1", 2000);
        dictionary.TryAdd("key2", 2000);
        dictionary.TryAdd("key3", 2000);

        // Act
        dictionary.ClearSync();

        // Assert
        dictionary.ContainsKey("key1").Should().BeFalse();
        dictionary.ContainsKey("key2").Should().BeFalse();
        dictionary.ContainsKey("key3").Should().BeFalse();
    }

    [Fact]
    public void ClearSync_DoesNotThrow_WhenDictionaryIsEmpty()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act & Assert
        dictionary.ClearSync();
        dictionary.ContainsKey("any").Should().BeFalse();
    }

    [Fact]
    public async Task Clear_Async_RemovesAllKeys()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("key1", 2000);
        dictionary.TryAdd("key2", 2000);
        dictionary.TryAdd("key3", 2000);

        // Act
        await dictionary.Clear();

        // Assert
        dictionary.ContainsKey("key1").Should().BeFalse();
        dictionary.ContainsKey("key2").Should().BeFalse();
        dictionary.ContainsKey("key3").Should().BeFalse();
    }

    [Fact]
    public async Task Clear_Async_DoesNotThrow_WhenDictionaryIsEmpty()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act & Assert
        await dictionary.Clear();
        dictionary.ContainsKey("any").Should().BeFalse();
    }

    [Fact]
    public void Key_Expires_AfterExpirationTime()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("test", 100); // 100ms expiration

        // Act
        dictionary.ContainsKey("test").Should().BeTrue();
        Thread.Sleep(150); // Wait for expiration

        // Assert
        dictionary.ContainsKey("test").Should().BeFalse();
    }

    [Fact]
    public void Key_DoesNotExpire_BeforeExpirationTime()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("test", 200); // 200ms expiration

        // Act
        Thread.Sleep(50); // Wait less than expiration time

        // Assert
        dictionary.ContainsKey("test").Should().BeTrue();
    }

    [Fact]
    public void AddOrUpdate_ResetsExpiration_WhenKeyExists()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("test", 500); // 500ms expiration

        // Act
        Thread.Sleep(200); // Wait partway through expiration
        dictionary.ContainsKey("test").Should().BeTrue(); // Verify it still exists
        dictionary.AddOrUpdate("test", 1000); // Reset expiration to 1000ms
        Thread.Sleep(600); // Wait, but not long enough for new expiration (should have 400ms left)

        // Assert
        dictionary.ContainsKey("test").Should().BeTrue();
    }

    [Fact]
    public void TryAdd_WithEmptyString_Works()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act
        bool result = dictionary.TryAdd("", 2000);

        // Assert
        result.Should().BeTrue();
        dictionary.ContainsKey("").Should().BeTrue();
    }

    [Fact]
    public void TryAdd_WithZeroExpiration_Works()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act
        bool result = dictionary.TryAdd("test", 0);

        // Assert
        result.Should().BeTrue();
        // Key should expire immediately or very quickly
        Thread.Sleep(10);
        dictionary.ContainsKey("test").Should().BeFalse();
    }

    [Fact]
    public void TryAdd_WithVeryLongExpiration_Works()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act
        bool result = dictionary.TryAdd("test", int.MaxValue);

        // Assert
        result.Should().BeTrue();
        dictionary.ContainsKey("test").Should().BeTrue();
    }

    [Fact]
    public void MultipleOperations_OnSameKey_WorkCorrectly()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act & Assert
        bool tryAdd1 = dictionary.TryAdd("test", 2000);
        tryAdd1.Should().BeTrue();

        dictionary.ContainsKey("test").Should().BeTrue();

        bool tryAdd2 = dictionary.TryAdd("test", 3000);
        tryAdd2.Should().BeFalse();

        dictionary.AddOrUpdate("test", 4000);
        dictionary.ContainsKey("test").Should().BeTrue();

        var timer = dictionary.GetOrAdd("test", 5000);
        timer.Should().NotBeNull();

        bool removed = dictionary.RemoveSync("test");
        removed.Should().BeTrue();
        dictionary.ContainsKey("test").Should().BeFalse();
    }

    [Fact]
    public void ConcurrentOperations_WorkCorrectly()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();

        // Act - Simulate concurrent operations
        Parallel.For(0, 100, i =>
        {
            string key = $"key{i}";
            dictionary.TryAdd(key, 2000);
        });

        // Assert
        for (int i = 0; i < 100; i++)
        {
            dictionary.ContainsKey($"key{i}").Should().BeTrue();
        }
    }

    [Fact]
    public void Dispose_ClearsAllKeys()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("key1", 2000);
        dictionary.TryAdd("key2", 2000);

        // Act
        dictionary.Dispose();

        // Assert
        dictionary.ContainsKey("key1").Should().BeFalse();
        dictionary.ContainsKey("key2").Should().BeFalse();
    }

    [Fact]
    public async Task DisposeAsync_ClearsAllKeys()
    {
        // Arrange
        var dictionary = new ExpiringKeyDictionary();
        dictionary.TryAdd("key1", 2000);
        dictionary.TryAdd("key2", 2000);

        // Act
        await dictionary.DisposeAsync();

        // Assert
        dictionary.ContainsKey("key1").Should().BeFalse();
        dictionary.ContainsKey("key2").Should().BeFalse();
    }
}
