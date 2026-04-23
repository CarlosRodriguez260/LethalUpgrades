using System.Dynamic;
using BepInEx.Configuration;

namespace LethalUpgrades;

/// <summary>
/// Configs
/// </summary

internal class ConfigurationController
{
    private ConfigEntry<string> ServerNameCfg;
    
    internal string ServerName
    {
        get
        {
            if(ServerNameCfg.Value == "")
            {
                return (string)ServerNameCfg.DefaultValue;
            }
            return ServerNameCfg.Value;
        }
        set => ServerNameCfg.Value = value;
    }

    public ConfigurationController(ConfigFile Config)
    {
        ServerNameCfg = Config.Bind("Server Settings", "Server Name", "Default Server Name", "A");
    }
}