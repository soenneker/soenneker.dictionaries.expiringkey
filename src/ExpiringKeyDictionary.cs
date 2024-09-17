using System;
using Soenneker.Dictionaries.ExpiringKey.Abstract;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using Soenneker.Extensions.ValueTask;
using System.Collections.Generic;

namespace Soenneker.Dictionaries.ExpiringKey;

/// <inheritdoc cref="IExpiringKeyDictionary"/>
public class ExpiringKeyDictionary : IExpiringKeyDictionary
{
    private readonly ConcurrentDictionary<string, Timer> _keyDict = new();

    public bool ContainsKey(string key)
    {
        return _keyDict.ContainsKey(key);
    }

    public void AddOrUpdate(string key, int expirationTimeMilliseconds)
    {
        _keyDict.AddOrUpdate(key, CreateTimer(key, expirationTimeMilliseconds), (_, _) => CreateTimer(key, expirationTimeMilliseconds));
    }

    public bool TryAdd(string key, int expirationTimeMilliseconds)
    {
        return _keyDict.TryAdd(key, CreateTimer(key, expirationTimeMilliseconds));
    }

    public Timer GetOrAdd(string key, int expirationTimeMilliseconds)
    {
        return _keyDict.GetOrAdd(key, CreateTimer(key, expirationTimeMilliseconds));
    }

    public ValueTask TryRemove(string key)
    {
        if (_keyDict.TryRemove(key, out Timer? timer))
        {
            return timer.DisposeAsync();
        }

        return ValueTask.CompletedTask;
    }

    public void TryRemoveSync(string key)
    {
        if (_keyDict.TryRemove(key, out Timer? timer))
        {
            timer.Dispose();
        }
    }

    public async ValueTask<bool> Remove(string key)
    {
        _keyDict.Remove(key, out Timer? timer);

        if (timer == null)
            return false;

        await timer.DisposeAsync();
        return true;
    }

    public bool RemoveSync(string key)
    {
        _keyDict.Remove(key, out Timer? timer);

        if (timer == null) 
            return false;
        
        timer.Dispose();
        return true;
    }

    private Timer CreateTimer(string key, int expirationTimeMilliseconds)
    {
        return new Timer(ExpireSync, key, expirationTimeMilliseconds, Timeout.Infinite);
    }

    private ValueTask Expire(object? state)
    {
        var key = (string) state!;
        return TryRemove(key);
    }

    private void ExpireSync(object? state)
    {
        var key = (string)state!;
        TryRemoveSync(key);
    }

    public void ClearSync()
    {
        Dispose();
    }

    public ValueTask Clear()
    {
        return DisposeAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        foreach (Timer timer in _keyDict.Values)
        {
            timer.Dispose();
        }

        _keyDict.Clear();
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);

        foreach (Timer timer in _keyDict.Values)
        {
            await timer.DisposeAsync().NoSync();
        }

        _keyDict.Clear();
    }
}