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
internal class StaminaPatching
{
    #region Stamina Tier 1
    internal static float stamina_t1 = 11 * 1.3f;
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    [HarmonyPostfix]
    static void StaminaTier1(PlayerControllerB __instance)
    {
        if (__instance == null) return;
        if (__instance != GameNetworkManager.Instance?.localPlayerController) return;

        if (LethalUpgradesBase.stamina_t1 && __instance.sprintTime!=stamina_t1)
        {
            __instance.sprintTime = stamina_t1;
        }
    }
    #endregion 

    #region Stamina Tier 2
    internal static float num3 = 1f;
    internal static float og_sprint = 4f * 8f; // 4f is original
    internal static float og_walk = 9f * 8f; // 9f is original
    internal static float og_sprintTime = 11f; // To keep stamina regen akin to original sprint timer
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    [HarmonyPostfix]
    static void StaminaTier2(PlayerControllerB __instance, bool ___isWalking)
    {
        if (__instance == null) return;
        if (__instance != GameNetworkManager.Instance?.localPlayerController) return;

        // Runs after original to keep adding more stamina
        // Must increase denominator values to balance it out
        // Original time to refill whole stamina bar staying still: ~14s
        // Upgrade time to refill whole stamina bar staying still: ~12s (~10% reduction)
        if(__instance.isSprinting){return;}
        if (LethalUpgradesBase.stamina_t2 && ___isWalking) 
        {
            __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter + Time.deltaTime / (og_sprintTime + og_walk) * num3, 0f, 1f);
        }
        else if (LethalUpgradesBase.stamina_t2 && !___isWalking)
        {
            __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter + Time.deltaTime / (og_sprintTime + og_sprint) * num3, 0f, 1f);
        }
    }
    #endregion 

    #region Stamina Tier 3
    #endregion 
}