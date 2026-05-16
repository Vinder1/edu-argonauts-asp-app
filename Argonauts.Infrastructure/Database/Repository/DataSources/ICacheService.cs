namespace Argonauts.Infrastructure.Database.Repository.DataSources;

/// <summary>
/// 
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<string?> GetAsync(string key);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    Task SetAsync(string key, string value, TimeSpan? expiry = null);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<bool> RemoveAsync(string key);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(string key);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<T?> GetJsonAsync<T>(string key) where T : class;
    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    Task SetJsonAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<bool> HSetAsync(string key, string field, string value);
    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Task<string?> HGetAsync(string key, string field);
    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<Dictionary<string, string>> HGetAllAsync(string key);
    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="field"></param>
    /// <returns></returns>
    Task<bool> HDelAsync(string key, string field);
    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<long> SAddAsync(string key, string value);
    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    Task<List<string>> SMembersAsync(string key);
    /// <summary>
    ///
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    Task<long> SRemAsync(string key, string value);
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task Flush();
}