using System.IO;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Paths;

namespace ImmersionMod;

public static class Paths // no reason for these to be private by the way, freely change future developers !!!!!!
{
    // private static readonly string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    // private static readonly string LabAPI = Path.Combine(AppData, "SCP Secret Laboratory", "LabAPI");
    // private static readonly string Configs = Path.Combine(LabAPI, "configs"); // static field 
    // private static string ConfigPort => Path.Combine(Configs, Server.Port.ToString()); // static property btw, this code runs each time it's accessed
    // public static string InstanceConfig => Path.Combine(ConfigPort, z5tm.Instance.Name); // so we don't have to reset it later ! also static property cuz it uses one. it's kinda like gpl 3.0 !
    // public static string Names => Path.Combine(InstanceConfig, "Names");
    // public static string ConfigPath => PathManager.Configs.FullName;
    public static string Names => Path.Combine(PathManager.Configs.FullName, Server.Port.ToString(), Plugin.Instance.Name, "Names");
    public static string Pronouns => Path.Combine(PathManager.Configs.FullName, Server.Port.ToString(), Plugin.Instance.Name, "Pronouns");
}