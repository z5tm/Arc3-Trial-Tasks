using System;
using System.Collections.Generic;
using CommandSystem;
using EasyTmp;
using JetBrains.Annotations;
using LabApi.Features.Wrappers;

namespace z5tmsfirstitmesolelyusinglabapi.Commands;

public static class UserInfo
{
    public static Dictionary<string, string>? GenderPreferences { get; set; } // userid, m/n/f // male, neutral, female
}


[UsedImplicitly]
[CommandHandler(typeof(ClientCommandHandler))]
public class GenderSet : ICommand
{
    string ICommand.Command => "gender";

    string[] ICommand.Aliases => ["gs", "genderset"];

    string ICommand.Description => "Choose your gender preference! `.gender m` for male, `.gender f` for female, and `.gender n` for gender-neutral!";

    bool ICommand.Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var plr = Player.Get(sender);
        if (plr?.GameObject == null)
        {
            response =
                "<color=red>Error</color><color=orange>!</color> <color=blue>Player</color> <color=orange>or</color> <color=blue>GameObject</color> <color=orange>was</color> <color=red>null</color><color=orange>.</color>";
            return false;
        }

        if (plr.DoNotTrack)
        {
            response =
                "<color=orange>This feature is not availablee for players with</color> <color=blue>DoNotTrack</color> <color=orange>due to the requirement of extra logic.</color>";
            return false;
        }

        if (arguments.Count != 1)
        {
            response = EasyArgs.Build().CmdArguments("gender male/female/neutral").Done();
            return false;
        }

        var arg0 = arguments.At(0);
        UserInfo.GenderPreferences ??= new Dictionary<string, string>();

        switch (arg0)
        {
            case "m" or "male" or "masc" or "hehim" or "he/him":
                UserInfo.GenderPreferences[plr.UserId] = "m";
                break;

            case "f" or "female" or "fem" or "sheher" or "she/her":
                UserInfo.GenderPreferences[plr.UserId] = "f";
                break;

            case "n" or "neutral" or "nonbinary" or "enby" or "theythem" or "they/them":
                UserInfo.GenderPreferences[plr.UserId] = "n";
                break;
            default:
                response = EasyArgs.Build().CmdArguments("gender male/female/neutral").Done();
                return false;
        }

        response = "<color=green>Success</color><color=orange>! Your pronoun preferences have been set.</color>";
        return true;
    }
}