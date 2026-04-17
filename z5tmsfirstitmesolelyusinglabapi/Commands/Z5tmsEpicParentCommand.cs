// using System;
// using System.Diagnostics.CodeAnalysis;
// using CommandSystem;
// using LabApi.Features.Console;
// using LabApi.Features.Wrappers;
//
// namespace z5tmsfirstitmesolelyusinglabapi.Commands;
//
//
// [CommandHandler(typeof(RemoteAdminCommandHandler))]
// // ReSharper disable InconsistentNaming
// public class Z5tmsEpicParentCommand : ICommand
// // ReSharper restore InconsistentNaming
// {
//     // public override void LoadGeneratedCommands()
//     // {
//     //     RegisterCommand(new GivePlayerEpicItem());
//     //     RegisterCommand(new CheckEpicAdrenalineUsers());
//     // }
//
//     public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
//     {
//         // foreach (var commands in Commands)
//         //     if (arguments.At(0) == commands.Key)
//         //     {
//         //         commands.Value.Execute(arguments, sender, out response);
//         //     }
//         
//         
//         response = "ls";
//         return true;
//     }
//
//     public string Command { get; } = "z5tm";
//     public string[] Aliases { get; } = ["z5"];
//     public string Description { get; } = "z5tmsepiclabapicommand";
// }