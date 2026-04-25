using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LethalUpgrades.Patches;
using TerminalApi.Classes;
using static TerminalApi.TerminalApi;
using LethalModDataLib.Attributes;
using LethalNetworkAPI;
using System.ComponentModel;
using LethalNetworkAPI.Utils;



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
        // harmony.PatchAll(typeof(DebugPatching)); //Uncomment to have logs in BepInEx console
        harmony.PatchAll(typeof(TokenPatching));
        harmony.PatchAll(typeof(HostClientPatching));
        ConfigManager = new ConfigurationController(Config);

        LethalUpgradesNetwork.Initiate();
        LethalUpgradesNetwork.InitializeNetworkCallbacks();

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
                            "- UTILITIES\n\n" +

                            "Each category has 3 tiers with differing costs, providing a plethora of different changes.\n" +
                            "A special token can be acquired by proving your loot-gathering and survival skills, which can be turned in for free legendary upgrades!\n\n" +
                            
                            "NOTE: Upgrades that cost money scale with amount of players present.\n" +
                            "To see information about each upgrade, type 'upgrade [UPGRADE] info'\n" +
                            "To buy an upgrade, type 'upgrade [UPGRADE] [TIER]'\n" +
                            "To learn how to get tokens, type 'upgrade token'\n" +
                            "To buy a legendary upgrade with a token, type 'upgrade token [UPGRADE]\n";
                return text;
            }, Category = "Other"
        });    

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
        "- Tier 1: Sprint 6% faster. Cost: $200\n" + //Done
        "- Tier 2: Walk/Crouch 12% faster. Cost: $300\n" + //Done
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
                var terminal = ActiveTerminal();
                var new_credits = terminal.groupCredits += 500;
                SyncTerminals(new_credits);
                return "Gave you 500 moneys for testing.\n";
            }, Category = "Other"
        });

        // AddCommand("give meter", new CommandInfo()
        // {
        //     DisplayTextSupplier = () =>
        //     {
        //         token_meter += 50;
        //         return "Filled up half your token meter.\n";
        //     }, Category = "Other"
        // });

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

                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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

                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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
                var terminal = ActiveTerminal();
                if (terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.stamina_t1.Value = true;
                stamina_t1 = true;

                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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

                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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

                var cost = 300;
                var terminal = ActiveTerminal();
                if (terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.movement_t1.Value = true;
                movement_t1 = true;

                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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

                var cost = 400;
                var terminal = ActiveTerminal();
                if(terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.movement_t2.Value = true;
                movement_t2 = true;

                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
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

                var cost = 500;
                var terminal = ActiveTerminal();
                if(terminal.groupCredits < cost)
                {
                    return $"Not enough credits for this upgrade. You need ${cost}\n";
                }
                var remainingCredits = terminal.groupCredits - cost;
                SyncTerminals(remainingCredits: remainingCredits);

                LethalUpgradesNetwork.movement_t3.Value = true;
                movement_t3 = true;

                return $"Upgrade acquired. New balance of ${terminal.groupCredits}\n";
            }, Category = "Other"
        });
        #endregion

        #region Utility Upgrades
        #endregion
    }
}
#endregion

#region Lethal Upgrades Network
public class LethalUpgradesNetwork
{
    public static LNetworkEvent syncer;
    // public static LNetworkEvent credit_syncer;
    public static LNetworkVariable<bool> health_t1;
    public static LNetworkVariable<bool> health_t2;
    public static LNetworkVariable<bool> health_t3;
    public static LNetworkVariable<bool> stamina_t1;
    public static LNetworkVariable<bool> stamina_t2;
    public static LNetworkVariable<bool> stamina_t3;
    public static LNetworkVariable<bool> movement_t1;
    public static LNetworkVariable<bool> movement_t2;
    public static LNetworkVariable<bool> movement_t3;
    public static LNetworkVariable<int> tokens;
    public static LNetworkVariable<int> client_credits;
    public static bool syncing = false;


    public static void Initiate()
    {
        syncer = LNetworkEvent.Connect("ChuitosLethalUpgrades_syncer", onServerReceived: OnClientJoinedRequest);
        // credit_syncer = LNetworkEvent.Connect("ChuitosLethalUpgrades_credit_syncer", onServerReceived: OnClientCreditSyncRequest);
        health_t1 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_health_t1", false);
        health_t2 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_health_t2", false);
        health_t3 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_health_t3", false);
        stamina_t1 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_stamina_t1", false);
        stamina_t2 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_stamina_t2", false);
        stamina_t3 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_stamina_t3", false);
        movement_t1 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_movement_t1", false);
        movement_t2 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_movement_t2", false);
        movement_t3 = LNetworkVariable<bool>.Connect("ChuitosLethalUpgrades_movement_t3", false);
        tokens = LNetworkVariable<int>.Connect("ChuitosLethalUpgrades_tokens", 0);
        client_credits = LNetworkVariable<int>.Connect("ChuitosLethalUpgrades_client_credits", 0);
    }

    private static void OnClientJoinedRequest(ulong clientId)
    {
        LethalUpgradesBase.mls.LogInfo($"Client {clientId} requested sync, sending current upgrade states...");
        
        // If host health variables are true
        if(LethalUpgradesBase.health_t1)
        {
            if(health_t1.Value != true)
            {
                health_t1.Value = true;
            }
            else
            {
                health_t1.Value = false;
                health_t1.Value = true;
            }
        }

        if(LethalUpgradesBase.health_t2)
        {
            if(health_t2.Value != true)
            {
                health_t2.Value = true;
            }
            else
            {
                health_t2.Value = false;
                health_t2.Value = true;
            }
        }

        if(LethalUpgradesBase.health_t3)
        {
            if(health_t3.Value != true)
            {
                health_t3.Value = true;
            }
            else
            {
                health_t3.Value = false;
                health_t3.Value = true;
            }
        }

        // If host stamina variables are true
        if(LethalUpgradesBase.stamina_t1)
        {
            if(stamina_t1.Value != true)
            {
                stamina_t1.Value = true;
            }
            else
            {
                stamina_t1.Value = false;
                stamina_t1.Value = true;
            }
        }

        if(LethalUpgradesBase.stamina_t2)
        {
            if(stamina_t2.Value != true)
            {
                stamina_t2.Value = true;
            }
            else
            {
                stamina_t2.Value = false;
                stamina_t2.Value = true;
            }
        }

        if(LethalUpgradesBase.stamina_t3)
        {
            if(stamina_t3.Value != true)
            {
                stamina_t3.Value = true;
            }
            else
            {
                stamina_t3.Value = false;
                stamina_t3.Value = true;
            }
        }

        // If host movement variables are true
        if(LethalUpgradesBase.movement_t1)
        {
            if(movement_t1.Value != true)
            {
                movement_t1.Value = true;
            }
            else
            {
                movement_t1.Value = false;
                movement_t1.Value = true;
            }
        }

        if(LethalUpgradesBase.movement_t2)
        {
            if(movement_t2.Value != true)
            {
                movement_t2.Value = true;
            }
            else
            {
                movement_t2.Value = false;
                movement_t2.Value = true;
            }
        }

        if(LethalUpgradesBase.movement_t3)
        {
            if(movement_t3.Value != true)
            {
                movement_t3.Value = true;
            }
            else
            {
                movement_t3.Value = false;
                movement_t3.Value = true;
            }
        }

        if(LethalUpgradesBase.tokens != 0)
        {
            if(tokens.Value != LethalUpgradesBase.tokens)
            {
                tokens.Value = LethalUpgradesBase.tokens;
            }
            else
            {
                tokens.Value = -1;
                tokens.Value = LethalUpgradesBase.tokens;
            }
        }
        LethalUpgradesBase.mls.LogInfo($"Sync sent for client {clientId} - Tokens: {LethalUpgradesBase.tokens}");
    }

    // public static void OnClientCreditSyncRequest(ulong clientId)
    // {
    //     client_credits.OnValueChanged += (oldValue, newValue) =>
    //     {
    //         LethalUpgradesBase.SyncTerminals(client_credits.Value);
    //     };
    // }

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

            tokens.OnValueChanged += (oldValue, newValue) =>
            {
                LethalUpgradesBase.tokens = newValue;
                LethalUpgradesBase.mls.LogInfo($"tokens synced to: {newValue}");
            };

            client_credits.OnValueChanged += async (oldValue, newValue) =>
            {
                LethalUpgradesBase.mls.LogInfo($"Original Client Credits: {oldValue}");
                LethalUpgradesBase.mls.LogInfo($"New Client Credits: {newValue}");
                if(LNetworkUtils.IsHostOrServer)
                {
                    // await Task.Delay(1500);
                    LethalUpgradesBase.SyncTerminals(client_credits.Value);
                }
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