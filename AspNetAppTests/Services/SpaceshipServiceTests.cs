using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using Argonauts.Application.Services.Abstractions;
using Argonauts.Application.Services.Implementations;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Entity.Player;
using Argonauts.Core.Repository;
using Argonauts.Core.Utility.Math;

namespace AspNetAppTests.Services;

#region CreateSpaceshipAsync Tests

public class SpaceshipServiceTests
{
    protected readonly Fixture _fixture;
    protected readonly Mock<ISpaceshipRepository> _spaceshipRepositoryMock;
    protected readonly Mock<IPlayerService> _playerServiceMock;
    protected readonly Mock<IGalaxyService> _galaxyServiceMock;
    protected readonly SpaceshipService _service;

    public SpaceshipServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _spaceshipRepositoryMock = new Mock<ISpaceshipRepository>();
        _playerServiceMock = new Mock<IPlayerService>();
        _galaxyServiceMock = new Mock<IGalaxyService>();

        _service = new SpaceshipService(
            _spaceshipRepositoryMock.Object,
            _playerServiceMock.Object,
            _galaxyServiceMock.Object,
            Mock.Of<ILogger<SpaceshipService>>());
    }

    [Test]
    public async Task CreateSpaceshipAsync_ValidInput_CreatesAndReturnsSpaceship()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var galaxyVersion = 5;
        var expectedSpaceship = CreateTestSpaceship(ownerId, galaxyVersion);

        // Игрок существует
        _playerServiceMock
            .Setup(ps => ps.PlayerExistsAsync(ownerId))
            .ReturnsAsync(true);

        // Галактика version
        _galaxyServiceMock
            .Setup(gs => gs.GetCurrentGalaxyVersionAsync())
            .ReturnsAsync(galaxyVersion);

        // Активного корабля нет
        _spaceshipRepositoryMock
            .Setup(repo => repo.GetAsync(ownerId))
            .ThrowsAsync(new KeyNotFoundException());

        // Создание корабля
        _spaceshipRepositoryMock
            .Setup(repo => repo.CreateAsync(ownerId, It.IsAny<Spaceship>()))
            .ReturnsAsync(expectedSpaceship);

        // Act
        var result = await _service.CreateSpaceshipAsync(ownerId);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Success).IsTrue();

        _playerServiceMock.Verify(ps => ps.PlayerExistsAsync(ownerId), Times.Once);
        _spaceshipRepositoryMock.Verify(repo => repo.CreateAsync(ownerId, It.IsAny<Spaceship>()), Times.Once);
    }

    [Test]
    public async Task CreateSpaceshipAsync_PlayerDoesNotExist_ReturnsInvalidResult()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        _playerServiceMock
            .Setup(ps => ps.PlayerExistsAsync(ownerId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.CreateSpaceshipAsync(ownerId);

        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.ErrorDescription).Contains($"Player {ownerId} does not exist");

        // Репозиторий не должен вызываться
        _spaceshipRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Guid>(), It.IsAny<Spaceship>()), Times.Never);
    }

    [Test]
    public async Task CreateSpaceshipAsync_AlreadyHasActiveSpaceship_ReturnsInvalidResult()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var galaxyVersion = 5;
        var existingShip = CreateTestSpaceship(ownerId, galaxyVersion);

        _playerServiceMock
            .Setup(ps => ps.PlayerExistsAsync(ownerId))
            .ReturnsAsync(true);

        _galaxyServiceMock
            .Setup(gs => gs.GetCurrentGalaxyVersionAsync())
            .ReturnsAsync(galaxyVersion);

        _spaceshipRepositoryMock
            .Setup(repo => repo.GetAsync(ownerId))
            .ReturnsAsync(existingShip);

        // Act
        var result = await _service.CreateSpaceshipAsync(ownerId);

        // Assert
        await Assert.That(result.Success).IsFalse();
        await Assert.That(result.ErrorDescription).Contains($"Player {ownerId} already has one");

        _spaceshipRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<Guid>(), It.IsAny<Spaceship>()), Times.Never);
    }

    [Test]
    public async Task CreateSpaceshipAsync_HasInactiveSpaceship_AllowsCreation()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var currentGalaxyVersion = 5;
        var oldVersion = 3;
        var oldShip = CreateTestSpaceship(ownerId, oldVersion);

        _playerServiceMock
            .Setup(ps => ps.PlayerExistsAsync(ownerId))
            .ReturnsAsync(true);

        _galaxyServiceMock
            .Setup(gs => gs.GetCurrentGalaxyVersionAsync())
            .ReturnsAsync(currentGalaxyVersion);

        // Есть корабль, но он устаревший
        _spaceshipRepositoryMock
            .Setup(repo => repo.GetAsync(ownerId))
            .ReturnsAsync(oldShip);

        var newShip = CreateTestSpaceship(ownerId, currentGalaxyVersion);
        _spaceshipRepositoryMock
            .Setup(repo => repo.CreateAsync(ownerId, It.IsAny<Spaceship>()))
            .ReturnsAsync(newShip);

        // Act
        var result = await _service.CreateSpaceshipAsync(ownerId);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Success).IsTrue();
    }

    #endregion

    #region GetSpaceshipAsync Tests

    [Test]
    public async Task GetSpaceshipAsync_ExistingOwner_ReturnsSpaceship()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var expectedShip = CreateTestSpaceship(ownerId);

        _spaceshipRepositoryMock
            .Setup(repo => repo.GetAsync(ownerId))
            .ReturnsAsync(expectedShip);

        // Act
        var result = await _service.GetSpaceshipAsync(ownerId);

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.OwnerId).IsEqualTo(ownerId);
    }

    [Test]
    public async Task GetSpaceshipAsync_NonExistingOwner_PropagatesKeyNotFoundException()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        _spaceshipRepositoryMock
            .Setup(repo => repo.GetAsync(ownerId))
            .ThrowsAsync(new KeyNotFoundException());

        // Act & Assert
        await Assert.That(async () => await _service.GetSpaceshipAsync(ownerId))
            .Throws<KeyNotFoundException>();
    }

    #endregion

    #region GetAllSpaceshipsAsync Tests

    [Test]
    public async Task GetAllSpaceshipsAsync_ReturnsAllSpaceships()
    {
        // Arrange
        var ships = _fixture.Build<Spaceship>()
            .CreateMany(5)
            .ToList();

        _spaceshipRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(ships);

        // Act
        var result = await _service.GetAllSpaceshipsAsync();

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result).Count().IsEqualTo(5);
    }

    [Test]
    public async Task GetAllSpaceshipsAsync_EmptyRepository_ReturnsEmptyList()
    {
        // Arrange
        _spaceshipRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        var result = await _service.GetAllSpaceshipsAsync();

        // Assert
        await Assert.That(result).IsEmpty();
    }

    #endregion

    #region CleanupInactiveSpaceshipsAsync Tests

    [Test]
    public async Task CleanupInactiveSpaceshipsAsync_RemovesCorrectCount()
    {
        // Arrange
        var currentVersion = 10;
        var allShips = _fixture.Build<Spaceship>().CreateMany(10).ToList();

        _galaxyServiceMock
            .Setup(gs => gs.GetCurrentGalaxyVersionAsync())
            .ReturnsAsync(currentVersion);

        _spaceshipRepositoryMock
            .SetupSequence(repo => repo.GetAllAsync())
            .ReturnsAsync(allShips)       // До очистки (10 кораблей)
            .ReturnsAsync(allShips.Take(7).ToList()); // После очистки (7 кораблей)

        _spaceshipRepositoryMock
            .Setup(repo => repo.DeleteInactiveAsync(currentVersion))
            .Returns(Task.CompletedTask);

        // Act
        var removedCount = await _service.CleanupInactiveSpaceshipsAsync();

        // Assert
        await Assert.That(removedCount).IsEqualTo(3); // 10 - 7 = 3
        _spaceshipRepositoryMock.Verify(repo => repo.DeleteInactiveAsync(currentVersion), Times.Once);
    }

    [Test]
    public async Task CleanupInactiveSpaceshipsAsync_NoInactiveShips_ReturnsZero()
    {
        // Arrange
        var currentVersion = 10;
        var ships = _fixture.Build<Spaceship>().CreateMany(5).ToList();

        _galaxyServiceMock
            .Setup(gs => gs.GetCurrentGalaxyVersionAsync())
            .ReturnsAsync(currentVersion);

        _spaceshipRepositoryMock
            .SetupSequence(repo => repo.GetAllAsync())
            .ReturnsAsync(ships)
            .ReturnsAsync(ships); // Количество не изменилось

        _spaceshipRepositoryMock
            .Setup(repo => repo.DeleteInactiveAsync(currentVersion))
            .Returns(Task.CompletedTask);

        // Act
        var removedCount = await _service.CleanupInactiveSpaceshipsAsync();

        // Assert
        await Assert.That(removedCount).IsEqualTo(0);
        _spaceshipRepositoryMock.Verify(repo => repo.DeleteInactiveAsync(currentVersion), Times.Once);
    }

    #endregion

    #region HasActiveSpaceshipAsync Tests

    [Test]
    [Arguments(5, 5)] // Версия == текущей (Active)
    [Arguments(10, 5)] // Версия > текущей (Active)
    public async Task HasActiveSpaceshipAsync_ActiveShip_ReturnsTrue(int shipVersion, int currentVersion)
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var ship = CreateTestSpaceship(ownerId, shipVersion);

        _spaceshipRepositoryMock
            .Setup(repo => repo.GetAsync(ownerId))
            .ReturnsAsync(ship);

        // Act
        var result = await _service.HasActiveSpaceshipAsync(ownerId, currentVersion);

        // Assert
        await Assert.That(result).IsTrue();
    }

    [Test]
    public async Task HasActiveSpaceshipAsync_InactiveShip_ReturnsFalse()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var ship = CreateTestSpaceship(ownerId, version: 3);
        int currentVersion = 5;

        _spaceshipRepositoryMock
            .Setup(repo => repo.GetAsync(ownerId))
            .ReturnsAsync(ship);

        // Act
        var result = await _service.HasActiveSpaceshipAsync(ownerId, currentVersion);

        // Assert
        await Assert.That(result).IsFalse();
    }

    [Test]
    public async Task HasActiveSpaceshipAsync_NoShip_ReturnsFalse()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        _spaceshipRepositoryMock
            .Setup(repo => repo.GetAsync(ownerId))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await _service.HasActiveSpaceshipAsync(ownerId, 5);

        // Assert
        await Assert.That(result).IsFalse();
    }
    #endregion

    private Spaceship CreateTestSpaceship(Guid? ownerId = null, int? version = null)
    {
        return _fixture.Build<Spaceship>()
            .With(s => s.OwnerId, ownerId ?? Guid.NewGuid())
            .With(s => s.GalaxyVersion, version ?? _fixture.Create<int>())
            .Create();
    }
}

