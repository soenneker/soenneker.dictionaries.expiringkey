[![](https://img.shields.io/nuget/v/soenneker.dictionaries.expiringkey.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.dictionaries.expiringkey/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.dictionaries.expiringkey/publish-package.yml?style=for-the-badge)](https://github.com/soenneker/soenneker.dictionaries.expiringkey/actions/workflows/publish-package.yml)
[![](https://img.shields.io/nuget/dt/soenneker.dictionaries.expiringkey.svg?style=for-the-badge)](https://www.nuget.org/packages/soenneker.dictionaries.expiringkey/)
[![](https://img.shields.io/github/actions/workflow/status/soenneker/soenneker.dictionaries.expiringkey/codeql.yml?label=CodeQL&style=for-the-badge)](https://github.com/soenneker/soenneker.dictionaries.expiringkey/actions/workflows/codeql.yml)

# Soenneker.Dictionaries.ExpiringKey

A concurrent dictionary that helps you efficiently manage keys with expiration times.

## Install

```bash
dotnet add package Soenneker.Dictionaries.ExpiringKey
```

## Quick start

```csharp
using Soenneker.Dictionaries.ExpiringKey.Abstract;

IExpiringKeyDictionary expiringKeyDictionary = /* resolve from DI */;
var result = expiringKeyDictionary.ContainsKey("value");
```

Checks if the provided key exists in the dictionary.

## What you get

- `IExpiringKeyDictionary` — A concurrent dictionary that helps you efficiently manage keys with expiration times.

## API at a glance

| API | What it does | Result / important behavior |
| --- | --- | --- |
| `IExpiringKeyDictionary.ContainsKey(key)` | Checks if the provided key exists in the dictionary. | True if the key exists; otherwise, false. |
| `IExpiringKeyDictionary.AddOrUpdate(key, expirationTimeMilliseconds)` | Adds a key with an expiration time, or updates the expiration time if the key already exists. | Returns no value; the requested change is complete when the method returns. |
| `IExpiringKeyDictionary.TryAdd(key, expirationTimeMilliseconds)` | Tries to add a key with an expiration time. | True if the key was added; otherwise, false. |
| `IExpiringKeyDictionary.GetOrAdd(key, expirationTimeMilliseconds)` | Gets the existing key or adds a new one with an expiration time if it does not exist. | The timer associated with the key. |
| `IExpiringKeyDictionary.TryRemove(key)` | Tries to remove the key asynchronously. | A ValueTask representing the asynchronous operation. |
| `IExpiringKeyDictionary.TryRemoveSync(key)` | Tries to remove the key synchronously. | Returns no value; the requested change is complete when the method returns. |
| `IExpiringKeyDictionary.Remove(key)` | Removes the key asynchronously. | A ValueTask representing the asynchronous operation and a boolean indicating success. |
| `IExpiringKeyDictionary.RemoveSync(key)` | Removes the key synchronously. | True if the key was removed; otherwise, false. |
| `IExpiringKeyDictionary.Clear()` | Clears all keys asynchronously. | A ValueTask representing the asynchronous operation. |

## Practical notes

- Dispose instances you own when their scope ends so held resources can be released.
