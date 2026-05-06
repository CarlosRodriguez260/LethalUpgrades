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
using System.Collections;

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
    internal static bool heal_50_active = false;
    internal static bool heal_timer_active = false;
    internal static int heal_timer_seconds = 0;
    internal static int max_health = 100;
    private static Coroutine activeHeal50Coroutine;
    private static Coroutine activeHealTimerCoroutine;

    [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
    [HarmonyPostfix]
    static void StartStopTimer(PlayerControllerB __instance)
    {
        if (__instance == null) return;
        if (__instance != GameNetworkManager.Instance?.localPlayerController) return;

        // Update max health based on upgrades
        if(LethalUpgradesBase.health_t3) { max_health = 150; }
        else if(LethalUpgradesBase.health_t1) { max_health = 120; }
        else { max_health = 100; }

        var local_hud = HUDManager.Instance;
        if(local_hud == null)
        {
            LethalUpgradesBase.mls.LogInfo("Could not find local player HUD");
            return;
        }

        int threshold = max_health / 2;
        
        // Above 50% health - start/continue 20-second heal timer
        if(LethalUpgradesBase.health_leg && __instance.health > threshold)
        {
            if(!heal_timer_active)
            {
                // Stop any existing timer coroutine
                if(activeHealTimerCoroutine != null)
                {
                    __instance.StopCoroutine(activeHealTimerCoroutine);
                }
                
                heal_timer_active = true;
                heal_timer_seconds = 0;
                activeHealTimerCoroutine = __instance.StartCoroutine(HealTimerCoroutine(__instance, local_hud, max_health));
            }
            else
            {
                LethalUpgradesBase.mls.LogInfo("Heal timer interrupted and reset!");
                if(activeHealTimerCoroutine != null)
                {
                    __instance.StopCoroutine(activeHealTimerCoroutine);
                }
                heal_timer_seconds = 0;
                activeHealTimerCoroutine = __instance.StartCoroutine(HealTimerCoroutine(__instance, local_hud, max_health));
            }
        }
        // Below 50% health - interrupt timer and start slow heal
        else if(LethalUpgradesBase.health_leg && __instance.health < threshold)
        {
            // Cancel timer if it was running
            if(heal_timer_active)
            {
                LethalUpgradesBase.mls.LogInfo("Heal timer interrupted - health dropped below 50%!");
                heal_timer_active = false;
                heal_timer_seconds = 0;
                
                if(activeHealTimerCoroutine != null)
                {
                    __instance.StopCoroutine(activeHealTimerCoroutine);
                    activeHealTimerCoroutine = null;
                }
            }
            
            // Start slow heal to 50% if not already running
            if(!heal_50_active)
            {
                // Stop any existing heal coroutine
                if(activeHeal50Coroutine != null)
                {
                    __instance.StopCoroutine(activeHeal50Coroutine);
                }
                
                heal_50_active = true;
                activeHeal50Coroutine = __instance.StartCoroutine(HealTo50Coroutine(__instance, local_hud, threshold));
            }
        }
    }

    [HarmonyPatch(typeof(GameNetworkManager), "Disconnect")]
    [HarmonyPostfix]
    static void StopAllHealCoroutines()
    {
        var localPlayer = GameNetworkManager.Instance?.localPlayerController;
        if (localPlayer != null)
        {
            if(activeHealTimerCoroutine != null)
            {
                localPlayer.StopCoroutine(activeHealTimerCoroutine);
                activeHealTimerCoroutine = null;
            }
            
            if(activeHeal50Coroutine != null)
            {
                localPlayer.StopCoroutine(activeHeal50Coroutine);
                activeHeal50Coroutine = null;
            }
        }
        
        heal_timer_active = false;
        heal_50_active = false;
        heal_timer_seconds = 0;
        
        LethalUpgradesBase.mls.LogInfo("All heal coroutines stopped");
    }

    private static IEnumerator HealTimerCoroutine(PlayerControllerB player, HUDManager local_hud, int maxHealth)
    {
        LethalUpgradesBase.mls.LogInfo("Heal timer started - 20 seconds until regeneration begins");
        
        // Wait 20 seconds, checking for interruptions
        while(heal_timer_seconds < 20 && heal_timer_active)
        {
            // Check if health dropped below threshold (interruption condition)
            int threshold = maxHealth / 2;
            if(player.health <= threshold)
            {
                LethalUpgradesBase.mls.LogInfo("Heal timer interrupted - health dropped below 50%");
                heal_timer_active = false;
                yield break;
            }
            
            // Check if player died or upgrade was removed
            if(player == null || player.isPlayerDead || !LethalUpgradesBase.health_leg)
            {
                LethalUpgradesBase.mls.LogInfo("Heal timer cancelled - invalid state");
                heal_timer_active = false;
                yield break;
            }
            
            heal_timer_seconds++;
            LethalUpgradesBase.mls.LogInfo($"Heal timer: {heal_timer_seconds}/20 seconds");
            yield return new WaitForSeconds(1f);
        }
        
        // Start regeneration if timer completed and still active
        if(heal_timer_active && heal_timer_seconds >= 20)
        {
            LethalUpgradesBase.mls.LogInfo($"Heal timer complete! Beginning regeneration to {maxHealth}");
            
            // Regenerate 2 HP per second until at max health
            while(heal_timer_active && player.health < maxHealth)
            {
                // Check for interruptions during regeneration
                int threshold = maxHealth / 2;
                if(player.health <= threshold)
                {
                    LethalUpgradesBase.mls.LogInfo("Regeneration interrupted - health dropped below 50%");
                    break;
                }
                
                if(player == null || player.isPlayerDead || !LethalUpgradesBase.health_leg)
                {
                    LethalUpgradesBase.mls.LogInfo("Regeneration cancelled - invalid state");
                    break;
                }
                
                // Heal 2 HP per second
                player.health = Mathf.Min(player.health + 2, maxHealth);
                local_hud.UpdateHealthUI(player.health, hurtPlayer: false);
                LethalUpgradesBase.mls.LogInfo($"Regeneration: {player.health}/{maxHealth}");
                
                yield return new WaitForSeconds(1f);
            }
            
            LethalUpgradesBase.mls.LogInfo(player.health >= maxHealth ? "Regeneration complete!" : "Regeneration stopped early");
            
            // Reset for next time
            heal_timer_active = false;
            heal_timer_seconds = 0;
        }
        
        activeHealTimerCoroutine = null;
    }

    private static IEnumerator HealTo50Coroutine(PlayerControllerB player, HUDManager local_hud, int threshold)
    {
        LethalUpgradesBase.mls.LogInfo("Slow heal started - healing to 50% health");
        
        while(heal_50_active && player.health < threshold)
        {
            // Check if player died or upgrade was removed
            if(player == null || player.isPlayerDead || !LethalUpgradesBase.health_leg)
            {
                LethalUpgradesBase.mls.LogInfo("Slow heal cancelled - invalid state");
                break;
            }
            
            // Handle critically injured state
            if(player.criticallyInjured && player.health >= 20)
            {
                player.criticallyInjured = false;
                player.playerBodyAnimator.SetBool("Limp", false);
                player.bleedingHeavily = false;
                player.healthRegenerateTimer = 0f;
                local_hud.UpdateHealthUI(player.health, hurtPlayer: false);
                LethalUpgradesBase.mls.LogInfo("Critically injured state cleared");
            }
            
            // Heal 1 HP every 2 seconds
            if(player.health < threshold)
            {
                player.health = Mathf.Min(player.health + 1, threshold);
                local_hud.UpdateHealthUI(player.health, hurtPlayer: false);
                LethalUpgradesBase.mls.LogInfo($"Slow heal: {player.health}/{threshold}");
            }
            
            yield return new WaitForSeconds(2f);
        }
        
        LethalUpgradesBase.mls.LogInfo("Slow heal complete - reached 50% health");
        heal_50_active = false;
        activeHeal50Coroutine = null;
    }
    #endregion
}