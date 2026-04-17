using System.ComponentModel;

namespace z5tmsfirstitmesolelyusinglabapi;

public class configuration
{
    // [Description("Testing!!!!")] public bool? Testswitch { get; set; } = null; // yeah i got no idea either. i woke up one day and this was here.
    [Description("Never set this to true!")] public bool IsEnabled { get; set; } = true;
    [Description("Always keep this on!")] public bool Debug { get; set; } = true;

    [Description("Whether to count zombies up from the first. If disabled, will use random numbers.")] public bool ZombieCount { get; set; } = false;
    // [Description("Whether to enable TLS 1.2 + 1.3 on startup. Please do not turn this off unless absolutely necessary.")] public bool EnforceSecurity { get; set; } = true;
    [Description("Whether to permit staff commands to show errors for the end user, or solely log errors in the game console.")] public bool AllowStaffExceptionRead { get; set; } = false;

    [Description(/*"Whether to enable downloading a list of RP names off github. Disable to use fallbacks."*/ "Whether to set RP names on spawn.")] public bool RPNameOnSpawn { get; set; } = true;

    [Description("Which file to pull the last names from? (Please use the example for the formatting!)")] 
    public string? RPLastNameFileName { get; set; } = /*https://raw.githubusercontent.com/danielmiessler/SecLists/refs/heads/master/Usernames/Names/*/"familynames-usa-top1000.txt";
    [Description("Which file to pull the first masculine names from? (Please use the example for the formatting!)")] 
    public string? RPMascFirstNameFileName { get; set; } = /*https://raw.githubusercontent.com/danielmiessler/SecLists/refs/heads/master/Usernames/Names/*/"malenames-usa-top1000.txt";
    [Description("Which file to pull the first feminine names from? (Please use the example for the formatting!)")] 
    public string? RPFemFirstNameFileName { get; set; } = /*https://raw.githubusercontent.com/danielmiessler/SecLists/refs/heads/master/Usernames/Names/*/"femalenames-usa-top1000.txt";
    // [Description("Which file to pull the gender neutral names from? (Please use the above examples for the formatting!)")] 
    // public string? RPGenFirstNameFileName { get; set; } = string.Empty; // could not find a website for this, but it's an option ! could use the family names mayb

    // [Description("Used if your server will recieve many players constantly spawning in.")] public bool? HighVolumeMode { get; set; } = false;
}