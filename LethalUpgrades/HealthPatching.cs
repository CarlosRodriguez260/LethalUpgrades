using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using System.Timers;

namespace LethalUpgrades.Patches;
internal class HealthPatching
{
    #region Health Tier 1 and 3
    // internal static bool health_t1_once = true;
    // [HarmonyPatch(typeof(StartOfRound), "ReviveDeadPlayers")]
    // [HarmonyPostfix]
    // static void HealthTier1Leg() 
    // { 
    //     if(LethalUpgradesBase.health_t1) 
    //     { 
    //         var sor = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
    //         for(int i = 0; i < sor.allPlayerScripts.Length; i++)
    //         {
    //             if(!LethalUpgradesBase.health_t3_leg)
    //             {
    //                 sor.allPlayerScripts[i].health = 120;
    //             }
    //             else
    //             {
    //                 sor.allPlayerScripts[i].health = 140;
    //             }
    //         }
    //     } 
    // }

    // Maybe find a way for it to run once instead of infinitely?
    [HarmonyPatch(typeof(StartOfRound), "Update")]
    [HarmonyPostfix]
    static void HealthTier13() 
    { 
        if(LethalUpgradesBase.health_t1) 
        { 
            var sor = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
            if (sor.inShipPhase)
            {
                for(int i = 0; i < sor.allPlayerScripts.Length; i++)
                {
                    if(!LethalUpgradesBase.health_t3)
                    {
                        sor.allPlayerScripts[i].health = 120;
                    }
                    else
                    {
                        sor.allPlayerScripts[i].health = 150;
                    }
                }
            }
        } 
    }

    // Runs once. Mostly for applying after buying upgrade.
    // [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    // [HarmonyPostfix]
    // static void HealthTier1LegOnce()
    // {
    //     if(LethalUpgradesBase.health_t1 && health_t1_once)
    //     {
    //         var sor = UnityEngine.Object.FindFirstObjectByType<StartOfRound>();
    //         for(int i = 0; i < sor.allPlayerScripts.Length; i++)
    //         {
    //             if(!LethalUpgradesBase.health_t3_leg)
    //             {
    //                 sor.allPlayerScripts[i].health = 120;
    //             }
    //             else
    //             {
    //                 sor.allPlayerScripts[i].health = 140;
    //             }
    //         }
    //         health_t1_once = false;
    //     }
    // }
    #endregion

    #region Health Tier 2
    internal static bool check_damage = false;
    internal static int last_damage = 0;
    internal static int last_health = 0;
    [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
    [HarmonyPrefix]
    static void HealthTier2(ref int damageNumber, PlayerControllerB __instance)
    {
        if (__instance == null) return;
        if (__instance != GameNetworkManager.Instance?.localPlayerController) return;

        if(LethalUpgradesBase.health_t2)
        {
            damageNumber = Mathf.RoundToInt(damageNumber * 0.90f);
            last_damage = damageNumber;
            last_health = __instance.health;
            check_damage = true;
        }
    }

    // Fix Bug: If over 100 health and get hit by source of damage that does not bring you lower than
    // 100 health, game still clamps player health at 100 afterwards.
    [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
    [HarmonyPostfix]
    static void HealthFix1(PlayerControllerB __instance)
    {
        if(check_damage)
        {
            if(last_health-last_damage>100)
            {
                __instance.health = last_health-last_damage;
            }
            last_damage = 0;
            last_health = 0;
            check_damage = false;
        }
    }
    #endregion

    #region Health Tier Legendary
    internal static bool heal_50 = false;
    internal static bool timer_20 = false;
    internal static int up_to = 0;
    internal static int max_health = 100;

    [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
    [HarmonyPostfix]
    static void StartStopTimer(PlayerControllerB __instance)
    {
        if (__instance == null) return;
        if (__instance != GameNetworkManager.Instance?.localPlayerController) return;

        if(LethalUpgradesBase.health_t3){max_health = 150;}
        else if(LethalUpgradesBase.health_t1){max_health = 120;}

        int threshold = max_health/2;
        if(LethalUpgradesBase.health_leg && __instance.health>threshold)
        {
            if(!timer_20)
            {
                timer_20 = true;
                UpTo20HealTo100(__instance, max_health);
            }
            else
            {
                LethalUpgradesBase.mls.LogInfo("Interrupted heal timer!");
                up_to = 0;
            }
        }
        else if(LethalUpgradesBase.health_leg && __instance.health<threshold && timer_20)
        {
            LethalUpgradesBase.mls.LogInfo("Interrupted heal timer, and now below 50% health!");
            up_to = 0;
            timer_20 = false;
        }

        if(LethalUpgradesBase.health_leg && __instance.health<threshold)
        {
            heal_50 = true;
            HealTo50(__instance, max_health/2);
        }
    }

    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    [HarmonyPostfix]
    static void StopTasks()
    {
        LethalUpgradesBase.mls.LogInfo("Stopping all heal methods");
        timer_20 = false;
        heal_50 = false;
        LethalUpgradesBase.mls.LogInfo("Stopped all heal methods");
    }

    static async Task UpTo20HealTo100(PlayerControllerB player, int max_health)
    {
        while(timer_20)
        {
            if(up_to<20)
            {
                if(!timer_20)
                {
                    break;
                }
                up_to++;
                LethalUpgradesBase.mls.LogInfo($"Heal timer is at {up_to} seconds");
                await Task.Delay(1000);
            }
            else
            {
                // 20 seconds passed
                player.health += 2;
                if(player.health>=max_health)
                {
                    player.health = max_health;
                    up_to = 0;
                    timer_20 = false;
                }
                await Task.Delay(1000);
            }
        }
    }

    static async Task HealTo50(PlayerControllerB player, int half_health)
    {
        while(player.health<half_health && heal_50)
        {
            if(player.health<half_health)
            {
                player.health += 1;
            }
            else
            {
                heal_50 = false;
            }
            await Task.Delay(2000);
        }
    }
    #endregion
}