using TerminalApi.Classes;
using static TerminalApi.TerminalApi;
using HarmonyLib;
using LethalNetworkAPI.Utils;
using UnityEngine.AI;
using GameNetcodeStuff;
using UnityEngine;

namespace LethalUpgrades.Store;

public class RotationalStore
{
    public static Modifier[] easy_mods = [
        new Modifier(1, false, "Shiny but Swifty", "Easy", "Spawnable scrap becomes 7% more valuable, but enemies move 7% faster.", []),
        new Modifier(2, false, "More Risk, More Reward", "Easy", "Increase spawnable scrap by 2, but add 2 indoor and outdoor power.", []),
        new Modifier(3, false, "Risk of Rain", "Easy", "Remove 2 indoor and outdoor power, but increase meteor shower event chance by 10%.", []),
        new Modifier(11, false, "Edmund's Moon", "Easy", "Time moves 15% slower, but decreases amount of scrap by 2.", []),
        new Modifier(12, false, "Bouncy House", "Easy", "Jump 10% higher, but your feet explode on fall damage and take 10% more fall damage.", []),
        new Modifier(13, false, "Pound for Pound", "Easy", "Scrap is 10% more valuable, but weighs 5 more pounds.", [])
    ];
    public static Modifier[] medium_mods = [
        new Modifier(4, false, "Miller's Moon", "Medium", "Spawnable scrap becomes 20% more valuable, but time moves 25% faster.", []),
        new Modifier(5, false, "Mothron's Dawn", "Medium", "Spawnable scrap becomes 15% more valuable and spawns 3 more scrap, but weather becomes eclipsed and indoor/outdoor power increases by 2.\nIf already eclipsed, reap the benefits!", []),
        new Modifier(8, false, "Watch Your Back", "Medium", "Increase spawnable scrap by 5, but add 3 indoor power and only bracken's spawn.", []),
        new Modifier(9, false, "Shaped Glass", "Medium", "Spawn 1.5x the amount of scrap, but lose half your health.", []),
        new Modifier(10, false, "Lightning Speed", "Medium", "Gain 2x movement speed, but enemies gain 3x movement speed.", []),

    ];
    public static Modifier[] hard_mods = [
        new Modifier(6, false, "Go Play Outside!", "Hard", "Removes all indoor power of the moon, but adds it to outdoor power.", []),
        new Modifier(7, false, "Go Play Inside!", "Hard", "Removes all outdoor power of the moon, but adds it to indoor power.", []),
        new Modifier(14, false, "Double it and give it to the next person!", "Hard", "1.5x scrap amount and 1.5x scrap value, but double indoor/outdoor power and max enemy spawns.", []),
        new Modifier(15, false, "Midas Touch", "Hard", "Only gold bars spawn as scrap, but scrap amount decreases by half and health reduces to 20.", []),
        new Modifier(16, false, "The End", "God help us all...", "3x scrap amount/value and double health. But...", [])
    ];
    
    public static (int easy_ind1, int easy_ind2, int easy_ind3) easy_indexes = (-1, -1, -1); // -1 = Not Seeded
    public static (int medium_ind1, int medium_ind2) medium_indexes = (-1, -1);
    public static int hard_index = -1;
    public static float indoor_delta = 0;
    public static float original_indoor = 0;
    public static float outdoor_delta = 0;
    public static float original_outdoor = 0;
    public static void StoreSetup()
    {
        #region Store Commands
        LethalUpgradesBase.mls.LogInfo("Store is awake!");

        TerminalNode store_node = CreateTerminalNode("WELCOME TO THE ROTATIONAL MODIFIER STORE!\n\nPlease make sure to look at the available modifiers before making a choice!\nOnce you choose a modifier, you cannot go to other moons until you come back to orbit!\n", clearPreviousText: true);
        TerminalKeyword store_keyword = CreateTerminalKeyword("rot store", isVerb: false, store_node);
        AddTerminalKeyword(store_keyword ,new CommandInfo()
        {
            Category = "Help", Description = "Look at the rotational modifier store!"
        });

        TerminalNode easy_node = CreateTerminalNode("EASY MODIFIERS\n\n", clearPreviousText: true);
        TerminalKeyword easy = CreateTerminalKeyword("Tier 1 Mod", isVerb: false, easy_node);
        AddTerminalKeyword(easy, new CommandInfo()
        {
            TriggerNode = easy_node,
            DisplayTextSupplier = () =>
            {
                Modifier easy_mod = easy_mods[easy_indexes.easy_ind1];
                string mod1 = $"Title: {easy_mod.mod_title}\n" +
                $"Difficulty: {easy_mod.difficulty}\n" +
                $"Description: {easy_mod.description}\n" +
                $"Type '{easy_mod.difficulty} 1' to activate.\n" +
                $"Active: {(!easy_mod.active ? "No" : "Yes")}\n\n";

                easy_mod = easy_mods[easy_indexes.easy_ind2];
                string mod2 = $"Title: {easy_mod.mod_title}\n" +
                $"Difficulty: {easy_mod.difficulty}\n" +
                $"Description: {easy_mod.description}\n" +
                $"Type '{easy_mod.difficulty} 2' to activate.\n" +
                $"Active: {(!easy_mod.active ? "No" : "Yes")}\n\n";

                easy_mod = easy_mods[easy_indexes.easy_ind3];
                string mod3 = $"Title: {easy_mod.mod_title}\n" +
                $"Difficulty: {easy_mod.difficulty}\n" +
                $"Description: {easy_mod.description}\n" +
                $"Type '{easy_mod.difficulty} 3' to activate.\n" +
                $"Active: {(!easy_mod.active ? "No" : "Yes")}\n\n";

                return mod1 + mod2 + mod3;
            }, Category = "rot store", Description = "Shows the list of available tier 1 modifiers"
        });

        TerminalNode medium_node = CreateTerminalNode("MEDIUM MODIFIERS\n\n", clearPreviousText: true);
        TerminalKeyword medium = CreateTerminalKeyword("Tier 2 Mod", isVerb: false, medium_node);
        AddTerminalKeyword(medium, new CommandInfo()
        {
            TriggerNode = medium_node,
            DisplayTextSupplier = () =>
            {
                Modifier medium_mod = medium_mods[medium_indexes.medium_ind1];
                string mod1 = $"Title: {medium_mod.mod_title}\n" +
                $"Difficulty: {medium_mod.difficulty}\n" +
                $"Description: {medium_mod.description}\n" +
                $"Type '{medium_mod.difficulty} 1' to activate.\n" +
                $"Active: {(!medium_mod.active ? "No" : "Yes")}\n\n";

                medium_mod = medium_mods[medium_indexes.medium_ind2];
                string mod2 = $"Title: {medium_mod.mod_title}\n" +
                $"Difficulty: {medium_mod.difficulty}\n" +
                $"Description: {medium_mod.description}\n" +
                $"Type '{medium_mod.difficulty} 2' to activate.\n" +
                $"Active: {(!medium_mod.active ? "No" : "Yes")}\n\n";

                return mod1 + mod2;
            }, Category = "rot store", Description = "Shows the list of available tier 2 modifiers"
        });

        TerminalNode hard_node = CreateTerminalNode("HARD MODIFIERS\n\n", clearPreviousText: true);
        TerminalKeyword hard = CreateTerminalKeyword("Tier 3 Mod", isVerb: false, hard_node);
        AddTerminalKeyword(hard, new CommandInfo()
        {
            TriggerNode = hard_node,
            DisplayTextSupplier = () =>
            {
                Modifier hard_mod = hard_mods[hard_index];
                string mod1 = $"Title: {hard_mod.mod_title}\n" +
                $"Difficulty: {hard_mod.difficulty}\n" +
                $"Description: {hard_mod.description}\n" +
                $"Type 'Hard' to activate.\n" +
                $"Active: {(!hard_mod.active ? "No" : "Yes")}\n\n";
                return mod1;
            }, Category = "rot store", Description = "Shows the available tier 3 modifier"
        });

        LethalUpgradesBase.mls.LogInfo("Added base rotational store commands!");

        AddCommand("Easy 1", new CommandInfo()
        {
            
            DisplayTextSupplier = () =>
            {
                var easy_mod1 = easy_mods[easy_indexes.easy_ind1];
                if(easy_mod1.active) return "Easy modifier 1 is already active!\n";

                var sor = StartOfRound.Instance;
                if(!sor.inShipPhase)
                {
                    return "You can only activate modifiers in orbit!\n";
                }

                var terminal = LethalUpgradesBase.ActiveTerminal();
                if(terminal.currentNode!=easy_node)
                {
                    return "Cannot select easy modifiers here!\n";
                }

                EasyModChanger(easy_mod1);
                
                return "Selected easy modifier 1.\n";
            }, Category = "Tier 1 Mod"
        });

        AddCommand("Easy 2", new CommandInfo()
        {
            
            DisplayTextSupplier = () =>
            {
                var easy_mod2 = easy_mods[easy_indexes.easy_ind2];
                if(easy_mod2.active) return "Easy modifier 2 is already active!\n";

                var sor = StartOfRound.Instance;
                if(!sor.inShipPhase)
                {
                    return "You can only activate modifiers in orbit!\n";
                }

                var terminal = LethalUpgradesBase.ActiveTerminal();
                if(terminal.currentNode!=easy_node)
                {
                    return "Cannot select easy modifiers here!\n";
                }

                EasyModChanger(easy_mod2);

                return "Selected easy modifier 2.\n";
            }, Category = "Tier 1 Mod"
        });

        AddCommand("Easy 3", new CommandInfo()
        {
            
            DisplayTextSupplier = () =>
            {
                var easy_mod3 = easy_mods[easy_indexes.easy_ind3];
                if(easy_mod3.active) return "Easy modifier 3 is already active!";

                var sor = StartOfRound.Instance;
                if(!sor.inShipPhase)
                {
                    return "You can only activate modifiers in orbit!\n";
                }

                var terminal = LethalUpgradesBase.ActiveTerminal();
                if(terminal.currentNode!=easy_node)
                {
                    return "Cannot select easy modifiers here!\n";
                }

                EasyModChanger(easy_mod3);
                
                return "Selected easy modifier 3.\n";
            }, Category = "Tier 1 Mod"
        });

        AddCommand("Medium 1", new CommandInfo()
        {
            
            DisplayTextSupplier = () =>
            {
                var medium_mod1 = medium_mods[medium_indexes.medium_ind1];
                if(medium_mod1.active) return "Medium modifier 1 is already active!\n";

                var sor = StartOfRound.Instance;
                if(!sor.inShipPhase)
                {
                    return "You can only activate modifiers in orbit!\n";
                }

                var terminal = LethalUpgradesBase.ActiveTerminal();
                if(terminal.currentNode!=medium_node)
                {
                    return "Cannot select medium modifiers here!\n";
                }

                MediumModChanger(medium_mod1);
                
                return "Selected medium modifier 1.\n";
            }, Category = "Tier 2 Mod"
        });

        AddCommand("Medium 2", new CommandInfo()
        {
            
            DisplayTextSupplier = () =>
            {
                var medium_mod2 = medium_mods[medium_indexes.medium_ind2];
                if(medium_mod2.active) return "Medium modifier 2 is already active!\n";

                var sor = StartOfRound.Instance;
                if(!sor.inShipPhase)
                {
                    return "You can only activate modifiers in orbit!\n";
                }

                var terminal = LethalUpgradesBase.ActiveTerminal();
                if(terminal.currentNode!=medium_node)
                {
                    return "Cannot select medium modifiers here!\n";
                }

                MediumModChanger(medium_mod2);
                
                return "Selected medium modifier 2.\n";
            }, Category = "Tier 2 Mod"
        });

        AddCommand("Hard", new CommandInfo()
        {
            
            DisplayTextSupplier = () =>
            {
                var hard_mod = hard_mods[hard_index];
                if(hard_mod.active) return "Hard modifier is already active!";

                var sor = StartOfRound.Instance;
                if(!sor.inShipPhase)
                {
                    return "You can only activate modifiers in orbit!\n";
                }

                var terminal = LethalUpgradesBase.ActiveTerminal();
                if(terminal.currentNode!=hard_node)
                {
                    return "Cannot select hard modifiers here!\n";
                }

                HardModChanger(hard_mod);
                
                return "Selected hard modifier. Good luck.\n";
            }, Category = "Tier 3 Mod"
        });
        #endregion
    }

    #region Easy Mods
    static void EasyModChanger(Modifier mod)
    {
        switch(mod.mod_id)
        {
            case 1:
                easy_mods[0].active = true;
                LethalUpgradesNetwork.mod1.Value = true;
                break;
            case 2:
                easy_mods[1].active = true;
                LethalUpgradesNetwork.mod2.Value = true;
                break;
            case 3:
                easy_mods[2].active = true;
                LethalUpgradesNetwork.mod3.Value = true;
                break;
            case 11:
                easy_mods[3].active = true;
                LethalUpgradesNetwork.mod11.Value = true;
                break;
            case 12:
                easy_mods[4].active = true;
                LethalUpgradesNetwork.mod12.Value = true;
                break;
            case 13:
                easy_mods[5].active = true;
                LethalUpgradesNetwork.mod13.Value = true;
                break;
        }
    }

    public static async Task EasyModCallbacks(Modifier mod)
    {
        if(!mod.active) return;

        RoundManager rm;
        TimeOfDay tod;
        PlayerControllerB player;
        switch(mod.mod_id)
        {
            case 1:
                // Spawnable scrap becomes 7% more valuable, but enemies move 7% faster.
                if(!LNetworkUtils.IsHostOrServer) return;

                rm = RoundManager.Instance;
                LethalUpgradesBase.mls.LogInfo($"Host has turned on mod 1");
                if(rm.scrapValueMultiplier > 0.4f)
                {
                    rm.scrapValueMultiplier *= 1.07f;
                }
                else
                {
                    rm.scrapValueMultiplier = 0.4f * 1.07f;
                }

                while(mod.active)
                {
                    await Task.Delay(1000);
                    // Speed changer is ModifySpeedAssignment()
                }

                rm.scrapValueMultiplier = 0.4f;
                LethalUpgradesBase.mls.LogInfo("Host has turned off mod 1");
                break;
            case 2:
                // Spawn 2 more scrap, but add 2 indoor and outdoor power
                rm = RoundManager.Instance;
                if (!LNetworkUtils.IsHostOrServer)
                {
                    rm.currentLevel.minScrap += 2;
                    rm.currentLevel.maxScrap += 2;
                }
                indoor_delta += 2;
                outdoor_delta += 2;

                while(mod.active)
                {
                    await Task.Delay(1000);
                }

                if(LNetworkUtils.IsHostOrServer)
                {
                    rm.currentLevel.minScrap -= 2;
                    rm.currentLevel.maxScrap -= 2;
                }
                break;
            case 3:
                // Remove 2 indoor and outdoor power, but increase chance of meteor rain event
                rm = RoundManager.Instance;
                indoor_delta -= 2;
                outdoor_delta -= 2;

                bool chance_overwriten = false;
                tod = TimeOfDay.Instance;
                while(mod.active)
                {
                    if(LNetworkUtils.IsHostOrServer && !chance_overwriten)
                    {
                        tod = TimeOfDay.Instance;
                        if(tod == null) continue;

                        chance_overwriten = true;
                        if(hard_mods[4].active) return;

                        tod.overrideMeteorChance = 100;
                    }
                    await Task.Delay(1000);
                }

                tod.overrideMeteorChance = -1;
                break;
            case 11:
                // Time moves 15% slower, but decreases amount of scrap by 2.
                if(!LNetworkUtils.IsHostOrServer) return;

                rm = RoundManager.Instance;
                rm.currentLevel.minScrap -= 2;
                rm.currentLevel.maxScrap -= 2;

                bool set_multiplier = false;
                tod = TimeOfDay.Instance;
                while(mod.active)
                {
                    if(!set_multiplier)
                    {
                        tod = TimeOfDay.Instance;
                        if(tod == null) continue;

                        set_multiplier = true;
                        tod.globalTimeSpeedMultiplier *= 0.85f;
                        // LethalUpgradesBase.mls.LogInfo("Applied time multiplier!");
                    }
                    await Task.Delay(1000);
                }

                rm.currentLevel.minScrap += 2;
                rm.currentLevel.maxScrap += 2;
                tod.globalTimeSpeedMultiplier /= 0.85f;
                break;
            case 12:
                // Jump 10% higher, but take 10% more fall damage.
                player = GameNetworkManager.Instance.localPlayerController;
                player.jumpForce *= 1.10f;

                while(mod.active)
                {
                    // Additional call to calculate fall damage: FallDamage()
                    await Task.Delay(1000);
                }

                player.jumpForce /= 1.10f;
                break;
            case 13:
                // Scrap is 10% more valuable, but weighs 5 more pounds.
                rm = RoundManager.Instance;
                if(!LNetworkUtils.IsHostOrServer) return;

                if(rm.scrapValueMultiplier != 0.4f)
                {
                    rm.scrapValueMultiplier *= 1.10f;
                }
                else
                {
                    rm.scrapValueMultiplier = 0.4f * 1.10f;
                }

                while(mod.active)
                {
                    // Additional call to add weight to items: AddedWeight()
                    await Task.Delay(1000);
                }

                rm.scrapValueMultiplier = 0.4f;
                break;
        }
    }
    #endregion

    #region Medium Mods
    static void MediumModChanger(Modifier mod)
    {
        switch(mod.mod_id)
        {
            case 4:
                medium_mods[0].active = true;
                LethalUpgradesNetwork.mod4.Value = true;
                break;
            case 5:
                medium_mods[1].active = true;
                LethalUpgradesNetwork.mod5.Value = true;
                break;
            case 8:
                medium_mods[2].active = true;
                LethalUpgradesNetwork.mod8.Value = true;
                break;
            case 9:
                medium_mods[3].active = true;
                LethalUpgradesNetwork.mod9.Value = true;
                break;
            case 10:
                medium_mods[4].active = true;
                LethalUpgradesNetwork.mod10.Value = true;
                break;
        }
    }

    public static async Task MediumModCallbacks(Modifier mod)
    {
        if(!mod.active) return;

        RoundManager rm;
        PlayerControllerB player;
        TimeOfDay tod;
        switch(mod.mod_id)
        {
            case 4:
                // Scrap becomes 20% more valuable, but time moves 25% faster
                if(!LNetworkUtils.IsHostOrServer) return;

                rm = RoundManager.Instance;
                if(rm.scrapValueMultiplier > 0.4f)
                {
                    rm.scrapValueMultiplier *= 1.20f;
                }
                else
                {
                    rm.scrapValueMultiplier = 0.4f * 1.20f;
                }

                bool set_multiplier = false;
                tod = TimeOfDay.Instance;
                while(mod.active)
                {
                    if(!set_multiplier)
                    {
                        if(tod == null) continue;

                        set_multiplier = true;
                        tod.globalTimeSpeedMultiplier *= 1.25f;
                        // LethalUpgradesBase.mls.LogInfo("Applied time multiplier!");
                    }
                    await Task.Delay(1000);
                }

                tod.globalTimeSpeedMultiplier /= 1.25f;
                rm.scrapValueMultiplier = 0.4f;
                break;
            case 5:
                // Spawnable scrap becomes 15% more valuable and spawns 3 more scrap, but weather becomes eclipsed and indoor/outdoor power increases by 2;
                // If already eclipsed, keep it that way.
                rm = RoundManager.Instance;

                if(LNetworkUtils.IsHostOrServer)
                {
                    if(rm.scrapValueMultiplier > 0.4f)
                    {
                        rm.scrapValueMultiplier *= 1.15f;
                    }
                    else
                    {
                        rm.scrapValueMultiplier = 0.4f * 1.07f;
                    }
                    rm.currentLevel.minScrap += 3;
                    rm.currentLevel.maxScrap += 3;
                }

                indoor_delta += 2;
                outdoor_delta += 2;
                
                var moon = rm.currentLevel;
                if(moon.currentWeather != LevelWeatherType.Eclipsed)
                {
                    moon.currentWeather = LevelWeatherType.Eclipsed;

                    var terminal = LethalUpgradesBase.ActiveTerminal();
                    var credits = terminal.groupCredits;
                    var sor = StartOfRound.Instance;
                    sor.ChangeLevelServerRpc(moon.levelID, newGroupCreditsAmount: credits);

                    MoonWeather weather_credits = new MoonWeather();
                    weather_credits.new_weather = LevelWeatherType.Eclipsed;
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
                }

                while(mod.active)
                {
                    await Task.Delay(1000);
                }

                if(LNetworkUtils.IsHostOrServer)
                {
                    rm.scrapValueMultiplier = 0.4f;
                    rm.currentLevel.minScrap -= 3;
                    rm.currentLevel.maxScrap -= 3;
                }
                break;
            case 8:
                // Increase spawnable scrap by 5, but add 3 indoor power and only bracken's spawn.
                rm = RoundManager.Instance;

                indoor_delta += 3;

                var enemies_copy = new List<SpawnableEnemyWithRarity>(rm.currentLevel.Enemies);
                if(LNetworkUtils.IsHostOrServer)
                {
                    // var enemies_copy = rm.currentLevel.Enemies; // This references, does not create a separate copy
                    rm.currentLevel.Enemies.RemoveAll(enemy => enemy.enemyType.enemyName != "Flowerman");
                    rm.currentLevel.Enemies.ForEach(enemy => enemy.enemyType.MaxCount = 10);
                    rm.currentLevel.minScrap += 5;
                    rm.currentLevel.maxScrap += 5;
                }

                while(mod.active)
                {
                    await Task.Delay(1000);
                }

                if(LNetworkUtils.IsHostOrServer)
                {
                    rm.currentLevel.Enemies = enemies_copy;
                    rm.currentLevel.minScrap -= 5;
                    rm.currentLevel.maxScrap -= 5;
                }
                break;
            case 9:
                // Spawn 1.5x the amount of scrap, but lose half your health
                rm = RoundManager.Instance;
                var old_min = rm.currentLevel.minScrap;
                var old_max = rm.currentLevel.maxScrap;
                player = GameNetworkManager.Instance.localPlayerController;
                if(LNetworkUtils.IsHostOrServer)
                {
                    var new_min_scrap = rm.currentLevel.minScrap*1.5f;
                    var new_max_scrap = rm.currentLevel.maxScrap*1.5f;
                    rm.currentLevel.minScrap = (int)new_min_scrap;
                    rm.currentLevel.maxScrap = (int)new_max_scrap;
                }
                player.health = player.health / 2;

                while(mod.active)
                {
                    await Task.Delay(1000);
                }

                if(LNetworkUtils.IsHostOrServer)
                {
                    rm.currentLevel.minScrap = old_min;
                    rm.currentLevel.maxScrap = old_max;
                }
                break;
            case 10:
                // Gain 2x movement speed, but enemies gain 3x movement speed
                player = GameNetworkManager.Instance.localPlayerController;
                player.movementSpeed *= 2;

                while(mod.active)
                {
                    await Task.Delay(1000);
                }

                player.movementSpeed /= 2;

                break;
        }
    }
    #endregion

    #region Hard Mods
    static void HardModChanger(Modifier mod)
    {
        switch(mod.mod_id)
        {
            case 6:
                hard_mods[0].active = true;
                LethalUpgradesNetwork.mod6.Value = true;
                break;
            case 7:
                hard_mods[1].active = true;
                LethalUpgradesNetwork.mod7.Value = true;
                break;
            case 14:
                hard_mods[2].active = true;
                LethalUpgradesNetwork.mod14.Value = true;
                break;
            case 15:
                hard_mods[3].active = true;
                LethalUpgradesNetwork.mod15.Value = true;
                break;
            case 16:
                hard_mods[4].active = true;
                LethalUpgradesNetwork.mod16.Value = true;
                break;
        }
    }

    public static async Task HardModCallbacks(Modifier mod)
    {
        if(!mod.active) return;

        RoundManager rm;
        PlayerControllerB player;
        SelectableLevel moon;
        HUDManager hud;
        TimeOfDay tod;
        switch(mod.mod_id)
        {
            case 6:
                // Remove all indoor power, and move it to outdoor power
                break;
            case 7:
                // Remove all outdoor power, and move it to indoor power
                break;
            case 14:
                // 2x scrap amount and 1.25x scrap value, but double indoor/outdoor power and max enemy spawns.
                rm = RoundManager.Instance;
                var enemies_copy = new List<SpawnableEnemyWithRarity>(rm.currentLevel.Enemies);
                indoor_delta += rm.currentLevel.maxEnemyPowerCount;
                outdoor_delta += rm.currentLevel.maxOutsideEnemyPowerCount;

                var old_min_scrap = rm.currentLevel.minScrap;
                var old_max_scrap = rm.currentLevel.maxScrap;
                if(LNetworkUtils.IsHostOrServer)
                {
                    rm.currentLevel.minScrap *= 2;
                    rm.currentLevel.maxScrap *= 2;
                    rm.scrapValueMultiplier *= 1.5f;
                    LethalUpgradesBase.mls.LogInfo($"New Min Scrap: {rm.currentLevel.minTotalScrapValue}");
                    LethalUpgradesBase.mls.LogInfo($"New Max Scrap: {rm.currentLevel.maxTotalScrapValue}");

                    rm.currentLevel.Enemies.ForEach(enemy => enemy.enemyType.MaxCount *= 2);
                }

                
                while(mod.active)
                {
                    await Task.Delay(1000);
                }

                if(LNetworkUtils.IsHostOrServer)
                {
                    rm.currentLevel.minScrap = old_min_scrap;
                    rm.currentLevel.maxTotalScrapValue = old_max_scrap;
                    rm.scrapValueMultiplier = 0.4f;
                    rm.currentLevel.Enemies = enemies_copy;
                }

                break;
            case 15:
                // Only gold bars spawn as scrap, but scrap amount decreases by half and health reduces to 20.
                rm = RoundManager.Instance;
                player = GameNetworkManager.Instance.localPlayerController;
                moon = rm.currentLevel;
                player.health = 20;
             
                if(!LNetworkUtils.IsHostOrServer) return;
                moon.minScrap /= 2;
                moon.maxScrap /= 2;

                var scrap_list = moon.spawnableScrap;
                var original_scrap_copy = new List<SpawnableItemWithRarity>(scrap_list);
                bool contains_bar = scrap_list.Any(scrap => scrap.spawnableItem.itemName == "Gold bar");
                if(contains_bar)
                {
                    LethalUpgradesBase.mls.LogInfo("Planet has gold bar in loot table!");
                    scrap_list.RemoveAll(scrap => scrap.spawnableItem.itemName != "Gold bar");
                }
                else
                {
                    // Add gold bars to loot table
                    LethalUpgradesBase.mls.LogInfo("Planet has no gold bar in loot table!");
                    var sor = StartOfRound.Instance;
                    var exp_loot_table = new List<SpawnableItemWithRarity>(sor.levels[0].spawnableScrap); // Experimentation, since it has gold bars

                    exp_loot_table.RemoveAll(scrap => scrap.spawnableItem.itemName != "Gold bar");
                    rm.currentLevel.spawnableScrap = exp_loot_table;
                }

                while(mod.active)
                {
                    await Task.Delay(1000);
                }

                moon.minScrap *= 2;
                moon.maxScrap *= 2;
                moon.spawnableScrap = original_scrap_copy;
                break;
            case 16:
                // 3x scrap amount and value and double health. But...
                // 3x indoor/outdoor power, eclipsed, and everything can spawn 3x as usual
                rm = RoundManager.Instance;

                indoor_delta += rm.currentLevel.maxEnemyPowerCount*3;
                outdoor_delta += rm.currentLevel.maxOutsideEnemyPowerCount*3;
                GameNetworkManager.Instance.localPlayerController.health *= 2;

                if(LNetworkUtils.IsHostOrServer)
                {
                    rm.currentLevel.minScrap *= 3;
                    rm.currentLevel.maxScrap *= 3;
                    rm.scrapValueMultiplier *= 3;
                }

                moon = rm.currentLevel;
                if(moon.currentWeather != LevelWeatherType.Eclipsed)
                {
                    moon.currentWeather = LevelWeatherType.Eclipsed;

                    var terminal = LethalUpgradesBase.ActiveTerminal();
                    var credits = terminal.groupCredits;
                    var sor = StartOfRound.Instance;
                    sor.ChangeLevelServerRpc(moon.levelID, newGroupCreditsAmount: credits);

                    MoonWeather weather_credits = new MoonWeather();
                    weather_credits.new_weather = LevelWeatherType.Eclipsed;
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
                }

                rm.currentLevel.Enemies.ForEach(enemy => enemy.enemyType.MaxCount *= 3);
                hud = HUDManager.Instance;
                hud.DisplayTip("What have you done...", "The curse on this modifier is too strong even for me to balance. May God have mercy on all of you...", true);

                bool chance_overwriten = false;
                tod = TimeOfDay.Instance;
                while(mod.active)
                {
                    if(LNetworkUtils.IsHostOrServer && !chance_overwriten)
                    {
                        if(tod == null) continue;

                        chance_overwriten = true;
                        tod.overrideMeteorChance = 1001;
                    }
                    await Task.Delay(1000);
                }

                if(LNetworkUtils.IsHostOrServer)
                {
                    rm.currentLevel.minScrap /= 3;
                    rm.currentLevel.maxScrap /= 3;
                    rm.scrapValueMultiplier = 0.4f;
                }
                tod.overrideMeteorChance = -1;
                break;
        }
    }
    #endregion

    #region Additional Calls
    static bool display_once = true;
    [HarmonyPatch(typeof(StartOfRound), "Update")]
    [HarmonyPostfix]
    static void DisplayPower(StartOfRound __instance)
    {
        if(__instance.inShipPhase && !display_once)
        {
            display_once = true;
            return;
        }

        if(__instance.inShipPhase) return;
        if(!display_once) return;

        var hud = HUDManager.Instance;
        var rm = RoundManager.Instance;

        original_indoor = rm.currentMaxInsidePower;
        original_outdoor = rm.currentLevel.maxOutsideEnemyPowerCount;

        LethalUpgradesBase.mls.LogInfo($"Original indoor power: {original_indoor}");
        LethalUpgradesBase.mls.LogInfo($"Original outdoor power: {original_outdoor}");
        LethalUpgradesBase.mls.LogInfo($"Indoor delta: {indoor_delta}");
        LethalUpgradesBase.mls.LogInfo($"Outdoor delta: {outdoor_delta}");

        rm.currentMaxInsidePower += indoor_delta;
        rm.currentLevel.maxOutsideEnemyPowerCount += (int)outdoor_delta;
        

        LethalUpgradesBase.mls.LogInfo($"Modified indoor power: {rm.currentMaxInsidePower}");
        LethalUpgradesBase.mls.LogInfo($"Modified outdoor power: {rm.currentLevel.maxOutsideEnemyPowerCount}");

        if(hard_mods[0].active)
        {
            // Only outside power
            LethalUpgradesBase.mls.LogInfo("Only outside power!");
            rm.currentLevel.maxOutsideEnemyPowerCount += (int)rm.currentMaxInsidePower;
            rm.currentMaxInsidePower = 0;
        }
        else if(hard_mods[1].active)
        {
            // Only inside power
            LethalUpgradesBase.mls.LogInfo("Only inside power!");
            rm.currentMaxInsidePower += rm.currentLevel.maxOutsideEnemyPowerCount;
            rm.currentLevel.maxOutsideEnemyPowerCount = 0;
            rm.currentMaxOutsidePower = 0;
        }

        display_once = false;
        hud.DisplayTip("Lethal Upgrades", $"Indoor Power: {rm.currentMaxInsidePower}\nOutdoor Power: {rm.currentLevel.maxOutsideEnemyPowerCount}");
    }

    [HarmonyPatch(typeof(NavMeshAgent), "set_speed")]
    [HarmonyPrefix]
    static void ModifySpeedAssignment(ref float value)
    {
        if(easy_mods[0].active)
        {
            value *= 1.07f;
        }
        if(medium_mods[4].active)
        {
            // LethalUpgradesBase.mls.LogInfo($"Setting x4 speed modifier to enemy!");
            value *= 3f;
        }

        if(value > 30f)
        {
            value = 30f;
        }
    }

    [HarmonyPatch(typeof(StartOfRound), "ChangeLevel")]
    [HarmonyPrefix]
    static bool DisallowTravel(StartOfRound __instance, ref int levelID)
    {
        var hud = HUDManager.Instance;
        foreach(Modifier mod in easy_mods)
        {
            if(mod.active && levelID != __instance.currentLevel.levelID)
            {
                hud.DisplayTip("Nuh uh uh!", "You are not allowed to switch moons once a modifier is active!", true);
                LethalUpgradesBase.mls.LogInfo("Yarhihar, not today!");
                return false;
            }
        }

        foreach(Modifier mod in medium_mods)
        {
            if(mod.active && levelID != __instance.currentLevel.levelID)
            {
                hud.DisplayTip("Nuh uh uh!", "You are not allowed to switch moons once a modifier is active!", true);
                return false;
            }
        }

        foreach(Modifier mod in hard_mods)
        {
            if(mod.active && levelID != __instance.currentLevel.levelID)
            {
                hud.DisplayTip("Nuh uh uh!", "You are not allowed to switch moons once a modifier is active!", true);
                return false;
            }
        }

        // If no mods active or switching weather, allow travel
        return true;
    }

    [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
    [HarmonyPrefix]
    static void FallDamage(ref int damageNumber, ref CauseOfDeath causeOfDeath, PlayerControllerB __instance)
    {
        if (__instance == null) return;
        if (__instance != GameNetworkManager.Instance?.localPlayerController) return;

        if(causeOfDeath == CauseOfDeath.Gravity && easy_mods[4].active)
        {
            LethalUpgradesBase.mls.LogInfo("Increasing fall damage!");
            damageNumber = Mathf.RoundToInt(damageNumber * 1.10f);

            float changer = Patches.UtilityPatching.changer;
            UnityEngine.Vector3 vector_changer = new UnityEngine.Vector3(0, changer, 0);
            if(changer == -0.9f)
            {
                changer = -1f;
            }
            else if(changer == -1f)
            {
                changer = -0.9f;
            }

            LethalUpgradesNetwork.fall_explosion_pos.Value = __instance.transform.position + vector_changer;
        }
    }

    internal static Dictionary<string, float> item_dict = new Dictionary<string, float>();
    [HarmonyPatch(typeof(GrabbableObject), "LateUpdate")]
    [HarmonyPostfix]
    static void AddedWeight(GrabbableObject __instance)
    {
        var item_name = __instance.itemProperties.itemName;
        var player = GameNetworkManager.Instance.localPlayerController;
        if(player == null) return; 

        if(!easy_mods[5].active)
        {
            if(item_dict.Count() > 0)
            {
                if(item_dict.ContainsKey(item_name))
                {
                    if(__instance.itemProperties.weight == item_dict[item_name])
                    {
                        // Reduce object back to original weight
                        LethalUpgradesBase.mls.LogInfo($"Restored original weight of {item_name}");
                        __instance.itemProperties.weight -= 0.05f;

                        player = GameNetworkManager.Instance.localPlayerController;
                        if(__instance.playerHeldBy == player)
                        {
                            player.carryWeight -= 0.05f;
                        }
                    }
                }
            }
            return;
        }

        bool is_equipment = false;
        switch(item_name)
        {
            case "Pro-flashlight":
            case "Shovel":
            case "Jetpack":
            case "Lockpicker":
            case "Radar-booster":
            case "Stun grenade":
            case "Boombox":
            case "Zap gun":
            case "Belt bag":
                // Do nothing to equipment
                is_equipment = true;
                break;
        }


        // Equipment is filtered out, only scrap past this point
        if(!is_equipment)
        {
            if(item_dict.Count() <= 0 || !item_dict.ContainsKey(item_name))
            {
                item_dict.Add(item_name, __instance.itemProperties.weight += 0.05f);
                __instance.itemProperties.weight += 0.05f;

                if(__instance.playerHeldBy == player)
                {
                    player.carryWeight += 0.05f;
                }
            }
            else
            {
                if(__instance.itemProperties.weight != item_dict[item_name])
                {
                    __instance.itemProperties.weight = item_dict[item_name];

                    if(__instance.playerHeldBy == player)
                {
                    player.carryWeight += 0.05f;
                }
                }
            }
        }
    }
    #endregion

    #region Mod Seeder
    [HarmonyPatch(typeof(StartOfRound), "Awake")]
    [HarmonyPostfix]
    static void SeedModifiers()
    {
        if(!LNetworkUtils.IsHostOrServer) return;
        var sor = StartOfRound.Instance;

        System.Random rand = new System.Random();
        List<int> selected = new List<int>();
        int random_index;
        int count;

        // Select 3 easy modifiers
        count = 3;
        while (count > 0)
        {
            random_index = rand.Next(easy_mods.Length);
            if(selected.Contains(random_index))
            {
                continue;
            }

            selected.Add(random_index);
            count--;
        }
        var easy_ind = (selected[0], selected[1], selected[2]);
        easy_indexes = easy_ind;
        LethalUpgradesNetwork.easy_indexes.Value = easy_ind;
        selected.Clear();

        // Select 2 medium modifiers
        count = 2;
        while (count > 0)
        {
            random_index = rand.Next(medium_mods.Length);
            if(selected.Contains(random_index))
            {
                continue;
            }

            selected.Add(random_index);
            count--;
        }
        var medium_ind = (selected[0], selected[1]);
        medium_indexes = medium_ind;
        LethalUpgradesNetwork.medium_indexes.Value = medium_ind;
        selected.Clear();

        // Select 1 hard modifier
        random_index = rand.Next(hard_mods.Length);
        hard_index = random_index;
        LethalUpgradesNetwork.hard_index.Value = random_index;
        LethalUpgradesBase.mls.LogInfo("Seeded first set of modifier events!");
    }

    [HarmonyPatch(typeof(HUDManager), "FillEndGameStats")]
    [HarmonyPrefix]
    static void ResetModifiers1()
    {
        StopAllModifiers();
        SeedModifiers();
    }

    [HarmonyPatch(typeof(HUDManager), "DisplayNewDeadline")]
    [HarmonyPrefix]
    static void ResetModifiers2()
    {
        StopAllModifiers();
        SeedModifiers();
    }

    [HarmonyPatch(typeof(StartOfRound), "EndOfGame")]
    [HarmonyPrefix]
    static void ResetModifiers3()
    {
        StopAllModifiers();
        SeedModifiers();
    }

    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    [HarmonyPrefix]
    static void StopAllModifiers()
    {
        easy_mods[easy_indexes.easy_ind1].active = false;
        easy_mods[easy_indexes.easy_ind2].active = false;
        easy_mods[easy_indexes.easy_ind3].active = false;
        medium_mods[medium_indexes.medium_ind1].active = false;
        medium_mods[medium_indexes.medium_ind2].active = false;
        hard_mods[hard_index].active = false;

        LethalUpgradesNetwork.mod1.Value = false;
        LethalUpgradesNetwork.mod2.Value = false;
        LethalUpgradesNetwork.mod3.Value = false;
        LethalUpgradesNetwork.mod4.Value = false;
        LethalUpgradesNetwork.mod5.Value = false;
        LethalUpgradesNetwork.mod6.Value = false;
        LethalUpgradesNetwork.mod7.Value = false;
        LethalUpgradesNetwork.mod8.Value = false;
        LethalUpgradesNetwork.mod9.Value = false;
        LethalUpgradesNetwork.mod10.Value = false;
        LethalUpgradesNetwork.mod11.Value = false;
        LethalUpgradesNetwork.mod12.Value = false;
        LethalUpgradesNetwork.mod13.Value = false;
        LethalUpgradesNetwork.mod14.Value = false;
        LethalUpgradesNetwork.mod15.Value = false;
        LethalUpgradesNetwork.mod16.Value = false;

        LethalUpgradesBase.mls.LogInfo($"Resetting power delta's");
        var rm = RoundManager.Instance;
        if(indoor_delta != 0 || hard_mods[1].active)
        {
            LethalUpgradesBase.mls.LogInfo($"Resetting indoor delta of {indoor_delta}");
            rm.currentMaxInsidePower = original_indoor;
            indoor_delta = 0;
        }
        if(outdoor_delta != 0 || hard_mods[0].active)
        {
            LethalUpgradesBase.mls.LogInfo($"Resetting outdoor delta of {outdoor_delta}");
            rm.currentLevel.maxOutsideEnemyPowerCount = (int)original_outdoor;
            outdoor_delta = 0;
        }
    }
}
#endregion

public class Modifier
{
    public int mod_id;
    public bool active;
    public string mod_title;
    public string difficulty;
    public string description;
    public string[] on_moons;

    public Modifier(int mod_id, bool active, string mod_title, string difficulty, string description, string[] on_moons)
    {
        this.mod_id = mod_id;
        this.active = active;
        this.mod_title = mod_title;
        this.difficulty = difficulty;
        this.description = description;
        this.on_moons = on_moons; // Empty array means available on all moons
    }
}