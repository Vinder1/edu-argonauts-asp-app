using Argonauts.Core.Entity.Exploration;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Infrastructure.Database.Mapping;
using AutoFixture;
using DbGalaxy = Argonauts.Infrastructure.Database.Entity.Galaxy;
using DbPlayer = Argonauts.Infrastructure.Database.Entity.Player;
using DbSpaceship = Argonauts.Infrastructure.Database.Entity.Spaceship;
using DbStar = Argonauts.Infrastructure.Database.Entity.Star;
using DomainPlayer = Argonauts.Core.Entity.Player.Player;
using DomainSpaceship = Argonauts.Core.Entity.Player.Spaceship;

public class AppToDbMapperTests
{
    protected readonly Fixture _fixture;
    private readonly AppToDbMapper _mapper;

    public AppToDbMapperTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        _mapper = new AppToDbMapper();
    }

    private Star CreateDomainStar()
    {
        return _fixture.Build<Star>()
            .Create();
    }

    [Test]
    public async Task Map_Star_DomainToDb_WithGalaxyVersion_ShouldMapAllProperties()
    {
        var domainStar = CreateDomainStar();
        var galaxyVersion = 42;

        var dbStar = _mapper.ToDbStar(domainStar, galaxyVersion);

        await Assert.That(dbStar.Radius).IsEqualTo(domainStar.Radius);
        await Assert.That(dbStar.AngleMilliradians).IsEqualTo(domainStar.AngleMilliradians);
        await Assert.That(dbStar.GalaxyVersion).IsEqualTo(galaxyVersion);
    }

    [Test]
    public async Task Map_Star_DomainToDb_ZeroGalaxyVersion_ShouldStillMap()
    {
        var domainStar = CreateDomainStar();
        var galaxyVersion = 0;

        var dbStar = _mapper.ToDbStar(domainStar, galaxyVersion);

        await Assert.That(dbStar.GalaxyVersion).IsEqualTo(0);
        await Assert.That(dbStar.Radius).IsEqualTo(domainStar.Radius);
        await Assert.That(dbStar.AngleMilliradians).IsEqualTo(domainStar.AngleMilliradians);
    }

    [Test]
    public async Task Map_Star_DbToDomain_ShouldMapAllPropertiesExceptGalaxyVersion()
    {
        var dbStar = _fixture.Build<Argonauts.Infrastructure.Database.Entity.Star>()
            .Create();

        var domainStar = _mapper.ToDomainStar(dbStar);

        await Assert.That(domainStar.Radius).IsEqualTo(dbStar.Radius);
        await Assert.That(domainStar.AngleMilliradians).IsEqualTo(dbStar.AngleMilliradians);
    }

    [Test]
    public async Task Map_Spaceship_DomainToDb_ShouldMapAllProperties()
    {
        var ownerId = Guid.NewGuid();
        var domainShip = new DomainSpaceship
        {
            OwnerId = ownerId,
            GalaxyVersion = 5,
            LocatedRadius = 300,
            LocatedAngleMilliradians = 120,
        };

        var dbShip = _mapper.ToDbSpaceship(domainShip);

        await Assert.That(dbShip.OwnerId).IsEqualTo(ownerId);
        await Assert.That(dbShip.GalaxyVersion).IsEqualTo(domainShip.GalaxyVersion);
        await Assert.That(dbShip.LocatedRadius).IsEqualTo(domainShip.LocatedRadius);
        await Assert.That(dbShip.LocatedAngleMilliradians).IsEqualTo(domainShip.LocatedAngleMilliradians);
    }

    [Test]
    public async Task Map_Spaceship_DbToDomain_ShouldMapAllProperties()
    {
        var ownerId = Guid.NewGuid();
        var dbShip = new DbSpaceship
        {
            OwnerId = ownerId,
            Owner = null,
            GalaxyVersion = 3,
            LocatedRadius = 250,
            LocatedAngleMilliradians = 60,
        };

        var domainShip = _mapper.ToDomainSpaceship(dbShip);

        await Assert.That(domainShip.OwnerId).IsEqualTo(ownerId);
        await Assert.That(domainShip.GalaxyVersion).IsEqualTo(dbShip.GalaxyVersion);
        await Assert.That(domainShip.LocatedRadius).IsEqualTo(dbShip.LocatedRadius);
        await Assert.That(domainShip.LocatedAngleMilliradians).IsEqualTo(dbShip.LocatedAngleMilliradians);
    }

    [Test]
    public async Task Map_Spaceship_RoundTrip_ShouldPreserveData()
    {
        var original = new DomainSpaceship
        {
            OwnerId = Guid.NewGuid(),
            GalaxyVersion = 7,
            LocatedRadius = 400,
            LocatedAngleMilliradians = 270
        };

        var db = _mapper.ToDbSpaceship(original);
        var result = _mapper.ToDomainSpaceship(db);

        await Assert.That(result.OwnerId).IsEqualTo(original.OwnerId);
        await Assert.That(result.GalaxyVersion).IsEqualTo(original.GalaxyVersion);
        await Assert.That(result.LocatedRadius).IsEqualTo(original.LocatedRadius);
        await Assert.That(result.LocatedAngleMilliradians).IsEqualTo(original.LocatedAngleMilliradians);
    }

    [Test]
    public async Task Map_Player_DomainToDb_ShouldMapAllProperties()
    {
        var playerId = Guid.NewGuid();
        var domainPlayer = new DomainPlayer
        {
            Id = playerId,
            Name = "Commander Shepard",
            RegisteredAt = DateTime.Now
        };

        var dbPlayer = _mapper.ToDbPlayer(domainPlayer);

        await Assert.That(dbPlayer.Id).IsEqualTo(playerId);
        await Assert.That(dbPlayer.Name).IsEqualTo(domainPlayer.Name);
    }

    [Test]
    public async Task Map_Player_DbToDomain_ShouldMapAllProperties()
    {
        var playerId = Guid.NewGuid();
        var dbPlayer = new DbPlayer
        {
            Id = playerId,
            Name = "John Doe",
            RegisteredAt = DateTime.Now
        };

        var domainPlayer = _mapper.ToDomainPlayer(dbPlayer);

        await Assert.That(domainPlayer.Id).IsEqualTo(playerId);
        await Assert.That(domainPlayer.Name).IsEqualTo(dbPlayer.Name);
    }

    [Test]
    public async Task Map_Player_RoundTrip_ShouldPreserveData()
    {
        var original = new DomainPlayer
        {
            Id = Guid.NewGuid(),
            Name = "Test Player",
            RegisteredAt = DateTime.UtcNow
        };

        var db = _mapper.ToDbPlayer(original);
        var result = _mapper.ToDomainPlayer(db);

        await Assert.That(result.Id).IsEqualTo(original.Id);
        await Assert.That(result.Name).IsEqualTo(original.Name);
    }

    [Test]
    public async Task Map_DomainToDb_WithGalaxyVersion_ShouldMapAllProperties()
    {
        var spaceshipId = Guid.NewGuid();
        var galaxyVersion = 42;
        var domainVisit = new SpaceshipStarVisit
        {
            Spaceship = new DomainSpaceship { OwnerId = spaceshipId },
            Star = CreateDomainStar(),
            VisitedAt = DateTime.Now,
            GalaxyVersion = galaxyVersion
        };

        var dbVisit = _mapper.ToDbSpaceshipVisit(domainVisit);

        await Assert.That(dbVisit.SpaceshipId).IsEqualTo(spaceshipId);
        await Assert.That(dbVisit.StarGalaxyVersion).IsEqualTo(galaxyVersion);
    }

    [Test]
    public async Task Galaxy_ToDbGalaxy_ShouldMapAllStars()
    {
        var star1 = CreateDomainStar() with { Radius = 1 };
        var star2 = CreateDomainStar() with { Radius = 3 };
        var star3 = CreateDomainStar() with { Radius = 2 };

        var stars = new Star[][] { [star1], [star2], [star3], };
        var starCollection = new StarCollection(stars);

        var domainGalaxy = new Galaxy
        {
            Version = 999,
            Stars = starCollection
        };

        var dbGalaxy = _mapper.ToDbGalaxy(domainGalaxy);

        await Assert.That(dbGalaxy).IsNotNull();
        await Assert.That(dbGalaxy.Version).IsEqualTo(999);
        await Assert.That(dbGalaxy.Stars).Count().IsEqualTo(3);

        await Assert.That(dbGalaxy.Stars).Contains(s => s.Radius == star1.Radius && s.GalaxyVersion == 999);

        foreach (var star in dbGalaxy.Stars)
        {
            await Assert.That(star.GalaxyVersion).IsEqualTo(999);
        }
    }

    [Test]
    public async Task Galaxy_ToDomainGalaxy_ShouldMapAllStars()
    {
        var dbGalaxy = new DbGalaxy
        {
            Version = 123,
            Stars =
            [
                new DbStar { GalaxyVersion = 123, Radius = 10, AngleMilliradians = 45 },
                new DbStar { GalaxyVersion = 123, Radius = 20, AngleMilliradians = 90 },
                new DbStar { GalaxyVersion = 123, Radius = 15, AngleMilliradians = 135 }
            ]
        };

        var domainGalaxy = _mapper.ToDomainGalaxy(dbGalaxy);

        await Assert.That(domainGalaxy).IsNotNull();
        await Assert.That(domainGalaxy.Version).IsEqualTo(dbGalaxy.Version);

        var allStars = domainGalaxy.Stars.GetAllStars().ToList();
        await Assert.That(allStars).Contains(s => s.Radius == 10 && s.AngleMilliradians == 45);
        await Assert.That(allStars).Contains(s => s.Radius == 20 && s.AngleMilliradians == 90);
        await Assert.That(allStars).Contains(s => s.Radius == 15 && s.AngleMilliradians == 135);
    }

    [Test]
    public async Task Galaxy_HandleEmptyStarCollection()
    {
        var dbGalaxy = new DbGalaxy
        {
            Version = 456,
            Stars = []
        };

        var domainGalaxy = _mapper.ToDomainGalaxy(dbGalaxy);

        await Assert.That(domainGalaxy).IsNotNull();
        await Assert.That(domainGalaxy.Version).IsEqualTo(456);
        await Assert.That(domainGalaxy.Stars.GetAllStars()).IsEmpty();
    }

    [Test]
    public async Task Galaxy_BidirectionalMapping_ShouldPreserveData()
    {
        var originalDbGalaxy = new DbGalaxy
        {
            Version = 333,
            Stars =
            [
                new DbStar { GalaxyVersion = 333, Radius = 2 },
                new DbStar { GalaxyVersion = 333, Radius = 1 },
            ]
        };

        var domainGalaxy = _mapper.ToDomainGalaxy(originalDbGalaxy);
        var mappedBackDbGalaxy = _mapper.ToDbGalaxy(domainGalaxy);

        await Assert.That(mappedBackDbGalaxy).IsNotNull();
        await Assert.That(mappedBackDbGalaxy.Version).IsEqualTo(originalDbGalaxy.Version);
        await Assert.That(mappedBackDbGalaxy.Stars).Count().IsEqualTo(originalDbGalaxy.Stars.Count);

        foreach (var originalStar in originalDbGalaxy.Stars)
        {
            await Assert.That(mappedBackDbGalaxy.Stars).Contains(s =>
                s.Radius == originalStar.Radius &&
                s.AngleMilliradians == originalStar.AngleMilliradians);
        }
    }
}
