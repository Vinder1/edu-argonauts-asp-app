namespace Argonauts.Infrastructure.Database.Entity;

public class Quest
{
    public Guid OwnerId { get; init; }
    public int Level { get; set; } = 1;
    public int Killed { get; set; }
    public Spaceship Spaceship { get; set; } = null!;
}
