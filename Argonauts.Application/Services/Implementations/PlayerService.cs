using Argonauts.Application.Services.Abstractions;
using Argonauts.Core.Entity.Player;
using Argonauts.Core.Repository;
using Microsoft.Extensions.Logging;

namespace Argonauts.Application.Services.Implementations;

/// <summary>
/// 
/// </summary>
/// <param name="playerRepository"></param>
/// <param name="logger"></param>
public class PlayerService(
    IPlayerRepository playerRepository,
    ILogger<PlayerService> logger) : IPlayerService
{
    private readonly IPlayerRepository _playerRepository = playerRepository
        ?? throw new ArgumentNullException(nameof(playerRepository));
    private readonly ILogger<PlayerService> _logger = logger
        ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public Task<Player?> GetPlayerAsync(Guid id)
    {
        return _playerRepository.GetAsync(id);
    }

    /// <inheritdoc/>
    public async Task<Player> UpdatePlayerNameAsync(Guid id, string newName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);
        var player = await _playerRepository.GetAsync(id);
        ArgumentNullException.ThrowIfNull(player);

        await _playerRepository.UpdateNameAsync(id, newName);
        var updatedPlayer = await _playerRepository.GetAsync(id);

        _logger.LogInformation("User (id={UserId}): Changed name to {NewName}", updatedPlayer!.Id, updatedPlayer.Name);

        return updatedPlayer;
    }

    /// <inheritdoc/>
    public Task DeletePlayerAsync(Guid id)
    {
        _logger.LogInformation("User (id={UserId}): Deleted his account", id);
        return _playerRepository.DeleteAsync(id);
    }

    /// <inheritdoc/>
    public async Task<bool> PlayerExistsAsync(Guid id)
    {
        var p = await _playerRepository.GetAsync(id);
        return p != null;
    }

    /// <inheritdoc/>
    public async Task<bool> PlayerLoginExistsAsync(string login)
    {
        var player = await _playerRepository.GetByLoginAsync(login);
        return player != null;
    }
}