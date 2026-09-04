using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Dictionaries.ExpiringKey.Abstract;

/// <summary>
/// A thread-safe set of string keys, each backed by an independent one-shot expiration timer.
/// </summary>
public interface IExpiringKeyDictionary: IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Checks if the provided key exists in the dictionary.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if the key exists; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    bool ContainsKey(string key);

    /// <summary>
    /// Adds a key with an expiration time, or updates the expiration time if the key already exists.
    /// </summary>
    /// <param name="key">The key to add or update.</param>
    /// <param name="expirationTimeMilliseconds">The expiration time in milliseconds for the key.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    void AddOrUpdate(string key, int expirationTimeMilliseconds);

    /// <summary>
    /// Tries to add a key with an expiration time.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="expirationTimeMilliseconds">The expiration time in milliseconds for the key.</param>
    /// <returns>True if the key was added; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    bool TryAdd(string key, int expirationTimeMilliseconds);

    /// <summary>
    /// Gets the existing key's timer without refreshing it, or adds a new key with an expiration time.
    /// </summary>
    /// <param name="key">The key to get or add.</param>
    /// <param name="expirationTimeMilliseconds">The expiration time in milliseconds for the key.</param>
    /// <returns>The timer associated with the key.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    Timer GetOrAdd(string key, int expirationTimeMilliseconds);

    /// <summary>
    /// Tries to remove the key asynchronously.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    ValueTask TryRemove(string key);

    /// <summary>
    /// Tries to remove the key synchronously.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    void TryRemoveSync(string key);

    /// <summary>
    /// Removes the key asynchronously.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>A ValueTask representing the asynchronous operation and a boolean indicating success.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    ValueTask<bool> Remove(string key);

    /// <summary>
    /// Removes the key synchronously.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>True if the key was removed; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is null.</exception>
    bool RemoveSync(string key);

    /// <summary>
    /// Clears and disposes all current key timers synchronously without disposing the dictionary.
    /// </summary>
    void ClearSync();

    /// <summary>
    /// Clears and disposes all current key timers asynchronously without disposing the dictionary.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    ValueTask Clear();
}
