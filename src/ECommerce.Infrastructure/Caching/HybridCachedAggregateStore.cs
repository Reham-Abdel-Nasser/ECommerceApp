using ECommerce.UseCases.Commons.Options;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Caching;

public sealed class HybridCachedAggregateStore<T>(HybridCache cache,
    IOptionsMonitor<CacheEntryPolicy> optionsMonitor) 
    : ICachedAggregateStore<T> where T : class
{
    private readonly CacheEntryPolicy _options = optionsMonitor.Get(typeof(T).Name);

    public async Task<T?> GetAsync(string key, CancellationToken ct = default)
    {
        var envelope = await cache.TryGetAsync<CacheEnvelope<T>>(key, ct);

        return envelope?.Payload;
    }

    public async Task<T> GetOrCreateAsync(string key, Func<CancellationToken, Task<T>> factory, CancellationToken ct = default)
    {
        // GET return it (update last access) , create

        var envelope = await cache.GetOrCreateAsync(
                key,
                async cancel =>
                {
                    var value = await factory(cancel);

                    var utcNow = DateTime.UtcNow;

                    return new CacheEnvelope<T> 
                    { 
                        Payload = value, 
                        CreatedAtUtc = utcNow, 
                        LastAccessedUtc =utcNow
                    };
                },
                CreateEntryOptionsForNewEnvelope(),
                cancellationToken: ct);

        await RefreshExpirationIfNeedAsync(key, envelope, ct);

        return envelope.Payload;
    }

    public async Task SetAsync(string key, T value, CancellationToken ct = default)
    {
        var existing = await cache.TryGetAsync<CacheEnvelope<T>>(key, ct);

        var envelope = new CacheEnvelope<T>
        {
            Payload = value,
            CreatedAtUtc = existing?.CreatedAtUtc ?? DateTime.UtcNow,
            LastAccessedUtc = DateTime.UtcNow
        };

        await SetOrRemoveIfExpiredAsync(key, envelope, ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
        => await cache.RemoveAsync(key, ct);

    private async Task RefreshExpirationIfNeedAsync(string key, CacheEnvelope<T> envelope, CancellationToken ct)
    {
        var utc = DateTime.UtcNow;

        var age = utc - envelope.LastAccessedUtc;

        if (age < TimeSpan.FromMinutes(_options.SlidingRefreshThresholdMinutes))
            return;

        var refreshed = new CacheEnvelope<T>
        {
            Payload = envelope.Payload,
            CreatedAtUtc = envelope.CreatedAtUtc,
            LastAccessedUtc = utc
        };

        await SetOrRemoveIfExpiredAsync(key, refreshed, ct);
    }

    private async Task SetOrRemoveIfExpiredAsync(string key, CacheEnvelope<T> refreshed, CancellationToken ct)
    {
        var expiration = CalculateExpiration(
                refreshed.CreatedAtUtc,
                refreshed.LastAccessedUtc,
                DateTime.UtcNow
            );

        if(expiration is null)
        {
            await cache.RemoveAsync(key, ct);
            return;
        }

        await cache.SetAsync(key, refreshed, CreateEntryOptions(expiration.Value), cancellationToken: ct);
    }

    private HybridCacheEntryOptions CreateEntryOptionsForNewEnvelope()
    {
        var utcNow = DateTime.UtcNow;

        var expiration = CalculateExpiration(utcNow, utcNow, utcNow) ?? throw new Exception();

        return CreateEntryOptions(expiration);
    }

    private HybridCacheEntryOptions CreateEntryOptions(TimeSpan expiration)
    {
        var localExpiration = TimeSpan.FromMinutes(_options.LocalCacheExpirationMinutes);

        if(localExpiration > expiration)
            localExpiration = expiration;

        return new HybridCacheEntryOptions {
            Expiration = expiration,
            LocalCacheExpiration = localExpiration
        };
    }

    private TimeSpan? CalculateExpiration(
        DateTime createdAtUtc,
        DateTime lastAccessedUtc,
        DateTime utcNow)
    {
        // sliding - absolute

        var absoluteRemaining = createdAtUtc
            .AddDays(_options.AbsoluteExpirationDays)
            .Subtract(utcNow);

        var slidingRemaining = lastAccessedUtc
            .AddDays(_options.SlidingExpirationDays)
            .Subtract(utcNow);

        if (absoluteRemaining <= TimeSpan.Zero || slidingRemaining <= TimeSpan.Zero)
            return null;

        return absoluteRemaining >= slidingRemaining ? absoluteRemaining : slidingRemaining;
    }

}
