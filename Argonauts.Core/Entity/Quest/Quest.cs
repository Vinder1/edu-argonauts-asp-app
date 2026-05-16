namespace Argonauts.Core.Entity.Quest;

public class Quest
{
    public int Level { get; set; } = 1;
    public int Killed { get; set; }

    public const int KillsRequired = 25;
    public bool IsCompleted => Killed >= KillsRequired;
}
