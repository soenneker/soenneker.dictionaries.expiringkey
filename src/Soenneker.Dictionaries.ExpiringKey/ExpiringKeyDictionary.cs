using Soenneker.Dictionaries.ExpiringKey.Abstract;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using Soenneker.Extensions.ValueTask;
using System.Collections.Generic;
using System;

namespace Soenneker.Dictionaries.ExpiringKey;

/// <inheritdoc cref="IExpiringKeyDictionary"/>
public sealed class ExpiringKeyDictionary : IExpiringKeyDictionary
{
    private readonly ConcurrentDictionary<string, Timer> _keyDict = new();

    public bool ContainsKey(string key)
    {
        return _keyDict.ContainsKey(key);
    }

    public void AddOrUpdate(string key, int expirationTimeMilliseconds)
    {
        Timer replacement = CreateStoppedTimer(key);

        while (true)
        {
            if (_keyDict.TryGetValue(key, out Timer? current))
            {
                if (!_keyDict.TryUpdate(key, replacement, current))
                    continue;

                replacement.Change(expirationTimeMilliseconds, Timeout.Infinite);
                current.Dispose();
                return;
            }

            if (_keyDict.TryAdd(key, replacement))
            {
                replacement.Change(expirationTimeMilliseconds, Timeout.Infinite);
                return;
            }
        }
    }

    public bool TryAdd(string key, int expirationTimeMilliseconds)
    {
        if (expirationTimeMilliseconds == 0)
        {
            var immediateTimer = CreateStoppedTimer(key);

            if (!_keyDict.TryAdd(key, immediateTimer))
            {
                immediateTimer.Dispose();
                return false;
            }

            TryRemoveSync(key);
            return true;
        }

        Timer timer = CreateStoppedTimer(key);

        if (_keyDict.TryAdd(key, timer))
        {
            timer.Change(expirationTimeMilliseconds, Timeout.Infinite);
            return true;
        }

        timer.Dispose();
        return false;
    }

    public Timer GetOrAdd(string key, int expirationTimeMilliseconds)
    {
        if (_keyDict.TryGetValue(key, out Timer? existing))
            return existing;

        Timer candidate = CreateStoppedTimer(key);

        while (true)
        {
            if (_keyDict.TryAdd(key, candidate))
            {
                candidate.Change(expirationTimeMilliseconds, Timeout.Infinite);
                return candidate;
            }

            if (_keyDict.TryGetValue(key, out existing))
            {
                candidate.Dispose();
                return existing;
            }
        }
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

        await timer.DisposeAsync().NoSync();
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

    private Timer CreateStoppedTimer(string key)
    {
        var state = new ExpirationState(this, key);
        var timer = new Timer(static value => ((ExpirationState)value!).Expire(), state, Timeout.Infinite, Timeout.Infinite);
        state.Timer = timer;
        return timer;
    }

    private void Expire(string key, Timer timer)
    {
        if (_keyDict.TryRemove(new KeyValuePair<string, Timer>(key, timer)))
            timer.Dispose();
    }

    private sealed class ExpirationState
    {
        private readonly ExpiringKeyDictionary _owner;
        private readonly string _key;

        public Timer Timer { get; set; } = null!;

        public ExpirationState(ExpiringKeyDictionary owner, string key)
        {
            _owner = owner;
            _key = key;
        }

        public void Expire() => _owner.Expire(_key, Timer);
    }

    public void ClearSync()
    {
        Dispose();
    }

    public ValueTask Clear()
    {
        return DisposeAsync();
    }

    /// <summary>
    /// Releases resources used by the current instance.
    /// </summary>
    public void Dispose()
    {
        foreach (Timer timer in _keyDict.Values)
        {
            timer.Dispose();
        }

        _keyDict.Clear();
    }

    /// <summary>
    /// Asynchronously releases resources used by the current instance.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async ValueTask DisposeAsync()
    {
        foreach (Timer timer in _keyDict.Values)
        {
            await timer.DisposeAsync().NoSync();
        }

        _keyDict.Clear();
    }
}
