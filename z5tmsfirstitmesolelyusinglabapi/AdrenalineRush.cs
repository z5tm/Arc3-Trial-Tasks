using System.Collections.Generic;
using LabApi.Events.Arguments.PlayerEvents;
using MEC;

namespace z5tmsfirstitmesolelyusinglabapi;

public class AdrenalineRush
{
    public static HashSet<ReferenceHub>? NotReady;
    public static HashSet<RecyclablePlayerId>? IsActive;
    public static Dictionary<ReferenceHub, int>? CoolDowners; // referencehub, cooldowntime
    
    private static List<CoroutineHandle>? _waiters;
    public static void Adrenaline(PlayerHurtEventArgs ev)
    {
        NotReady ??= [];
        if (!(ev.Player.Health <= ev.Player.MaxHealth * 0.25) || NotReady.Contains(ev.Player.ReferenceHub) || !ev.Player.IsAlive || ev.Player.IsDestroyed || ev.Player.Health == 0) return; // ezpz logic.
        ev.Player.SendHint("<color=orange>[</color><color=red>ADRENALINE RUSH</color><color=orange>]</color>", 5);
        ev.Player.EnableEffect<CustomPlayerEffects.Invigorated>(1, 5, true); // i hope that's the right one?!?!?!
        _waiters ??= new List<CoroutineHandle>();
        _waiters.Add(Timing.RunCoroutine(WaitFor30Seconds(ev.Player.ReferenceHub)));
    }

    private static IEnumerator<float> WaitFor30Seconds(ReferenceHub user) // "what is my purpose?" "you wait for 30 seconds" "oh my god.."
    {
        NotReady ??= []; NotReady.Add(user);
        IsActive ??= []; IsActive.Add(user._playerId); // i loove you reference hubb
        CoolDowners ??= new Dictionary<ReferenceHub, int>();
        yield return Timing.WaitForSeconds(5.0f);
        var name = user; // woo less stuff !!!! and less NREs lol
        IsActive.Remove(name._playerId);
        // CoolDowners.Add(name, 25);
        for (var i = 25; i != -1; i--) // run on beginning, if true continue, run before each iteration
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (user == null || user.gameObject == null) yield break;
            CoolDowners[name] = i; // replace the value (cd) using the key (name) !!!
            yield return Timing.WaitForSeconds(1.0f);
        }
        if (user != null) NotReady.Remove(user);
        // ReSharper restore ConditionIsAlwaysTrueOrFalse
    } // i do NOT believe you SL, user can 100% be null lol

    public static void Reset()
    {
        NotReady?.Clear();
        if (_waiters == null) return;
        
        Timing.KillCoroutines(_waiters.ToArray());
        _waiters.Clear();
    }
}