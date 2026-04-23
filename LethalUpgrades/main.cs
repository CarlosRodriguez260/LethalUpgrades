using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using LethalUpgrades.Patches;
using TerminalApi;
using TerminalApi.Classes;
using static TerminalApi.TerminalApi;
using UnityEngine;
using LethalModDataLib.Attributes;
using LethalModDataLib;


namespace LethalUpgrades;
[BepInPlugin(modGUID, modName, modVersion)]
[BepInDependency("MaxWasUnavailable.LethalModDataLib")]
public class LethalUpgradesBase : BaseUnityPlugin
{
    private const string modGUID = "ChuitosLethalUpgrades";
    private const string modName = "Lethal Upgrades Mod";
    private const string modVersion = "0.1";

    private readonly Harmony harmony = new Harmony (modGUID);
    internal static LethalUpgradesBase Instance;
    internal static ManualLogSource mls;
    internal static ConfigurationController ConfigManager;

    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool health_t1 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool health_t2 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool health_t3 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool health_leg = false;

    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool stamina_t1 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool stamina_t2 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool stamina_t3= false;

    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool movement_t1 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool movement_t2 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool movement_t3 = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
        mls.LogInfo("LethalUpgrades at your service!");
        // harmony.PatchAll(typeof(StartOfRoundPatch));
        harmony.PatchAll(typeof(HealthPatching));
        harmony.PatchAll(typeof(StaminaPatching));
        harmony.PatchAll(typeof(MovementPatching));
        harmony.PatchAll(typeof(DebugPatching));
        ConfigManager = new ConfigurationController(Config);

        #region Terminal Commands
        AddCommand("upgrade",
        "Thank you for joining the *LETHAL UPGRADES* program.\n" +
        "In exchange for credits, we can provide upgrades that work for everyones suit's or equipment.\n" +
        "We currently provide the following types of [UPGRADES]:\n" +
        "- Health\n" +
        "- Stamina\n"+
        "- Movement\n" +
        "- Utilities\n\n" +

        "Each category has 3 tiers with differing costs. If you manage to unlock all tiers in one category,\n" +
        "you get a special token to access a new, legendary upgrade free of charge!. Thank you for trusting us with your mission.\n\n" +
        
        "To see information about each upgrade, type 'upgrade [UPGRADE] info'\n" +
        "To buy an upgrade, type 'upgrade [UPGRADE] [TIER]'\n");

        AddCommand("upgrade health info",
        "These upgrades affect your health. They consist of the following:\n" +
        "- Tier 1: Gain +20 additional health. Cost: ▮200\n" +
        "- Tier 2: Reduce all incoming damage by 5%. Cost: ▮300\n" +
        "- Tier 3: Gain an adaptive regeneration ability. Cost: ▮400\n" +
        "- Legendary: Gain +30 additional health.\n\n" +
        "NOTE: Health upgrades only apply while in orbit.\n");

        AddCommand("upgrade stamina info",
        "These upgrades affect your stamina. They consist of the following:\n" +
        "- Tier 1: Increase stamina amount by 30%. Cost: ▮250\n" +
        "- Tier 2: Improve stamina regen by 5%. Cost: ▮300\n" +
        "- Tier 3: Reduced stamina consumption when heavy (>=40 lbs) by 50%. Cost: ▮400\n" +
        "- Legendary: When in danger, gain infinite stamina. After escaping danger, goes on cooldown for 30 seconds.\n");

        AddCommand("upgrade movement info",
        "These upgrades affect your movement. They consist of the following:\n" +
        "- Tier 1: Sprint 6% faster. Cost: ▮100\n" +
        "- Tier 2: Walk/Crouch 12% faster. Cost: ▮250\n" +
        "- Tier 3: Jump height increased. Cost: ▮300\n" +
        "- Legendary: All movement speed is 5% faster. When in danger, gain 10% more movement speed. After escaping danger, goes on cooldown for 30 seconds.\n");

        AddCommand("upgrade utilities info",
        "These upgrades affect your equipment or utilities. They consist of the following:\n" +
        "- Tier 1: Increase ALL battery capacities by 10%. Cost: ▮300\n" +
        "- Tier 2: Reduce cost of all store items by 10%. Cost: ▮450\n" +
        "- Tier 3: Flashlights items can pass through the inverse teleporter. Cost: ▮500\n" +
        "- Legendary: All equipment weighs 0 pounds.\n");

        AddCommand("give money", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                terminal.groupCredits += 500;
                return "Gave you 500 moneys for testing.\n";
            }, Category = "Other"
        });
        #endregion

        #region Health Upgrades
        AddCommand("upgrade health 1", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(health_t1)
                {
                    return "You already have this upgrade!\n";
                }

                var cost = 200;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ▮{cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                health_t1 = true;
                return $"Upgrade acquired. New balance of ▮{terminal.groupCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade health 2", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(health_t2)
                {
                    return "You already have this upgrade!\n";
                }

                var cost = 300;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if (!health_t1)
                {
                    return "You require the tier 1 health upgrade before this!\n";
                }

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ▮{cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                health_t2 = true;
                return $"Upgrade acquired. New balance of ▮{terminal.groupCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade health 3", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(health_t3)
                {
                    return "You already have this upgrade!\n";  
                }

                var cost = 400;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if (!health_t1)
                {
                    return $"Come on man you don't even have the tier 1...\n";
                }
                if (!health_t2)
                {
                    return $"You require the tier 2 health upgrade before this!\n";
                }

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ▮{cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                health_t3 = true;
                return $"Upgrade acquired. New balance of ▮{terminal.groupCredits}\n";
            }, Category = "Other"
        });
        #endregion

        #region Stamina Upgrades
        AddCommand("upgrade stamina 1", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(stamina_t1)
                {
                    return "You already have this upgrade!\n";  
                }

                var cost = 250;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ▮{cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                stamina_t1 = true;

                var sor = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
                for(int i = 0; i < sor.allPlayerScripts.Length; i++)
                {
                    sor.allPlayerScripts[i].sprintTime = sor.allPlayerScripts[i].sprintTime * 1.3f; 
                }
                return $"Upgrade acquired. New balance of ▮{terminal.groupCredits}\n";
            }, Category = "Other"
        });
        #endregion

        #region Movement Upgrades
        AddCommand("upgrade movement 1", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(movement_t1)
                {
                    return "You already have this upgrade!\n";  
                }

                var cost = 100;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ▮{cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                movement_t1 = true;

                return $"Upgrade acquired. New balance of ▮{terminal.groupCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade movement 2", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(movement_t2)
                {
                    return "You already have this upgrade!\n";  
                }

                var cost = 250;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if(!movement_t1)
                {
                    return "You need the tier 1 upgrade!\n";
                }

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ▮{cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                movement_t2 = true;

                return $"Upgrade acquired. New balance of ▮{terminal.groupCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade movement 3", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(movement_t3)
                {
                    return "You already have this upgrade!\n";  
                }

                var cost = 350;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if (!movement_t1)
                {
                    return "Come on man you don't even have the tier 1...\n";
                }
                if (!movement_t2)
                {
                    return "You require the tier 2 movement upgrade before this!\n";
                }

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ▮{cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                movement_t3 = true;

                return $"Upgrade acquired. New balance of ▮{terminal.groupCredits}\n";
            }, Category = "Other"
        });
        #endregion

        #region Utility Upgrades
        #endregion
    }
}
