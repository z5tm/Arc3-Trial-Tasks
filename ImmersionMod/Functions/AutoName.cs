using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using ImmersionMod.Commands;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace ImmersionMod.Functions;

public static class AutoName
{
    private static Dictionary<int, int>? ZombieCount { get; set; } // playerid, player's zombie number
    private static Dictionary<string, string>? Names { get; set; } // userid (steamid64), displayname

    public static void RoundInitialization() // RPNameOnSpawn check is in InitEvents.
    {
        ZombieCount = null;
        Names = null;
        UserInfo.GenderPreferences ??= new Dictionary<string, string>(); // we want this to persist at least a bit, so we don't always set it here!

        foreach (var plr in Player.ReadyList)
        {
            if ((plr.DisplayName.ToLower().Contains("she/her") || plr.Nickname.ToLower().Contains("she/her")) && !plr.DoNotTrack)
            {
                UserInfo.GenderPreferences[plr.UserId] = "f";
                Logger.Info($"\"{plr.Nickname}\"'s auto-naming preferences have been set to feminine.");
            }

            if ((plr.DisplayName.ToLower().Contains("he/him") || plr.Nickname.ToLower().Contains("he/him")) && !plr.DoNotTrack)
            {
                UserInfo.GenderPreferences[plr.UserId] = "m";
                Logger.Info($"\"{plr.Nickname}\"'s auto-naming preferences have been set to masculine.");
            }
            
            if ((plr.DisplayName.ToLower().Contains("they/them") || plr.Nickname.ToLower().Contains("they/them")) && !plr.DoNotTrack)
            {
                UserInfo.GenderPreferences[plr.UserId] = "n";
                Logger.Info($"\"{plr.Nickname}\"'s auto-naming preferences have been set to gender-neutral.");
                plr.SendHint("Note: We recommend choosing your own name during RP, the gender neutral names are currently unimplemented.", 15);
            }
        }
    }

    public static void AutoNaming (PlayerChangedRoleEventArgs ev) // RPNameOnSpawn check is in InitEvents.
    {
        Logger.Info($"[AutoNaming] Player \"{ev.Player.Nickname}\" has changed their role to {ev.NewRole.RoleTypeId}");
        if (ev.NewRole.RoleTypeId is RoleTypeId.Spectator or RoleTypeId.Filmmaker or RoleTypeId.None or RoleTypeId.Destroyed)
        {
            Logger.Debug("[AutoNaming] SaveNamesOnDeath running!");
            Names ??= new Dictionary<string, string>(); // i love you modern C#
            Names[ev.Player.UserId] = ev.Player.ReferenceHub.nicknameSync.Network_displayName;
            ev.Player.DisplayName = string.Empty;
            Logger.Debug($"[AutoNaming] SaveNamesOnDeath done. {Names[ev.Player.UserId]}");
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
            if (Plugin.Instance.Config.ExtraDebugLogging) Logger.Debug("[AutoNaming-Zombie] Continuing with zombie logic.");
            if (ev.Player.ReferenceHub.gameObject == null)
            {
                Logger.Warn("[AutoNaming-Zombie] Target's GameObject was null! Returning to prevent NRE.");
                return;
            }
            Logger.Debug("[AutoNaming-Zombie] Target's GameObject wasn't null!");
            string prevName;
            if (Plugin.Instance.Config.ZombieCount)
            {
                if (ev.Player.ReferenceHub.gameObject == null)
                {
                    Logger.Warn("[AutoNaming-Zombie] Target's GameObject was null! Returning to prevent NRE.");
                    return;
                }
                
                Logger.Debug($"[AutoNaming-Zombie-Count] Player \"{ev.Player.Nickname}\" was a zombie, and ZombieCount was true.");
                ZombieCount ??= new Dictionary<int, int> { { ev.Player.PlayerId, 1 } }; // if ZombieCount was null, create a new dictionary and add the player to it with ID 1!
                if ((Names?.TryGetValue(ev.Player.UserId, out prevName) ?? false) && ZombieCount.TryGetValue(ev.Player.PlayerId, out var zombieNum)) // PATH 1: they have a ZombieCount, and they have a previous name.
                {
                    if (Plugin.Instance.Config.ExtraDebugLogging) 
                        Logger.Debug($"[AutoNaming-Zombie-Count] Player \"{ev.Player.Nickname}\" was a zombie, zombieNum was found in ZombieCount, and their previous name was found. Using previous number for this user.");
                    ev.Player.DisplayName = "SCP-049-2" + $"-{zombieNum}" + $" ({prevName})";
                    return;
                }

                if (ZombieCount.TryGetValue(ev.Player.PlayerId, out zombieNum)) // PATH 2: they are in ZombieNum, but have no known previous name.
                {
                    if (Plugin.Instance.Config.ExtraDebugLogging) 
                        Logger.Debug($"[AutoNaming-Zombie-Count] Player \"{ev.Player.Nickname}\" has no known previous name, but was found in ZombieCount.");
                    ev.Player.DisplayName = "SCP-049-2-" + zombieNum;
                    return;
                }

                // by now they aren't in ZombieNum, so we add.
                if (Plugin.Instance.Config.ExtraDebugLogging) 
                    Logger.Debug($"[AutoNaming-ZombieCount] Player \"{ev.Player.Nickname}\" was not in ZombieCount! Adding.");
                ZombieCount[ev.Player.PlayerId] = ZombieCount.Count + 1;

                if (Names?.TryGetValue(ev.Player.UserId, out prevName) ?? false) // PATH 3: they are in names, with a default to false if Names is null
                {
                    if (Plugin.Instance.Config.ExtraDebugLogging)
                        Logger.Debug($"[AutoNaming-ZombieCount] Player \"{ev.Player.Nickname}\" was not a zombie. A previous name was found, so using \"{prevName}\".");
                    ev.Player.DisplayName = "SCP-049-2-" + ZombieCount[ev.Player.PlayerId] + $"({prevName})";
                    return;
                }

                if (Plugin.Instance.Config.ExtraDebugLogging)
                    Logger.Debug($"[AutoNaming-ZombieCount] Player \"{ev.Player.Nickname}\" is a first time zombie. Previous name not found.");
                
                ev.Player.DisplayName = "SCP-049-2-" + ZombieCount[ev.Player.PlayerId]; // PATH 4: they are not in names, so we just use ZombieCount!
                return;
            }
            Logger.Debug("[AutoNaming-Zombie] ZombieCount was false. Continuing with RNG-based naming.");

            if (ev.Player.ReferenceHub.gameObject == null)
            {
                Logger.Warn("[AutoNaming-Zombie] Target's GameObject was null! Returning to prevent NRE.");
                return;
            }

            if (Names?.TryGetValue(ev.Player.UserId, out prevName) ?? false) // PATH 3: they are in names, with a default to false if Names is null
            {
                if (Plugin.Instance.Config.ExtraDebugLogging)
                    Logger.Debug($"[AutoNaming-Zombie] Player \"{ev.Player.Nickname}\" is now a zombie. Name was found, so using \"{val}\" + \"{prevName}\"!!");
                ev.Player.DisplayName = "SCP-049-2-" + val + $" ({prevName})";
                return;
            }

            if (Plugin.Instance.Config.ExtraDebugLogging)
                Logger.Debug($"\"{ev.Player.Nickname}\"'s role was Scp0492. We also found no previous name. Setting nickname to SCP-049-2-{val}.");
            ev.Player.DisplayName = "SCP-049-2-" + val;
            return;
        }

        if (Plugin.Instance.FileLengths?.Count == 0 || Plugin.Instance.FileLengths == null)
        {
            Logger.Warn("FileLengths was null or zero. FemNames, MascNames, and LastNames all failed to load. Canceling.");
            return;
        }
        
        max = Plugin.Instance.FileLengths.Min();
        Logger.Info($"Maximum line to pick for RNG has been set to: \"{max}\".");

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
        Logger.Info($"Made it past RNG. val = {val}, result = {result}, max = {max}.");

        string? genderPreference = null;
        
        UserInfo.GenderPreferences?.TryGetValue(ev.Player.UserId, out genderPreference);

        genderPreference ??= "f";
        
        switch (genderPreference)
        {
            case "f":
            {
                if (Plugin.Instance.Config.ExtraDebugLogging)
                    Logger.Debug($"[AutoName-Feminine] Getting feminine name and lastname for \"{ev.Player.Nickname}\".");
                
                var feminineName = Plugin.Instance.FemNames?.ElementAt(val);
                var lastName = Plugin.Instance.Surnames?.ElementAt(val);
                
                if (Plugin.Instance.Config.ExtraDebugLogging)
                    Logger.Debug("[AutoName-Feminine] " + feminineName + lastName + $" (\"{ev.Player.Nickname}\")");
                
                if (feminineName != null && lastName != null)
                    ev.Player.DisplayName = feminineName + " " + lastName;
                
                else
                {
                    ev.Player.DisplayName = Defaults.DefaultFeminineName;
                    Logger.Error($"[AutoName-Feminine] Line {val} in FemNames, or line {val} in LastNames doesn't exist!"); // this previously interpolated the entirety of FemNames and LastNames. o-o
                }
                break;
            }
            case "m":
            {
                if (Plugin.Instance.Config.ExtraDebugLogging)
                    Logger.Debug($"[AutoName-Masculine] Getting masculine name and lastname for \"{ev.Player.Nickname}\".");
                
                var mascName = Plugin.Instance.MascNames?.ElementAt(val);
                var lastName = Plugin.Instance.Surnames?.ElementAt(val);
                
                if (Plugin.Instance.Config.ExtraDebugLogging)
                    Logger.Debug($"[AutoName-Masculine] Setting \"{ev.Player.Nickname}\"'s name to \"{mascName} {lastName}\".");
            
                if (mascName != null && lastName != null)
                    ev.Player.DisplayName = mascName + " " + lastName;
                else
                {
                    Logger.Error($"[AutoName-Masculine] Line {val} in {Plugin.Instance.MascNames}, or line {val} in {Plugin.Instance.Surnames} doesn't exist! Falling back.");
                    ev.Player.DisplayName = Defaults.DefaultMasculineName;
                }
                break;
            }
            case "n":
            {
                var genderNeutralName = Defaults.DefaultGenderNeutralName;
                
                if (Plugin.Instance.Config.ExtraDebugLogging)
                    Logger.Debug($"[AutoName-Neutral] Gender neutral. Setting name to \"{genderNeutralName}\". Fallback value, this is NOT a random name. It is simply a fallback until we can find a gender neutral dataset.");
                
                ev.Player.DisplayName = genderNeutralName;
                break;
            }
        }
    }
}