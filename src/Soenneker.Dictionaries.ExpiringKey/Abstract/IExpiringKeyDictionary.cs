using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Dictionaries.ExpiringKey.Abstract;

/// <summary>
/// A concurrent dictionary that helps you efficiently manage keys with expiration times
/// </summary>
public interface IExpiringKeyDictionary: IDisposable, IAsyncDisposable
{
    /// <summary>
    /// Checks if the provided key exists in the dictionary.
    /// </summary>
    /// <param name="key">The key to check.</param>
    /// <returns>True if the key exists; otherwise, false.</returns>
    bool ContainsKey(string key);

    /// <summary>
    /// Adds a key with an expiration time, or updates the expiration time if the key already exists.
    /// </summary>
    /// <param name="key">The key to add or update.</param>
    /// <param name="expirationTimeMilliseconds">The expiration time in milliseconds for the key.</param>
    void AddOrUpdate(string key, int expirationTimeMilliseconds);

    /// <summary>
    /// Tries to add a key with an expiration time.
    /// </summary>
    /// <param name="key">The key to add.</param>
    /// <param name="expirationTimeMilliseconds">The expiration time in milliseconds for the key.</param>
    /// <returns>True if the key was added; otherwise, false.</returns>
    bool TryAdd(string key, int expirationTimeMilliseconds);

    /// <summary>
    /// Gets the existing key or adds a new one with an expiration time if it does not exist.
    /// </summary>
    /// <param name="key">The key to get or add.</param>
    /// <param name="expirationTimeMilliseconds">The expiration time in milliseconds for the key.</param>
    /// <returns>The timer associated with the key.</returns>
    Timer GetOrAdd(string key, int expirationTimeMilliseconds);

    /// <summary>
    /// Tries to remove the key asynchronously.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    ValueTask TryRemove(string key);

    /// <summary>
    /// Tries to remove the key synchronously.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    void TryRemoveSync(string key);

    /// <summary>
    /// Removes the key asynchronously.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>A ValueTask representing the asynchronous operation and a boolean indicating success.</returns>
    ValueTask<bool> Remove(string key);

    /// <summary>
    /// Removes the key synchronously.
    /// </summary>
    /// <param name="key">The key to remove.</param>
    /// <returns>True if the key was removed; otherwise, false.</returns>
    bool RemoveSync(string key);

    /// <summary>
    /// Clears all keys synchronously.
    /// </summary>
    void ClearSync();

    /// <summary>
    /// Clears all keys asynchronously.
    /// </summary>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    ValueTask Clear();
}
