using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TerminalApi.Classes;
using static TerminalApi.TerminalApi;
using LethalModDataLib.Attributes;
using LethalNetworkAPI;
using LethalNetworkAPI.Utils;
using LethalUpgrades.Patches;
using LethalUpgrades.Store;




namespace LethalUpgrades;
[BepInPlugin(modGUID, modName, modVersion)]
[BepInDependency("MaxWasUnavailable.LethalModDataLib")]
[BepInDependency("atomic.terminalapi")]
[BepInDependency("LethalNetworkAPI")]
// [BepInDependency("OdinSerializer")] // BepInEx/core

#region LethalUpgradesBase Class
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
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool utility_t1 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool utility_t2 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool utility_t3 = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool utility_leg = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static bool reroll = false;
    [ModData(LethalModDataLib.Enums.SaveWhen.OnSave, LethalModDataLib.Enums.LoadWhen.OnLoad, LethalModDataLib.Enums.SaveLocation.CurrentSave)]
    public static int pre_scum_scredits = 0;
    public static bool show_explosion = true;
    public static bool shovel_jump = false;

    public static Terminal ActiveTerminal()
    {
        Terminal[] terminals = UnityEngine.Object.FindObjectsByType<Terminal>(UnityEngine.FindObjectsSortMode.None);
        foreach(Terminal terminal in terminals)
        {
            if(terminal.terminalInUse)
            {
                mls.LogInfo("Found active terminal!");
                return terminal;
            }
        }
        mls.LogInfo("Did not find active terminal");
        return null;
    }
    public static void SyncTerminals(int remainingCredits)
    {
        Terminal[] all_terminals = UnityEngine.Object.FindObjectsByType<Terminal>(UnityEngine.FindObjectsSortMode.None);
        foreach(Terminal terminal in all_terminals)
        {
            mls.LogInfo($"Syncing credits for terminal {terminal.currentNode} which is active? -> {terminal.terminalInUse}");
            if(LNetworkUtils.IsHostOrServer)
            {
                pre_scum_scredits = remainingCredits;
                mls.LogInfo($"Pre-scum credits: {pre_scum_scredits}");
                terminal.SyncGroupCreditsServerRpc(remainingCredits, terminal.numberOfItemsInDropship);
            }
            else if(!LNetworkUtils.IsHostOrServer)
            {
                LethalUpgradesNetwork.client_credits.Value = remainingCredits;
            }
        }
    }

    void Awake()
    {
        show_explosion = true;
        shovel_jump = false;

        if (Instance == null)
        {
            Instance = this;
        }

        mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
        mls.LogInfo("LethalUpgrades at your service!");
        harmony.PatchAll(typeof(HealthPatching));
        harmony.PatchAll(typeof(StaminaPatching));
        harmony.PatchAll(typeof(MovementPatching));
        // harmony.PatchAll(typeof(DebugPatching)); //Uncomment to have logs in BepInEx console
        harmony.PatchAll(typeof(TokenPatching));
        harmony.PatchAll(typeof(HostClientPatching));
        harmony.PatchAll(typeof(UtilityPatching));
        harmony.PatchAll(typeof(RotationalStore));
        ConfigManager = new ConfigurationController(Config);

        LethalUpgradesNetwork.Initiate();
        LethalUpgradesNetwork.InitializeNetworkCallbacks();
        RotationalStore.StoreSetup();

        AddCommand("upgrade", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                var text = "Thank you for joining the *LETHAL UPGRADES* program.\n" +
                            "In exchange for credits, we can provide upgrades that should boost your quota-reaching efficiency!.\n" +
                            "We currently provide the following types of [UPGRADES]:\n" +
                            "- HEALTH\n" +
                            "- STAMINA\n"+
                            "- MOVEMENT\n" +
                            "- UTILITY\n\n" +

                            "Each category has 3 tiers with differing costs, providing a plethora of different changes.\n" +
                            "A special token can be acquired by proving your loot-gathering and survival skills, which can be turned in for free legendary upgrades!\n\n" +

                            "NOTE: Upgrades that cost money scale with amount of players present.\n\n" +
                            "To see information about each upgrade, type 'upgrade [UPGRADE] info'\n\n" +
                            "To buy an upgrade, type 'upgrade [UPGRADE] [TIER]'\n\n" +
                            "To learn how to get tokens, type 'upgrade token'\n\n" +
                            "To buy a legendary upgrade with a token, type 'upgrade token [UPGRADE]\n\n" +
                            "To see all available terminal commands from this mod, type and go to 'Other'\n";
                return text;
            }, Category = "Help",
            Description = "Brief overview of upgrades employees can acquire."
        });

        AddCommand("upgrade token",
        "Special tokens, denoted by Ŧ, are utilized to obtain legendary upgrades from each tier. " +
        "Unlike normal upgrades, you can use a token to buy any of the available legendary ones.\n\n" +
        "These tokens can only be acquired via good, cummulative performances on moons. Good luck!\n");

        AddCommand("upgrade health info",
        "These upgrades affect your health. They consist of the following:\n" +
        "- Tier 1: Gain +20 additional health. Cost: $200\n" +
        "- Tier 2: Reduce all incoming damage by 10%. Cost: $300\n" +
        "- Tier 3: Gain +30 additional health. Cost: $400\n" +
        "- Legendary: Gain an adaptive regeneration ability.\n\n" +
        "NOTE: Health-increasing upgrades only apply while in orbit.\n");

        AddCommand("upgrade stamina info",
        "These upgrades affect your stamina. They consist of the following:\n" +
        "- Tier 1: Decrease running stamina usage. Cost: $300\n" +
        "- Tier 2: Improve stamina regen by 10%. Cost: $400\n" +
        "- Tier 3: Reduced stamina penalties when heavy (>=50 lbs). Cost: $500\n" +
        "- Legendary: When damaged, regardless of amount or source, gain full stamina back.\n");

        AddCommand("upgrade movement info",
        "These upgrades affect your movement. They consist of the following:\n" +
        "- Tier 1: Sprint 6% faster. Cost: $250\n" +
        "- Tier 2: Walk/Crouch 10% faster. Cost: $350\n" +
        "- Tier 3: Jump height increased by 25%. Cost: $400\n" +
        "- Legendary: While critically injured, always gain more movement speed. Become intangible as well, but goes on cooldown for 120 seconds.\n");

        AddCommand("upgrade utility info",
        "These upgrades affect your equipment or utilities. They consist of the following:\n" +
        "- Tier 1: Increase flashlight battery capacities by 10%. Cost: $250\n" +
        "- Tier 2: Shovel deals double damage, visually explodes and allows you to shovel jump. Cost: $350\n" + //Done
        "   + By default, explosion does no damage.\n" +
        "   + Only shovel deals damage.\n" +
        "   + To turn explosion visual on or off, type 'shovel explosion' in the terminal.\n" +
        "   + To turn shovel jump on or off, type 'shovel jump' in the terminal.\n" +
        "   + Shovel jumping costs 2 hp and explosion must also be on.\n" +
        "- Tier 3: All equipment weighs 0 pounds. Cost: $400\n" +
        "- Legendary: Unlock 1 weather re-roll for the moon you orbit. Resets after going back to orbit.\n");

        // AddCommand("give money hehe", new CommandInfo()
        // {
        //     DisplayTextSupplier = () =>
        //     {
        //         var terminal = ActiveTerminal();
        //         var new_credits = terminal.groupCredits += 500;
        //         SyncTerminals(new_credits);
        //         return "Gave you 500 moneys for testing.\n";
        //     },
        // });

        // AddCommand("give token", new CommandInfo()
        // {
        //     DisplayTextSupplier = () =>
        //     {
        //         LethalUpgradesNetwork.tokens.Value += 1;
        //         return "Filled up half your token meter.\n";
        //     }, Category = "Other"
        // });

        AddCommand("shovel explosion", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                show_explosion = !show_explosion;
                shovel_jump = false;

                return $"Shovel Explosion set to: {show_explosion}.\n";
            }, Category = "Other"
        });

        AddCommand("shovel jump", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(!show_explosion)
                {
                    return $"You need to turn on shovel explosion before this!.\n";
                }

                shovel_jump = !shovel_jump;

                return $"Shovel Jump set to: {shovel_jump}.\n";
            }, Category = "Other"
        });

        AddCommand("upgrade list", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                string returner = "Upgrade List\n\n";

                var upgrades = new Dictionary<string, bool>
                {
                    {"Health Tier 1", health_t1},
                    {"Health Tier 2", health_t2},
                    {"Health Tier 3", health_t3},
                    {"Health Legendary", health_leg},
                    {"Stamina Tier 1", stamina_t1},
                    {"Stamina Tier 2", stamina_t2},
                    {"Stamina Tier 3", stamina_t3},
                    {"Stamina Legendary", stamina_leg},
                    {"Movement Tier 1", movement_t1},
                    {"Movement Tier 2", movement_t2},
                    {"Movement Tier 3", movement_t3},
                    {"Movement Legendary", movement_leg},
                    {"Utility Tier 1", utility_t1},
                    {"Utility Tier 2", utility_t2},
                    {"Utility Tier 3", utility_t3},
                    {"Utility Legendary", utility_leg},
                };

                foreach(var (key, value) in upgrades)
                {
                    returner += $"{key} is set to: {value}\n";
                }

                return returner;
            }, Category = "Other",
            Description = "Lists all upgrades and which ones you have so far."
        });

        #region Legendaries
        AddCommand("upgrade token health", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(health_leg)
                {
                    return "You already have the legendary health upgrade!\n";
                }

                if(tokens <= 0)
                {
                    return "You need a token to buy the legendary health upgrade.\n";
                }

                tokens -= 1;
                LethalUpgradesNetwork.tokens.Value = tokens;
                LethalUpgradesNetwork.health_leg.Value = true;
                health_leg = true;

                return "Acquired legendary health upgrade!\n";
            }, Category = "Other"
        });

        AddCommand("upgrade token stamina", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(stamina_leg)
                {
                    return "You already have the legendary stamina upgrade!\n";
                }

                if(tokens <= 0)
                {
                    return "You need a token to buy the legendary stamina upgrade.\n";
                }

                tokens -= 1;
                LethalUpgradesNetwork.tokens.Value = tokens;
                LethalUpgradesNetwork.stamina_leg.Value = true;
                stamina_leg = true;

                return "Acquired legendary stamina upgrade!\n";
            }, Category = "Other"
        });

        AddCommand("upgrade token utility", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(utility_leg)
                {
                    return "You already have the legendary utility upgrade!\n";
                }

                if(tokens <= 0)
                {
                    return "You need a token to buy the legendary utility upgrade.\n";
                }

                tokens -= 1;
                LethalUpgradesNetwork.tokens.Value = tokens;
                LethalUpgradesNetwork.utility_leg.Value = true;
                utility_leg = true;
                LethalUpgradesNetwork.reroll.Value = true;
                reroll = true;

                return "Acquired legendary utility upgrade!\nRe-rolls are active.\n";
            }, Category = "Other"
        });

        AddCommand("upgrade token movement", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(movement_leg)
                {
                    return "You already have the legendary movement upgrade!\n";
                }

                if(tokens <= 0)
                {
                    return "You need a token to buy the legendary movement upgrade.\n";
                }

                tokens -= 1;
                LethalUpgradesNetwork.tokens.Value = tokens;
                LethalUpgradesNetwork.movement_leg.Value = true;
                movement_leg = true;

                return "Acquired legendary movement upgrade!\n";
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
                var terminal = ActiveTerminal();
                if (terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.health_t1.Value = true;
                health_t1 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade health 2", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if (!health_t1)
                {
                    return "You require the tier 1 health upgrade before this!\n";
                }
                if(health_t2)
                {
                    return "You already have this upgrade!\n";
                }

                var cost = 300;
                var terminal = ActiveTerminal();
                if(terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.health_t2.Value = true;
                health_t2 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade health 3", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if (!health_t2)
                {
                    return "You require the tier 2 health upgrade before this!\n";
                }
                if(health_t3)
                {
                    return "You already have this upgrade!\n";
                }

                var cost = 400;
                var terminal = ActiveTerminal();
                if(terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.health_t3.Value = true;
                health_t3 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
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
                var terminal = ActiveTerminal();
                if (terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.stamina_t1.Value = true;
                stamina_t1 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade stamina 2", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if (!stamina_t1)
                {
                    return "You require the tier 1 stamina upgrade before this!\n";
                }
                if(stamina_t2)
                {
                    return "You already have this upgrade!\n";
                }

                var cost = 400;
                var terminal = ActiveTerminal();
                if(terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.stamina_t2.Value = true;
                stamina_t2 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade stamina 3", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if (!stamina_t2)
                {
                    return "You require the tier 2 stamina upgrade before this!\n";
                }
                if(stamina_t3)
                {
                    return "You already have this upgrade!\n";
                }

                var cost = 500;
                var terminal = ActiveTerminal();
                if(terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.stamina_t3.Value = true;
                stamina_t3 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
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

                var cost = 250;
                var terminal = ActiveTerminal();
                if (terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.movement_t1.Value = true;
                movement_t1 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade movement 2", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if (!movement_t1)
                {
                    return "You require the tier 1 movement upgrade before this!\n";
                }
                if(movement_t2)
                {
                    return "You already have this upgrade!\n";
                }

                var cost = 350;
                var terminal = ActiveTerminal();
                if(terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.movement_t2.Value = true;
                movement_t2 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade movement 3", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if (!movement_t2)
                {
                    return "You require the tier 2 movement upgrade before this!\n";
                }
                if(movement_t3)
                {
                    return "You already have this upgrade!\n";
                }

                var cost = 400;
                var terminal = ActiveTerminal();
                if(terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.movement_t3.Value = true;
                movement_t3 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
            }, Category = "Other"
        });
        #endregion

        #region Utility Upgrades
        AddCommand("upgrade utility 1", new CommandInfo()
        {

            DisplayTextSupplier = () =>
            {
                if(utility_t1)
                {
                    return "You already have this upgrade!\n";
                }

                var cost = 250;
                var terminal = ActiveTerminal();
                if (terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.utility_t1.Value = true;
                utility_t1 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade utility 2", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if (!utility_t1)
                {
                    return "You require the tier 1 utility upgrade before this!\n";
                }
                if(utility_t2)
                {
                    return "You already have this upgrade!\n";
                }

                var cost = 350;
                var terminal = ActiveTerminal();
                if(terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.utility_t2.Value = true;
                utility_t2 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
            }, Category = "Other"
        });

        AddCommand("upgrade utility 3", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if (!utility_t2)
                {
                    return "You require the tier 2 utility upgrade before this!\n";
                }
                if(utility_t3)
                {
                    return "You already have this upgrade!\n";
                }

                var cost = 400;
                var terminal = ActiveTerminal();
                if(terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.utility_t3.Value = true;
                utility_t3 = true;

                return $"Upgrade acquired. New balance of ${remainingCredits}\n";
            }, Category = "Other"
        });

        LevelWeatherType[] weather_names = [
            LevelWeatherType.None,
            LevelWeatherType.DustClouds,
            LevelWeatherType.Rainy,
            LevelWeatherType.Stormy,
            LevelWeatherType.Foggy,
            LevelWeatherType.Flooded,
            LevelWeatherType.Eclipsed
        ];

        AddCommand("weather reroll", new CommandInfo()
        {
            DisplayTextSupplier = () =>
            {
                if(!utility_leg)
                {
                    return "You require the legendary utility upgrade to use this!\n";
                }
                if(!reroll)
                {
                    return "You have no re-rolls left!\n";
                }

                var sor = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
                var moon = sor.currentLevel;
                var old_weather = moon.currentWeather;

                LethalUpgradesNetwork.reroll.Value = false;
                reroll = false;

                Random rand = new Random();
                int random_index = rand.Next(weather_names.Length);
                LevelWeatherType new_weather = weather_names[random_index];
                moon.currentWeather = new_weather;

                var terminal = ActiveTerminal();
                var credits = terminal.groupCredits;
                sor.ChangeLevelServerRpc(moon.levelID, newGroupCreditsAmount: credits);

                MoonWeather weather_credits = new MoonWeather();
                weather_credits.new_weather = new_weather;
                weather_credits.credits = credits;
                if(LNetworkUtils.IsHostOrServer)
                {
                    LethalUpgradesNetwork.current_weather.SendClients(weather_credits);
                }
                else
                {
                    LethalUpgradesNetwork.current_weather.SendClients(weather_credits);
                    LethalUpgradesNetwork.current_weather.SendServer(weather_credits);
                }

                return $"Re-rolled weather. What did you get in {moon.PlanetName}? :)\n";
            }, Category = "Other",
            Description = "Re-roll the weather of the moon you are orbiting to a random one! Can be done once per orbit."
        });

        #endregion
    }
}
#endregion

public class MoonWeather
{
    public LevelWeatherType new_weather;
    public int credits;

    public MoonWeather()
    {
        new_weather = LevelWeatherType.None;
        credits = 0;
    }
}

#region Lethal Upgrades Network
public class LethalUpgradesNetwork
{
    public static LNetworkEvent syncer;
    public static LNetworkMessage<MoonWeather> current_weather;
    public static LNetworkVariable<bool> health_t1;
    public static LNetworkVariable<bool> health_t2;
    public static LNetworkVariable<bool> health_t3;
    public static LNetworkVariable<bool> health_leg;
    public static LNetworkVariable<bool> stamina_t1;
    public static LNetworkVariable<bool> stamina_t2;
    public static LNetworkVariable<bool> stamina_t3;
    public static LNetworkVariable<bool> stamina_leg;
    public static LNetworkVariable<bool> movement_t1;
    public static LNetworkVariable<bool> movement_t2;
    public static LNetworkVariable<bool> movement_t3;
    public static LNetworkVariable<bool> movement_leg;
    public static LNetworkVariable<bool> utility_t1;
    public static LNetworkVariable<bool> utility_t2;
    public static LNetworkVariable<bool> utility_t3;
    public static LNetworkVariable<bool> utility_leg;
    public static LNetworkVariable<bool> reroll;
    public static LNetworkVariable<int> tokens;
    public static LNetworkVariable<int> token_meter;
    public static LNetworkVariable<int> client_credits;
    public static LNetworkVariable<UnityEngine.Vector3> shovel_explosion_pos;
    public static bool syncing = false;

    // Rotational Store Network Variables
    public static LNetworkVariable<bool> mod1;
    public static LNetworkVariable<bool> mod2;
    public static LNetworkVariable<bool> mod3;
    public static LNetworkVariable<bool> mod4;
    public static LNetworkVariable<bool> mod5;
    public static LNetworkVariable<bool> mod6;
    public static LNetworkVariable<bool> mod7;
    public static LNetworkVariable<bool> mod8;


    public static LNetworkVariable<(int, int, int)> easy_indexes;
    public static LNetworkVariable<(int, int)> medium_indexes;
    public static LNetworkVariable<int> hard_index;

    public static void Initiate()
    {
        syncer = LNetworkEvent.Connect("ChuitosLethalUpgrades_syncer", onServerReceived: OnClientJoinedRequest);
        current_weather = LNetworkMessage<MoonWeather>.Connect("ChuitosLethalUpgrades_current_weather", onServerReceived: HostReceivesWeather, onClientReceived: ClientReceivesWeather, onClientReceivedFromClient: ClientReceivesWeatherFromClient);
        shovel_explosion_pos = LNetworkVariable<UnityEngine.Vector3>.Connect("ChuitosLethalUpgrades_shovel_explosion_pos", UnityEngine.Vector3.zero);
        health_t1 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_health_t1", false);
        health_t2 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_health_t2", false);
        health_t3 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_health_t3", false);
        health_leg = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_health_leg", false);
        stamina_t1 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_stamina_t1", false);
        stamina_t2 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_stamina_t2", false);
        stamina_t3 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_stamina_t3", false);
        stamina_leg = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_stamina_leg", false);
        movement_t1 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_movement_t1", false);
        movement_t2 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_movement_t2", false);
        movement_t3 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_movement_t3", false);
        movement_leg = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_movement_leg", false);
        utility_t1 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_utility_t1", false);
        utility_t2 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_utility_t2", false);
        utility_t3 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_utility_t3", false);
        utility_leg = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_utility_leg", false);
        reroll = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_reroll", false);
        tokens = LNetworkVariable<int>.Connect("ChuitosLethalUpgrades_tokens", 0);
        token_meter = LNetworkVariable<int>.Connect("ChuitosLethalUpgrades_token_meter", 0);
        client_credits = LNetworkVariable<int>.Connect("ChuitosLethalUpgrades_client_credits", 0);

        easy_indexes = LNetworkVariable<(int, int, int)>.Connect("ChuitosLethalUpgrades_easy_indexes", RotationalStore.easy_indexes);
        medium_indexes = LNetworkVariable<(int, int)>.Connect("ChuitosLethalUpgrades_medium_indexes", RotationalStore.medium_indexes);
        hard_index = LNetworkVariable<int>.Connect("ChuitosLethalUpgrades_hard_index", RotationalStore.hard_index);

        mod1 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_mod1", false);
        mod2 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_mod2", false);
        mod3 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_mod3", false);
        mod4 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_mod4", false);
        mod5 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_mod5", false);
        mod6 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_mod6", false);
        mod7 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_mod7", false);
        mod8 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_mod8", false);
    }

    private static void OnClientJoinedRequest(ulong clientId)
    {
        LethalUpgradesBase.mls.LogInfo($"Client {clientId} requested sync, sending current upgrade states...");

        bool[] local_bool_arr = [
            LethalUpgradesBase.health_t1,
            LethalUpgradesBase.health_t2,
            LethalUpgradesBase.health_t3,
            LethalUpgradesBase.health_leg,
            LethalUpgradesBase.stamina_t1,
            LethalUpgradesBase.stamina_t2,
            LethalUpgradesBase.stamina_t3,
            LethalUpgradesBase.stamina_leg,
            LethalUpgradesBase.movement_t1,
            LethalUpgradesBase.movement_t2,
            LethalUpgradesBase.movement_t3,
            LethalUpgradesBase.movement_leg,
            LethalUpgradesBase.utility_t1,
            LethalUpgradesBase.utility_t2,
            LethalUpgradesBase.utility_t3,
            LethalUpgradesBase.utility_leg,
            LethalUpgradesBase.reroll,
        ];

        LNetworkVariable<bool>[] net_bool_arr = [
            LethalUpgradesNetwork.health_t1,
            LethalUpgradesNetwork.health_t2,
            LethalUpgradesNetwork.health_t3,
            LethalUpgradesNetwork.health_leg,
            LethalUpgradesNetwork.stamina_t1,
            LethalUpgradesNetwork.stamina_t2,
            LethalUpgradesNetwork.stamina_t3,
            LethalUpgradesNetwork.stamina_leg,
            LethalUpgradesNetwork.movement_t1,
            LethalUpgradesNetwork.movement_t2,
            LethalUpgradesNetwork.movement_t3,
            LethalUpgradesNetwork.movement_leg,
            LethalUpgradesNetwork.utility_t1,
            LethalUpgradesNetwork.utility_t2,
            LethalUpgradesNetwork.utility_t3,
            LethalUpgradesNetwork.utility_leg,
            LethalUpgradesNetwork.reroll
        ];

        for(int i = 0; i<local_bool_arr.Length; i++)
        {
            if(local_bool_arr[i])
            {
                if(net_bool_arr[i].Value != true)
                {
                    net_bool_arr[i].Value = true;
                }
                else
                {
                    net_bool_arr[i].Value = false;
                    net_bool_arr[i].Value = true;
                }
            }
        }

        if(LethalUpgradesBase.tokens > 0)
        {
            var old_tokens = tokens.Value;
            tokens.Value = -1;
            tokens.Value = old_tokens;
        }

        var old_easy_indexes = RotationalStore.easy_indexes;
        easy_indexes.Value = (-2, -2, -2);
        easy_indexes.Value = old_easy_indexes;

        var old_medium_indexes = RotationalStore.medium_indexes;
        medium_indexes.Value = (-2, -2);
        medium_indexes.Value = old_medium_indexes;

        var old_hard_index = RotationalStore.hard_index;
        hard_index.Value = -2;
        hard_index.Value = old_hard_index;

        foreach(Modifier mod in RotationalStore.easy_mods)
        {
            // Easy Mods IDs: 1, 2, 3
            if(mod.mod_id == 1)
            {
                if(mod.active)
                {
                    if(mod1.Value != true)
                    {
                        mod1.Value = true;
                    }
                    else
                    {
                        mod1.Value = false;
                        mod1.Value = true;
                    }
                }
            }
            else if(mod.mod_id == 2)
            {
                if(mod.active)
                {
                    if(mod2.Value != true)
                    {
                        mod2.Value = true;
                    }
                    else
                    {
                        mod2.Value = false;
                        mod2.Value = true;
                    }
                }
            }
            else if(mod.mod_id == 3)
            {
                if(mod.active)
                {
                    if(mod3.Value != true)
                    {
                        mod3.Value = true;
                    }
                    else
                    {
                        mod3.Value = false;
                        mod3.Value = true;
                    }
                }
            }
        }
        

        foreach(Modifier mod in RotationalStore.medium_mods)
        {
            // Medium Mods IDs: 4, 5, 8
            if(mod.mod_id == 4)
            {
                if(mod.active)
                {
                    if(mod4.Value != true)
                    {
                        mod4.Value = true;
                    }
                    else
                    {
                        mod4.Value = false;
                        mod4.Value = true;
                    }
                }
            }
            else if(mod.mod_id == 5)
            {
                if(mod.active)
                {
                    if(mod5.Value != true)
                    {
                        mod5.Value = true;
                    }
                    else
                    {
                        mod5.Value = false;
                        mod5.Value = true;
                    }
                }
            }
            else if(mod.mod_id == 8)
            {
                if(mod.active)
                {
                    if(mod8.Value != true)
                    {
                        mod8.Value = true;
                    }
                    else
                    {
                        mod8.Value = false;
                        mod8.Value = true;
                    }
                }
            }
        }

        foreach(Modifier mod in RotationalStore.hard_mods)
        {
            // Hard Mods IDs: 6, 7
            if(mod.mod_id == 6)
            {
                if(mod.active)
                {
                    if(mod6.Value != true)
                    {
                        mod6.Value = true;
                    }
                    else
                    {
                        mod6.Value = false;
                        mod6.Value = true;
                    }
                }
            }
            else if(mod.mod_id == 7)
            {
                if(mod.active)
                {
                    if(mod7.Value != true)
                    {
                        mod7.Value = true;
                    }
                    else
                    {
                        mod7.Value = false;
                        mod7.Value = true;
                    }
                }
            }
        }

        LethalUpgradesBase.mls.LogInfo($"Sync sent for client {clientId} - Tokens: {LethalUpgradesBase.tokens}");
    }

    public static void HostReceivesWeather(MoonWeather weatherCredits, ulong clientId)
    {
        var sor = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
        var moon = sor.currentLevel;
        moon.currentWeather = weatherCredits.new_weather;

        sor.ChangeLevelServerRpc(moon.levelID, newGroupCreditsAmount: weatherCredits.credits);
    }

    public static void ClientReceivesWeather(MoonWeather weatherCredits)
    {
        var sor = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
        var moon = sor.currentLevel;
        moon.currentWeather = weatherCredits.new_weather;

        sor.ChangeLevelServerRpc(moon.levelID, newGroupCreditsAmount: weatherCredits.credits);
    }

    public static void ClientReceivesWeatherFromClient(MoonWeather weatherCredits, ulong clientId)
    {
        var sor = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
        var moon = sor.currentLevel;
        moon.currentWeather = weatherCredits.new_weather;

        sor.ChangeLevelServerRpc(moon.levelID, newGroupCreditsAmount: weatherCredits.credits);
    }

    public static void InitializeNetworkCallbacks()
    {
        if(!syncing)
        {
            health_t1.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.health_t1 = newValue;
                LethalUpgradesBase.mls.LogInfo($"health_t1 synced to: {newValue}");
            };

            health_t2.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.health_t2 = newValue;
                LethalUpgradesBase.mls.LogInfo($"health_t2 synced to: {newValue}");
            };

            health_t3.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.health_t3 = newValue;
                LethalUpgradesBase.mls.LogInfo($"health_t3 synced to: {newValue}");
            };

            health_leg.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.health_leg = newValue;
                LethalUpgradesBase.mls.LogInfo($"health_leg synced to: {newValue}");
            };

            stamina_t1.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.stamina_t1 = newValue;
                LethalUpgradesBase.mls.LogInfo($"stamina_t1 synced to: {newValue}");
            };

            stamina_t2.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.stamina_t2 = newValue;
                LethalUpgradesBase.mls.LogInfo($"stamina_t2 synced to: {newValue}");
            };

            stamina_t3.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.stamina_t3 = newValue;
                LethalUpgradesBase.mls.LogInfo($"stamina_t3 synced to: {newValue}");
            };

            stamina_leg.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.stamina_leg = newValue;
                LethalUpgradesBase.mls.LogInfo($"stamina_leg synced to: {newValue}");
            };

            movement_t1.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.movement_t1 = newValue;
                LethalUpgradesBase.mls.LogInfo($"movement_t1 synced to: {newValue}");
            };

            movement_t2.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.movement_t2 = newValue;
                LethalUpgradesBase.mls.LogInfo($"movement_t2 synced to: {newValue}");
            };

            movement_t3.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.movement_t3 = newValue;
                LethalUpgradesBase.mls.LogInfo($"movement_t3 synced to: {newValue}");
            };

            movement_leg.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.movement_leg = newValue;
                LethalUpgradesBase.mls.LogInfo($"movement_leg synced to: {newValue}");
            };

            utility_t1.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.utility_t1 = newValue;
                LethalUpgradesBase.mls.LogInfo($"utility_t1 synced to: {newValue}");
            };

            utility_t2.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.utility_t2 = newValue;
                LethalUpgradesBase.mls.LogInfo($"utility_t2 synced to: {newValue}");
            };

            utility_t3.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.utility_t3 = newValue;
                LethalUpgradesBase.mls.LogInfo($"utility_t3 synced to: {newValue}");
            };

            utility_leg.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.utility_leg = newValue;
                LethalUpgradesBase.mls.LogInfo($"utility_leg synced to: {newValue}");
            };

            tokens.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.tokens = newValue;
                LethalUpgradesBase.mls.LogInfo($"tokens synced to: {newValue}");
            }; // Callback not needed for token_meter. Handled locally.

            reroll.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.reroll = newValue;
                LethalUpgradesBase.mls.LogInfo($"rerollsynced to: {newValue}");
            };

            client_credits.OnValueChanged += async (oldValue, newValue) =>
            {
                LethalUpgradesBase.mls.LogInfo($"Original Client Credits: {oldValue}");
                LethalUpgradesBase.mls.LogInfo($"New Client Credits: {newValue}");
                if(LNetworkUtils.IsHostOrServer)
                {
                    LethalUpgradesBase.pre_scum_scredits = newValue;
                    LethalUpgradesBase.mls.LogInfo($"Pre-scum credits: {LethalUpgradesBase.pre_scum_scredits}");
                    await Task.Delay(1500);
                    LethalUpgradesBase.SyncTerminals(client_credits.Value);
                }
            };

            shovel_explosion_pos.OnValueChanged += (oldValue, newValue) =>
            {
                var sor = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
                if(sor.inShipPhase) return;
                // if(sor.shipIsLeaving) return;

                if(LethalUpgradesBase.show_explosion)
                {
                    LethalUpgradesBase.mls.LogInfo("Spawning explosion...");
                    if(LethalUpgradesBase.shovel_jump)
                    {
                        Landmine.SpawnExplosion(newValue, true, 0, 10, 2, 20);
                    }
                    else
                    {
                        Landmine.SpawnExplosion(newValue, true, 0, 0, 0, 0);
                    }
                    LethalUpgradesBase.mls.LogInfo("Explosion spawned!");
                }
            };

            easy_indexes.OnValueChanged += (OldValue, newValue) =>
            {
              RotationalStore.easy_indexes = newValue;
            };

            medium_indexes.OnValueChanged += (OldValue, newValue) =>
            {
              RotationalStore.medium_indexes = newValue;
            };

            hard_index.OnValueChanged += (OldValue, newValue) =>
            {
              RotationalStore.hard_index = newValue;
            };

            mod1.OnValueChanged += (OldValue, newValue) =>
            {
                RotationalStore.easy_mods[0].active = newValue;
                RotationalStore.EasyModCallbacks(RotationalStore.easy_mods[0]);
                LethalUpgradesBase.mls.LogInfo($"Activated mod 1 with new value of {newValue}");
            };

            mod2.OnValueChanged += (OldValue, newValue) =>
            {
                RotationalStore.easy_mods[1].active = newValue;
                RotationalStore.EasyModCallbacks(RotationalStore.easy_mods[1]);
                LethalUpgradesBase.mls.LogInfo($"Activated mod 2 with new value of {newValue}");
            };

            mod3.OnValueChanged += (OldValue, newValue) =>
            {
                RotationalStore.easy_mods[2].active = newValue;
                RotationalStore.EasyModCallbacks(RotationalStore.easy_mods[2]);
                LethalUpgradesBase.mls.LogInfo($"Activated mod 3 with new value of {newValue}");
            };

            mod4.OnValueChanged += (OldValue, newValue) =>
            {
                RotationalStore.medium_mods[0].active = newValue;
                RotationalStore.MediumModCallbacks(RotationalStore.medium_mods[0]);
                LethalUpgradesBase.mls.LogInfo($"Activated mod 4 with new value of {newValue}");
            };

            mod5.OnValueChanged += (OldValue, newValue) =>
            {
                RotationalStore.medium_mods[1].active = newValue;
                RotationalStore.MediumModCallbacks(RotationalStore.medium_mods[1]);
                LethalUpgradesBase.mls.LogInfo($"Activated mod 5 with new value of {newValue}");
            };

            mod8.OnValueChanged += (OldValue, newValue) =>
            {
                RotationalStore.medium_mods[2].active = newValue;
                RotationalStore.MediumModCallbacks(RotationalStore.medium_mods[2]);
                LethalUpgradesBase.mls.LogInfo($"Activated mod 8 with new value of {newValue}");
            };

            mod6.OnValueChanged += (OldValue, newValue) =>
            {
                RotationalStore.hard_mods[0].active = newValue;
                // Host prioritizes spawns, but clients can still see power level
                RotationalStore.HardModCallbacks(RotationalStore.hard_mods[0]);
            };

            mod7.OnValueChanged += (OldValue, newValue) =>
            {
                RotationalStore.hard_mods[1].active = newValue;
                // Host prioritizes spawns, but clients can still see power level
                RotationalStore.HardModCallbacks(RotationalStore.hard_mods[1]);
            };
        }
    }
}
#endregion

#region MULTIPLAYER ISSUES
/// ISSUE #1:
/// IF EVERYONE IS CONNECTED AT THE SAME TIME, SYNC FINE
/// IF UPGRADE IS BOUGHT AND SOMEONE JOINS LATER, ITS DESYNCED
/// FIND A WAY TO FIX THIS. TRY AND ALWAYS MAKE NEW PEOPLE SYNC TO HOST WHEN THEY JOIN.
/// HOST ALWAYS HAS UPDATED VALUES.
///
/// STATUS: FIXED
/// ----------------------------------------------
/// ISSUE #2:
/// TERMINAL CREDITS CAN DESYNC WHEN ANY PLAYER BUYS UPGRADE
///
/// STATUS: FIXED
#endregion

#region BUG ISSUES
/// BUG #1:
/// IF YOU BUY AN UPGRADE BEFORE OR WHEN LANDED ON A MOON
/// AND YOU SAVE SCUM, YOU KEEP UPGRADES BUT TERMINAL MONEY RESETS
/// PRIOR TO HAVING THEM.
///
/// STATUS: FIXED (?)
#endregion