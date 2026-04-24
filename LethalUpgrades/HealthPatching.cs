using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

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
        if(LethalUpgradesBase.health_t2)
        {
            damageNumber = Mathf.RoundToInt(damageNumber * 0.95f);
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
    //TODO
    #endregion
}