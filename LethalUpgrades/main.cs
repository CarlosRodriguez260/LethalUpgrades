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

    private readonly Harmony harmony = new Harmony(modGUID);
    internal static LethalUpgradesBase Instance;
    internal static ManualLogSource mls;
    internal static ConfigurationController ConfigManager;

    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static int tokens = 0;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static int token_meter = 0;

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
    public static bool stamina_leg = false;

    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool movement_t1 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool movement_t2 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool movement_t3 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool movement_leg = false;

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
        harmony.PatchAll(typeof(DebugPatching)); //Uncomment to have logs in BepInEx console
        harmony.PatchAll(typeof(TokenPatching));
        ConfigManager = new ConfigurationController(Config);

        #region Terminal Commands
        AddCommand("upgrade",
        "Thank you for joining the *LETHAL UPGRADES* program.\n" +
        "In exchange for credits, we can provide upgrades that should boost your quota-reaching efficiency!.\n" +
        "We currently provide the following types of [UPGRADES]:\n" +
        "- Health\n" +
        "- Stamina\n"+
        "- Movement\n" +
        "- Utilities\n\n" +

        "Each category has 3 tiers with differing costs, providing a plethora of different changes.\n" +
        "A special token can be acquired by proving your loot-gathering and survival skills, which can be turned in for free legendary upgrades!\n\n" +
        
        "To see information about each upgrade, type 'upgrade [UPGRADE] info'\n" +
        "To buy an upgrade, type 'upgrade [UPGRADE] [TIER]'\n" +
        "To learn how to get tokens, type 'upgrade token'\n");

        AddCommand("upgrade token",
        "Special tokens, denoted by Ŧ, are utilized to obtain legendary upgrades from each tier. " +
        "Unlike normal upgrades, you can use a token to buy any of the available legendary ones.\n\n" +
        "These tokens can only be acquired via good, cummulative performances on moons. Good luck!\n");
        
        AddCommand("upgrade health info",
        "These upgrades affect your health. They consist of the following:\n" +
        "- Tier 1: Gain +20 additional health. Cost: $200\n" + //Done
        "- Tier 2: Reduce all incoming damage by 5%. Cost: $300\n" + //Done
        "- Tier 3: Gain +30 additional health. Cost: $400\n" + //Done
        "- Legendary: Gain an adaptive regeneration ability.\n\n" + 
        "NOTE: Health-increasing upgrades only apply while in orbit.\n");

        AddCommand("upgrade stamina info",
        "These upgrades affect your stamina. They consist of the following:\n" +
        "- Tier 1: Decrease running stamina usage. Cost: $300\n" + //Done
        "- Tier 2: Improve stamina regen by 10%. Cost: $400\n" + //Done
        "- Tier 3: Reduced stamina penalties when heavy (>=50 lbs). Cost: $500\n" +
        "- Legendary: When damaged, regardless of amount or source, gain full stamina back.\n");

        AddCommand("upgrade movement info",
        "These upgrades affect your movement. They consist of the following:\n" +
        "- Tier 1: Sprint 6% faster. Cost: $150\n" + //Done
        "- Tier 2: Walk/Crouch 12% faster. Cost: $275\n" + //Done
        "- Tier 3: Jump height increased by 25%. Cost: $350\n" + //Done
        "- Legendary: While critically injured, become invisible.\n");

        AddCommand("upgrade utilities info",
        "These upgrades affect your equipment or utilities. They consist of the following:\n" +
        "- Tier 1: Increase ALL battery capacities by 10%. Cost: $300\n" +
        "- Tier 2: Reduce cost of all store items by 10%. Cost: $450\n" +
        "- Tier 3: Flashlights items can pass through the inverse teleporter. Cost: $500\n" +
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

        AddCommand("give meter", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                token_meter += 50;
                return "Filled up half your token meter.\n";
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
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                health_t1 = true;
                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                health_t2 = true;
                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                health_t3 = true;
                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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

                var cost = 300;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                stamina_t1 = true;
                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade stamina 2", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(stamina_t2)
                {
                    return "You already have this upgrade!\n";  
                }

                var cost = 400;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if(!stamina_t1)
                {
                    return "You require the tier 1 stamina upgrade before this!\n";
                }

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                stamina_t2 = true;
                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade stamina 3", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(stamina_t3)
                {
                    return "You already have this upgrade!\n";  
                }

                var cost = 500;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if(!stamina_t1)
                {
                    return $"Come on man you don't even have the tier 1...\n";
                }
                if(!stamina_t2)
                {
                    return "You require the tier 2 stamina upgrade before this!\n";
                }

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                stamina_t3 = true;
                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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

                var cost = 150;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                movement_t1 = true;

                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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

                var cost = 275;
                var terminal = UnityEngine.Object.FindFirstObjectByType<Terminal>();
                var groupCredits = terminal.groupCredits;

                if(!movement_t1)
                {
                    return "You need the tier 1 upgrade!\n";
                }

                if (groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                movement_t2 = true;

                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                terminal.groupCredits = groupCredits - cost;
                movement_t3 = true;

                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
            }, Category = "Other"
        });
        #endregion

        #region Utility Upgrades
        #endregion
    }
}
