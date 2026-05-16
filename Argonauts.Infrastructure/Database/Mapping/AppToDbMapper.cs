using Argonauts.Core.Entity.Galaxy;
using Argonauts.Infrastructure.Database.Entity;
using Riok.Mapperly.Abstractions;
using Balance = Argonauts.Core.Entity.Player.Balance;
using Galaxy = Argonauts.Infrastructure.Database.Entity.Galaxy;
using SpaceshipCondition = Argonauts.Core.Entity.Player.SpaceshipCondition;
using Star = Argonauts.Infrastructure.Database.Entity.Star;
using Quest = Argonauts.Core.Entity.Quest.Quest;

namespace Argonauts.Infrastructure.Database.Mapping;

/// <summary>
/// 
/// </summary>
// [Mapper(UseReferenceHandling = true)]
[Mapper]
public partial class AppToDbMapper
{
    // ======================================[Star]==============================

    ///
    [MapperIgnoreTarget(nameof(Star.Galaxy))]
    [MapperIgnoreTarget(nameof(Star.GalaxyVersion))]
    [MapperIgnoreTarget(nameof(Star.VisitedByShips))]
    private partial Star ToDbStar(Core.Entity.Galaxy.Star entity);

    ///
    public Star ToDbStar(Core.Entity.Galaxy.Star entity, int galaxyVersion)
    {
        var star = ToDbStar(entity);
        star.GalaxyVersion = galaxyVersion;
        return star;
    }

    ///
    public ICollection<Star> ToDbStars(IEnumerable<Core.Entity.Galaxy.Star> entities, int galaxyVersion)
    {
        var list = new List<Star>(1500);
        foreach (var s in entities)
        {
            list.Add(ToDbStar(s, galaxyVersion));
        }
        return list;
    }

    ///
    [MapperIgnoreSource(nameof(Star.GalaxyVersion))]
    [MapperIgnoreSource(nameof(Star.Galaxy))]
    [MapperIgnoreSource(nameof(Star.VisitedByShips))]
    // [MapperIgnoreSource(nameof(Repository.DbEntity.Star.Galaxy))]
    public partial Core.Entity.Galaxy.Star ToDomainStar(Star entity);

    ///
    public partial IEnumerable<Core.Entity.Galaxy.Star> ToDomainStars(IEnumerable<Star> entity);


    //===============================[Spaceship]=================================================

    ///
    public Spaceship ToDbSpaceship(Core.Entity.Player.Spaceship src) => new()
    {
        GalaxyVersion = src.GalaxyVersion,
        OwnerId = src.OwnerId,
        LocatedAngleMilliradians = src.LocatedAngleMilliradians,
        LocatedRadius = src.LocatedRadius,
    };

    ///
    public Core.Entity.Player.Spaceship ToDomainSpaceship(Spaceship src) => new()
    {
        GalaxyVersion = src.GalaxyVersion,
        OwnerId = src.OwnerId,
        LocatedAngleMilliradians = src.LocatedAngleMilliradians,
        LocatedRadius = src.LocatedRadius,
    };

    ///
    public partial IEnumerable<Core.Entity.Player.Spaceship> ToDomainSpaceships(IEnumerable<Spaceship> src);

    // ================================[Player]================================================

    /// 
    public partial Player ToDbPlayer(Core.Entity.Player.Player src);
    /// 
    public partial Core.Entity.Player.Player ToDomainPlayer(Player src);


    // ==================================[Galaxy]=============================================

    ///
    [UserMapping]
    public Galaxy ToDbGalaxy(Core.Entity.Galaxy.Galaxy src) => new()
    {
        Version = src.Version,
        Stars = ToDbStars(src.Stars.GetAllStars(), src.Version)
    };

    ///
    public Core.Entity.Galaxy.Galaxy ToDomainGalaxy(Galaxy src) => new()
    {
        Version = src.Version,
        Stars = new StarCollection(GroupStarsByRadius(ToDomainStars(src.Stars)))
    };

    private static Core.Entity.Galaxy.Star[][] GroupStarsByRadius(IEnumerable<Core.Entity.Galaxy.Star> stars)
    {
        return [ .. stars
            .GroupBy(s => s.Radius)
            .OrderBy(g => g.Key)
            .Select(g => g.OrderBy(s => s.AngleMilliradians).ToArray())];
    }

    // ==============================[SpaceshipStarVisit]=======================================

    /// 
    [MapperIgnoreTarget(nameof(SpaceshipStarVisit.Star))]
    [MapperIgnoreTarget(nameof(SpaceshipStarVisit.Spaceship))]
    [MapProperty(nameof(Core.Entity.Exploration.SpaceshipStarVisit.Spaceship.OwnerId), nameof(SpaceshipStarVisit.SpaceshipId))]
    [MapProperty(nameof(Core.Entity.Exploration.SpaceshipStarVisit.GalaxyVersion), nameof(SpaceshipStarVisit.StarGalaxyVersion))]
    [MapProperty($"{nameof(Core.Entity.Exploration.SpaceshipStarVisit.Star)}.{nameof(Core.Entity.Galaxy.Star.Radius)}", nameof(SpaceshipStarVisit.StarRadius))]
    [MapProperty($"{nameof(Core.Entity.Exploration.SpaceshipStarVisit.Star)}.{nameof(Core.Entity.Galaxy.Star.AngleMilliradians)}", nameof(SpaceshipStarVisit.StarAngleMilliradians))]
    public partial SpaceshipStarVisit ToDbSpaceshipVisit(Core.Entity.Exploration.SpaceshipStarVisit src);

    ///
    public partial ICollection<SpaceshipStarVisit> ToDbSpaceshipVisits(ICollection<Core.Entity.Exploration.SpaceshipStarVisit> src);

    ///    
    [MapperIgnoreSource(nameof(SpaceshipStarVisit.SpaceshipId))]
    [MapProperty(nameof(SpaceshipStarVisit.StarGalaxyVersion), nameof(Core.Entity.Exploration.SpaceshipStarVisit.GalaxyVersion))]
    [MapperIgnoreSource(nameof(SpaceshipStarVisit.StarRadius))]
    [MapperIgnoreSource(nameof(SpaceshipStarVisit.StarAngleMilliradians))]
    public partial Core.Entity.Exploration.SpaceshipStarVisit ToDomainSpaceshipVisit(SpaceshipStarVisit src);

    ///
    public partial ICollection<Core.Entity.Exploration.SpaceshipStarVisit> ToDomainSpaceshipVisits(ICollection<SpaceshipStarVisit> src);


    //=======================================[Quest]============================================
    ///
    [MapperIgnoreTarget(nameof(Quest.IsCompleted))]
    [MapperIgnoreSource(nameof(Entity.Quest.Spaceship))]
    [MapperIgnoreSource(nameof(Entity.Quest.OwnerId))]
    public partial Quest ToDomainQuest(Entity.Quest src);
    ///
    [MapperIgnoreTarget(nameof(Entity.Quest.Spaceship))]
    [MapperIgnoreSource(nameof(Quest.IsCompleted))]
    public partial Entity.Quest ToDbQuest(Quest src, Guid ownerId);

    //=======================================[Resources]=======================================
    ///
    [MapperIgnoreSource(nameof(Entity.Balance.OwnerId))]
    [MapperIgnoreSource(nameof(Entity.Balance.Spaceship))]
    public partial Balance ToDomainBalance(Entity.Balance src);
    ///
    [MapperIgnoreTarget(nameof(Spaceship))]
    public partial Entity.Balance ToDbBalance(Balance src, Guid ownerId);
    
    ///
    [MapperIgnoreSource(nameof(Entity.Balance.OwnerId))]
    [MapperIgnoreSource(nameof(Entity.Balance.Spaceship))]
    public partial SpaceshipCondition ToDomainSpaceshipCondition(Entity.SpaceshipCondition src);
    ///
    [MapperIgnoreTarget(nameof(Spaceship))]
    public partial Entity.SpaceshipCondition ToDbSpaceshipCondition(SpaceshipCondition src, Guid ownerId);
}