using Argonauts.Application.Dto;
using Argonauts.Core.Entity.Galaxy;
using Argonauts.Core.Entity.Player;
using Argonauts.Web.Requests;
using Riok.Mapperly.Abstractions;

namespace Argonauts.Web.Mapping;

/// <summary>
/// 
/// </summary>
[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AppToEndpointsMapper
{
    ///
    [MapProperty(nameof(MoveSpaceshipRequest.NewAngle), nameof(Star.AngleMilliradians))]
    [MapProperty(nameof(MoveSpaceshipRequest.NewRadius), nameof(Star.Radius))]
    [MapperIgnoreTarget(nameof(Star.Type))]
    public partial Star ToStarFromMovementRequest(MoveSpaceshipRequest src);

    ///
    public partial UserDto ToUserDto(Player src);
}