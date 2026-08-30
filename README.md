[![](https://img.shields.io/nuget/v/soenneker.dictionaries.expiringkey.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.dictionaries.expiringkey/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.dictionaries.expiringkey/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.dictionaries.expiringkey/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.dictionaries.expiringkey.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.dictionaries.expiringkey/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.dictionaries.expiringkey/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.dictionaries.expiringkey/actions/workflows/codeql.yml)

# Soenneker.Dictionaries.ExpiringKey

A thread-safe in-memory set of string keys with an independent one-shot expiration timer per key.

## Installation

```bash
dotnet add package Soenneker.Dictionaries.ExpiringKey
```

## Usage

```csharp
using Soenneker.Dictionaries.ExpiringKey;

await using var recentEvents = new ExpiringKeyDictionary();

bool firstObservation = recentEvents.TryAdd(
    eventId,
    expirationTimeMilliseconds: 60_000);

if (!firstObservation)
{
    // The key is still inside its original one-minute lifetime.
}
```

`TryAdd` does not refresh an existing key. To add the key or restart its expiration from now, use `AddOrUpdate`:

```csharp
recentEvents.AddOrUpdate(eventId, expirationTimeMilliseconds: 60_000);
```

`GetOrAdd` returns the timer for compatibility. When the key already exists, its original timer and expiration remain unchanged. Do not call `Change` or `Dispose` on the returned timer; doing so can desynchronize the timer from dictionary membership.

## Expiration values

- `0` adds the key successfully and schedules it for immediate asynchronous removal.
- Positive values specify the one-shot delay in milliseconds.
- `Timeout.Infinite` (`-1`) keeps the key until explicit removal, clear, or disposal.
- Values below `-1` are rejected before the dictionary is mutated.

Timer callbacks are scheduled by the runtime, so removal can occur shortly after the nominal deadline rather than at an exact instant.

## Removal, clearing, and disposal

```csharp
bool removed = await recentEvents.Remove(eventId);
await recentEvents.TryRemove(anotherEventId); // no result when absence is unimportant

await recentEvents.Clear(); // removes all keys; the dictionary remains reusable
```

`Remove` reports whether the key existed. `TryRemove` is the no-result variant. Synchronous counterparts are available when asynchronous timer disposal is unnecessary.

`Clear`/`ClearSync` remove the current keys but allow subsequent use. `Dispose`/`DisposeAsync` are terminal, release all timers, and make later operations throw `ObjectDisposedException`.

This type creates one `Timer` per key. For very large cardinalities, prefer a bucketed or centrally scheduled expiration structure.
