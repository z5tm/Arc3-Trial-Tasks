using System.ComponentModel;

namespace ImmersionMod;

public class Config
{
    [Description("This toggles logging that may not be entirely necessary.")] public bool ExtraDebugLogging { get; set; } = false;

    [Description("Whether to count zombies up from the first. If disabled, will use random numbers.")] public bool ZombieCount { get; set; } = false;
    [Description("Whether to permit staff commands to show errors for the end user, or solely log errors in the game console.")] public bool AllowStaffExceptionRead { get; set; } = false;

    [Description(/*"Whether to enable downloading a list of RP names off github. Disable to use fallbacks."*/ "Whether to set RP names on spawn.")] public bool RPNameOnSpawn { get; set; } = true;

    [Description("Which file to pull the last names from? (Please use the example for the formatting!)")] 
    public string? RPSurnameFileName { get; set; } = /*https://raw.githubusercontent.com/danielmiessler/SecLists/refs/heads/master/Usernames/Names/*/"familynames-usa-top1000.txt";
    [Description("Which file to pull the first masculine names from? (Please use the example for the formatting!)")] 
    public string? RPMascFirstNameFileName { get; set; } = /*https://raw.githubusercontent.com/danielmiessler/SecLists/refs/heads/master/Usernames/Names/*/"malenames-usa-top1000.txt";
    [Description("Which file to pull the first feminine names from? (Please use the example for the formatting!)")] 
    public string? RPFemFirstNameFileName { get; set; } = /*https://raw.githubusercontent.com/danielmiessler/SecLists/refs/heads/master/Usernames/Names/*/"femalenames-usa-top1000.txt";
}