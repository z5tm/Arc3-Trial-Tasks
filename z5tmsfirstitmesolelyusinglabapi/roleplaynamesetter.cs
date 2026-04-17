using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;
using z5tmsfirstitmesolelyusinglabapi.Commands;

namespace z5tmsfirstitmesolelyusinglabapi;

public static class RolePlayNameSetter
{
    private static Dictionary<int, int>? ZombieCount { get; set; } // playerid, zombiecount
    private static Dictionary<string, string>? Names { get; set; } // userid (steamid64), displayname

    public static void RoundStartingShit ()
    {
        ZombieCount = null;
        Names = null;
        UserInfo.GenderPreferences ??= new Dictionary<string, string>(); // i think we want this to persist, so we keep !

        foreach (var plr in Player.ReadyList)
        {
            if (plr.DisplayName.ToLower().Contains("she/her") || plr.Nickname.ToLower().Contains("she/her") && !plr.DoNotTrack)
            {
                UserInfo.GenderPreferences[plr.UserId] = "f"; // userid, female -- for name preferences !
            }

            if (plr.DisplayName.ToLower().Contains("he/him") || plr.Nickname.ToLower().Contains("he/him") && !plr.DoNotTrack)
            {
                UserInfo.GenderPreferences[plr.UserId] = "m"; // userid, male -- for name preferences !
            }
            
            if (plr.DisplayName.ToLower().Contains("they/them") || plr.Nickname.ToLower().Contains("they/them") && !plr.DoNotTrack)
            {
                UserInfo.GenderPreferences[plr.UserId] = "n"; // userid, neutral -- for name preferences !
            }
        }
    }

    public static void DoRoleplayNamingShit (PlayerChangedRoleEventArgs ev)
    {
        Logger.Info($"[DoRoleplayShit] Player {ev.Player.Nickname} has changed their role to {ev.NewRole.RoleTypeId}");
        if (ev.NewRole.RoleTypeId is RoleTypeId.Spectator or RoleTypeId.Filmmaker or RoleTypeId.None or RoleTypeId.Destroyed)
        {
            Logger.Info("[DoRoleplayShit] SaveNamesOnDeath running!");
            Names ??= new Dictionary<string, string>(); // i love you modern C#
            Names[ev.Player.UserId] = ev.Player.ReferenceHub.nicknameSync.Network_displayName;
            ev.Player.DisplayName = string.Empty;
            Logger.Info($"[DoRoleplayShit] SaveNamesOnDeath done. {Names[ev.Player.UserId]}");
            return;
        }

        switch (ev.NewRole.RoleTypeId)
        {
            case RoleTypeId.Scp049:
                ev.Player.DisplayName = "SCP-049 \"The Plague Doctor\"";
                return;
            case RoleTypeId.Scp079:
                ev.Player.DisplayName = "SCP-079 \"Old Al\"";
                return;
            case RoleTypeId.Scp939:
                ev.Player.DisplayName = "SCP-939 \"With Many Voices\"";
                return;
            case RoleTypeId.Scp096:
                ev.Player.DisplayName = "SCP-096 \"The Shy Guy\"";
                return;
            case RoleTypeId.Scp106:
                ev.Player.DisplayName = "SCP-106 \"Larry\"";
                return;
            case RoleTypeId.Scp173:
                ev.Player.DisplayName = "SCP-173 \"The Statue\"";
                return;
            case RoleTypeId.Scp3114:
                ev.Player.DisplayName = "SCP-3114";
                return;
        }

        var bytes = new byte[4];

        var max = 6000;
        int result;
        using (var rng = RandomNumberGenerator.Create())
        {
            do
            {
                rng.GetBytes(bytes);
                result = BitConverter.ToInt32(bytes, 0) & int.MaxValue;
            } while (result > (int.MaxValue - (int.MaxValue % max)));
        }

        int val = result % max;

        if (ev.NewRole.RoleTypeId == RoleTypeId.Scp0492)
        {
            Logger.Info("[DoRoleplayShit] Continuing with zombie logic.!");
            if (ev.Player.ReferenceHub.gameObject == null) return;
            Logger.Info("[DoRoleplayShit] Target's GameObject wasn't null!");
            string prevName;
            if (z5tm.Instance.Config.ZombieCount)
            {
                Logger.Info($"[DoRoleplayShit] Player {ev.Player.Nickname} was a zombie, and ZombieCount was true.");
                ZombieCount ??= new Dictionary<int, int> { { ev.Player.PlayerId, 1 } };
                if ((Names?.TryGetValue(ev.Player.UserId, out prevName) ?? false) &&
                    ZombieCount.TryGetValue(ev.Player.PlayerId, out var zombieNum)) // PATH 1: they have a zombiecount, and they have a previous name.
                {
                    Logger.Info(
                        $"[DoRoleplayShit] Player {ev.Player.Nickname} was a zombie, ZombieNum was found in ZombieCount, and their previous name was found. Using old zombiecount.");
                    ev.Player.DisplayName = "SCP-049-2" + $"-{zombieNum}" + $"({prevName})";
                    return;
                }

                if (ZombieCount.TryGetValue(ev.Player.PlayerId, out zombieNum)) // PATH 2: they are in zombienum, but have no known previous name.
                {
                    Logger.Info($"[DoRoleplayShit] Player {ev.Player.Nickname} was not found in Name, just continuing with ZombieCount, if user was in the zombiecount.");
                    ev.Player.DisplayName = "SCP-049-2-" + zombieNum;
                    return;
                }

                // by now they aren't in zombienum, so we add.
                Logger.Info($"[DoRoleplayShit] Player {ev.Player.Nickname} was not in ZombieCount!! Adding.");
                ZombieCount.Add(ev.Player.PlayerId, ZombieCount.Count + 1);

                if (Names?.TryGetValue(ev.Player.UserId, out prevName) ?? false) // PATH 3: they are in names, with a default to false is Names is null
                {
                    Logger.Info($"[DoRoleplayShit] Player {ev.Player.Nickname} was not a zombie. Name was found, so using \"{prevName}\"!!");
                    ev.Player.DisplayName = "SCP-049-2-" + ZombieCount[ev.Player.PlayerId] + $"({prevName})";
                    return;
                }

                Logger.Info($"[DoRoleplayShit] Player {ev.Player.Nickname} was not a zombie. Name was also not found, so using a fallback!!"); // PATH 4: new zombie, no name!
                ev.Player.DisplayName = "SCP-049-2-" + ZombieCount[ev.Player.PlayerId];
                return;
            }

            Logger.Info("ZombieCount was false!");

            if (ev.Player.ReferenceHub.gameObject == null)
            {
                Logger.Error("[ZombieShit] Target's GameObject was null!");
                return;
            }

            if (Names?.TryGetValue(ev.Player.UserId, out prevName) ?? false) // PATH 3: they are in names, with a default to false is Names is null
            {
                Logger.Info($"[DoRoleplayShit] Player {ev.Player.Nickname} is now a zombie. Name was found, so using \"{val}\" + \"{prevName}\"!!");
                ev.Player.DisplayName = "SCP-049-2-" + val + $" ({prevName})";
                return;
            }

            Logger.Info($"{ev.Player.Nickname}'s role was Scp0492. We also found no previous name. Setting nickname to SCP-049-2-{val}.");
            ev.Player.DisplayName = "SCP-049-2-" + val;
        }

        if (z5tm.Instance.Maxes?.Count == 0 || z5tm.Instance.Maxes == null)
        {
            Logger.Info("Maxes was zero. FemNames, MascNames, and LastNames all failed to load. Canceling.");
            return;
        }
        max = z5tm.Instance.Maxes.Min();
        Logger.Info($"Maxes's lowest was {max}.");
        // int result;
        using (var rng = RandomNumberGenerator.Create())
        {
            do
            {
                rng.GetBytes(bytes);
                result = BitConverter.ToInt32(bytes, 0) & int.MaxValue;
            }
            while (result > (int.MaxValue - (int.MaxValue % max)));
        }
        val = result % (max);
        Logger.Info($"Made it past RNG. val = {val}, result = {result}, max = {max}, MaxLength = {z5tm.Instance.LastNames?.Length}");

        var genderPreference = "f";
        
        UserInfo.GenderPreferences?.TryGetValue(ev.Player.UserId, out genderPreference);

        genderPreference ??= "f";
        
        switch (genderPreference)
        {
            case "f":
            {
                Logger.Info($"Feminine path. Getting feminine name and lastname, to combine.");
                var feminineName = z5tm.Instance.FemNames?.ElementAt(val);
                var lastName = z5tm.Instance.LastNames?.ElementAt(val);
                Logger.Info(feminineName + lastName + ev.Player.Nickname + "ChaosInsurgencyPath");
            
                if (feminineName != null && lastName != null)
                    ev.Player.DisplayName = feminineName + " " + lastName;
                else if (!z5tm.Instance.Config.RPNameOnSpawn) Logger.Error($"Line {val} in {z5tm.Instance.FemNames}, or line {val} in {z5tm.Instance.LastNames} doesn't exist!");
                break;
            }
            case "m":
            {
                Logger.Info($"Masculine path. Getting masculine name and lastname, to combine.");
                var mascName = z5tm.Instance.MascNames?.ElementAt(val);
                var lastName = z5tm.Instance.LastNames?.ElementAt(val);
                Logger.Info(mascName +" "+ lastName +" ("+ ev.Player.Nickname + ") FoundationForcesPath");
            
                if (mascName != null && lastName != null)
                    ev.Player.DisplayName = mascName + " " + lastName;
                else if (!z5tm.Instance.Config.RPNameOnSpawn) Logger.Error($"Line {val} in {z5tm.Instance.MascNames}, or line {val} in {z5tm.Instance.LastNames} doesn't exist!");
                break;
            }
            case "n":
            {
                var lastName = "Luca Smith";
                Logger.Info($"Gender neutral. Setting name to \"{lastName}\". Fallback value, this is NOT a random name. It is simply a fallback until we can find a gender neutral dataset.");
                
                ev.Player.DisplayName = lastName;
                break;
            }
        }
    }
}

// public static class HttpShit
// {
//     private static readonly HttpClient Client = new(); // prevent socket exhaustion by using a static readonly
//     public static async Task<string?> AsyncReadingFile(this string url, int targetLine)
//     {
//         using var response = await Client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
//         using var stream = await response.Content.ReadAsStreamAsync();
//         using var reader = new StreamReader(stream);
//         var currentLineNumber = 0;
//
//         while (await reader.ReadLineAsync() is { } line)
//         {
//             currentLineNumber++;
//
//             if (currentLineNumber == targetLine)
//             {
//                 return line;
//             }
//         }
//
//         return null;
//         // try
//         // {
//         //     var initRequest = new HttpRequestMessage(HttpMethod.Get, url);
//         //     initRequest.Headers.Range = new(0, 25);
//         //     var response = await Client.SendAsync(initRequest).ConfigureAwait(false);
//         //     var content = await response.Content.ReadAsStringAsync();
//         //     var firstLine = content.Split(["\r\n", "\r", "\n"], StringSplitOptions.None).FirstOrDefault();
//         //     return firstLine;
//         // }
//         // catch (HttpRequestException e)
//         // {
//         //     Logger.Error($"HTTP Request Exception: {e.Message}, {e.HelpLink}");
//         // }
//         // catch (Exception e)
//         // {
//         //     Logger.Error(e.Message + "Error!!!!!!!");
//         // }
//         //
//         // return null;
//     }
// }