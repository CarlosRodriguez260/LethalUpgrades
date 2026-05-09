using System.ComponentModel;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using System.Collections;
using TMPro;
using System.Diagnostics.CodeAnalysis;

namespace LethalUpgrades.Patches;
internal class MovementPatching
{
    #region Movement Tier 1
    internal static float movement_t1 = 2.25f * 1.06f; // 1.10f | 2f for testing
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
    static void MovementTier1(PlayerControllerB __instance)
    {
        if (!LethalUpgradesBase.movement_t1) return;
        if (__instance != GameNetworkManager.Instance?.localPlayerController) return;
        
        var sprintMultiplierField = Traverse.Create(__instance).Field("sprintMultiplier");
        float currentMultiplier = sprintMultiplierField.GetValue<float>();
        
        if (__instance.isSprinting)
        {
            // Calculate what the value SHOULD be after original Lerp
            float targetMultiplier = movement_t1;
            float newMultiplier = Mathf.Lerp(currentMultiplier, targetMultiplier, Time.deltaTime * 1f);
            sprintMultiplierField.SetValue(newMultiplier);
        }
    }
    #endregion 

    #region Movement Tier 2
    // internal static float movement_t2 = 4.6f * 1.10f; // 4.6 is normal, in-game movement speed
    internal static bool apply_once1 = false;
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
    static void MovementTier2(PlayerControllerB __instance)
    {
        if (__instance == null) return;
        if (__instance != GameNetworkManager.Instance.localPlayerController) return;

        if(LethalUpgradesBase.movement_t2 && !apply_once1)
        {
            __instance.movementSpeed *= 1.10f;
            apply_once1 = true;
        }
    }
    #endregion

    #region Movement Tier 3
    // internal static float movement_t3 = 13f * 1.25f;
    internal static bool apply_once2 = false;
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
    static void MovementTier3(PlayerControllerB __instance)
    {
        if(__instance == null) return;
        if(__instance != GameNetworkManager.Instance?.localPlayerController) return;

        if(LethalUpgradesBase.movement_t3 && !apply_once2)
        {
            __instance.jumpForce *= 1.25f;
            apply_once2 = true;
        }
    }
    #endregion

    #region Movement Legendary
    internal static bool ninja;
    internal static bool ninja_cooldown;
    [HarmonyPatch(typeof(EnemyAI), "PlayerIsTargetable")]
    [HarmonyPostfix]
    static void Invulnerability(EnemyAI __instance, ref PlayerControllerB playerScript, ref bool __result)
    {
        if(!LethalUpgradesBase.movement_leg) return;

        if(!ninja) return;

        if(playerScript.health < 20)
        {
            __result = false;
            // LethalUpgradesBase.mls.LogInfo($"Removed player from {__instance.enemyType} targeting.");
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
    [HarmonyPrefix]
    static void MovementLeg0(PlayerControllerB __instance, ref int damageNumber)
    {
        if(!LethalUpgradesBase.movement_leg) return;

        if(__instance.health - damageNumber < 20)
        {
            if(!ninja && !ninja_cooldown)
            {
                ninja = true;
                damageNumber = 0;
                __instance.health = 5;
                HUDManager.Instance.DisplayTip("Old War Stealthkit", "You are phasing in and out of reality...");
                LethalUpgradesBase.mls.LogInfo($"Ninja on!");
            }
            if(LethalUpgradesBase.movement_t2){__instance.movementSpeed = 4.6f * 1.10f * 1.5f;}
            else{__instance.movementSpeed = 4.6f * 1.5f;}
            LethalUpgradesBase.mls.LogInfo($"Critical Movement speed = {__instance.movementSpeed}");
        }
    }

    [HarmonyPatch(typeof(PlayerControllerB), "MakeCriticallyInjured")]
    [HarmonyPostfix]
    static void MovementLeg1(PlayerControllerB __instance)
    {
        if(!LethalUpgradesBase.movement_leg) return;

        if(!__instance.criticallyInjured || !(__instance.health < 20))
        {
            if(ninja)
            {
                ninja_cooldown = true;
                ninja = false;
                LethalUpgradesBase.mls.LogInfo($"Ninja off!");
                Cooldown();
            }
            if(LethalUpgradesBase.movement_t2){__instance.movementSpeed = 4.6f * 1.10f;}
            else{__instance.movementSpeed = 4.6f;}
            LethalUpgradesBase.mls.LogInfo($"Non-Critical Movement speed = {__instance.movementSpeed}");
        }
    }

    static async Task Cooldown()
    {
        int time = 0;
        var player = GameNetworkManager.Instance.localPlayerController;
        LethalUpgradesBase.mls.LogInfo($"Ninja cooldown counting down...");
        while(time < 120)
        {
            if(player.isPlayerDead || player.disconnectedMidGame)
            {
                if(LethalUpgradesBase.movement_t2){player.movementSpeed = 4.6f * 1.10f;}
                else{player.movementSpeed = 4.6f;}
                LethalUpgradesBase.mls.LogInfo($"Cooldown interrupted.");
                break;
            }
            time += 1;
            await Task.Delay(1000);
        }
        ninja_cooldown = false;
        LethalUpgradesBase.mls.LogInfo($"Ninja cooldown completed!");
    }
    #endregion 
}