using System.Numerics;
using GameNetcodeStuff;
using HarmonyLib;
using JetBrains.Annotations;
using LethalNetworkAPI.Utils;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using Unity.Collections;

namespace LethalUpgrades.Patches;
internal class UtilityPatching
{
    #region Utility Tier 1
    internal static int upgraded_flash_duration = (int)Math.Round(140f * 1.1f);
    internal static int upgraded_proflash_duration = (int)Math.Round(300f * 1.1f);

    [HarmonyPatch(typeof(FlashlightItem), "ItemActivate")]
    [HarmonyPostfix]
    static void UtilityTier1(FlashlightItem __instance)
    {
        var player = GameNetworkManager.Instance.localPlayerController;
        if(LethalUpgradesBase.utility_t1)
        {
            // Battery Usage Property = Time to drain battery in seconds [Zeekeers name it better >:(]
            if(player.isHoldingObject && __instance.playerHeldBy==player && __instance.itemProperties.requiresBattery)
            {
                if(__instance.itemProperties.itemName == "Flashlight")
                {
                    __instance.itemProperties.batteryUsage = upgraded_flash_duration;
                    // LethalUpgradesBase.mls.LogInfo($"Battery Usage Post-upgrade: {__instance.itemProperties.batteryUsage}");
                }
                else if(__instance.itemProperties.itemName == "Pro-flashlight")
                {
                    __instance.itemProperties.batteryUsage = upgraded_proflash_duration;
                    // LethalUpgradesBase.mls.LogInfo($"Battery Usage Post-upgrade: {__instance.itemProperties.batteryUsage}");
                }
            }
        }
        else
        {
            // LethalUpgradesBase.mls.LogInfo($"Battery Usage Pre-upgrade of {__instance.itemProperties.itemName}: {__instance.itemProperties.batteryUsage}");
        }
    }
    #endregion

    #region Utility Tier 2
    [HarmonyPatch(typeof(Shovel), "HitShovel")]
    [HarmonyPrefix]
    static void UtilityTier2(Shovel __instance)
    {
        if (LethalUpgradesBase.utility_t2)
        {
            var player = GameNetworkManager.Instance.localPlayerController;
            if (LethalUpgradesBase.utility_t2)
            {
                if(player.isHoldingObject && __instance.playerHeldBy==player)
                {
                    __instance.shovelHitForce = 2;
                }
            }
        }
    }

    static int changer_low = -2;
    static int changer = 0;
    static int changer_high = 2;
    [HarmonyPatch(typeof(Shovel), "HitShovel")]
    [HarmonyPostfix]
    static void UtilityTier2Explosion(Shovel __instance)
    {
        if (LethalUpgradesBase.utility_t2)
        {
            var player = GameNetworkManager.Instance.localPlayerController;
            if(player.isHoldingObject && __instance.playerHeldBy==player)
            {
                UnityEngine.Vector3 vector_changer = new UnityEngine.Vector3(0, 0, changer);

                if(changer<changer_high)
                {
                    changer++;
                }
                else if(changer==changer_high)
                {
                    changer = changer_low;
                }

                    
                LethalUpgradesNetwork.shovel_explosion_pos.Value = player.transform.position + vector_changer;
                LethalUpgradesBase.mls.LogInfo($"Shovel used by {player.playerClientId}");
                // Landmine.SpawnExplosion(player.transform.position, true, 0, 0, 0, 100);
            }
        }
    }
    #endregion

    #region Utility Tier 3
    #endregion
}