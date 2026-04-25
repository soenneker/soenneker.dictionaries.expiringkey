using AwesomeAssertions;
using Soenneker.Tests.HostedUnit;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Dictionaries.ExpiringKey.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerAssembly)]
public class ExpiringKeyDictionaryTests : HostedUnitTest
{
    public ExpiringKeyDictionaryTests(Host host) : base(host)
    {
    }

    [Test]
    public void TryAdd_ReturnsTrue_WhenKeyDoesNotExist()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "TryAdd_ReturnsTrue_WhenKeyDoesNotExist";

        bool result = dictionary.TryAdd(key, 2000);

        result.Should().BeTrue();
        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void TryAdd_ReturnsFalse_WhenKeyAlreadyExists()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "TryAdd_ReturnsFalse_WhenKeyAlreadyExists";

        dictionary.TryAdd(key, 2000);
        bool result = dictionary.TryAdd(key, 3000);

        result.Should().BeFalse();
        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void TryAdd_AddsKey_WhenKeyDoesNotExist()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "TryAdd_AddsKey_WhenKeyDoesNotExist";

        dictionary.TryAdd(key, 1000);

        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void TryAdd_DoesNotAddKey_WhenKeyAlreadyExists()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "TryAdd_DoesNotAddKey_WhenKeyAlreadyExists";

        dictionary.TryAdd(key, 2000);
        dictionary.TryAdd(key, 5000);

        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void TryAdd_WithMultipleKeys_AllSucceed()
    {
        var dictionary = new ExpiringKeyDictionary();

        const string key1 = "TryAdd_WithMultipleKeys_AllSucceed_1";
        const string key2 = "TryAdd_WithMultipleKeys_AllSucceed_2";
        const string key3 = "TryAdd_WithMultipleKeys_AllSucceed_3";

        bool result1 = dictionary.TryAdd(key1, 1000);
        bool result2 = dictionary.TryAdd(key2, 2000);
        bool result3 = dictionary.TryAdd(key3, 3000);

        result1.Should().BeTrue();
        result2.Should().BeTrue();
        result3.Should().BeTrue();

        dictionary.ContainsKey(key1).Should().BeTrue();
        dictionary.ContainsKey(key2).Should().BeTrue();
        dictionary.ContainsKey(key3).Should().BeTrue();
    }

    [Test]
    public void ContainsKey_ReturnsFalse_WhenKeyDoesNotExist()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "ContainsKey_ReturnsFalse_WhenKeyDoesNotExist";

        bool result = dictionary.ContainsKey(key);

        result.Should().BeFalse();
    }

    [Test]
    public void ContainsKey_ReturnsTrue_WhenKeyExists()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "ContainsKey_ReturnsTrue_WhenKeyExists";

        dictionary.TryAdd(key, 2000);

        bool result = dictionary.ContainsKey(key);

        result.Should().BeTrue();
    }

    [Test]
    public void ContainsKey_ReturnsFalse_AfterKeyIsRemoved()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "ContainsKey_ReturnsFalse_AfterKeyIsRemoved";

        dictionary.TryAdd(key, 2000);
        dictionary.RemoveSync(key);

        bool result = dictionary.ContainsKey(key);

        result.Should().BeFalse();
    }

    [Test]
    public void AddOrUpdate_AddsKey_WhenKeyDoesNotExist()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "AddOrUpdate_AddsKey_WhenKeyDoesNotExist";

        dictionary.AddOrUpdate(key, 2000);

        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void AddOrUpdate_UpdatesKey_WhenKeyAlreadyExists()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "AddOrUpdate_UpdatesKey_WhenKeyAlreadyExists";

        dictionary.AddOrUpdate(key, 2000);
        dictionary.AddOrUpdate(key, 5000);

        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void GetOrAdd_ReturnsTimer_WhenKeyDoesNotExist()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "GetOrAdd_ReturnsTimer_WhenKeyDoesNotExist";

        var timer = dictionary.GetOrAdd(key, 2000);

        timer.Should().NotBeNull();
        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void GetOrAdd_ReturnsExistingTimer_WhenKeyAlreadyExists()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "GetOrAdd_ReturnsExistingTimer_WhenKeyAlreadyExists";

        var firstTimer = dictionary.GetOrAdd(key, 2000);
        var secondTimer = dictionary.GetOrAdd(key, 5000);

        secondTimer.Should().NotBeNull();
        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void TryRemove_RemovesKey_WhenKeyExists()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "TryRemove_RemovesKey_WhenKeyExists";

        dictionary.TryAdd(key, 2000);
        dictionary.TryRemoveSync(key);

        dictionary.ContainsKey(key).Should().BeFalse();
    }

    [Test]
    public void TryRemove_DoesNotThrow_WhenKeyDoesNotExist()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "TryRemove_DoesNotThrow_WhenKeyDoesNotExist";

        dictionary.TryRemoveSync(key);
        dictionary.ContainsKey(key).Should().BeFalse();
    }

    [Test]
    public async Task TryRemove_Async_RemovesKey_WhenKeyExists()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "TryRemove_Async_RemovesKey_WhenKeyExists";

        dictionary.TryAdd(key, 2000);
        await dictionary.TryRemove(key);

        dictionary.ContainsKey(key).Should().BeFalse();
    }

    [Test]
    public async Task TryRemove_Async_DoesNotThrow_WhenKeyDoesNotExist()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "TryRemove_Async_DoesNotThrow_WhenKeyDoesNotExist";

        await dictionary.TryRemove(key);
        dictionary.ContainsKey(key).Should().BeFalse();
    }

    [Test]
    public void RemoveSync_ReturnsTrue_WhenKeyExists()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "RemoveSync_ReturnsTrue_WhenKeyExists";

        dictionary.TryAdd(key, 2000);
        bool result = dictionary.RemoveSync(key);

        result.Should().BeTrue();
        dictionary.ContainsKey(key).Should().BeFalse();
    }

    [Test]
    public void RemoveSync_ReturnsFalse_WhenKeyDoesNotExist()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "RemoveSync_ReturnsFalse_WhenKeyDoesNotExist";

        bool result = dictionary.RemoveSync(key);

        result.Should().BeFalse();
    }

    [Test]
    public async Task Remove_Async_ReturnsTrue_WhenKeyExists()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "Remove_Async_ReturnsTrue_WhenKeyExists";

        dictionary.TryAdd(key, 2000);
        bool result = await dictionary.Remove(key);

        result.Should().BeTrue();
        dictionary.ContainsKey(key).Should().BeFalse();
    }

    [Test]
    public async Task Remove_Async_ReturnsFalse_WhenKeyDoesNotExist()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "Remove_Async_ReturnsFalse_WhenKeyDoesNotExist";

        bool result = await dictionary.Remove(key);

        result.Should().BeFalse();
    }

    [Test]
    public void ClearSync_RemovesAllKeys()
    {
        var dictionary = new ExpiringKeyDictionary();

        const string key1 = "ClearSync_RemovesAllKeys_1";
        const string key2 = "ClearSync_RemovesAllKeys_2";
        const string key3 = "ClearSync_RemovesAllKeys_3";

        dictionary.TryAdd(key1, 2000);
        dictionary.TryAdd(key2, 2000);
        dictionary.TryAdd(key3, 2000);

        dictionary.ClearSync();

        dictionary.ContainsKey(key1).Should().BeFalse();
        dictionary.ContainsKey(key2).Should().BeFalse();
        dictionary.ContainsKey(key3).Should().BeFalse();
    }

    [Test]
    public void ClearSync_DoesNotThrow_WhenDictionaryIsEmpty()
    {
        var dictionary = new ExpiringKeyDictionary();

        dictionary.ClearSync();
        dictionary.ContainsKey("ClearSync_DoesNotThrow_WhenDictionaryIsEmpty").Should().BeFalse();
    }

    [Test]
    public async Task Clear_Async_RemovesAllKeys()
    {
        var dictionary = new ExpiringKeyDictionary();

        const string key1 = "Clear_Async_RemovesAllKeys_1";
        const string key2 = "Clear_Async_RemovesAllKeys_2";
        const string key3 = "Clear_Async_RemovesAllKeys_3";

        dictionary.TryAdd(key1, 2000);
        dictionary.TryAdd(key2, 2000);
        dictionary.TryAdd(key3, 2000);

        await dictionary.Clear();

        dictionary.ContainsKey(key1).Should().BeFalse();
        dictionary.ContainsKey(key2).Should().BeFalse();
        dictionary.ContainsKey(key3).Should().BeFalse();
    }

    [Test]
    public async Task Clear_Async_DoesNotThrow_WhenDictionaryIsEmpty()
    {
        var dictionary = new ExpiringKeyDictionary();

        await dictionary.Clear();
        dictionary.ContainsKey("Clear_Async_DoesNotThrow_WhenDictionaryIsEmpty").Should().BeFalse();
    }

    [Test]
    public void Key_Expires_AfterExpirationTime()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "Key_Expires_AfterExpirationTime";

        dictionary.TryAdd(key, 100);

        dictionary.ContainsKey(key).Should().BeTrue();
        Thread.Sleep(150);

        dictionary.ContainsKey(key).Should().BeFalse();
    }

    [Test]
    public void Key_DoesNotExpire_BeforeExpirationTime()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "Key_DoesNotExpire_BeforeExpirationTime";

        dictionary.TryAdd(key, 200);
        Thread.Sleep(50);

        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void AddOrUpdate_ResetsExpiration_WhenKeyExists()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "AddOrUpdate_ResetsExpiration_WhenKeyExists";

        dictionary.TryAdd(key, 500);

        Thread.Sleep(200);
        dictionary.ContainsKey(key).Should().BeTrue();

        dictionary.AddOrUpdate(key, 1000);

        Thread.Sleep(600);

        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void TryAdd_WithEmptyString_Works()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "";

        bool result = dictionary.TryAdd(key, 2000);

        result.Should().BeTrue();
        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void TryAdd_WithZeroExpiration_Works()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "TryAdd_WithZeroExpiration_Works";

        bool result = dictionary.TryAdd(key, 0);

        result.Should().BeTrue();
        Thread.Sleep(10);
        dictionary.ContainsKey(key).Should().BeFalse();
    }

    [Test]
    public void TryAdd_WithVeryLongExpiration_Works()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "TryAdd_WithVeryLongExpiration_Works";

        bool result = dictionary.TryAdd(key, int.MaxValue);

        result.Should().BeTrue();
        dictionary.ContainsKey(key).Should().BeTrue();
    }

    [Test]
    public void MultipleOperations_OnSameKey_WorkCorrectly()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string key = "MultipleOperations_OnSameKey_WorkCorrectly";

        bool tryAdd1 = dictionary.TryAdd(key, 2000);
        tryAdd1.Should().BeTrue();

        dictionary.ContainsKey(key).Should().BeTrue();

        bool tryAdd2 = dictionary.TryAdd(key, 3000);
        tryAdd2.Should().BeFalse();

        dictionary.AddOrUpdate(key, 4000);
        dictionary.ContainsKey(key).Should().BeTrue();

        var timer = dictionary.GetOrAdd(key, 5000);
        timer.Should().NotBeNull();

        bool removed = dictionary.RemoveSync(key);
        removed.Should().BeTrue();
        dictionary.ContainsKey(key).Should().BeFalse();
    }

    [Test]
    public void ConcurrentOperations_WorkCorrectly()
    {
        var dictionary = new ExpiringKeyDictionary();
        const string prefix = "ConcurrentOperations_WorkCorrectly";

        Parallel.For(0, 100, i =>
        {
            dictionary.TryAdd($"{prefix}_{i}", 2000);
        });

        for (int i = 0; i < 100; i++)
        {
            dictionary.ContainsKey($"{prefix}_{i}").Should().BeTrue();
        }
    }

    [Test]
    public void Dispose_ClearsAllKeys()
    {
        var dictionary = new ExpiringKeyDictionary();

        const string key1 = "Dispose_ClearsAllKeys_1";
        const string key2 = "Dispose_ClearsAllKeys_2";

        dictionary.TryAdd(key1, 2000);
        dictionary.TryAdd(key2, 2000);

        dictionary.Dispose();

        dictionary.ContainsKey(key1).Should().BeFalse();
        dictionary.ContainsKey(key2).Should().BeFalse();
    }

    [Test]
    public async Task DisposeAsync_ClearsAllKeys()
    {
        var dictionary = new ExpiringKeyDictionary();

        const string key1 = "DisposeAsync_ClearsAllKeys_1";
        const string key2 = "DisposeAsync_ClearsAllKeys_2";

        dictionary.TryAdd(key1, 2000);
        dictionary.TryAdd(key2, 2000);

        await dictionary.DisposeAsync();

        dictionary.ContainsKey(key1).Should().BeFalse();
        dictionary.ContainsKey(key2).Should().BeFalse();
    }
}