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
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    [HarmonyPostfix]
    static void StaminaTier3(PlayerControllerB __instance)
    {
        if (__instance == null) return;
        if (__instance != GameNetworkManager.Instance?.localPlayerController) return;

        if(LethalUpgradesBase.stamina_t3 && __instance.isSprinting && __instance.carryWeight>=1.48f)
        {
            __instance.sprintMeter = Mathf.Clamp(__instance.sprintMeter + Time.deltaTime / og_sprintTime * __instance.carryWeight/8 * num3, 0f, 1f);
        }
    }
    // internal static float static_weight = 0;
    // internal static float copy_player_weight = 0;
    // internal static float delta_weight = 0;
    // internal static bool active = false;
    // internal static int frameCounter = 0;

    // [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    // [HarmonyPrefix]
    // static void PreWeight(PlayerControllerB __instance)
    // {
    //     frameCounter++;
    //     float beforeWeight = __instance.carryWeight;
        
    //     if(!active)
    //     {
    //         static_weight = __instance.carryWeight;
    //         LethalUpgradesBase.mls.LogInfo($"[Frame {frameCounter}] NOT ACTIVE: Set static_weight = {static_weight:F4}");
    //     }
    //     else
    //     {
    //         delta_weight = __instance.carryWeight - copy_player_weight;
    //         static_weight += delta_weight;
    //         LethalUpgradesBase.mls.LogInfo($"[Frame {frameCounter}] ACTIVE: currentWeight={__instance.carryWeight:F4}, copy={copy_player_weight:F4}, delta={delta_weight:F4}, static_weight={static_weight:F4}");
    //     }

    //     if(LethalUpgradesBase.stamina_t3 && static_weight >= 1.48 && !active)
    //     {
    //         LethalUpgradesBase.mls.LogInfo($"[Frame {frameCounter}] ACTIVATING! static_weight={static_weight:F4} >= 1.48");
    //         active = true;
    //         float newWeight = __instance.carryWeight - (__instance.carryWeight-1)/2;
    //         __instance.carryWeight = newWeight;
    //         copy_player_weight = __instance.carryWeight;
    //         LethalUpgradesBase.mls.LogInfo($"[Frame {frameCounter}] New weight: {newWeight:F4}, copy_player_weight={copy_player_weight:F4}");
    //     }
    //     else if(LethalUpgradesBase.stamina_t3 && static_weight < 1.48 && active)
    //     {
    //         LethalUpgradesBase.mls.LogInfo($"[Frame {frameCounter}] DEACTIVATING! static_weight={static_weight:F4} < 1.48");
    //         __instance.carryWeight = static_weight;
    //         active = false;
    //         copy_player_weight = 0;
    //         static_weight = 0;
    //         delta_weight = 0;
    //         LethalUpgradesBase.mls.LogInfo($"[Frame {frameCounter}] Restored weight to: {__instance.carryWeight:F4}");
    //     }
        
    //     LethalUpgradesBase.mls.LogInfo($"[Frame {frameCounter}] Final weight: {__instance.carryWeight:F4}, active={active}");
    // }
    #endregion 
}