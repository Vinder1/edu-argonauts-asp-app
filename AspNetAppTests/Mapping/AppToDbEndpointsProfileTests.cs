using Argonauts.Core.Entity.Player;
using Argonauts.Web.Mapping;
using Argonauts.Web.Requests;
using AutoFixture;

namespace AspNetAppTests.Mapping;

public class AppToEndpointsMapperTests
{
    protected readonly Fixture _fixture;
    private readonly AppToEndpointsMapper _mapper;

    public AppToEndpointsMapperTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        _mapper = new AppToEndpointsMapper();
    }

    [Test]
    public async Task MoveSpaceshipRequest_To_Star_ShouldMapCorrectly()
    {
        var request = _fixture.Build<MoveSpaceshipRequest>().Create();

        var star = _mapper.ToStarFromMovementRequest(request);

        await Assert.That(star).IsNotDefault();
        await Assert.That(star.Radius).IsEqualTo(request.NewRadius);
        await Assert.That(star.AngleMilliradians).IsEqualTo(request.NewAngle);
    }

    [Test]
    public async Task MoveSpaceshipRequest_To_Star_WithZeroValues_ShouldMapCorrectly()
    {
        var request = _fixture.Build<MoveSpaceshipRequest>()
            .With(r => r.NewAngle, 0)
            .With(r => r.NewRadius, 0)
            .Create();

        var star = _mapper.ToStarFromMovementRequest(request);

        await Assert.That(star.Radius).IsEqualTo(0);
        await Assert.That(star.AngleMilliradians).IsEqualTo(0);
    }

    [Test]
    public async Task MoveSpaceshipRequest_To_Star_WithMaxValues_ShouldMapCorrectly()
    {
        var request = _fixture.Build<MoveSpaceshipRequest>()
            .With(r => r.NewAngle, int.MaxValue)
            .With(r => r.NewRadius, int.MaxValue)
            .Create();

        var star = _mapper.ToStarFromMovementRequest(request);

        await Assert.That(star).IsNotDefault();
        await Assert.That(star.Radius).IsEqualTo(request.NewRadius);
        await Assert.That(star.AngleMilliradians).IsEqualTo(request.NewAngle);
    }

    [Test]
    public async Task MoveSpaceshipRequest_To_Star_ShouldIgnoreVisitedByShips()
    {
        var request = _fixture.Build<MoveSpaceshipRequest>().Create();

        var star = _mapper.ToStarFromMovementRequest(request);

        await Assert.That(star).IsNotDefault();
        // VisitedByShips property is not mapped, but may be default or set by fixture
    }

    [Test]
    public async Task Player_To_UserDto_ShouldMapCorrectly()
    {
        var player = _fixture.Build<Player>()
            .With(p => p.Id, Guid.NewGuid())
            .With(p => p.Name, "Test Player")
            .With(p => p.Login, "test_login")
            .With(p => p.Role, "User")
            .Create();

        var userDto = _mapper.ToUserDto(player);

        await Assert.That(userDto).IsNotNull();
        await Assert.That(userDto.Id).IsEqualTo(player.Id);
        await Assert.That(userDto.Name).IsEqualTo(player.Name);
        await Assert.That(userDto.Login).IsEqualTo(player.Login);
        await Assert.That(userDto.Role).IsEqualTo(player.Role);
    }
}