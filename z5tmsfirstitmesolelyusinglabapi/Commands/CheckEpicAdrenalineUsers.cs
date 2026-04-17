using System;
using CommandSystem;
using EasyTmp;
using LabApi.Features.Wrappers;

namespace z5tmsfirstitmesolelyusinglabapi.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class CheckEpicAdrenalineUsers : ICommand
{
    // foreach check on notready comp. to player.readylist
    
    public string Command { get; } = "r";
    public string[] Aliases { get; } = ["rushin", "rushers", "rush"];
    public string Description { get; } = "Check Adrenaline Rushers Info!";
    // private List<ReferenceHub>? _listOfPlayers;
    

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!Player.Get(sender)?.RemoteAdminAccess ?? true)
        {
            response = "<color=red>ERROR:</color> <color=orange>no RA or no player.</color>";
            return false;
        }
        // _listOfPlayers ??= [];
        // var listOfPlayers = _listOfPlayers;
        // listOfPlayers.AddRange(from plr in Player.ReadyList where !AdrenalineRush.NotReady?.Contains(plr.ReferenceHub) ?? true select plr.ReferenceHub);

        var responseBuilder = EasyArgs.Build().Blue("AdrenallineInfo:");
        foreach (var plr in Player.ReadyList)
        {
            var isActive = (AdrenalineRush.IsActive != null && AdrenalineRush.IsActive.Contains(plr.ReferenceHub._playerId)).ToString();
            if (AdrenalineRush.CoolDowners?.TryGetValue(plr.ReferenceHub, out var coolDown) ?? false)
            {
                responseBuilder.NewLine().Blue(plr.DisplayName)
                    .NewLine().Orange("IsActive: ").Blue(isActive)
                    .NewLine().Orange("Cooldown:")
                    .Space().Blue(coolDown.ToString());
            }
            else
            {
                responseBuilder.NewLine().Blue(plr.DisplayName)
                    .NewLine().Orange("IsActive: ").Blue((AdrenalineRush.IsActive != null && AdrenalineRush.IsActive.Contains(plr.ReferenceHub._playerId)).ToString())
                    .NewLine().Orange("Cooldown:")
                    .Space().Blue("0");
            }
        }
        response = responseBuilder.Done();
        return true;
    }
}