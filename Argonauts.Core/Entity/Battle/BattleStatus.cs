namespace Argonauts.Core.Entity.Battle;

/// <summary>
/// 
/// </summary>
public class BattleStatus
{
    /// <summary>
    /// 
    /// </summary>
    public List<BattleMember> Members { get; set; } = [];

    /// <summary>
    /// 
    /// </summary>
    public BattleType BattleType { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public void ProcessMove()
    {
        var rnd = new Random();
        var aliveMembers = Members.Where(m => m.Alive()).ToArray();
        // set missing moves
        foreach (var m in aliveMembers.Where(m => m.Move == ""))
        {
            m.Move = "attack";
        }
        // set missing targets
        var aiArray = aliveMembers.Where(m => m.IsAI).Select(m => m.Id).ToArray();
        var playerArray = aliveMembers.Where(m => !m.IsAI).Select(m => m.Id).ToArray();

        foreach (var m in Members.Where(m => m.TargetId == null))
        {
            var arr = m.IsAI ? playerArray : aiArray;
            if (arr.Length == 0)
                continue;
            m.TargetId = arr[rnd.Next() % arr.Length];
        }

        //calc defense
        var def = new Dictionary<Guid, int>();
        foreach (var i in aliveMembers.Select(m => m.Id))
        {
            def[i] = 0;
        }
        foreach (var mem in aliveMembers.Where(m => m.Move == "defend"))
        {
            if (mem.TargetId == null)
                continue;
            def[mem.TargetId.Value]++;
        }

        foreach (var m in Members)
        {
            if (m.Move == "attack")
            {
                var target = Members.FirstOrDefault(mem => mem.Id == m.TargetId);
                if (target == null)
                    continue;
                var damage = m.Power / 5;
                damage /= def[target.Id] + 1;

                target.Health -= damage;
            }
        }

        // set everything to default
        foreach (var m in Members)
        {
            m.Move = "";
            m.TargetId = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool Continue()
    {
        return Members.Count(m => m.Alive()) > 1 && Members.Any(m => m.Alive() && !m.IsAI);
    }
}