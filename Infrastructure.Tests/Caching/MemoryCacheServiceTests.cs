using Infrastructure.Caching;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Tests.Caching;

public class MemoryCacheServiceTests
{
    private readonly MemoryCacheService _cache;

    public MemoryCacheServiceTests()
    {
        _cache = new MemoryCacheService(new MemoryCache(new MemoryCacheOptions()));
    }

    [Fact]
    public async Task SetAndGet_ReturnsCachedValue()
    {
        await _cache.SetAsync("key", "value", TimeSpan.FromMinutes(1));

        var result = await _cache.GetAsync<string>("key");

        Assert.Equal("value", result);
    }

    [Fact]
    public async Task Get_NonExistentKey_ReturnsNull()
    {
        var result = await _cache.GetAsync<string>("missing");

        Assert.Null(result);
    }

    [Fact]
    public async Task Remove_DeletesCachedValue()
    {
        await _cache.SetAsync("key", "value", TimeSpan.FromMinutes(1));

        await _cache.RemoveAsync("key");

        var result = await _cache.GetAsync<string>("key");
        Assert.Null(result);
    }

    [Fact]
    public async Task Set_WithShortTtl_ExpiresEntry()
    {
        await _cache.SetAsync("key", "value", TimeSpan.FromMilliseconds(50));

        await Task.Delay(100);

        var result = await _cache.GetAsync<string>("key");
        Assert.Null(result);
    }
}
