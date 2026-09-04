using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.Dictionaries.ExpiringKey.Abstract;

namespace Soenneker.Dictionaries.ExpiringKey;

public sealed class ExpiringKeyDictionary : IExpiringKeyDictionary
{
    private readonly ConcurrentDictionary<string, Timer> _keyDict = new();
    private readonly object _lifecycleLock = new();
    private bool _disposed;

    public bool ContainsKey(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        lock (_lifecycleLock)
        {
            ThrowIfDisposed();
            return _keyDict.ContainsKey(key);
        }
    }

    public void AddOrUpdate(string key, int expirationTimeMilliseconds)
    {
        ArgumentNullException.ThrowIfNull(key);
        ValidateExpiration(expirationTimeMilliseconds);

        lock (_lifecycleLock)
        {
            ThrowIfDisposed();

            Timer replacement = CreateStoppedTimer(key);
            replacement.Change(expirationTimeMilliseconds, Timeout.Infinite);

            if (_keyDict.TryGetValue(key, out Timer? current))
            {
                _keyDict[key] = replacement;
                current.Dispose();
            }
            else
            {
                _keyDict.TryAdd(key, replacement);
            }
        }
    }

    public bool TryAdd(string key, int expirationTimeMilliseconds)
    {
        ArgumentNullException.ThrowIfNull(key);
        ValidateExpiration(expirationTimeMilliseconds);

        lock (_lifecycleLock)
        {
            ThrowIfDisposed();

            if (_keyDict.ContainsKey(key))
                return false;

            Timer timer = CreateStoppedTimer(key);
            timer.Change(expirationTimeMilliseconds, Timeout.Infinite);

            if (_keyDict.TryAdd(key, timer))
                return true;

            timer.Dispose();
            return false;
        }
    }

    public Timer GetOrAdd(string key, int expirationTimeMilliseconds)
    {
        ArgumentNullException.ThrowIfNull(key);
        ValidateExpiration(expirationTimeMilliseconds);

        lock (_lifecycleLock)
        {
            ThrowIfDisposed();

            if (_keyDict.TryGetValue(key, out Timer? existing))
                return existing;

            Timer timer = CreateStoppedTimer(key);
            timer.Change(expirationTimeMilliseconds, Timeout.Infinite);
            _keyDict.TryAdd(key, timer);
            return timer;
        }
    }

    public async ValueTask TryRemove(string key)
    {
        Timer? timer = Take(key);

        if (timer is not null)
            await timer.DisposeAsync().ConfigureAwait(false);
    }

    public void TryRemoveSync(string key) => Take(key)?.Dispose();

    public async ValueTask<bool> Remove(string key)
    {
        Timer? timer = Take(key);

        if (timer is null)
            return false;

        await timer.DisposeAsync().ConfigureAwait(false);
        return true;
    }

    public bool RemoveSync(string key)
    {
        Timer? timer = Take(key);

        if (timer is null)
            return false;

        timer.Dispose();
        return true;
    }

    private Timer? Take(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        lock (_lifecycleLock)
        {
            ThrowIfDisposed();
            _keyDict.TryRemove(key, out Timer? timer);
            return timer;
        }
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
        lock (_lifecycleLock)
        {
            _keyDict.TryRemove(new KeyValuePair<string, Timer>(key, timer));
        }

        timer.Dispose();
    }

    private sealed class ExpirationState
    {
        private readonly ExpiringKeyDictionary _owner;
        private readonly string _key;

        internal Timer Timer { get; set; } = null!;

        internal ExpirationState(ExpiringKeyDictionary owner, string key)
        {
            _owner = owner;
            _key = key;
        }

        internal void Expire() => _owner.Expire(_key, Timer);
    }

    public void ClearSync()
    {
        Timer[] timers = Drain(markDisposed: false);

        for (var i = 0; i < timers.Length; i++)
            timers[i].Dispose();
    }

    public async ValueTask Clear()
    {
        Timer[] timers = Drain(markDisposed: false);

        for (var i = 0; i < timers.Length; i++)
            await timers[i].DisposeAsync().ConfigureAwait(false);
    }

    public void Dispose()
    {
        Timer[] timers = Drain(markDisposed: true);

        for (var i = 0; i < timers.Length; i++)
            timers[i].Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        Timer[] timers = Drain(markDisposed: true);

        for (var i = 0; i < timers.Length; i++)
            await timers[i].DisposeAsync().ConfigureAwait(false);
    }

    private Timer[] Drain(bool markDisposed)
    {
        lock (_lifecycleLock)
        {
            if (_disposed)
            {
                if (!markDisposed)
                    ThrowIfDisposed();

                return Array.Empty<Timer>();
            }

            if (markDisposed)
                _disposed = true;

            Timer[] timers = _keyDict.Values.ToArray();
            _keyDict.Clear();
            return timers;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ExpiringKeyDictionary));
    }

    private static void ValidateExpiration(int expirationTimeMilliseconds)
    {
        if (expirationTimeMilliseconds < Timeout.Infinite)
            throw new ArgumentOutOfRangeException(nameof(expirationTimeMilliseconds));
    }
}
