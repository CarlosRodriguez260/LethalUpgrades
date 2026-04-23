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
    internal static float movement_t2 = 4.6f * 1.12f;
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
    static void MovementTier2(PlayerControllerB __instance)
    {
        if (__instance == null) return;
        if (__instance != GameNetworkManager.Instance?.localPlayerController) return;

        if (LethalUpgradesBase.movement_t2)
        {
            __instance.movementSpeed = movement_t2;
        }
    }
    #endregion

    #region movement Tier 3
    internal static float movement_t3 = 13f * 1.25f;
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
    static void MovementTier3(PlayerControllerB __instance)
    {
        if (__instance == null) return;
        if (__instance != GameNetworkManager.Instance?.localPlayerController) return;

        if (LethalUpgradesBase.movement_t3)
        {
            __instance.jumpForce = movement_t3;
        }
    }
    #endregion 
}