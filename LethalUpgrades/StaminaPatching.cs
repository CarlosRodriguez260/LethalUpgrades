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
    #endregion 

    #region Stamina Tier 3
    #endregion 
}