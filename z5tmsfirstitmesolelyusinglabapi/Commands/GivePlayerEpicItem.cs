using System;
using CommandSystem;
using EasyTmp;
using JetBrains.Annotations;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;

namespace z5tmsfirstitmesolelyusinglabapi.Commands;

[UsedImplicitly]
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class GivePlayerEpicItem : ICommand
{
    public string Command { get; } = "z5give";
    public string[] Aliases { get; } = ["z5g"];
    public string Description { get; } = "GivePlayersItem";
    // private string Usage { get; } = EasyArgs.Build().CmdArguments("give opt-userid itemid").Done();
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var plr = Player.Get(sender);
        if (plr == null)
        {
            response = "Player cannot be null.";
            return false;
        }
        if (!sender.CheckPermission(PlayerPermissions.GivingItems))
        {
            response = "<color=red>Failure</color><color=orange>: you do not have the</color> <color=orange>\"</color><color=blue>GivingItems</color><color=orange>\" permission.</color>";
            return false;
        }
        try
        {
            switch (arguments.Count)
            {
                // conditon zero: user has no idea what they're doing
                case 0:
                    response = EasyArgs.Build().CmdArguments("give opt-userid itemid").Done();;
                    return false;
                // condition 1: user just wants to give themselves an item
                case 1:
                {
                    int.TryParse(arguments.At(0), out int itemId);
                    
                    if (!plr.IsAlive || !plr.Connection.isAuthenticated || !plr.Connection.isReady || plr.IsDisarmed)
                    {
                        response = "<color=orange>General</color> <color=red>exception</color><color=orange>. Please ensure the user is connected and not disarmed.</color>"; // fix later i'm tired
                        return false;
                    }


                    if (!plr.IsInventoryFull)
                    {
                        plr.AddItem((ItemType)itemId);
                        response = $"<color=green>Success</color><color=orange>!</color> <color=blue>{(ItemType)itemId}</color> <color=orange>has been given.</color>";
                        return true;
                    }
                    else
                    {
                        response = "<color=red>Failure</color><color=orange>. User's inventory is</color> <color=red>full</color><color=orange>.</color>";
                        return false;
                    }
                }
                // condition 2: user wants to give somebody an item !
                case 2:
                {
                    int.TryParse(arguments.At(0), out var playerId);
                    int.TryParse(arguments.At(1), out var itemId);
                    var plrToGive = Player.Get(playerId);
                    if (plrToGive == null)
                    {
                        response = $"<color=red>Error!</color>\n<color=orange>Player at ID</color> <color=blue>{playerId}</color><color=orange> was</color> <color=red>null</color><color=orange>.</color>";
                        return false;
                    }
                    if (!plrToGive.IsInventoryFull)
                        plrToGive.AddItem((ItemType)itemId);
                    response = $"<color=green>Success</color><color=orange>!</color> <color=blue>{(ItemType)itemId}</color> <color=orange>has been given to</color> <color=blue>{plrToGive.DisplayName}</color><color=orange>!</color>";
                    return true;
                }
                // condition *: user is tired.
                default:
                {
                    response = EasyArgs.Build().CmdArguments("give opt-userid itemid").Done();
                    return false;
                }
            }
        }
        catch (NullReferenceException e)
        {
            Logger.Error($"NullReferenceException: {e.Message}"); // so useful!
            response = !z5tm.Instance.Config.AllowStaffExceptionRead ? "<color=orange>One of the values provided ended up being</color> <color=red>null</color><color=orange>. Please ask the server administrators to either enable</color> <color=blue>AllowStaffExceptionRead</color> <color=orange>to read the error, or check the game console. </color>" : $"<color=red>NullReferenceException</color><color=orange> caused by one of the provided values.</color>\n{e.Message}";
            return false;
        }
        catch (Exception e)
        {
            Logger.Debug($"Exceptioon! \"{e.Message}\"");
            response = !z5tm.Instance.Config.AllowStaffExceptionRead ? "<color=orange>One of the values provided ended up in an</color> <color=red>exception</color><color=orange>. Please ask the server administrators to either enable</color> <color=blue>AllowStaffExceptionRead</color> <color=orange>to read the error, or check the game console. </color>" : $"<color=red>Exception</color><color=orange>, likely caused by one of the provided values.</color>\n{e.Message}";
            return false;
        }
    }
}