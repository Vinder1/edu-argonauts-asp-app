using System.Text.Json;
using Valkey.Glide;
using static Valkey.Glide.ConnectionConfiguration;

namespace Argonauts.Infrastructure.Database.Repository.DataSources;

/// <summary>
/// 
/// </summary>
public class ValkeyCacheService(StandaloneClientConfiguration config) : ICacheService
{
    private readonly StandaloneClientConfiguration _config = config;
    private GlideClient? _client;

    private GlideClient GetClient() => _client ??= GlideClient.CreateClient(_config).GetAwaiter().GetResult();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<bool> ExistsAsync(string key)
    {
        var client = GetClient();
        return await client.ExistsAsync(key);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<string?> GetAsync(string key)
    {
        var client = GetClient();
        return await client.GetAsync(key);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<bool> RemoveAsync(string key)
    {
        var client = GetClient();
        return await client.DeleteAsync(key);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
    {
        var client = GetClient();
        await client.SetAsync(key, value);
        if (expiry.HasValue)
            await client.ExpireAsync(key, expiry.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<T?> GetJsonAsync<T>(string key) where T : class
    {
        var json = await GetAsync(key);
        return json == null ? null : JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    public async Task SetJsonAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
    {
        var json = JsonSerializer.Serialize(value);
        await SetAsync(key, json, expiry);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<bool> HSetAsync(string key, string field, string value)
    {
        var client = GetClient();
        return await client.HashSetAsync(key, field, value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    public async Task<string?> HGetAsync(string key, string field)
    {
        var client = GetClient();
        return await client.HashGetAsync(key, field);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<Dictionary<string, string>> HGetAllAsync(string key)
    {
        var client = GetClient();
        var result = await client.HashGetAsync(key);
        return new Dictionary<string, string>(result.Select(i => new KeyValuePair<string, string>((string)i.Key!, (string)i.Value!)));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    public async Task<bool> HDelAsync(string key, string field)
    {
        var client = GetClient();
        return await client.HashDeleteAsync(key, field);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<long> SAddAsync(string key, string value)
    {
        var client = GetClient();
        return await client.SetAddAsync(key, value) ? 1L : 0L;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<List<string>> SMembersAsync(string key)
    {
        var client = GetClient();
        var result = await client.SetMembersAsync(key);
        return result.Select(m => (string)m!).ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<long> SRemAsync(string key, string value)
    {
        var client = GetClient();
        return await client.SetRemoveAsync(key, value) ? 1L : 0L;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public Task Flush()
    {
        var client = GetClient();
        return client.FlushAllDatabasesAsync();
    }
}