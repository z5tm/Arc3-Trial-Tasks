using System;
using CommandSystem;
using EasyTmp;
using ImmersionMod.Functions;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;

namespace ImmersionMod.Commands;

[UsedImplicitly] // love you rider
[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class RusherInformation : ICommand
{
    public string Command { get; } = "r";
    public string[] Aliases { get; } = ["rushin", "rushers", "rush", "rusherinformation"];
    public string Description { get; } = "[ImmersionMod] Information regarding AdrenalineRushers.!";
    
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);
        if (player == null)
        {
            response = EasyArgs.Build()
                .Red("FAILURE:")
                .Space().Orange("Player was null.")
                .Done();
            return false;
        }
        
        if (!player.RemoteAdminAccess)
        {
            response = EasyArgs.Build()
                .Red("FAILURE:")
                .Space().Orange("No RA permission.")
                .Done();
            return false;
        }

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